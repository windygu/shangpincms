(function (window, $, undefined) {
    var dragSrcEl = null, listBox = null, startY = 0, endY = 0, placeholders = $(), i = 0;

    $.fn.sortable = function (options) {

        options = $.extend({
            connectWith: false,
            ondrop: null,
        }, options);

        return this.each(function () {

            listBox = this;

            var index, items = $(this).children(options.items).attr('draggable', 'true');
            var placeholder = $('<' + (/^ul|ol$/i.test(this.tagName) ? 'li' : 'div') + ' class="sortable">');

            function e(event) {
                if (!event) {    // 兼容IE浏览器
                    event = window.event;
                    event.target = event.srcElement;
                    event.layerX = event.offsetX;
                    event.layerY = event.offsetY;
                }
                event.mx = event.pageX || event.clientX + document.body.scrollLeft; // 计算鼠标指针X轴距离
                event.my = event.pageY || event.clientY + document.body.scrollTop;  // 计算鼠标指针Y轴距离
                return event;   // 返回标准化的事件对象
            }

            $(this).data('items', options.items);
            placeholders = placeholders.add(placeholder);

            if (options.connectWith) {
                $(options.connectWith).add(this).data('connectWith', options.connectWith);
            }

            items.on('dragstart', function (event) {
                if ($(this).attr("class") == "alActive_list_cell" || $(this).attr("class") == "alActive_list_cell_FF") {
                    if ($("#alIndex_ending_list li").length == 1 && $(this).parent().attr("id") != "collection") {
                        return false;
                    }
                }

                this.style.opacity = '0.6';
                dragSrcEl = this;
                index = $(this).index();
                event.originalEvent.dataTransfer.effectAllowed = 'move';
                event.originalEvent.dataTransfer.setData('Text', 'dummy');

                //新增自动滚动事件
                event = e(event.originalEvent);
                startY = event.my;


            }).on('dragenter', function (e) {

                if (!items.is(dragSrcEl) && options.connectWith !== $(dragSrcEl).parent().data('connectWith')) {
                    return true;
                }

                //placeholders.insertBefore(this);
                $(this)[placeholder.index() < $(this).index() ? 'after' : 'before'](placeholder);
                $(dragSrcEl).hide();

            }).not('a[href], img').on('selectstart', function () {
                this.dragDrop && this.dragDrop();
                return false;
            }).end().add(placeholders).on('dragover', function (event) {

                if (!items.is(dragSrcEl) && options.connectWith !== $(dragSrcEl).parent().data('connectWith')) {
                    return true;
                }

                if (event.preventDefault) {
                    event.preventDefault();
                }
                event.originalEvent.dataTransfer.dropEffect = 'move';

                //新增自动滚动事件
                event = e(event.originalEvent);
                endY = event.my;
                //console.log(endY);

                if (endY < 200) {
                    i -= 10;
                    $('#mx-rightcontent').scrollTop(i);
                }
                if (endY > 600) {
                    i += 10;
                    $('#mx-rightcontent').scrollTop(i);
                }

                return false;

            }).on('dragend', function (e) {

                this.style.opacity = '1.0';
                $(dragSrcEl).show();
                placeholders.detach();

                if (options.ondrop) {
                    options.ondrop();
                }
                if ($.browser.mozilla) {
                    if ($(this).attr("class") == "alActive_list_cell_FF") {
                        $(this).removeClass("alActive_list_cell_FF");
                        $(this).addClass("alActive_list_cell");
                    }
                }


            }).on('drop', function (e) {

                if (e.stopPropagation) {
                    e.stopPropagation();
                }

                placeholders.filter(':visible').after(dragSrcEl);

                //再次加入列表事件
                this.style.opacity = '1.0';
                $(dragSrcEl).show();
                placeholders.detach();
                if (options.ondrop) {
                    options.ondrop();
                }
                initStyle();
                return false;

            });

            //移出列表事件
            $("#collection").on('dragenter', function (e) {

                $(this).append($(dragSrcEl));
                // $(this).find("li").removeClass("alActive_list_cell");
                //  $(this).find("li.alActive_list_cell").css({ "opacity": 1, "display": "block" });

                if ($.browser.mozilla) {
                    $(this).find("li").removeClass("alActive_list_cell");
                    $(this).find("li").addClass("alActive_list_cell_FF");
                } else {
                    $(this).find("li.alActive_list_cell").css({ "opacity": 1, "display": "block" });
                }
            });

        });

    };

    function initStyle() {
        if ($.browser.mozilla) {
            if ($("#alIndex_ending_list").find("li").attr("class") == "alActive_list_cell_FF") {
                $("#alIndex_ending_list").find("li").removeClass("alActive_list_cell_FF");
                $("#alIndex_ending_list").find("li").addClass("alActive_list_cell");
            }
        }
    }
})(window, jQuery);