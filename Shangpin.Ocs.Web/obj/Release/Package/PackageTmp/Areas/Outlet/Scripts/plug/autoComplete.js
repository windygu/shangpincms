//add by zhangzhegnwen 
// 品牌调用js ,品牌按组显示

//页面品牌控件调用唯一需要添加的js文件
//	$(function(){	  
//	    //$("#BrandName")表示点击后要显示品牌的控件

//		//BrandNo接受品牌ID的控件ID，BrandName接受品牌名称的控件ID
//true:预加载  false:不预先加载

//SubformID,选择后直接提交的form，没有时为空
//functionName,选择后如不直接提交，调用脚本提交的函数名,没有时为空

//		$("#BrandName").selectBrand('BrandNo','BrandName',true,SubformID,functionName);	
//	});		

String.prototype.endWith = function (s) {
    if (s == null || s == "" || this.length == 0 || s.length > this.length)
        return false;
    if (this.substring(this.length - s.length) == s)
        return true;
    else
        return false;
    return true;
}

String.prototype.startWith = function (s) {
    if (s == null || s == "" || this.length == 0 || s.length > this.length)
        return false;
    if (this.substr(0, s.length) == s)
        return true;
    else
        return false;
    return true;
}
function contains(string, substr, isIgnoreCase) {
    if (isIgnoreCase) {
        string = string.toLowerCase();
        substr = substr.toLowerCase();
    }
    var startChar = substr.substring(0, 1);
    var strLen = substr.length;
    for (var j = 0; j < string.length - strLen + 1; j++) {
        if (string.charAt(j) == startChar)//如果匹配起始字符,开始查找
        {
            if (string.substring(j, j + strLen) == substr)//如果从j开始的字符与str匹配，那ok
            {
                return true;
            }
        }
    }
    return false;
}

jQuery.fn.selectBrandNew = function (objID, objName, IsLoad, SubformID, functionName) {
    var _seft = this;
    var url = "/outlet/subject/AjaxBrandNew?IDControl=" + objID + "&nameControl=" + objName + "&SubformID=" + SubformID + "&functionName=" + functionName + "&selectType=1";
    var targetWidth = 320;  //宽度
    var targetHeight = 250;  //高度
    var targetId = $("<iframe id='QueryBrandIframe'  marginheight='0' marginwidth='0' scrolling='auto' frameborder='0'  style='border:3px solid #8492A9;display:none;width:" + targetWidth + "px; height :" + targetHeight + "px;background-color:#EDEDED;z-index:999'></iframe>");
    if (IsLoad) {
        targetId.attr("src", url);
        $('#AjaxBrandIframe').append(targetId);
    }
    this.click(function (e) {

        var A_top = $(this).offset().top + $(this).outerHeight(true);
        var A_left = $(this).offset().left;
        //左右定位
        if ($(this).offset().left > $(document.body).width() - targetWidth) {
            A_left = $(this).offset().left - targetWidth + $(this).width();
        }
        if (!($("#QueryBrandIframe").length > 0)) {
            if (targetId.attr("src") == "" || typeof (targetId.attr("src")) == "undefined") {
                targetId.attr("src", url);
                $('body').append(targetId);
                targetId.show().css({ "position": "absolute", "top": A_top + "px", "left": A_left + "px" });

                if ($("#" + objName).val() == "")
                    changeBrandListItemsStatus("");
            }
            else {
                targetId.show().css({ "position": "absolute", "top": A_top + "px", "left": A_left + "px" });

                if ($("#" + objName).val() == "")
                    changeBrandListItemsStatus("");
            }
        } else {
            $("#QueryBrandIframe").show();
        }
    });
    //支持输入定位
    this.keyup(function () {
        changeBrandListItemsStatus(this.value);
    });

    $(document).click(function (event) {
        if (event.target.id != _seft.selector.substring(1)) {
            targetId.hide();
        }
    });
    return this;
}
//inputTxt输入的内容 
function changeBrandListItemsStatus(inputTxt) {
    //alert(inputTxt);
    //首字母
    var firstLetter = inputTxt.substring(0, 1).toUpperCase();
    //包含品牌的div
    var divBrandList = document.getElementById("QueryBrandIframe").contentWindow.document.getElementById("brand" + firstLetter);
    //所有的div
    var divs = document.getElementById("QueryBrandIframe").contentWindow.document.getElementsByTagName("div");
    //所有的span
    var brandEnNameSpans = document.getElementById("QueryBrandIframe").contentWindow.document.getElementsByTagName("span");

    //执行隐藏首字母div和品牌列表div的操作 
    for (var i = 0; i < divs.length; i++) {
        //如果输入内容为空。显示所有的内容
        $(divs[i]).css("display", "");
        if (inputTxt != "") {
            if ($(divs[i]).attr("name") == "firstLetter") {
                if ($(divs[i]).attr("id") != "link" + firstLetter)
                    $(divs[i]).css("display", "none");
            }
            else if ($(divs[i]).attr("name") == "itemContent") {
                if ($(divs[i]).attr("id") != "brand" + firstLetter)
                    $(divs[i]).css("display", "none");
            }
        }
    }
    //隐藏不是输入项的品牌 
    $(divBrandList).children("span").each(function () {
        var brandEnName = $(this).attr("brandEnName");
        if (brandEnName != null && brandEnName != "undefined") {
            $(this).css("display", "");
            //if(!contains(brandEnName,inputTxt,true))
            if (!brandEnName.toLowerCase().startWith(inputTxt.toLowerCase())) {
                $(this).css("display", "none");
            }

        }
    });
}
