$(function () {
    var myLayout = $('body').layout({
        west__size: 200
        // RESIZE Accordion widget when panes resize
      , west__onresize: $.layout.callbacks.resizePaneAccordions
    });
    $('#side-menu').metisMenu();

});

$(function () {
    $(window).bind("load resize", function () {
        topOffset = 50;
        width = (this.window.innerWidth > 0) ? this.window.innerWidth : this.screen.width;
        if (width < 768) {
            $('div.navbar-collapse').addClass('collapse');
            topOffset = 100; // 2-row-menu
        } else {
            $('div.navbar-collapse').removeClass('collapse');
        }

        height = ((this.window.innerHeight > 0) ? this.window.innerHeight : this.screen.height) - 1;
        height = height - topOffset;
        if (height < 1) height = 1;
        if (height > topOffset) {
            $("#page-wrapper").css("min-height", (height) + "px");
        }
    });

    var url = window.location;
    var element = $('#side-menu li a').filter(function() {
        if (this.href == "") {
            return false;
        }
        return this.href == url;
    }).addClass('active').parent().parent().addClass('in').parent().addClass('active');
});

$(function () {
    $(".multi_select")
        .multiselect()
        .end();
});
