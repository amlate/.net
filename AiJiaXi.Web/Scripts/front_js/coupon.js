if (typeof (DYSDETAIL) == "undefined") {
    var Coupon = new Object();    // 创建登录页面命名空间
}

style = '';

var Coupon = {

	defaultClick:function(){
		
		$('#assetChangeBox div').css('background-color','#0269b6');

		/*$('#assetChangeBox input').on("input",(function(){
			var code = $(this).val();
			if(code.length>5){
				$('#assetChangeBox div').css('background-color','#2ba870');
			}else{
				$('#assetChangeBox div').attr('style','');
			}
		}));*/

		bindClick($('section.coupon span.back'),function(){
		    //pageTurn('/space');
            history.go(-1);
		});

		bindClick($('#assetChangeBox div'),function(){
			if($(this).attr("style")){ //按钮变绿
			    if (!$('#assetChangeBox input').val()) { Msg('请输入洗衣券', 2); return; }
			    var openid= $('#assetChangeBox').attr("data-openid");
				var code = $('#assetChangeBox input').val();
				var data = { openid: openid, code: code };
				ajaxLocal('/ajax/ExchangeVoucher', data, function (json) {
				    if (json.Valid) {                     
						
				        var html = '';

					    if (json.amount >= 100) {
						    style = 'font-size:32px';
						}

						html += '<li class="">';					
						html += ' <span class="l"></span>';
						html += '<span class="r"></span>';
						html += '<font style="float:left;">¥<b style="' + style + '">' + json.amount + '元</b></font>';
						html += '<p class="f" style=" text-align:right;">' + json.Desc + '</p>';
						html += '<p style=" text-align:right;">使用范围:' + json.Range + '</p>';
						html += '<p style=" text-align:right;">期限:' + json.EndTime + '</p>';
						html += '</li>';





						$('section.coupon ul').append(html);
						//Msg('兑换成功');
						Msg(json.Msg, 1);
						setTimeout(function () { $('section.coupon ul li').removeClass('none'); }, 10);
						$('#assetChangeBox input').val('');
					}else{
					    Msg(json.Msg, 2);
					}
				});
			}
		});
		
		bindClick($('li'),function(){
			$(this).parent().find('b').removeClass('sel');
			$(this).find('b').addClass('sel');
		});

	},
		
	/**
	* @ 页面初始化
	*/		
	init_page:function(type){
		Coupon.defaultClick();
	}
}