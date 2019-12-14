
$(document).ready(function () {
    $("#SelectGender").change(function () {
        window.location.href = $(this).val();
    });

    loadImg();

    $("[relationtype]").bind("click", function () {
        showPicEditeRegionDiv(this);
    }); 
});
var opratorobjPic = ""; //操作块对象
//单击弹出层编辑
function showPicEditeRegionDiv(obj) {
    opratorobjPic = $(obj);
    $("input:file[name='imgfile']").val("");
    $("img[title='yulan']").attr("src", "");
    $("input:hidden[name='RelationTypeID']").val($(obj).attr("RelationType"));
    $("input:hidden[name='PagePositionID']").val($(obj).attr("opratorRegion"));
    $("input:hidden[name='savePicSize']").val($(obj).attr("fontcolordata")); 
    $("input:[name='PictureName']").val("");
    $("input:[name='LinkAddress']").val("http://");
    $(".dispalayRegion").hide();

    $("#spanColorFont").text($(obj).attr("fontcolordata"));
    $("body").append('<div id="windownBG" style="height:' + $(document).height() + 'px;opacity:0.5;z-index: 9999;display:block;left:0px;top:0px;position:absolute;width:100%;background: none repeat scroll 0 0 #000000;"></div>');
    $("#type" + $(obj).attr("relationtype")).css({ 'top': ($(window).height() / 2) - 200 + $(window).scrollTop(), 'left': ($(window).width() / 2) - 410, 'display': 'block', 'z-index': '99999' });

    if ($(obj).attr("relationtype") == "3") {
        GetPicRelationLink();
        return;
    }
}


//获取关联链接
function GetPicRelationLink() {
    $.ajax({
        type: "POST",
        url: "/ShangPin/HomePage/GetPictureManagerJson",
        data: "RelationTypeID=" + $("input[name='RelationTypeID']").val() + "&PagePositionID=" + $("input:hidden[name='PagePositionID']").val() + "&RelationGender=" + $("input:hidden[name='RelationGender']").val(),
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("img[title='yulan']").attr("src", "/ReadPic/GetPic.ashx?width=100&height=100&pictureFileNo=" + data.PictureFileNo + "&type=2");
                $("input:hidden[name='ImgNO']").val(data.PictureFileNo);
                $("input:hidden[name='PagePositionID']").val(data.PagePosition);
                $("#type3form input:[name='PictureName']").val(data.PictureName);
                if (data.LinkAddress != "" || data.LinkAddress != '') {
                    $("input:text[name='LinkAddress']").val(data.LinkAddress);
                } else { $("input:text[name='LinkAddress']").val("http://"); }

            } else { }

        }
    });
}

function loadImg() {

    $("img[lazy]").each(function () {
        $(this).attr("src", $(this).attr("lazy")).removeAttr("lazy");
    });
}
//保存区块关联并且替换模板
function SaveSWfsPictureManagerContent(formobj) {
    if (formobj == "type3form") {
        if ($("#" + formobj + " input[name='PictureName']").val().length > 0) {
            var msgStr = $("#" + formobj + " input[name='PictureName']").val();
            if (!CountCharStr(msgStr, 30)) 
            {
                alert("图片名称为15汉字或者30字符");
                return false;
            }
        }  
    }
    //异步提交表单
    $("#" + formobj).ajaxSubmit(function (data) {
        data = eval("(" + data + ")");
        if (data.status == 0) {
            alert(data.message); 
            return;
        }
        opratorobjPic.attr("relationtype", data.relationtype);
        if (data.relationtype == "3") {            
            opratorobjPic.empty();
            opratorobjPic.append('<a href="#"><img src="' + data.message.ExpandPicFile + '"/></a>');
        }
        $('#windownBG').remove();
        $(".dispalayRegion").hide();
        //重新绑定事件
        $("#loadTemplate a").click(function (event) {
            event.preventDefault();
        });

    });
} 
 
 

//判断字符个数是否超出限制
function CountCharStr(msg, len) { 
    var nums = msg.replace(/[^\x00-\xff]/g, "00").length;
    if (nums > len) {
        return false;
    }
    return true;
}
 