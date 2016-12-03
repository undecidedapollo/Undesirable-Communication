(function () {
    var module = angular.module("UndesirableCommunication_Chat");
    module.factory("ChatService", ['$q', '$window', '$http', function ($q, $window, $http) {

        var currentUsername = null;
        var currentUserId = null;
        var currentConnection = null;

        var clearAllData = function () {
            currentUserId = null;
            currentConnection = null;
            currentUsername = null;
        };

        var registerUser = function (username) {
            return $http.post("api/Chat/Register", { Username: username });
        };

        var checkStatus = function (userId) {
            return $http.post("api/Chat/CheckStatus", { Id: userId });
        };

        var getMessages = function (chatId, userId) {
            return $http.get("api/Chat/GetRecentMessages?chatId=" + chatId + "&currentUserId=" + userId);
        };

        var sendMessage = function (chatId, userId, message) {
            return $http.post("api/Chat/SendMessage", { ChatId: chatId, CurrentUserId: userId, Content: message});
        };

        var ExitChat = function (chatId, userId) {
            return $http.post("api/Chat/ExitChat", { Id: chatId, CurrentUserId: userId });
        };



        return {
            currentUsername: currentUsername,
            currentUserId: currentUserId,
            currentConnection: currentConnection,
            registerUser: registerUser,
            checkStatus: checkStatus,
            getMessages: getMessages,
            sendMessage: sendMessage,
            ExitChat: ExitChat,
            clearAllData: clearAllData
        };

    }]);
}());