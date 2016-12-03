(function () {
    var app = angular.module("UndesirableCommunication_Chat");
    app.controller("ChatController", [
        '$scope' , function (
        $scope) {
            $scope.test = "TESTIng!@#";
            
            $scope.States = {
                LOADING: "LOADING"
            };

            var defaultDatabase = firebase.database();

            defaultDatabase.ref('PendingUsers/').once("value").then(function (snapshot) {
                var un = snapshot.val().User;
            });


            $scope.CurrentState = $scope.States.LOADING;

        }]);
}());