var InUserInfo= {
    defultBind: function() {
        $("#fankui .gre2").bind("click", function () {
            if ($("#fankuiContent").val() == '')
            {
                Msg('请输入内容！', 2);
                return;
            }
            var openid = $("#fankui").attr("data-openId");         
            var userInput = $("#fankui textarea").val();
            InUserInfo.feedback(userInput, openid);
      
        });
    },
    feedback: function (value, openid) {
        var item= {value:value,openid:openid}
        $.post("/Ajax/UserFeedback", item, function (data) {
            if (data.Valid) {
                $("#fankui textarea").val("");
                Space.BackPage("fankui");
                Msg('您已成功提交意见反馈，我们会认真进行处理！', 1);
            } else {
                Msg(data.Msg,3);
            }

         
          // alert(2);
        });
    }
}