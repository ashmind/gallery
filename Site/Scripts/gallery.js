global = {
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
    };
}

var gallery = {
    openAlbum : function(album, options) {
        options = options || {};
        var albumLink = $("#left a.album-name[data-id='" + album + "']");
        if (albumLink.length == 0)
            return;

        if (!albumLink.hasClass('selected')) {
            this._loadAlbum(album, albumLink, options.whenOpened);
            return true;
        }
        else if (options.whenOpened) {
            options.whenOpened();
            return false;
        }
    },
    
    openItem : function(item, album) {
        if (album) {
            var that = this;
            this.openAlbum(album, { whenOpened: function() { that.openItem(item); }});
            return;
        }
        
//        global.noHistoryRecordWhileClosingLightbox = true;
//        $.fancybox.close();        
        
        if (!item)
            return;
            
        var a = $(".wall a[data-name='" + item + "']");
        $("#main").scrollTop(a.position().top);
        
        a.click();

        this.history.add(album || this.history.current().album, item);
    },
    
    _loadAlbum : function(album, albumLink, whenLoaded) {
        if(window.stop) {
            window.stop();
        }
        else if(document.execCommand) {
            document.execCommand("Stop", false);
        }
        
        $('#left').find('.selected').removeClass('selected');
        albumLink.addClass('selected');

        $('#main').hide();
        this.history.add(album);
        
        document.title = document.title.replace(/:.+$/, "") + ": " + albumLink.text();
        
        $.get(albumLink.attr('href'), {}, function(html) {
            $('#main').html(html).show();
            gallery.setupCurrentAlbum();

            if (whenLoaded)
                whenLoaded();
        });        
    },
    
    setupCurrentAlbum : function() {
        function getItemData(a) {
            var data = a.data('data');
            if (!data) {
                data = eval('(' + a.attr('data-json') + ')');
                a.data('data', data);
            }

            return data;
        }

        function createActionLinks(a) {
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
                download += "<a href='" + downloadData.action + "/" + downloadData.sizes[name] + "'>" + name + "</a>";
            }
            download += "</span>";

            return "<span class='image-actions'>" +
                [ locate, deleteOrRestore, download ]
                    .filter(function(item) { return !!item; })
                    .join('<span class="separator">|</span>') +
            "</span>";
        }

        $(".wall a.image-view").fancybox({
            type            : 'image',
            padding	        : 0,
            prevEffect	    : 'fade',
            nextEffect	    : 'fade',
            loop            : false,

            beforeShow : function() {
                gallery.lightboxOpen = true;

                var a = $(this.element);
                var data = getItemData(a);
                gallery.history.add(a.parents('.album-view').attr('data-id'), data.name);

                this.title = createActionLinks($(this.element));
                return true;
            },

            afterShow : function() {
                var a = $(this.element);
                $(".image-actions .locate").click(function(event) {
                    event.preventDefault();
                    gallery.openItem($(this).attr('data-itemname'), $(this).attr('data-albumid'));
                });

                $(".image-actions .delete, .image-actions .restore").click(function(event) {
                    event.preventDefault();
                    var item = a.parents(".item");
                    $.get($(this).attr('href'), {}, function(isDeleted) {
                        $.fancybox.next();
                        var deleted = $("section.to-delete");
                        if (isDeleted.toLowerCase() == 'true') {
                            deleted.append(item);
                        }
                        else {
                            deleted.prevAll('section').append(item);
                        }
                    });
                });

                return true;
            },

            beforeClose : function() {
                var a = $(this.element);
                if (!global.noHistoryRecordWhileClosingLightbox)
                    gallery.history.add(a.parents('.album-view').attr('data-id'));
                
                global.noHistoryRecordWhileClosingLightbox = false;
                gallery.lightboxOpen = false;
                return true;
            },

            helpers : {
                overlay : {
                    opacity: 0.8,
                    css : {
                        'background-color': '#000'
                    }
                },

                title : {
                    type: 'over'
                },

                thumbs	: {
                    width	: 50,
                    height	: 50
                }
            }
        });

        $("#main").scrollTop(0);
        var imgs = $(".wall img");
        imgs.dragout();

        var lazyImgs = imgs.filter("[data-lazysrc]");
        lazyImgs.lazyload({
            placeholder : lazyImgs.eq(0).attr('src'),
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
};

$(function() {
    gallery.history.setup(global.rootUrl);
    gallery.history.subscribe(function(album, item) { gallery.openItem(item, album); });
    var current = gallery.history.current();
    if (!gallery.openAlbum(current.album))
        gallery.setupCurrentAlbum();
    
    gallery.openItem(current.item);

    $("#left a.album-name").live('click', function(event) {
        event.preventDefault();
        gallery.openAlbum($(this).attr('data-id'));
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

function reloadAlbum() {
    gallery._loadAlbum($("#left a.album-name.selected"));
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