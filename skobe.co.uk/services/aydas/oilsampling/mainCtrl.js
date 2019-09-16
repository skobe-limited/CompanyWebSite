(function () {

	var app = angular.module('app', []);

	app.factory('myRequester', function ($q, $http) {
		var urlx = "http://skobe.co.uk/api/services/";
		if (window.location.href.indexOf(".local") > -1) {
			urlx = "http://localhost:62238/services/";
		}

		return {
			postData: function (action, req) {
				var deferred = $q.defer();

				$http({
					url: urlx + action,
					method: 'POST',
					data: req,
					headers: { 'Content-Type': 'application/json;charset=utf-8' }
				}).then(function (response) {
					deferred.resolve(response);
				}, function (error) {
					deferred.reject(error);
				});
				return deferred.promise;
			},
			getData: function (action, req) {
				var deferred = $q.defer();

				$http({
					url: urlx + action,
					method: 'GET',
					headers: { 'Content-Type': 'application/json;charset=utf-8' }
				}).then(function (response) {
					deferred.resolve(response);
				}, function (error) {
					deferred.reject(error);
				});
				return deferred.promise;
			}

		};
	});

})();


(function () {

	angular.module('app').controller('mainCtrl', ['$scope', '$timeout', 'myRequester', '$filter', '$compile', '$interval', '$location', function ($scope, $timeout, myRequester, $filter, $compile, $interval, $location) {

		// #region INITIALS
		$scope.submitButtonText = "Submit";
		$scope.itemIndex = -1;
		$scope.s = {
			samples: []
		};

		$scope.defaultSample = {
			Language: "English",
			OilReportStandard: "IEC Standard",
			OffLoad: "Yes",
			OnLoad: "Yes"
		};

		$scope.sample = angular.copy($scope.defaultSample);
		// #endregion

		// #region SAMPLE CRUD
		$scope.resetSample = function () {
			$scope.sample = angular.copy($scope.defaultSample);
			$scope.itemIndex = -1;
		};

		$scope.addSample = function () {
			if ($scope.itemIndex == -1) {
				$scope.s.samples.push($scope.sample);
			}
			else {
				$scope.s.samples[$scope.itemIndex] = angular.copy($scope.sample);
			}
			$scope.resetSample();
		};

		$scope.removeSample = function (index) {
			$scope.s.samples.splice(index, 1);
			$scope.itemIndex = -1;
		};

		$scope.cloneSample = function (index) {
			$scope.sample = angular.copy($scope.s.samples[index]);
			$('#sampleModal').modal('show');
		};

		$scope.editSample = function (index) {
			$scope.itemIndex = index;
			$scope.sample = angular.copy($scope.s.samples[index]);
			$('#sampleModal').modal('show');
		};

		$scope.calcAge = function () {
			if ($scope.sample.YearOfManufacture && $scope.s.SamplingDate) {
				$scope.sample.OilAge = $scope.s.SamplingDate.getFullYear() - $scope.sample.YearOfManufacture;
			}
		}
		// #endregion

		$scope.doit = function () {
			$scope.formInProgress = true;
			$scope.submitButtonText = "Please wait...";
			myRequester.postData("OilSamplingSave", $scope.s).then(function (data) {
				if (data.data == "OK") {
					alert("Your form submitted.");
					$scope.reload();
				}
				else {
					alert("Some error(s) ocurred :(\n\nPlease try again later.");
				}
				$scope.submitButtonText = "Submit";
				$scope.formInProgress = false;
			});
		}

		$scope.init = function () {
			$scope.formInProgress = false;
		};

		$scope.reload = function () {
			location.reload();
		};
		$scope.init();

	}]);
	angular.module('app').controller('viewCtrl', ['$scope', '$timeout', 'myRequester', '$filter', '$compile', '$interval', '$location', function ($scope, $timeout, myRequester, $filter, $compile, $interval, $location) {

		// #region INITIALS
		$scope.s = {};
		// #endregion

		$scope.loadForm = function (FormId) {
			$scope.formInProgress = true;
			$scope.submitButtonText = "Please wait...";
			myRequester.getData("ViewForm?FormId="+FormId).then(function (data) {
				if (data.data) {
					console.log(data.data);
					$scope.s = data.data;
					console.log($scope.s.samples[0]);
				}
				else {
					alert("Some error(s) ocurred :(\n\nPlease try again later.");
				}
				$scope.formInProgress = false;
			});
		}

		$scope.init = function () {
			var params = location.href.split("=");
			if (params.length == 1) {
				alert("invalid link!");
			}
			else {
				$scope.loadForm(params[1]);
			}
		};

		$scope.init();

	}]);

})()
