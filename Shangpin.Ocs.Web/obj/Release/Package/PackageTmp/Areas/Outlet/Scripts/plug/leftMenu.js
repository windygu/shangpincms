$(function () {

    /*leftMenu handel*/
    $("#mx-leftmenu li").on('click', function () {
        var that = $(this),
			childLen = that.find('p').length;

        that.addClass('cur').siblings('li').removeClass('cur');
        $("#mx-leftmenu li").find('p').hide();

        if (childLen > 0) {

            that.find('p').show();

            //return false;
        }

    });

});