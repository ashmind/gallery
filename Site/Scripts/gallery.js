global = {
    lightboxNotYetShown : true,
    noHistoryRecordWhileClosingLightbox : false
};

if (Array.prototype.filter) {
    Array.prototype.filter = function(filter) {
        var result = [];
        for (var i = 0; i < this.length; i++) {
            if (!filter(this[i], i))
                continue;

            result.push(this[i]);
        }
        return result;
    }
}

$(function() {
    $.history.init(function(hash) {
        if ($.history.noaction)
            return;

        var parts = hash.replace(/^\//, '').split('/');
        var albumLink = $("#left a.album-name[data-id='" + parts[0] + "']");

        if (albumLink.length == 0)
            return;

        var selectItem = function() {
            if (parts.length == 1)
                return;
            
            var a = $(".wall a[data-name='" + parts[1] + "']");
            $("#main").scrollTop(a.position().top);
            
            $.history.noaction = true;
            if (global.lightboxNotYetShown) {
                a.click();
            }
            else {
                a.parent().toggleClass('selected');
            }
            $.history.noaction = false;
        };

        global.noHistoryRecordWhileClosingLightbox = true;
        $.fancybox.close();

        if (!albumLink.hasClass('selected')) {
            loadAlbum(albumLink, selectItem);
        }
        else {            
            selectItem();
        }
    });
    setupAlbum();

    $("#left a.album-name").live('click', function(event) {
        event.preventDefault();
        $.history.load("/" + $(this).attr('data-id'));
    });

    $("#left section.folders a.more").live('click', function(event) {
        event.preventDefault();

        var a = $(this);
        $.get(a.attr('data-ajax'), {}, function(html) {
            a.replaceWith(html);
        });
    });

    $("#left section.people a.more").click(function() {
        $(this).remove();
        $("#left section.people div.more").show();        
    });

    $(".expand-to-delete").live('click', function(event) {
        event.preventDefault();

        $("section.to-delete").css('display', 'block');
        $(this).hide();

        $("#main").scroll();
    });
});

function loadAlbum(a, onsuccess) {
    if(window.stop) {
        window.stop();
    }
    else if(document.execCommand) {
        document.execCommand("Stop", false);
    }

    $.get(a.attr('href'), {}, function(html) {
        $('#main').html(html);

        $('#left').find('.selected').removeClass('selected');
        a.addClass('selected');

        setupAlbum();

        document.title = document.title.replace(/:.+$/, "") + ": " + a.text();

        if (onsuccess)
            onsuccess();
    });        
}

function reloadAlbum() {
    loadAlbum($("#left a.album-name.selected"));
}

function setupAlbum() {
    function getItemData(a) {
        var data = a.data('data');
        if (!data) {
            data = eval('(' + a.attr('data-json') + ')');
            a.data('data', data);
        }

        return data;
    }

    $(".wall a.image-view").fancybox({
        type            : 'image',
        padding	        : 0,
        transitionIn	: 'fade',
        transitionOut	: 'fade',
        overlayOpacity  : 0.8,
        overlayColor    : '#000',
        titlePosition   : 'over',

        onComplete : function(currentArray, currentIndex) {
            global.lightboxNotYetShown = false;

            var a = $(currentArray).eq(currentIndex);
            var data = getItemData(a);

            a.parent().toggleClass('selected');
            if (!$.history.noaction) {
                $.history.noaction = true;
                $.history.load("/" + a.parents('.album-view').attr('data-id') + '/' + data.name);
                $.history.noaction = false;
            }

            $("#fancybox-wrap .locate").click(function(event) {
                event.preventDefault();                
                $.history.load("/" + $(this).attr('data-albumid') + '/' + $(this).attr('data-itemname'));
            });

            $("#fancybox-wrap .delete, #fancybox-wrap .restore").click(function(event) {
                event.preventDefault();
                var item = a.parents(".item");
                $.get($(this).attr('href'), {}, function(deletesCount) {
                    $.fancybox.close();
                    var deleted = $("section.to-delete");
                    if (deletesCount > 0) {
                        deleted.append(item);
                    }
                    else {
                        deleted.prevAll('section').append(item);
                    }
                });
            });
        },

        titleFormat : function(title, currentArray, currentIndex) {
            var a = $(currentArray).eq(currentIndex);
            var data = getItemData(a);

            var locate = "";
            if (data.primaryAlbumID) {
                locate = "<a class='locate' " +
                    "data-albumID='" + data.primaryAlbumID + "' " +
                    "data-itemname='" + data.name + "' " +
                    "href='javascript:void(\"locate\")'" +
                ">Locate</a>";
            }

            var deleteOrRestoreKey = data.actions['delete'] ? 'delete' : 'restore';
            var deleteOrRestoreData = data.actions[deleteOrRestoreKey];
            var deleteOrRestore = "<a class='" + deleteOrRestoreKey + "' href='" + deleteOrRestoreData.action + "'>" + deleteOrRestoreData.text + "</a>";
            var download = "<span>Download: ";
            var downloadData = data.actions.download;
            for (var name in downloadData.sizes) {
                download += "<a href='" + downloadData.action + "/" + downloadData.sizes[name] + "'>" + name + "</a>"
            }
            download += "</span>";

            return "<span id='fancybox-title-over' class='image-actions'>" +
                [ locate, deleteOrRestore, download, "<a href='" + data.actions.comment + "'>Comment</a>" ]
                    .filter(function(item) { return !!item; })
                    .join('<span class="separator">|</span>')
            "</span>";
        },

        onClosed : function(currentArray, currentIndex) {
            var a = $(currentArray).eq(currentIndex);

            if (!global.noHistoryRecordWhileClosingLightbox) {
                $.history.noaction = true;
                $.history.load("/" + a.parents('.album-view').attr('data-id'));
                $.history.noaction = false;
            }
            global.noHistoryRecordWhileClosingLightbox = false;
        }
    });

    $("#main").scrollTop(0);
    var imgs = $(".wall img");
    imgs.lazyload({
        placeholder : imgs.eq(0).attr('src'),
        container   : $("#main"),
        effect      : "fadeIn"
    });

    setupSecurityPanel();

    /*$('.wall').dragToSelect({
        selectables  : '.item',
        selectOnMove : true/ *,
        onHide       : function () {
            var selected = $('.wall .item.selected');
            if (selected.length > 0) {
                $("#main").css('right', '12.6em');
                $("#right-menu").show();
            }
            else {
                $("#right-menu").hide();
                $("#main").css('right', '0');
            }
        }* /
    });*/
}

function setupSecurityPanel() {
    $("a.album-visible-to").fancybox({
        transitionIn	: 'fade',
        transitionOut	: 'fade',
        overlayOpacity  : 0.8,
        overlayColor    : '#000',
        onComplete      : function() {
            $('form.grant img')
                .click(function() {
                    var group = $(this).parent();

                    group.toggleClass('granted');
                    var hidden = group.find('input');
                    hidden.val(hidden.val() == "" ? group.attr('data-key') : "");
                });

            $('form.grant').ajaxForm({
                success : function() {
                    $.fancybox.close();
                    reloadAlbum();
                }
            });
        }
    });
}