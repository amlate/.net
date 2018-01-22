if (typeof (DYSDETAIL) == "undefined") {
    var Order = new Object();    // 创建登录页面命名空间
}

var Order = {

	defaultClick:function(){
		bindClick($('header.reback span.back'),function(){
		    //pageTurn('/order/orderlist/0');
		    history.go(-1);
		});

		bindClick($('ul.pay li'),function(){
			$('ul.pay li').find('i.pay').removeClass('sel');
			$(this).find('i.pay').addClass('sel');
			$('#payType').val($(this).find('i').attr('data-type'));
		});

		bindClick($('#submit'), function () {
		    var countyId = store('countyId');
		    var openid = $('#orderId').attr("data-openid");
			var payType = $('#payType').val();
			var orderId = $('#orderId').val();
			var data = { type: payType, orderId: orderId, countyId: countyId };
			if(payType=='other'){//线下
			    ajaxLocal('/ajax/UpdataOrderState', data, function (json) {
			        if (json.Valid) {
			            Msg('支付成功');
			            pageTurn('/FrontPage/orderlist?type=1&openid=' + json.openid);
			        } else {
			            Msg(json.Msg, 2);
			        }
				});
			}else if(payType=='balance'){//余额
			    //var data = { type: payType, orderId: orderId };
			    ajaxLocal('/ajax/UpdataOrderState', data, function (json) {
			        if (json.Valid) {
						Msg('支付成功');
						pageTurn('/FrontPage/orderlist?type=1&openid=' + json.openid);
					}else{
						Msg(json.Msg,2);
					}
				});
			} else if (payType == 'wechat') {//微信			    
			    var form = $("<form></form>");
			    var openidi = $('<input type="text" name="openid" />');
			    openidi.attr('value', openid);
			    var countyIdi = $('<input type="text" name="countyId" />');
			    countyIdi.attr('value', countyId);
			    var bodyi = $('<input type="text" name="body" />');
			    bodyi.attr('value', "白洗么订单支付");
			    var totalFeei = $('<input type="text" name="totalFee" />');
			    totalFeei.attr('value', 0);
			    var orderSn = $('<input type="text" name="orderSn" />');
			    orderSn.attr('value', orderId);
			    form.attr('action', "/FrontPage/PayMoney/");
			    form.attr('method', 'post');
			    form.append(openidi);
			    form.append(orderSn);
			    form.append(bodyi);
			    form.append(countyIdi);
			    form.append(totalFeei);
			    form.submit();
			}
		});
	},

	wechatPay:function(data){
		var json = {
			appid: data.appid,
			noncestr: data.nonceStr,
			package: data.package,
			partnerid: data.partnerId,
			prepayid: data.prepayId,
			timestamp: data.timeStamp,
			sign: data.sign
		};
		//console.log(json);
		wxpay.payment(json, cb_success, cb_failure);
	},
	
	init_page:function(type){
		Order.defaultClick();
	}
}

function cb_success(data){
	console.log('success');
}

function cb_failure(data){
	console.log('fail');
}