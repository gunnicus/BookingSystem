(function () {

    angular.module('booking', [])
        .controller('BookingCtrl', function ProductCtrl($scope) {
            $scope.debug = false;
            $scope.errorMessage = null;
            
            $scope.bookingDetail = { Name: '', Phone: '', Email: '', CheckInDatetime: null, CheckOutDatetime: null };
            
            $scope.bookings = [];
            
            var bookingsHub = $.connection.bookingsHub;

            $scope.placeBooking = function () {

                $scope.errorMessage = null;


                bookingsHub.state.debug = $scope.debug;
                bookingsHub.server.placeBooking($scope.bookingDetail)
                    .done(function () {
                        $scope.bookingDetail.Name = '';
                        $scope.bookingDetail.Phone = '';
                        $scope.bookingDetail.Email = '';
                        $scope.bookingDetail.CheckInDatetime = null;
                        $scope.bookingDetail.CheckOutDatetime = null;
                    })
                    .fail(function (error) {
                        console.log('Invocation of PlaceBooking failed. : ' + error);
                        $scope.errorMessage = "We couldn't place you booking, ensure all endpoints are running and try again!";
                    });
            };

            bookingsHub.client.bookingReceived = function (data) {
                
                $scope.$apply(function (scope) {
                    scope.bookings.push({ number: data.BookingNumber, name: data.BookingItem.Name, phone: data.BookingItem.Phone, email: data.BookingItem.Email, checkIn: data.BookingItem.CheckInDatetime, checkOut: data.BookingItem.CheckOutDatetime,  status: 'Booked' });
                });
                
            };

            bookingsHub.client.bookingCancelled = function (data) {
                $scope.$apply(function (scope) {
                    var idx = retrieveBookingIndex(scope, data.BookingNumber);
                    if (idx >= 0) {
                        scope.bookings[idx].status = 'Cancelled';
                    }
                });
            };
            
            $scope.cancelBooking = function (number) {
                $scope.errorMessage = null;

                var idx = retrieveBookingIndex($scope, number);
                if (idx >= 0) {
                    $scope.bookings[idx].status = 'Cancelling';
                }

                bookingsHub.state.debug = $scope.debug;
                bookingsHub.server.cancelBooking(number)
                    .fail(function () {
                        $scope.errorMessage = "We couldn't cancel you booking, ensure all endpoints are running and try again!";
                    });
            };

            $scope.checkIn = function (number) {
                $scope.errorMessage = null;

                var idx = retrieveBookingIndex($scope, number);
                if (idx >= 0) {
                    $scope.bookings[idx].status = 'Pending';
                }

                bookingsHub.state.debug = $scope.debug;
                bookingsHub.server.checkIn(number)
                    .fail(function () {
                        $scope.errorMessage = "We couldn't checkin you booking, ensure all endpoints are running and try again!";
                    });
            };

            bookingsHub.client.checkedIn = function (data) {
                $scope.$apply(function (scope) {
                    var idx = retrieveBookingIndex(scope, data.BookingNumber);
                    if (idx >= 0) {
                        scope.bookings[idx].status = 'CheckedIn';
                    }
                });
            };

            $scope.checkOut = function (number) {
                $scope.errorMessage = null;

                var idx = retrieveBookingIndex($scope, number);
                if (idx >= 0) {
                    $scope.bookings[idx].status = 'Pending';
                }

                bookingsHub.state.debug = $scope.debug;
                bookingsHub.server.checkOut(number)
                    .fail(function () {
                        $scope.errorMessage = "We couldn't checkout you booking, ensure all endpoints are running and try again!";
                    });
            };

            bookingsHub.client.checkedOut = function (data) {
                $scope.$apply(function (scope) {
                    var idx = retrieveBookingIndex(scope, data.BookingNumber);
                    if (idx >= 0) {
                        scope.bookings[idx].status = 'CheckedOut';
                    }
                });
            };
            
            function retrieveBookingIndex(scope, bookingNumber) {
                var idx = 0;

                for (; idx < scope.bookings.length; idx++) {
                    if (scope.bookings[idx].number === bookingNumber) {
                        return idx;
                    }
                }

                return -1;
            }

            $.connection.hub.start()
                .done(function () {
                    console.log('Now connected, connection ID=' + $.connection.hub.id);
                })
                .fail(function () { console.log('Could not Connect!'); });
        });

}())