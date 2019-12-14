$(function () {

    /*leftMenu handel*/
    /*
    $("#mx-leftmenu li > a").on('click', function () {
    var that = $(this).closest('li');

    if (that.hasClass('cur') == true) {
    that.removeClass('cur');
    } else {
    that.addClass('cur');
    }

    });
    $("#mx-leftmenu li > a").toggle(function () {
    $(this).parent('li').find('p').show();
    }, function () {
    $(this).parent('li').find('p').hide();
    });

    $("#mx-leftmenu p a").on('click', function () {
    $("#mx-leftmenu p a").removeClass('cur');
    $(this).addClass('cur');
    });
    */

    /*rightcontent handel*/
    var $window = $("#mx-rightcontent-fixbarbg"),
		$nav = $("#mx-rightcontent-fixbar"),

    //弹出对话框
		windowShow = function () {
		    var mianH = parseInt($(window).height() - 154),
				topH = parseInt(mianH - 79);
		    $($window).css({ "height": mianH, "overflow-y": "scroll" });
		    //$($nav).css({ "bottom": topH });
		},

    //关闭对话框
		windowClose = function () {
		    $($window).css({ "height": 79, "overflow-y": "hidden" });
		    //$($nav).css({ "bottom": 0 });
		};

    $("#mx-rightcontent-fixbar a").click(function () {

        //层内容默认隐藏
        $(".mx-rightcontent-msgbox").hide();

        //console.log($(this).attr("class"));
        var isClick = $(this).attr("class");

        $(this).siblings("a").removeClass("curr");
        if (isClick != "curr") {
            $(this).addClass("curr");
            windowShow();
            return false;
        } else {
            $(this).removeClass("curr");
            windowClose();
            return false;
        }
    });

    $("#mx-btn-add").on("click", function () {
        $.get("/outlet/subject/Create", function (res) {
            if (res) {
                $("#mx-rightcontent-fixbarbg > .fixbarcontent").empty();
                $("#mx-rightcontent-fixbarbg > .fixbarcontent").html(res);
                $("#mx-rightcontent-addbox").show();
            }
        });

        return false;
    });

    $("a[name='edit']").on("click", function () {
        $.get("/outlet/subject/Edit", { subjectNo: this.id, t: Math.random() }
                    , function (res) {
                        if (res) {
                            $("#mx-rightcontent-fixbarbg > .fixbarcontent").empty();                            
                            windowShow();
                            $("#mx-rightcontent-fixbarbg > .fixbarcontent").html(res);
                            $("#mx-rightcontent-addbox").show();
                        }
                    });

        return false;
    });

    $("#mx-btn-del").on("click", function () {
        $("#mx-rightcontent-delbox").show();
        return false;
    });

    $("#mx-btn-search").on("click", function () {
        $("#mx-rightcontent-searchbox").show();
        return false;
    });

    $("#mx-btn-folder").on("click", function () {
        $("#mx-rightcontent-folderbox").show();
        return false;
    });


    //排序拖拽  

    //今日新开  
    if ($('#alIndex_today').length > 0) {
        var tnData = [];
        $('#alIndex_today_list li').each(function () {
            tnData.push($(this).attr('pordId'));
        });
        $('#alIndex_today_list ul').sortable({
            ondrop: function () {

                var tcData = [];
                $('#alIndex_today_list li').each(function () {
                    tcData.push($(this).attr('pordId'));
                });

                if (tcData.toString() != tnData.toString()) {
                    $('#alIndex_today_list').data('items', tcData);
                } else {
                    $('#alIndex_today_list').removeData('items');
                }
            }
        });
    }

    //正在进行
    if ($('#alIndex_doing').length > 0) {
        var enData = [];
        $('#alIndex_doing_list li').each(function () {
            enData.push($(this).attr('pordId'));
        });
        $('#alIndex_doing_list ul').sortable({
            ondrop: function () {

                var ecData = [];
                $('#alIndex_doing_list li').each(function () {
                    ecData.push($(this).attr('pordId'));
                });
                if (ecData.toString() != enData.toString()) {
                    $('#alIndex_doing_list').data('items', ecData);
                } else {
                    $('#alIndex_doing_list').removeData('items');
                }

            }
        });

    }

    //倒计时热卖
    if ($('#alIndex_ending').length > 0) {
        var enDatam = [];
        $('#alIndex_ending_list li').each(function () {
            enDatam.push($(this).attr('pordId'));
        });
        $('#alIndex_ending_list ul').sortable({
            ondrop: function () {

                var ecDatam = [];
                $('#alIndex_ending_list li').each(function () {
                    ecDatam.push($(this).attr('pordId'));
                });
                if (ecDatam.toString() != enDatam.toString()) {
                    $('#alIndex_ending_list').data('items', ecDatam);
                } else {
                    $('#alIndex_ending_list').removeData('items');
                }

            }
        });

    }

    //频道活动
    if ($('#alIndex_channel').length > 0) {
        var enDatac = [];
        $('#alIndex_channel_list li').each(function () {
            enDatac.push($(this).attr('pordId'));
        });
        $('#alIndex_channel_list ul').sortable({
            ondrop: function () {

                var ecDatac = [];
                $('#alIndex_channel_list li').each(function () {
                    ecDatac.push($(this).attr('pordId'));
                });
                if (ecDatac.toString() != enDatac.toString()) {
                    $('#alIndex_channel_list').data('items', ecDatac);
                } else {
                    $('#alIndex_channel_list').removeData('items');
                }

            }
        });

    }

    //保存修改过后的数据
    //$('#saveData_btn').on('click', function () {

    //    var getTodayData = $('#alIndex_today_list').data('items'),
    //        getSaleingData = $('#alIndex_doing_list').data('items'),
    //		getEndData = $('#alIndex_ending_list').data('items');
    //    var channelNo = $("#selChannelNo").val();
    //    if (getTodayData != undefined) {
    //        //console.log($('#alIndex_today_list').data('items')+ "\n顺序保存成功！\n");
    //        //alert($('#alIndex_today_list').data('items'));
    //        var subjectNos = $('#alIndex_today_list').data('items');
    //        $.ajax({
    //            url: "/outlet/subject/AjaxSaveSubjectSort",
    //            type: "post",
    //            data: { subjectNos: subjectNos.toString(), channelNo: channelNo, type: 1 },
    //            dataType: "json",
    //            success: function (data) {
    //                if (data.result == "1") {
    //                    alert(data.message);
    //                    window.location.href = gettimestampurl(window.location.href);
    //                    //window.location.reload();
    //                }
    //                else {
    //                    alert(data.message);
    //                    return false;
    //                }
    //            }
    //        });
    //    } else {
    //        //console.log("顺序没有改变！\n");
    //    }
    //    if (getSaleingData != undefined) {
    //        var subjectNoStr = $('#alIndex_doing_list').data('items');
    //        $.ajax({
    //            url: "/outlet/subject/AjaxSaveSubjectSort",
    //            type: "post",
    //            data: { subjectNos: subjectNoStr.toString(), channelNo: channelNo, type: 3 },
    //            dataType: "json",
    //            success: function (data) {
    //                if (data.result == "1") {
    //                    alert(data.message);
    //                    window.location.href = gettimestampurl(window.location.href);
    //                    //window.location.reload();
    //                }
    //                else {
    //                    alert(data.message);
    //                    return false;
    //                }
    //            }
    //        });
    //    } else {
    //        //console.log("顺序没有改变！\n");
    //    }
    //    if (getEndData != undefined) {
    //        //console.log($('#alIndex_ending_list').data('items')+ "\n顺序保存成功！\n");
    //        //alert($('#alIndex_ending_list').data('items'));
    //        var subjectNo = $('#alIndex_ending_list').data('items');
    //        $.ajax({
    //            url: "/outlet/subject/AjaxSaveSubjectSort",
    //            type: "post",
    //            data: { subjectNos: subjectNo.toString(), channelNo: channelNo, type: 2 },
    //            dataType: "json",
    //            success: function (data) {
    //                if (data.result == "1") {
    //                    alert(data.message);
    //                    window.location.href = gettimestampurl(window.location.href);
    //                    //window.location.reload();
    //                }
    //                else {
    //                    alert(data.message);
    //                    return false;
    //                }
    //            }
    //        });
    //    } else {
    //        //console.log("顺序没有改变！\n");
    //    }

    //    return false;

    //});

    //保存修改过后的数据  by zhangwei edit 20140618
    $('#saveData_btn,#saveData_btn1').on('click', function () {
        var getTodayData = $('#alIndex_today_list').data('items'),
            getSaleingData = $('#alIndex_doing_list').data('items'),
			getEndData = $('#alIndex_ending_list').data('items'),
            getChannelData = $("#alIndex_channel_list").data('items');
        var channelNo = $("#selChannelNo").val(); //频道
        var showType = $("#Channel").val(); //展示渠道
        var showDateTime = $("#txtShowDateTime").val(); //预期排序日期
        var msg = "";
        var iserror = false;
        var channelTxt = "";
        if (channelNo != '') {
            channelTxt = $.trim($('#selChannelNo option:selected').text());
        }
        if (channelNo == "") {
            alert("请选择频道页面！");
            return false;
        }
        if (showType == "") {
            alert("请选择展示渠道！");
            return false;
        }
        if (channelNo == "3") {
            if (showDateTime == "") {
                alert("请设置预排序日期！");
                return false;
            }
        }
        LoadMsgBegin("保存中...");
        //今日新开
        if (getTodayData != undefined) {
            var subjectNos = $('#alIndex_today_list').data('items');
            $.ajax({
                url: "/outlet/subject/AjaxSaveSubjectSort",
                type: "post",
                data: { subjectNos: subjectNos.toString(), channelNo: channelNo, type: 1, showType: showType, showDateTime: showDateTime },
                dataType: "json",
                async: false,
                success: function (data) {
                    if (data.result == "1") {
                        msg = msg + "今日新开,";
                        return true;
                    }
                    else {
                        alert(data.message);
                        return false;
                    }
                }
            });
            iserror = true;
        }
        //正在进行
        if (getSaleingData != undefined) {
            var subjectNoStr = $('#alIndex_doing_list').data('items');
            $.ajax({
                url: "/outlet/subject/AjaxSaveSubjectSort",
                type: "post",
                data: { subjectNos: subjectNoStr.toString(), channelNo: channelNo, type: 3, showType: showType, showDateTime: showDateTime },
                dataType: "json",
                async: false,
                success: function (data) {
                    if (data.result == "1") {
                        msg = msg + "正在进行,";
                        return true;
                    }
                    else {
                        alert(data.message);
                        return false;
                    }
                }
            });
            iserror = true;
        }
        //即将结束
        if (getEndData != undefined) {
            var subjectNo = $('#alIndex_ending_list').data('items');
            $.ajax({
                url: "/outlet/subject/AjaxSaveSubjectSort",
                type: "post",
                data: { subjectNos: subjectNo.toString(), channelNo: channelNo, type: 2, showType: showType, showDateTime: showDateTime },
                dataType: "json",
                async: false,
                success: function (data) {
                    if (data.result == "1") {
                        msg = msg + "即将结束,";
                        return true;
                    }
                    else {
                        alert(data.message);
                        return false;
                    }
                }
            });
            iserror = true;
        }
        //频道活动
        if (getChannelData != undefined) {
            var subjectNo = $('#alIndex_channel_list').data('items');
            $.ajax({
                url: "/outlet/subject/AjaxSaveSubjectSort",
                type: "post",
                data: { subjectNos: subjectNo.toString(), channelNo: channelNo, type: 0, showType: showType },
                dataType: "json",
                async: false,
                success: function (data) {
                    if (data.result == "1") {
                        msg = msg + channelTxt + "频道";
                        return true;
                    }
                    else {
                        alert(data.message);
                        return false;
                    }
                }
            });
            iserror = true;
        }
        if (!iserror) {
            LoadMsgEnd();
            alert("顺序没有改变，无法保存！");
            return false;
        } else {
            LoadMsgEnd();
            var lastIndex = msg.lastIndexOf(',');
            if (lastIndex > -1) {
                msg = msg.substring(0, lastIndex) + msg.substring(lastIndex + 1, msg.length);
            }
            alert("【" + msg + "】的活动排序保存成功！");
            return true;
        }
        return false;
    });

    if ($('#alActive_list').length > 0) {

        //商品列表
        var lnData = [];
        $('#alActive_list li').each(function () {
            lnData.push($(this).attr('pordId'));
        });
        $('#alActive_list ul').sortable({
            ondrop: function () {

                var lcData = [];
                $('#alActive_list li').each(function () {
                    lcData.push($(this).attr('pordId'));
                });

                if (lcData.toString() != lnData.toString()) {
                    $('#alActive_list').data('items', lcData);
                } else {
                    $('#alActive_list').removeData('items');
                }
            }
        });

    }

    $('#saveList_btn,#saveList_btn1').on('click', function () {
        LoadMsgBegin("保存中...");
        var getListData = $('#alActive_list').data('items');   // 移动过的商品
        var subjectNo = $("#hidSubjectNo").val();              //分组编号
        var productNos = $("#hidproductNos").val();            // 点击排序后的商品 
        var _subjectNo = $("#hidsubjectNoN").val();
        var sortType = $("#hidSortType").val();                // 排序类型及选项值
        var showType = $("#hidShowType").val();              // 展示渠道

        if (sortType == "-1") {
            alert("请选择排序类型");
            LoadMsgEnd();
            return false;
        }
        var isAutoBottom = "1";                                //是否默认沉底 1：yes ，0：no
        if (!$("input[name=ckbSweepwood]").attr("checked")) {
            isAutoBottom = "0";
        }
        if (getListData == undefined) {
            if (hidproductNos != null && hidproductNos != "") {
                getListData = productNos;
            }
            else {
                alert("顺序没有改变，无法保存！");
                LoadMsgEnd();
                return false;
            }
        }

        if (sortType == null || sortType == "") {
            sortType = $("#hidSortRuleType").val();
        }
        //console.log($('#alActive_list').data('items')+ "\n顺序保存成功！\n");
        //alert($('#alActive_list').data('items'));
        //var prodNos = $('#alActive_list').data('items');
        $.ajax({
            url: "/outlet/subject/AjaxSaveProductSort",
            type: "post",
            data: { subjectNo: subjectNo, productNos: getListData.toString(), sortType: sortType, showType: showType, IsAutoBottom: isAutoBottom },
            dataType: "json",
            success: function (data) {
                if (data.result == "1") {
                    alert(data.message);
                    LoadMsgEnd();
                    //window.location.href = gettimestampurl(window.location.href);
                    // window.location.reload();
                    if (showType == "0")
                        window.location.href = "/outlet/subject/OutletProductView?subjectNo=" + _subjectNo + "&SCategoryNo=" + subjectNo;
                    else
                        window.location.href = "/outlet/subject/OutletProductMobileView?subjectNo=" + _subjectNo + "&SCategoryNo=" + subjectNo;
                }
                else {
                    alert(data.message);
                    LoadMsgEnd();
                    return false;
                }
            }
        });
    });
});