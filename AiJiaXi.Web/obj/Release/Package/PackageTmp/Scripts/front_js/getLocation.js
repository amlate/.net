        //转换回调
        translateCallback = function (data) {
            if (data.status === 0) {
                var pointFromData = data.points[0];
                var geoc = new BMap.Geocoder();
                geoc.getLocation(pointFromData, function (rs) {
                    var addComp = rs.addressComponents;
                    // alert(addComp.district);

                   // $("#countyName").html(addComp.district);
                    //$("#cityName").html(addComp.city);

                    //定位后请求INDEX页进行刷新
                    // pageTurn('/baixime/Index/' + addComp.city + '/' + addComp.district + '/2/');

              
                    //定位请求当前位置

                    //ajaxData['cityName'] = addComp.city;//城市名
                    //ajaxData['countyName'] = addComp.district;//区域名
                    //var data = putAjaxData(ajaxData);


               
                    //ajaxLocal('/Other/GetCity', data, function (json) {   
                    ajaxLocal('/Other/GetCity?cityName=' + addComp.city + '&countyName=' + addComp.district, null, function (json) {
                        //城市ID
                        store('PcityId', json.cityId);
                        //区域ID
                        store('cityId', json.countyId);
                        //城市名称
                        store('PcityName', json.cityName);
                        //区域名称
                        store('cityName', json.countyName);
                         
                         $("#countyName").html(addComp.district);
                        $("#cityName").html(addComp.city);

                    });

                   // alert(addComp.province + ", " + addComp.city + ", " + addComp.district + ", " + addComp.street + ", " + addComp.streetNumber);
                });
            }
        }

        function getLocation() {
            if (store('cityId') == 0) {//为0代表没有选择区域                           // alert(1);                          }            else {  //如果已经有区域则不进行继续定位               // alert(2);                return;            }

            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(showPosition, showError);
            } 
        }

        function showPosition(position) {
            var lat = position.coords.latitude;
            var lon = position.coords.longitude;
            var point = new BMap.Point(lon, lat);
            var convertor = new BMap.Convertor();
            var pointArr = [];
            pointArr.push(point);
            convertor.translate(pointArr, 1, 5, translateCallback);
        }

        function showError(error) {
            switch (error.code) {
                case error.PERMISSION_DENIED:
                    alert("User denied the request for Geolocation");
                    break;
                case error.POSITION_UNAVAILABLE:
                    alert("Location information is unavailable.");
                    break;
                case error.TIMEOUT:
                    alert("The request to get user location timed out.");
                    break;
                case error.UNKNOWN_ERROR:
                    alert("An unknown error occurred.");
                    break;
            }
        }
        getLocation();