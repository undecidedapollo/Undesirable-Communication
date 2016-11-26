(function () {
    var app = angular.module("UndesirableCommunication_Chat");
    app.controller("ChatController", [
        '$scope' , function (
        $scope) {
            $scope.test = "TESTIng!@#";
            
            $scope.States = {
                LOADING: "LOADING"
            };

            $scope.CurrentState = $scope.States.LOADING;

        }]);
}());