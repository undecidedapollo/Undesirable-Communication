'use strict';

angular
.module('UndesirableCommunication', [
    "UndesirableCommunication_Chat",
    "ngRoute",
    "toastr"
]).config(['$provide', '$routeProvider', function ($provide, $routeProvider) {
    'use strict';
    $routeProvider
    .when('/chat', {
        templateUrl: '/views/chat.html',
    })
    .when('/signup', {
        templateUrl: '/views/signup.html',
    })
    .when('/', {
        templateUrl: '/views/home.html',
    })
    .otherwise({
        redirectTo: '/'
    });
}]);


