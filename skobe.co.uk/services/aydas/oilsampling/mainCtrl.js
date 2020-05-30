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

		// Fruits
		$scope.testList = [
			{ text: "DGA analysis", desc: "IEC 60567 / ASTM D3612" },
			{ text: "Moisture", desc: "IEC 60814 / ASTM D1533" },
			{ text: "Disruptive / Breakdown voltage", desc: "IEC 60156 / ASTM D877/1816" },
			{ text: "Dielectric dissipation factor (tan d)", desc: "IEC 60247 / ASTM D924" },
			{ text: "Determination of elements (metal, sulphur)", desc: "DIN 51399-1" },
			{ text: "DC resistivity", desc: "IEC 60247 / ASTM D1169" },
			{ text: "Solids content, overall", desc: "IEC 60422, Annex C" },
			{ text: "Determination of sludge", desc: "IEC 60422, Annex C" },
			{ text: "Density 15 C", desc: "ISO 12185 or 3675 / ASTM D1298" },
			{ text: "Visual inspection", desc: "-" },
			{ text: "Neutralization number / acidity", desc: "IEC 62021 / ASTM D974" },
			{ text: "Interfacial tension", desc: "ASTM D 971-12" },
			{ text: "Viscosity (40 C, 100 C)", desc: "ISO 3104 / ASTM D445" },
			{ text: "Oxidation stability", desc: "IEC 61125 / ASTM D2440" },
			{ text: "Color index", desc: "ISO 2049 / ASTM D1500" },
			{ text: "Flash point", desc: "ISO 2719 / ASTM D93" },
			{ text: "Pour point", desc: "ISO 3016 / ASTM D97" },
			{ text: "PCB-content", desc: "IEC 61619 / ASTM D4059" },
			{ text: "Corrosive sulphur (copper strip)", desc: "IEC 62535 / ASTM D1275B" },
			{ text: "Corrosive sulphur (silver strip)", desc: "DIN 51353" },
			{ text: "DBDS content", desc: "IEC 62697-1 / ASTM" },
			{ text: "Inhibitor content", desc: "IEC 60666 / ASTM D4768" },
			{ text: "Metal passivator", desc: "IEC 60666" },
			{ text: "Particle count", desc: "IEC 60970 / ASTM D6786" },
			{ text: "Furan Analysis (with calculated DP)", desc: "IEC 61198 / ASTM D5837" },
			{ text: "Methanol, Ethanol (with calculated DP)", desc: "ASTM D 7843" },
			{ text: "Insulation paper DP and moisture content", desc: "IEC 60450 / ASTM D4243" },
			{ text: "Other oil quality tests:    ", desc: "" },
		];




		// Toggle selection for a given fruit by name
		$scope.toggleSelection = function toggleSelection(test) {
			var idx = $scope.sample.RequestedTests.indexOf(test);

			// Is currently selected
			if (idx > -1) {
				$scope.sample.RequestedTests.splice(idx, 1);
			}

			// Is newly selected
			else {
				$scope.sample.RequestedTests.push(test);
			}
		};

		$scope.defaultSample = {
			Language: "English",
			OilReportStandard: "IEC Standard",
			RequestedTests:[]
		};

		$scope.sample = angular.copy($scope.defaultSample);
		// #endregion

		// #region SAMPLE CRUD
		$scope.resetSample = function () {
			$scope.sample = angular.copy($scope.defaultSample);
			$scope.itemIndex = -1;
		};

		$scope.addSample = function () {
			$scope.sample.RequestedTests = $scope.sample.RequestedTests.join('/');
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
