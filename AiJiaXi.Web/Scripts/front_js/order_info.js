if (typeof (DYSDETAIL) == "undefined") {
    var Order = new Object();    // 创建登录页面命名空间
}

var pageH = 0;

var Order = {

    defaultClick: function () {
		$('section.order_info div.order ul li span').css('left',(170-$(window).width())/10);
		$('section.order_info div.order ul li.doing').nextAll().addClass('next');
		$('section.order_info div.order ul li.doing').nextAll().find('s i').addClass('g');
		$(window).resize(function(){
			$('section.order_info div.order ul li span').css('left',(170-$(window).width())/10);
		});

		$('section.order_details div.imgArea').css('width',$(window).width()-20);

		$('section.order_details table tr').each(function(){
			$(this).find('td').eq(0).addClass('gre');
			if($(this).index()%2==1){$(this).addClass('grey');}
		});

        //订单详情页
		bindClick($('#details'), function () {
			//if(!$(this).find('span').attr('style')){Order.TurnPage('order_details');}
		});
	    //绑定订单详情页，选择优惠券
		bindClick($('#coupon'), function () {
		    if (!$(this).find('span').attr('style')) { Order.TurnPage('order_coupon'); }
		});
        //优惠券返回事件
		bindClick($('section.order_coupon header.reback span.back'), function () {		  
		    Order.BackPage('order_coupon');
		});


		bindClick($('section.order_details header.reback span.back'), function () {

			Order.BackPage('order_details');
		});

		bindClick($('section.order_details div.xiaci img'),function(){
			$('#imgShow').attr('src',$(this).attr('src')); 
			$('section.order_details div.imgWarp').show();
			setTimeout(function(){
				$('#imgShow').addClass('show');
			},10);

			setTimeout(function(){
				$('#imgShow').addClass('bot');
			},310);

			setTimeout(function(){
				$('#imgClose').addClass('show');
			},460);
		});
		
		bindClick($('#imgClose'),function(){
			$('section.order_details div.imgWarp').hide();
			$('#imgClose').removeClass('show');
			$('#imgShow').removeClass('show');
			$('#imgShow').removeClass('bot');
		});

		bindClick($('#imgShow'),function(){
			$('section.order_details div.imgWarp').hide();
			$('#imgClose').removeClass('show');
			$('#imgShow').removeClass('show');
			$('#imgShow').removeClass('bot');
		});
	},

	TurnPage:function(type){
		$('section.'+type).show();
		setTimeout(function(){
			transform($('section.'+type),'-100%');
			transform($('section.order_info'),'-200%');
		},10);
		setTimeout(function(){
			$('section.order_info').hide();
		},420);
	},

	BackPage:function(type){
		$('section.order_info').show();
		setTimeout(function(){
			transform($('section.'+type),'0');
			transform($('section.order_info'),'-100%');
		},10);
		setTimeout(function(){
			$('section.'+type).hide();
		},420);
	},
		
	/**
	* @ 页面初始化
	*/		
	init_page:function(type){
		Order.defaultClick();
	}
}