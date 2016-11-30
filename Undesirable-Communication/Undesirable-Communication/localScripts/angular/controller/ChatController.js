(function () {
    var app = angular.module("UndesirableCommunication_Chat");
    app.controller("ChatController", [
        '$scope' , function (
        $scope) {
            $scope.test = "TESTIng!@#";
            
            $scope.States = {
                LOADING: "LOADING"
            };

            var chatHub = $.connection.chatHub;

            registerClientMethods(chatHub);

            // Start Hub
            $.connection.hub.start().done(function () {

                registerEvents(chatHub);

            });

            $scope.CurrentState = $scope.States.LOADING;

        }]);
}());