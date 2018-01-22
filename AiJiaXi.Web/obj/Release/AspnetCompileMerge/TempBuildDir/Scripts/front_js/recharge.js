if (typeof (DYSDETAIL) == "undefined") {
    var Recharge = new Object();    // 创建登录页面命名空间
}

var Recharge = {

	defaultClick:function(){
		
		$('section.recharge div.selMoney').click(function(){
			$('section.recharge_choose').show();
			setTimeout(function(){
				transform($('section.recharge'),'-100%');
				transform($('section.recharge_choose'),'-100%');
			},10);
		});

		$('section.recharge_choose header.reback span').click(function(){
			transform($('section.recharge'),'0%');
			transform($('section.recharge_choose'),'0%');
			setTimeout(function(){
				$('section.recharge_choose').hide();
			},410);
		});

		$('section.recharge header.reback span').click(function(){
		    //pageTurn('/space');
		    history.go(-1);
		});

		$('section.recharge_choose footer').click(function(){
			$('section.recharge_pay').show();
			setTimeout(function(){
				transform($('section.recharge_choose'),'-200%');
				transform($('section.recharge_pay'),'-100%');
			},10);
			setTimeout(function(){
				$('section.recharge_choose').hide();
			},410);
		});

		$('section.recharge_pay header.reback span').click(function(){
			$('section.recharge_choose').show();
			setTimeout(function(){
				transform($('section.recharge_choose'),'-100%');
				transform($('section.recharge_pay'),'0%');
			},10);
			setTimeout(function(){
				$('section.recharge_pay').hide();
			},410);
		});

		$('section.recharge_choose ul li').click(function(){
			$('section.recharge_choose ul li b').removeClass('sel');
			$(this).find('b').addClass('sel');
			var payMoney = $(this).attr('data-num');
			$('#chargeNum').val(payMoney);
			$('#chargeNumShow').html(payMoney+'元');
		});

		$('#assetChangeBox input').keyup(function(){
			var v = $(this).val();
			if(v.length>3){
			    $('#assetChangeBox div.btn').css('background-color', '#0269b6');
			}else{
				$('#assetChangeBox div.btn').attr('style','');
			}
		});

		$('#assetChangeBox div.btn').click(function(){
			if($(this).attr('style')){
				var v = $('#assetChangeBox input').val();
				ajaxData['cardId'] = v;
				var data = putAjaxData(ajaxData);
				ajaxLocal('/ajax/useRechargeCard',data,function(json){
					if(json.result=='Success'){
						Recharge.CodeUsed();
					}else{
						Msg(json.msg,2);
					}
				});
			}
		});

		//$('#submit').click(function(){
		//    var openid = $('#openid').val();
		//    var chargeNum = parseInt($('#chargeNum').val());
		//    var data = { totalFee: chargeNum, body: "充值", openid: openid };
		//    ajaxLocal('/ajax/GoWeixinPay', data, function (data) {
		//        console.log(data);
		//        if (data.isSuccess) {
		//            wx.config({
		//                debug: true, // 开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印。
		//                appId: data.other.appId, // 必填，公众号的唯一标识
		//                timestamp: data.other.timeStamp, // 必填，生成签名的时间戳
		//                nonceStr: data.other.nonceStr, // 必填，生成签名的随机串
		//                signature: data.other.paySign,// 必填，签名，见附录1
		//                jsApiList: data.other.jsApiList // 必填，需要使用的JS接口列表，所有JS接口列表见附录2
		//            });
		//            wx.ready(function () {
		//                wx.chooseWXPay({
		//                    timestamp: data.other.timeStamp, // 支付签名时间戳，注意微信jssdk中的所有使用timestamp字段均为小写。但最新版的支付后台生成签名使用的timeStamp字段名需大写其中的S字符
		//                    nonceStr: data.other.nonceStr, // 支付签名随机串，不长于 32 位
		//                    package: data.other.package, // 统一支付接口返回的prepay_id参数值，提交格式如：prepay_id=***）
		//                    signType: data.other.signType, // 签名方式，默认为'SHA1'，使用新版支付需传入'MD5'
		//                    paySign: data.other.paySign, // 支付签名
		//                    success: function (res) {
		//                        // 支付成功后的回调函数

		//                    },
		//                    fail: function (res) {
		//                        alert(JSON.stringify(res));
		//                    }
		//                });
		//            });
        //             var userid = $('#userid').val();
        //             var item = { userid: userid, orderid: data.other.orderNumber, tradeMoney: chargeNum };
		//                $.post("/ajax/RechargeRecord", item, function (data) {
		//                    console.log(data);
		//                    if (!data.Valid) {
		//                        Msg("服务器异常，请放弃交易，如已付款请联系客服！", 2);
		//                    }
		//                });
		//			}else{
		//            Msg(data.msg, 2);
		//			}
		//		});			
		//});


		$('#submit').click(function () {
		    var countyId = store('countyId');
		    var openid = $('#openid').val();
		    var chargeNum =$('#chargeNum').val();
		    var form = $("<form></form>");
		    var countyIdi = $('<input type="text" name="countyId" />');
		    countyIdi.attr('value', countyId);
		    var openidi = $('<input type="text" name="openid" />');
		    openidi.attr('value', openid);
		    var bodyi = $('<input type="text" name="body" />');
		    bodyi.attr('value',"白洗么充值");
		    var totalFeei = $('<input type="text" name="totalFee" />');
		    totalFeei.attr('value', chargeNum);
		    form.attr('action',"/FrontPage/PayMoney/");
		    form.attr('method', 'post');
		    form.append(openidi);
		    form.append(bodyi);
		    form.append(totalFeei);
		    form.append(countyIdi);
		    form.submit();
		});




		$('#xieyi').click(function(){
			$('section.recharge_xieyi').show();
			setTimeout(function(){
				transform($('section.recharge_xieyi'),'-100%');
				transform($('section.recharge_choose'),'-200%');
			},10);
			setTimeout(function(){
				$('section.recharge_choose').hide();
			},410);
		});

		$('section.recharge_xieyi header.reback span').click(function(){
			$('section.recharge_choose').show();
			setTimeout(function(){
				transform($('section.recharge_choose'),'-100%');
				transform($('section.recharge_xieyi'),'0%');
			},10);
			setTimeout(function(){
				$('section.recharge_xieyi').hide();
			},410);
		});

	},

	CodeUsed:function(){
		$('section.recharge_choose div.change').css('height',120);
		$('section.recharge_choose div.change div.msg').css('height',120);
		$('section.recharge_choose div.change div.msg').css('top',0);
		$('section.recharge_choose span.cz_msg').html('验证通过可选择充值金额');
		$('section.recharge_choose ul.money li span').show();
	},
		
	/**
	* @ 页面初始化
	*/		
	init_page:function(type){
		Recharge.defaultClick();
	}
}