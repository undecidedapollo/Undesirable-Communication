(function () {
    var app = angular.module("UndesirableCommunication_Chat");
    app.controller("SignupController", [
        '$scope', 'ChatService', "toastr", "$location" , function (
        $scope, ChatService, toastr, $location) {

            $scope.FindChatPartner = function () {
                if ($scope.ChatName == null || $scope.ChatName.length <= 2 || $scope.ChatName.length > 25) {
                    $scope.Error = "Your username must be between 3 and 25 characters.";
                }

                ChatService.currentUsername = $scope.ChatName;
                
                $location.url("/chat");

            };


        }]);
}());