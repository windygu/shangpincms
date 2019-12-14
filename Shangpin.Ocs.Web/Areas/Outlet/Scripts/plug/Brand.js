// 品牌调用js ,品牌按组显示



//    //页面品牌控件调用唯一需要添加的js文件
//	$(function(){	  
//	    //$("#BrandName")表示点击后要显示品牌的控件
//		//selectBrand参数分别代表接受品牌ID的控件ID，接受品牌名称的控件ID
//true:预加载  false:不预先加载
//		$("#BrandName").selectBrand('BrandNo','BrandName',true);	
//	});		


jQuery.fn.selectBrand = function (objID, objName, IsLoad, SubformID, functionName) {
    var _seft = this;
    var url = "/outlet/subject/AjaxBrand?IDControl=" + objID + "&nameControl=" + objName + "&SubformID=" + SubformID + "&functionName=" + functionName;
    var targetWidth = 320;  //框架宽度
    var targetHeight = 250;
    var targetId = $("<iframe id='BrandIframe'  marginheight='0' marginwidth='0' scrolling='auto' frameborder='0'  style='border:3px solid #8492A9;display:none;width:" + targetWidth + "px; height :" + targetHeight + "px;background-color:#FFFFFF'></iframe>");
    if (IsLoad) {
        targetId.attr("src", url);
        $('body').append(targetId);
    }
    this.click(function (e) {

        var A_top = $(this).offset().top + $(this).outerHeight(true);
        var A_left = $(this).offset().left;
        //左右定位
        if ($(this).offset().left > $(document.body).width() - targetWidth) {
            A_left = $(this).offset().left - targetWidth + $(this).width();
        }

        if (targetId.attr("src") == "" || typeof (targetId.attr("src")) == "undefined") {
            targetId.attr("src", url);
            $('body').append(targetId);
            targetId.show().css({ "position": "absolute", "top": A_top + "px", "left": A_left + "px" });
        }
        else {
            targetId.show().css({ "position": "absolute", "top": A_top + "px", "left": A_left + "px" });
        }
    });

    this.keyup(function () {
        targetId.attr("src", url + "#" + this.value.substr(this.value.length - 1, 1));
    });

    $(document).click(function (event) {
        if (event.target.id != _seft.selector.substring(1)) {
            targetId.hide();
        }
    });
    return this;
}


 			
function Submit(varStyle) {
    if (varStyle = '4') {
        var picName = $('#PicName').val();
        if (picName == '') {
            $("#formAddBackMessage_result_msg").text("请填写图片名称");
            return false;
        }
    }
    if (varStyle == '4'|| varStyle == '5'|| varStyle == '6'|| varStyle == '10') {
        var picName = $('#PicName').val();
        if (picName == '') {
            $("#formAddOutSiteMessage_result_msg").text("请填写图片名称");
            return false;
        }
        var picFile = $('#PicFile').val();
        if (picFile == '') {
            $("#formAddOutSiteMessage_result_msg").text("请选择图片");
            return false;
        }
    }
    if (varStyle = '7') {
        var picName = $('#PicName').val();
        if (picName == '') {
            $("#formAddVideoMessage_result_msg").text("请填写视频名称");
            return false;
        }
        var picFile = $('#PicFile').val();
        if (picFile == '') {
            $("#formAddVideoMessage_result_msg").text("请选择图片");
            return false;
        }
        var videoAddress = $('#VideoAddress').val();
        if (videoAddress == '') {
            $("#formAddVideoMessage_result_msg").text("请填写链接地址");
            return false;
        }
        var videoDesc = $('#VideoDesc').val();
        if (videoDesc == '') {
            $("#formAddVideoMessage_result_msg").text("请填写视频描述");
            return false;
        }
    }
    if (varStyle = '8') {
        var picName = $('#PicName').val();
        if (picName == '') {
            $("#formAddBigMessage_result_msg").text("请填写大图图片名称");
            return false;
        }
        var picFile = $('#PicFile').val();
        if (picFile == '') {
            $("#formAddBigMessage_result_msg").text("请选择大图图片");
            return false;
        }
        var pictureDesc = $('#PictureDesc').val();
        if (pictureDesc == '') {
            $("#formAddBigMessage_result_msg").text("请填写图片描述");
            return false;
        }
    }
    if (varStyle = '9') {
        var picName = $('#PicName').val();
        if (picName == '') {
            $("#formAddSmallMessage_result_msg").text("请填写小图图片名称");
            return false;
        }
        var picFile = $('#PicFile').val();
        if (picFile == '') {
            $("#formAddSmallMessage_result_msg").text("请选择小图图片");
            return false;
        }
        var pictureDesc = $('#PictureDesc').val();
        if (pictureDesc == '') {
            $("#formAddSmallMessage_result_msg").text("请填写图片描述");
            return false;
        }
    }
    if (varStyle = '10') {
        var pictureDesc = $('#PictureDesc').val();
        if (pictureDesc == '') {
            $("#formAddProIntroMessage_result_msg").text("请填写图片描述");
            return false;
        }
    }
    $('#formUpdateBrandMessage').submit();

}
