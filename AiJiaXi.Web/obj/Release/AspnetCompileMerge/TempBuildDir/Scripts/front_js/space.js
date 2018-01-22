if (typeof (DYSDETAIL) == "undefined") {
    var Space = new Object();    // 创建登录页面命名空间
}

var pageH = 0;
var Space = {

	defaultClick:function(){
		pageH = $('section').height();
		if(pageH<530){
			$('footer').css('position','relative');
			$('footer').css('padding-top',4);
			$('footer div.btn').css('margin-top',0);
		}

		bindClick($('#space_more'),function(){
			Space.TurnMore('space_more');
		});

		bindClick($('section.space span.back'),function(){
			//pageTurn('/');
		});

		bindClick($('section.space_more span.back'),function(){
			Space.BackMore('space_more');
		});

		bindClick($('section.pageChild span.back'),function(){
			var page = $(this).attr('page-url');
			Space.BackPage(page);
			$("#fankui textarea").val("");
		});

		bindClick($('section.space_more ul.menu li'),function(){
			var page = $(this).attr('page-url');
			Space.TurnPage(page);
		});

	},

	BackPage:function(page){
		$('section.space_more').show();
		setTimeout(function(){
			transform($('section.space_more'),'-100%');
			transform($('#'+page),'0');
			flashReset();
		},10);
		setTimeout(function(){
			$('#'+page).hide();
		},420);
	},

	TurnPage:function(page){
		$('#'+page).show();
		setTimeout(function(){
			transform($('section.space_more'),'-200%');
			transform($('#'+page),'-100%');
			flashStart();
		},10);
		setTimeout(function(){
			$('section.space_more').hide();
		},420);
	},

	TurnMore:function(){
		$('section.space_more').show();
		setTimeout(function(){
			transform($('section.space_more'),'-100%');
			transform($('section.space'),'-100%');
		},10);
		setTimeout(function(){
			$('section.space').hide();
		},420);
	},

	BackMore:function(){
		$('section.space').show();
		setTimeout(function(){
			transform($('section.space_more'),'0');
			transform($('section.space'),'0');
		},10);
		setTimeout(function(){
			$('section.space_more').hide();
		},420);
	},
		
	/**
	* @ 页面初始化
	*/		
	init_page:function(type){
		Space.defaultClick();
	}
}