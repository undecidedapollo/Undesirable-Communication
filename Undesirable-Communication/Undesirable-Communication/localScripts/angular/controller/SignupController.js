(function () {
    var app = angular.module("UndesirableCommunication_Chat");
    app.controller("SignupController", [
        '$scope', 'ChatService', "toastr", "$location" , function (
        $scope, ChatService, toastr, $location) {

            $scope.FindChatPartner = function () {
                if ($scope.ChatName == null || $scope.ChatName.length <= 5 || $scope.ChatName.length > 25) {
                    $scope.Error = "Your username must be between 6 and 25 characters.";
                    return;
                }

                ChatService.currentUsername = $scope.ChatName;
                
                $location.url("/chat");

            };


        }]);
}());