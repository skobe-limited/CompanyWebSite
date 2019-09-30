(function () {

	var app = angular.module('app', []);

	app.factory('myRequester', function ($q, $http) {
		var urlx = "http://skobe.co.uk/api/invoice/";
		if (window.location.href.indexOf(".local") > -1) {
			urlx = "http://localhost:62238/invoice/";
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
		$scope.invoice = {
			details: [],
			InvoiceDate: new Date()
		};
		// #endregion

		// #region CRUD
		$scope.resetDetail = function () {
			$scope.description = "";
			$scope.rate = "";
			$scope.quantity = "";

		};

		$scope.addDetail = function () {
			$scope.invoice.details.push({Description:$scope.description,Rate:$scope.rate,Quantity:$scope.quantity});
			$scope.resetDetail();
		};

		$scope.removeDetail = function (index) {
			$scope.invoice.details.splice(index, 1);
		};


		$scope.calcTotal = function () {
			var total = 0;
			for (var i = 0; i < $scope.invoice.details.length; i++) {
				total += ($scope.invoice.details[i].Rate * $scope.invoice.details[i].Quantity);
			}
			return total;
		}
		// #endregion


		$scope.saveInvoice = function () {
			$scope.formInProgress = true;
			$scope.submitButtonText = "Please wait...";
			myRequester.postData("InvoiceSave", $scope.invoice).then(function (data) {
				if (data.data == "OK") {
					location.href="./";
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

		$scope.init();

	}]);
	angular.module('app').controller('listCtrl', ['$scope', '$timeout', 'myRequester', '$filter', '$compile', '$interval', '$location', function ($scope, $timeout, myRequester, $filter, $compile, $interval, $location) {

		$scope.invoiceList = [];
		$scope.loadList = function () {
			myRequester.getData("InvoiceList").then(function (data) {
				if (data.data) {
					console.log(data.data);
					$scope.invoiceList = data.data;
				}
				else {
					alert("Some error(s) ocurred :(\n\nPlease try again later.");
				}
				$scope.formInProgress = false;
			});
		}

		$scope.init = function () {
			$scope.loadList();
		};

		$scope.init();

	}]);
	angular.module('app').controller('viewCtrl', ['$scope', '$timeout', 'myRequester', '$filter', '$compile', '$interval', '$location', function ($scope, $timeout, myRequester, $filter, $compile, $interval, $location) {

		// #region INITIALS
		$scope.invoiceDefaults = {
			CompanyName: "Skobe Limited",
			Address: "Unit 1j Leroy House, 436 Essex Road, N1 3QP London, England",
			CompanyRegNr: "12000753",
			WebUrl: "http://skobe.co.uk",
			Phone: "+44 7984 631 138",
			BankAccountName: "Skobe Limited",
			BankName: "Barclays",
			SortCode: "20-84-13",
			BankAccountNr: "03901998",
			IBAN: "GB72 BUKB 2084 1303 9019 98",
			SwiftCode: "BUKBGB22"
			//IBAN: "GB12 BUKB 20810073252973 / BARCGB22",
			
		};
		// #endregion

		$scope.loadInvoice = function (InvoiceNumber) {
			$scope.formInProgress = true;
			myRequester.getData("ViewInvoice?InvoiceNumber=" + InvoiceNumber).then(function (data) {
				if (data.data) {
					console.log(data.data);
					$scope.invoice = data.data;
				}
				else {
					alert("Some error(s) ocurred :(\n\nPlease try again later.");
				}
				$scope.formInProgress = false;
			});
		};

		$scope.calcTotal = function () {
			var total = 0;
			if ($scope.invoice && $scope.invoice.details) {
				for (var i = 0; i < $scope.invoice.details.length; i++) {
					total += ($scope.invoice.details[i].Rate * $scope.invoice.details[i].Quantity);
				}
			}
			return total;
		}

		$scope.init = function () {
			var params = location.href.split("=");
			if (params.length == 1) {
				alert("invalid link!");
			}
			else {
				$scope.loadInvoice(params[1]);
			}
		};

		$scope.init();

	}]);

})()
