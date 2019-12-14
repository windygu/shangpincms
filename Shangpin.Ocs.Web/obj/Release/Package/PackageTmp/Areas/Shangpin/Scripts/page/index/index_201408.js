$(function(){

	/*首页图片轮播 start*/
    
    //首先取得图片队列
    var imgqueue = (function(){
        var ret = [];
        $("ul.switchsort a").each(function(){
            ret.push($(this).attr("mximg"));
        });

        return ret;
    })();

    //设置滚动时的DOM变化
    var indexerStyle = function(index){
        //设置图片区域的超链接
        $("a[mximg-indexer]").removeClass("cur");
        var cur = $("a[mximg-indexer='" + index + "']");
        cur.addClass("cur");
        $("a#switchhref").attr("href", cur.attr("href")).attr("title", cur.attr("title"));
    };

    //初始化MXImageSwitch
    var MXSwitcher = $.MXImage.Switchover({
        imgqueue: imgqueue,
        wrapper: $("#switchhref"),
        preload: function(structor, switcher){
            if(!switcher.last){
                var last = switcher.last = this.last();
                if(structor.status !== 2){
                    switcher.wrapper.removeClass("loaderhide");
                    last && last.img && last.img.animate({opacity: 0.5}, 300);
                }
                else{
                    switcher.wrapper.addClass("loaderhide");
                }
            }
            indexerStyle(this.index());
        },
        auto: 7000
    });

    //读取第一张图片
    MXSwitcher.replace($("#switchhref img").eq(0), 0);
    MXSwitcher.current(0, {
        preload: function(){},
        onload: function(){},
        always: function(structor, switcher){
            switcher.autostart();
        }
    });

    //索引区域鼠标触发事件
    $("a[mximg-indexer]").on("mouseenter", function (e) {
        var $indexer = parseInt($(this).attr("mximg-indexer"));

        if(!isNaN($indexer)){
            MXSwitcher.current($indexer);
        }

        return false;

    });

    //上一幅的点击事件
    $("#switchprev").on("click", function(){
        window.getSelection ? window.getSelection().removeAllRanges() : document.selection.empty();
        MXSwitcher.prev();
        return false;
    });

    //下一幅的点击事件
    $("#switchnext").on("click", function(){
        window.getSelection ? window.getSelection().removeAllRanges() : document.selection.empty();
        MXSwitcher.next();
        return false;
    });

    //鼠标移动到图片轮换区域时，停止自动滚动，移出时恢复自动滚动
    $("#spIndex_mxImageSwitch").hover(
        function(){
            MXSwitcher.autostop();
            $("#sp-index-imgswitch-control").css({"visibility":"visible"});
        },
        function(){
            //这里要用restart方法
            MXSwitcher.autorestart();
            $("#sp-index-imgswitch-control").css({"visibility":"hidden"});
        }
    );

    /*首页图片轮播 end*/

    //热门潮流，国际大牌切换
    if($("#spTabs").length > 0){
        var mytab = SP.plug.tab.create({
            wrapper: "#spTabs",
            contentWrapper: "#spTab_content",
            first: "mytab-1",
            trigger: "hover",
            effector: "direct",
            curClass: "curr",//new
            auto:false
        });
    }

    //本周热门商品切换
    if($("#hot_tabs").length > 0){
        var mytab = SP.plug.tab.create({
            wrapper: "#hot_tabs",
            contentWrapper: "#hotTab_content",
            first: "hottab-1",
            trigger: "hover",
            effector: "direct",
            curClass: "curr",//new
            auto:false
        });
    }



    /*楼层logo从左向右轮播start*/

    for(var i=0; i<$(".spIndex_imgroll_wrapper").length; i++){
            imgScroll(i);     
    };

   //从左向右公共滚动
   function imgScroll(index){

        var indexScroller = SP.plug.scroller.create("#spIndex_imgroll_wrapper_"+index, "#spIndex_imgroll_wrapper_"+index+" ul", "#spIndex_imgroll_wrapper_"+index+" li").screenCount(1, 0).onStep(function(index){
        }).auto(0, "#spIndex_imgroll_wrapper_"+index, false, function(){
            SP.config.lazyload.each();
        });

    }

    /*function imgScroll(index){

        var indexLazyload = SP.config.lazyload.group("#spIndex_imgroll_wrapper_"+index, "#spIndex_imgroll_wrapper_"+index+" img" , "index_scroll"+index);
        SP.config.lazyload.each();

        var indexScroller = SP.plug.scroller.create("#spIndex_imgroll_wrapper_"+index, "#spIndex_imgroll_wrapper_"+index+" ul", "#spIndex_imgroll_wrapper_"+index+" li").screenCount(1, 0).onStep(function(index){
            var indexer = $("#spIndex_imgroll_index"+index+" li");
            indexer.removeClass("ciron").eq(index).addClass("ciron");
        }).up("#spIndex_imgroll_prev"+index, 1, "click", function(){
            var max = this.count() - (this.screenCount || 1);
            if (this.current() == 0) {
                this.current(max + 1);
            }
            return true;
        }, function(el){
        }, function(){
            indexLazyload.grouper("index_scroll"+index).each();
        }).down("#spIndex_imgroll_next"+index, 1, "click", function(){
            var max = this.count() - (this.screenCount || 1);
            if (this.current() == max) {
                this.current(-1);
            }
            return true;
        }, function(el){
        }, function(){
            indexLazyload.grouper("index_scroll"+index).each();
        }).auto(0, "#spIndex_imgroll_prev"+index+",#spIndex_imgroll_next"+index+",#spIndex_imgroll_wrapper_"+index, false, function(){

            indexLazyload.grouper("index_scroll"+index).each();

        });
        
        $("#spIndex_imgroll_index"+index).delegate("li", "click", function(e){
            var index = $(this).attr("imgid");
            indexScroller.scrollTo(index, 400, false, function(){
                indexLazyload.grouper("index_scroll"+index).each();
            });
            return false;
        });
    }*/


    
   
    /*楼层logo从左向右轮播 end*/

});

