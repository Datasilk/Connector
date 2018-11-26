S.dashboard = {
    init: function () {
        //load button events
        $('header .user').on('click', () => S.dashboard.tab.open('Profile'));
        $('header .options').on('click', S.dashboard.options.show);

        //load options menu button events
        $('.options-menu a').each((ev) => {
            let a = $(ev);
            const apath = a.get().getAttribute('href'); 
            a.on('click', (e) => {
                S.dashboard.tab.open(apath);
                e.cancleBubble = true;
                e.preventDefault();
                return false;
            });
        });

        //load timeline
        S.dashboard.tab.open('Timeline');
    },

    tab: {
        open: function (path, data) {
            //deselect selected tab
            $('header ul.tabs li').removeClass('selected');

            //generate id & compiled path
            let id = path.replace(/\//g, '-');
            let pathdata = '';
            if (data != null && typeof data == 'object') {
                for (let prop in data) {
                    pathdata += '/' + encodeURI(prop.replace(/\s/g, '+')) + '/' + encodeURI(data[prop].replace(/\s/g, '+'))
                }
            }
            const compiledpath = path.toLowerCase() + pathdata;

            //check for existing tab
            let tab = $('header ul.tabs li.tab-' + id);
                //existing tab is already selected
            if (tab.hasClass('selected')) {
                return;
            } else if (tab.length > 0) {
                //show existing tab
                tab.addClass('selected');
                $('.dash-body > .page').hide();
                $('.dash-body > .page-' + id).show();
                return;
            }
            //load new tab
            $.ajax({
                url: '/api/' + path + '/View', data: data,
                dataType:'json',
                success: function (d) {
                    //generate tab
                    let li = $('#template_tab').html()
                        .replace('#li-class#', 'tab-' + id + ' selected')
                        .replace('#href#', compiledpath)
                        .replace('#label#', d.title)
                        .replace('#icon#', d.icon);
                    $('header ul.tabs').append(li);
                    $('.tab-' + id + ' a').on('click', (e) => {
                        S.dashboard.tab.open(path, data);
                        e.cancleBubble = true;
                        e.preventDefault();
                        return false;
                    })

                    //create content
                    let div = document.createElement('div');
                    div.className = 'page page-' + id;
                    div.innerHTML = d.html;
                    $('.dash-body').append(div);

                    //load js file (if neccessary)
                    if (d.jsfile != null && d.jsfile != '') {
                        S.util.js.load(d.jsfile, 'js-' + id);
                    }

                    //load css file (if neccessary)
                    if (d.cssfile != null && d.cssfile != '') {
                        S.util.css.load(d.cssfile, 'css-' + id);
                    }

                    //show content
                    $('.dash-body > .page').hide();
                    $('.dash-body > .page-' + id).show();

                },
                error: function (d) {

                }
            });
        }
    },

    options: {
        show: function () {
            $('.options-menu').addClass('show');
            $(document.body).on('click', (e) => {
                $('.options-menu').removeClass('show');
                $(document.body).off('click');
            });
        }
    }
};

S.dashboard.init();