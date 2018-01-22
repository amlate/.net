if (typeof (DYSDETAIL) == "undefined") {
    var OrderList = new Object();    // 创建登录页面命名空间
}

var thisIndex = 0;
var LastIndex = 2;
var timeIndex = '';

var OrderList = {

	defaultClick:function(){
		bindClick($('section.order_list nav span'),function(){
		 
			if(!$(this).hasClass('sel')){
				$('section.order_list nav span').removeClass('sel');
				$(this).addClass('sel');
				var index = $(this).index();
				OrderList.TurnPage(index);
				index = index*100;
				transform($('section.order_list nav>i'),index+'%');
			}
		});

		bindClick($('section.order_list li'), function () {
           
		    var orderId = $(this).attr('data-id');		 
			OrderList.TurnPageOrder(orderId);
		});

		bindClick($('section.order_list header.reback span'),function(){
		    //pageTurn('/space');
		 
            history.go(-1);
		});

		bindClick($('section.order_info header.reback span'), function () {		
			OrderList.TurnPageList();
		});

     
		bindClick($('#order_cancer'), function () {

		   var confirmMsg= $('#order_cancer').attr('data-msg');
		   var cancer = confirm(confirmMsg+ "?");
			if(cancer == true){
			    var orderId = $(this).attr('data-value');
			    var openid = $(this).attr('data-openid');
			    var data = { orderId: orderId, openid: openid };
			    ajaxLocal('/ajax/CancelOrder', data, function (json) {
				    if (json.Valid) {
						Msg('订单取消成功');
						OrderList.TurnPageList();
				        //pageTurn('/order/orderlist/2');
                        location.reload();
				    } else {
				        console.log(json.Msg);
						Msg(json.Msg,2);
					}	
				});
			}
		});

		$('section.order_list nav span').eq(selIndex).addClass('sel');
		transform($('section.order_list nav>i'),selIndex*100+'%');
		if(selIndex==2){LastIndex=0;}
		OrderList.TurnPage(selIndex);
	},

	TurnPage:function(index){
		thisIndex = index;
		clearTimeout(timeIndex);
		$('section.order_list>div').eq(thisIndex).show();
		setTimeout(function(){
			flashLeftStart();
			flashLeftReset();
			transform($('section.order_list>div').eq(LastIndex),'-200%');
			transform($('section.order_list>div').eq(thisIndex),'-100%');
		},10);
		timeIndex = setTimeout(function(){
			$('section.order_list>div').eq(LastIndex).hide();
			transform($('section.order_list>div').eq(LastIndex),'0%');
			LastIndex = thisIndex;
			//alert(index);
		},320);
	},

	TurnPageOrder:function(orderId){
		$('section.order_info').show();
		setTimeout(function(){
			transform($('section.order_list'),'-100%');
			transform($('section.order_info'),'-100%');
		},10);
		setTimeout(function(){
			$('section.order_list').hide();
		},420);		
		var data = { orderId: orderId };
		ajaxLocal('/ajax/GetOrderInfo', data, function (json) {
		 
		    console.log(json);
		    if (json.Valid) {

		        //获取优惠券列表
		        var list = json.Other.voucherList;
		        var htm = '';
		        for (var i = 0; i < list.length; i++) {
		            // alert(list[i].Name);//访问每一个的属性

		            var style = '';
		            if (list[i].Amount >= 100) {
		                style = 'font-size:32px';
		            }

		            htm += '<li class="">';
		            htm += '<input type="hidden" id="coupon' + list[i].Id + '" value="' + list[i].Id + '"/>';
		            htm += ' <span class="l"></span>';
		            htm += '<span class="r"></span>';
		            htm += '<font style="float:left;">¥<b style="' + style + '">' + list[i].Amount + '元</b></font>';
		            htm += '<p class="f" style=" text-align:right;">' + list[i].Desc + '</p>';
		            htm += '<p style=" text-align:right;">品类:洗衣券</p>';
		            htm += '<p style=" text-align:right;">期限:' + list[i].EndTime + '</p>';
		            htm += '</li>';
		        }
		        $('#order_coupon_ul').html(htm);

		        //总金额
		        var totalFee = 0;
		        //优惠券金额
		        var couponJe = 0;
                //优惠券ID
		        var couponId='';
		        //点击选择优惠券列表
		        bindClick($('#order_coupon_ul li'), function () {
		            //主键ID，隐藏列
		            couponId = $(this).find('input').val();	
		            //优惠券金额
		            var amount = $(this).find('font b').html();
		            $('#order_info_assetFee').html(amount);

		          
		            var order_info_totalFee = $('#order_info_totalFee').html().replace('元', '');
		            if (order_info_totalFee != '') {
		                totalFee = parseInt(order_info_totalFee);
		            }
		            if (amount.replace('元', '') != '') {
		                couponJe = parseInt(amount.replace('元', ''));
		            }

		            //实际支付金额
		            $('#order_info_orderFee').html((totalFee - couponJe) + '元');

		            Order.BackPage('order_coupon');
		        });



		        if (json.Msg.OrderStatus == 0) {
		            $('#order_pay_btn').parent().show();
		            $('#order_kefu').parent().show();
		            $('#order_cancer').parent().show();
		            $('#order_cancer').html("取消订单");
		            $('#order_cancer').attr('data-msg',"取消订单");
		            $('#order_grade').parent().hide();
		            $('#order_ok').parent().hide();
		            bindClick($('#order_pay_btn'), function () { pageTurn('/frontpage/pay?orderNo=' + json.Msg.OrderNo + "&openid=" + json.Other.openid + "&voucherId=" + couponId+'&countyId='+store('countyId'));});
		        } else if (json.Msg.OrderStatus == 6) {
		            $('#order_pay_btn').parent().hide();
		            $('#order_kefu').parent().show();
		            $('#order_ok').parent().show();
		            $('#order_cancer').parent().hide();
		            $('#order_grade').parent().hide();
		            bindClick($('#order_ok'), function () {
		                ajaxLocal('/ajax/ConfirmOrder',data, function(data) {
		                    if (data.Valid) {
		                        Msg(data.Msg, 1);
		                        if (data.IsJump) {
		                            pageTurn('/frontpage/turntable?openid=' + data.openid + '&orderNo=' + orderId + '&countyId=' + data.countyId);
		                        } else {
		                            pageTurn('/frontpage/orderlist?type=2&openid=' + data.openid);
		                        }		                        
		                    } else {
		                        Msg(data.Msg, 2);
		                    }
		                });
		            });
		        } else if (json.Msg.OrderStatus == 7 || json.Msg.OrderStatus == 8 || json.Msg.OrderStatus == 11) {
		            $('#order_pay_btn').parent().hide();
		            $('#order_kefu').parent().show();
		            $('#order_cancer').parent().hide();
		            $('#order_ok').parent().hide();
		            $('#order_grade').parent().show();
		            bindClick($('#order_grade'), function() { pageTurn('/frontpage/ordergrade?orderNo=' + json.Msg.OrderNo + "&openid=" + json.Other.openid); });
		        } else if (json.Msg.OrderStatus == 10 || json.Msg.OrderStatus == 9) {
		            $('#order_pay_btn').parent().hide();
		            $('#order_kefu').parent().show();
		            $('#order_cancer').parent().hide();
		            $('#order_ok').parent().hide();
		            $('#order_grade').parent().hide();
		        } else {
		            $('#order_pay_btn').parent().hide();
		            $('#order_kefu').parent().show();
		            $('#order_ok').parent().hide();
		           // $('#order_cancer').html("退款");
		           // $('#order_cancer').attr('data-msg', "退款");
		             $('#order_cancer').parent().hide();
		            $('#order_grade').parent().hide();
		        }

		        $('#order_cancer').attr('data-value', json.Msg.OrderNo);
		        $('#order_cancer').attr('data-openid', json.Other.openid);
				$('#order_info_date').html(json.Other.RiseTime);
				$('#order_info_id').html(json.Msg.OrderNo);
				$('#order_info_num').html(json.Other.itemTotal + "件");
			    $('#order_info_assetFee').html('选择优惠券');
				$('#order_info_deliveryFee').html(json.Msg.Freight + "元");
				$('#order_info_totalFee').html(json.Msg.TotalPrice + "元");

				$('#order_info_orderFee').html(json.Msg.Fact + "元");
				$('#order_info_address').html('<p>' + json.Other.Contact + '</p><p>' + json.Other.UserAddress + '</p>');
		        //$('#order_info_statusMsg').html(json.Msg.statusMsg);
				$('#order_info_statusMsg').html(json.Other.dateMsg);
				
				$('#order_wuliu_info').html(json.Other.wuliu);
				$('section.order_info div.order ul li').removeClass();
				$('section.order_info div.order ul li s i').removeClass('g');
				var state = json.Msg.OrderStatus;
				if (state == "0") {
				    state = 0;
				} else if (state == "1" || state == "2") {
				    state = 1;
				} else if (state == "4"|| state == "3") {
				    state = 2;
				} else if (state == "5") {
				    state = 3;
				} else {
				    state = 4;
				}
				$('section.order_info div.order ul li').eq(parseInt(state)).addClass('doing');
				$('section.order_info div.order ul li.doing').nextAll().addClass('next');
				$('section.order_info div.order ul li.doing').nextAll().find('s i').addClass('g');
				//alert(json.Other.xiaciHtml);
				$('#xiaci_table').html(json.Other.xiaciHtml);
				$(".xiaci").html(json.Other.img);
                	
             
		    } else {
		     
				//Msg(json.msg,2);
			}
		});
	},

	TurnPageList:function(){
		$('section.order_list').show();
		setTimeout(function(){
			transform($('section.order_list'),'0');
			transform($('section.order_info'),'0');
		},10);
		setTimeout(function(){
			$('section.order_info').hide();
		},420);
	},
		
	/**
	* @ 页面初始化
	*/		
	init_page:function(type){
		OrderList.defaultClick();
	}
}