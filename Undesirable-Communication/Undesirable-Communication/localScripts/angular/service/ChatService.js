(function () {
    var module = angular.module("UndesirableCommunication_Chat");
    module.factory("ChatService", ['$q', '$window', function ($q, $window) {

        var currentUsername = null;

        return {
            currentUsername: currentUsername
        };

    }]);
}());