(function () {
    var app = angular.module("UndesirableCommunication_Chat");
    app.controller("ChatController", [
        '$scope', 'ChatService', '$location', '$interval', '$timeout', function (
        $scope, ChatService, $location, $interval, $timeout) {
            $scope.test = "TESTIng!@#";

            $scope.States = {
                LOADING: "LOADING",
                LOADING_POST_REGISTER: "LOADING_POST_REGISTER",
                CHATTING: "CHATTING"
            };

            var registerUser = function () {
                var chosenUsername = ChatService.currentUsername;

                if (chosenUsername == null) {
                    $location.url("/signup");
                    return;
                }

                $scope.operationPending++;
                ChatService.registerUser(chosenUsername).then(onRegisterSuccess, onRegisterError);
            };

            var onRegisterSuccess = function (data) {
                $scope.operationPending--;
                var result = data.data;

                if (result == null) {
                    $location.url("/signup");
                    return;
                }

                ChatService.currentUserId = result.UserId;
                $scope.CurrentState = $scope.States.LOADING_POST_REGISTER;
                $scope.LoadingMessage = "Finding a user to chat with...";

                cancelInterval();

                $scope.interval = $interval(checkStatusInterval, 2500);
            };

            var onRegisterError = function (data) {
                $scope.operationPending--;
                $location.url("/signup");
                return;
            };

            var getRandomInt = function (min, max) {
                return Math.floor(Math.random() * (max - min + 1)) + min;
            }

            var checkStatusInterval = function () {
                $scope.StatusIteration++;
                if ($scope.operationPending != 0) {
                    return;
                }

                if ($scope.startCheckTime != null) {
                    var currentTime = new Date().getTime();

                    if ((currentTime - $scope.startCheckTime) > 60000) {
                        $scope.LoadingMessage = "There appears to be no one online. We'll keep trying...";
                    } else {
                        if ($scope.StatusIteration % 3 == 0) {
                            $scope.LoadingMessage = $scope.LoadingMessages[getRandomInt(0, $scope.LoadingMessages.length - 1)];
                        }
                    }

                } else {
                    $scope.startCheckTime = new Date().getTime();
                }


                var currentChatId = ChatService.currentUserId;

                if (currentChatId == null) {
                    $scope.CurrentState = $scope.States.LOADING;
                    registerUser();
                    return;
                }

                $scope.operationPending++;
                ChatService.checkStatus(currentChatId).then(onCheckStatusSuccess, onCheckStatusError);
            };

            var onCheckStatusSuccess = function (data) {
                $scope.operationPending--;
                var result = data.data;

                if (result == null) {
                    $location.url("/signup");
                    return;
                }

                if (result.IsRegistered == false) {
                    $scope.CurrentState = $scope.States.LOADING;
                    registerUser();
                    return;
                }

                if (result.IsPending == true) {
                    return;
                }

                var newConnection = result.connection.Id;

                if (newConnection == null) {
                    return;
                }

                cancelInterval();

                $scope.interval = $interval(checkMessages, 1000);

                ChatService.currentConnection = newConnection;

                $scope.ChatPartnerName = result.connection.OtherUsersName;


                $scope.CurrentState = $scope.States.CHATTING;
            };

            var onCheckStatusError = function (data) {
                $scope.operationPending--;
                $location.url("/signup");
            };

            var checkMessages = function () {

                if ($scope.operationPending != 0) {
                    return;
                }

                var currentUserId = ChatService.currentUserId;
                var currentConnection = ChatService.currentConnection;

                if (currentUserId == null || currentConnection == null) {
                    $location.url("/signup");
                }

                $scope.operationPending++;
                ChatService.getMessages(currentConnection, currentUserId).then(checkMessagesSuccess, checkMessagesError);



            };

            var checkMessagesSuccess = function (data) {
                $scope.operationPending--;

                var messageList = data.data;

                if (messageList == null) {
                    return;
                }


                var anyNew = false;
                angular.forEach(messageList.Messages, function (curMess) {

                    var exists = false;

                    for (var i = 0; i < $scope.MessageList.length; i++) {
                        var existMess = $scope.MessageList[i];

                        if (curMess.Id == existMess.Id) {
                            exists = true;
                            break;
                        }
                    }

                    if (exists == false) {
                        curMess.IsUser = false;
                        $scope.MessageList.push(curMess);
                        anyNew = true;
                    }
                });

                if (anyNew) {
                    $timeout(function () {
                        updateScrollBar();
                    }, 100);

                }
            };

            var checkMessagesError = function (data) {
                $scope.operationPending--;

                var result = data.data;

                if (result == null) {
                    $location.url("/signup");
                }

                if (result.Action == "groupNotFound") {
                    findNewChatPartner();
                } else if (result.Action == "userNotFound") {
                    $location.url("/signup");
                }
            };

            var cancelInterval = function () {
                if ($scope.interval != null) {
                    $interval.cancel($scope.interval);
                }
            };

            $scope.sendMessage = function () {
                var currentMessage = $scope.CurrentMessage;

                if (currentMessage == "" || currentMessage == null) {
                    return;
                }

                var currentUserId = ChatService.currentUserId;
                var currentConnection = ChatService.currentConnection;

                if (currentUserId == null || currentConnection == null) {
                    $location.url("/signup");
                }

                $scope.CurrentMessage = "";

                var newMessage = {
                    Content: currentMessage,
                    IsRequestingUser: true
                };

                $scope.MessageList.push(newMessage);

                $scope.operationPending++;
                ChatService.sendMessage(currentConnection, currentUserId, currentMessage).then(
                    function (data) {
                        onSendMessageSuccess(data, newMessage);
                    }, function (data) {
                        onSendMessageFailure(data, newMessage);
                    });
            };

            var onSendMessageSuccess = function (data, message) {
                $scope.operationPending--;
                var currMess = data.data;

                if (currMess == null) {
                    return;
                }

                if (message == null);

                message.Id = currMess.Id;
                message.TimeSent = currMess.TimeSent;
                message.Author = currMess.Author;
            };

            var onSendMessageFailure = function (data, message) {
                $scope.operationPending--;

                var result = data.data;

                if (result == null) {
                    $location.url("/signup");
                }

                if (result.Action == "groupNotFound") {
                    findNewChatPartner();
                } else if (result.Action == "userNotFound") {
                    $location.url("/signup");
                }

                if (message == null) {
                    return;
                }

                message.Content = "Message Failed: " + message.Content;
            };

            $scope.$on("$destroy", function () {
                cancelInterval();
                sendSignoff();
            });

            $(window).on('keydown', function (e) {
                if (e.which == 13) {
                    $scope.sendMessage()
                    return false;
                }
            });

            $scope.GoBack = function () {
                sendSignoff();
                $location.url("/signup");
            };

            var sendSignoff = function () {
                var currentUserId = ChatService.currentUserId;
                var currentConnection = ChatService.currentConnection;

                if (currentUserId == null || currentConnection == null) {
                    return;
                }

                ChatService.ExitChat(currentConnection, currentUserId);
            };

            var findNewChatPartner = function () {
                var currentUsername = ChatService.currentUsername;

                startup();
            };

            $scope.FindSomeoneElse = function () {
                sendSignoff();
                findNewChatPartner();
            };

            var startup = function () {
                $scope.startCheckTime = null;
                $scope.StatusIteration = 0;
                $scope.operationPending = 0;
                $scope.CurrentState = $scope.States.LOADING;
                $scope.MessageList = [];
                ChatService.clearAllData();
                registerUser();
            };

            $scope.LoadingMessages = [
                "Still can't find a user...",
                "Where are the users...",
                "Donde esta la useras???",
                "You user? Where partner?",
                "Marco.........",
                "I spy with my little eye a partner...JK"
            ];


            var updateScrollBar = function () {

                $timeout(function () {
                    try {
                        var objDiv = document.getElementById("messages-content");
                        objDiv.scrollTop = objDiv.scrollHeight;
                    } catch (ex) {

                    }
                }, 150);




            };

            startup();

        }]);
}());