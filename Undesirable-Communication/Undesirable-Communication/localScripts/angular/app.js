'use strict';

angular
.module('UndesirableCommunication', [
    "UndesirableCommunication_Chat",
    "ngRoute"
]).config(['$provide', '$routeProvider', function ($provide, $routeProvider) {
    'use strict';
    $routeProvider
    .when('/chat', {
        templateUrl: '/views/chat.html',
    })
      .otherwise({
          redirectTo: '/chat'
      });
}]);


