if (typeof (DYSDETAIL) == "undefined") {
    var Award = new Object();    // 创建登录页面命名空间
}
style = '';

var Award = {

    defaultClick: function () {


        bindClick($('header span.back'), function () {          
            history.go(-1);
        });
        bindClick($('div .btn.grey'), function () {
           var openid= $('div .btn.grey').attr("data-openid");
           var count = $('div .btn.grey').attr("data-count");
            if (count>0) {
                location.href("/FrontPage/Turntable?openid=" + openid);
            }
            
        });

    },

    /**
	* @ 页面初始化
	*/
    init_page: function (type) {
        Award.defaultClick();
    }
}