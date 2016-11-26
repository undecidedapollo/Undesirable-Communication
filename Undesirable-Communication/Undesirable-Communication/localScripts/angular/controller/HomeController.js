(function () {
    var app = angular.module("UndesirableCommunication_Chat");
    app.controller("HomeController", [
        '$scope', '$location', function ($scope, $location) {

            $scope.GoToSignup = function () {
                $location.url("/signup");
            };

        }]);
}());