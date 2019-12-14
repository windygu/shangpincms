$(function () {

    /*leftMenu handel*/
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
        $.get("/shangpin/subject/CreateSubject.html"
                    , function (res) {
                        if (res) {
                            $("#mx-rightcontent-fixbarbg > .fixbarcontent").empty();
                            $("#mx-rightcontent-fixbarbg > .fixbarcontent").html(res);
                        }
                    });
        $("#mx-rightcontent-addbox").show();
        return false;
    });

    $("a[name='edit']").on("click", function () {
        $.get("/shangpin/subject/EditSubject.html", { subjectNo: this.id, ran: Math.random() }
                    , function (res) {
                        if (res) {
                            $("#mx-rightcontent-fixbarbg > .fixbarcontent").empty();
                            $("#mx-rightcontent-fixbarbg > .fixbarcontent").html(res);

                            //执行页面事件
                            domHender();
                        }
                    });
        windowShow();
        $("#mx-rightcontent-addbox").show();
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
    if ($('#alIndex_today').length > 0) {

        //今日特卖
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

    if ($('#alIndex_ending').length > 0) {

        //倒计时热卖
        var enData = [];
        $('#alIndex_ending_list li').each(function () {
            enData.push($(this).attr('pordId'));
        });
        $('#alIndex_ending_list ul').sortable({
            ondrop: function () {

                var ecData = [];
                $('#alIndex_ending_list li').each(function () {
                    ecData.push($(this).attr('pordId'));
                });

                if (ecData.toString() != enData.toString()) {
                    $('#alIndex_ending_list').data('items', ecData);
                } else {
                    $('#alIndex_ending_list').removeData('items');
                }

            }
        });

    }

    //保存修改过后的数据
    $('#saveData_btn').on('click', function () {

        var getTodayData = $('#alIndex_today_list').data('items'),
			getEndData = $('#alIndex_ending_list').data('items');
        var channelNo = $("#selChannelNo").val();
        if (getTodayData != undefined) {
            //console.log($('#alIndex_today_list').data('items')+ "\n顺序保存成功！\n");
            //alert($('#alIndex_today_list').data('items'));
            var subjectNos = $('#alIndex_today_list').data('items');
            $.ajax({
                url: "/outlet/subject/AjaxSaveSubjectSort",
                type: "post",
                data: { subjectNos: subjectNos.toString(), channelNo: channelNo, type: 1 },
                dataType: "json",
                success: function (data) {
                    if (data.result == "1") {
                        alert(data.message);
                        window.location.href = gettimestampurl(window.location.href);
                        //window.location.reload();
                    }
                    else {
                        alert(data.message);
                        return false;
                    }
                }
            });
        } else {
            //console.log("顺序没有改变！\n");
        }

        if (getEndData != undefined) {
            //console.log($('#alIndex_ending_list').data('items')+ "\n顺序保存成功！\n");
            //alert($('#alIndex_ending_list').data('items'));
            var subjectNo = $('#alIndex_ending_list').data('items');
            $.ajax({
                url: "/outlet/subject/AjaxSaveSubjectSort",
                type: "post",
                data: { subjectNos: subjectNo.toString(), channelNo: channelNo, type: 2 },
                dataType: "json",
                success: function (data) {
                    if (data.result == "1") {
                        alert(data.message);
                        window.location.href = gettimestampurl(window.location.href);
                        //window.location.reload();
                    }
                    else {
                        alert(data.message);
                        return false;
                    }
                }
            });
        } else {
            //console.log("顺序没有改变！\n");
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

    //保存修改过后的商品数据
    $('#saveList_btn').on('click', function () {

        var getListData = $('#alActive_list').data('items');
        var subjectNo = $("#hidSubjectNo").val();
        var categoryNo = $("#hidcategoryNo").val(); 
        if (getListData != undefined) {
            //console.log($('#alActive_list').data('items')+ "\n顺序保存成功！\n");
            //alert($('#alActive_list').data('items'));
            var prodNos = $('#alActive_list').data('items');
            $.ajax({
                url: "/shangpin/subject/AjaxSaveProductSort.html",
                type: "post",
                data: { subjectNo: subjectNo, scategoryNo: categoryNo, productNos: prodNos.toString() },
                dataType: "json",
                success: function (data) {
                    if (data.result == "1") {
                        alert(data.message);
                        window.location.href = gettimestampurl(window.location.href);
                        //window.location.reload();
                    }
                    else {
                        alert(data.message);
                        return false;
                    }
                }
            });
        } else {
            //console.log("顺序没有改变！\n");
        }

        return false;

    });

});