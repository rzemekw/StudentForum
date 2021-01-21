// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    const placeHolder = $('#modalPlaceHolder');
    $('a[data-toggle="modal-window"]').click(function () {
        var url = $(this).data('url');
        $.get(url).done(function (data) {
            placeHolder.html(data);
            placeHolder.find('.modal').modal('show');
            placeHolder.find('form').submit(onFormSubmit);
        })
    });

    placeHolder.on('hidden.bs.modal', function () {
        placeHolder.empty();
    });

    var allowSubmit = true;
    function onFormSubmit() {

        if (!allowSubmit)
            return false;
        else
            allowSubmit = false;

        var form = $(this);
        var actionUrl = form.attr('action');
        var data = form.serialize();

        $.post(actionUrl, data)
            .always(function () {
                placeHolder.find('span[data-valmsg-for]').empty();
                $('#other-errors').empty();
                allowSubmit = true;
            })
            .done(function (response) {
                var content = response.value;
                if (content.redirect) {
                    window.location.href = content.redirect;
                }
                else {
                    for (var obj of content) {
                        placeHolder.find('span')
                            .filter(function () {
                                return $(this).attr('data-valmsg-for') == obj.key
                            })
                            .append(document.createTextNode(obj.error));
                    }
                }
            })
            .fail(function (response) {
                if (response.responseJSON.value)
                    $('#other-errors').append(response.responseJSON.value);
                else
                    $('#other-errors').append("Failed to handle your request, please try again");
            });
        return false;
    };

    function adaptSidebarHeight() {
        $('#groups-menu').css('top', $('header').height() + 20);
        $('#groups-menu').height(function () {
            return $(this).css('top') - $(this).css('bottom');
        });
    }

    adaptSidebarHeight();

    $(window).resize(adaptSidebarHeight);

    $('#groups-menu > button').click(function () {
        $('#groups-menu').toggleClass('hide');
    })

    $('#groups-container').mCustomScrollbar({
        theme: "dark-thin"
    });

    function loadGroups() {
        var url = new URL($('#groups-container').data('url'));
        var groupsUrl = $('#groups-container').data('groups-url');

        fetch(url).then(r => r.json())
            .then(d => {
                var ul = $('#groups-container > div ul');
                var otherLi;
                d.forEach(function (item) {
                    var li = $('<li>')
                    var universityDiv = $('<div>');
                    var collapseIcon = $('<i>');
                    collapseIcon.addClass('fas fa-chevron-down');
                    var aUniversity = $('<a>');
                    universityDiv.append(collapseIcon);

                    if (!item.groups.length) {
                        collapseIcon.addClass('no-answers');
                    }

                    universityDiv.append(aUniversity);
                    var ul2 = $('<ul>');
                    li.append(universityDiv);
                    li.append(ul2);

                    if (item.universityId) {
                        aUniversity.text(item.universityName);
                        aUniversity.attr("href", "google.com");
                        ul.append(li);
                    }
                    else {
                        aUniversity.text('Other');
                        otherLi = li;
                    }

                    item.groups.forEach(function (group) {
                        var li2 = $('<li>');
                        var aGroup = $('<a>');
                        li2.append(aGroup);

                        li2.data('id', group.id);

                        aGroup.attr('href', groupsUrl + '/' + group.id);
                        aGroup.text(group.name);
                        ul2.append(li2);

                        if (group.newTopicAvailable) {
                            li2.append($('<i class="fas fa-exclamation-circle new-topic-icon"></i>'));
                        }
                        if (group.newAnswerAvailable) {
                            li2.append($('<i class="fas fa-exclamation-circle new-answer-icon"></i>'));
                        }
                    });

                    ul.append(otherLi);
                });

                $('#groups-container ul div i').click(function () {
                    $(this).parents('li').toggleClass('collapsed');
                })
            });
    }

    if ($('#groups-container').length) {
        loadGroups();
    }

    $('.topic-footer .show-answers').click(function () {
        var url = $(this).data('url');

        fetch(url).then(r => r.json())
            .then(answers => {
                for (var answer of answers) {
                    var aDiv = $('<div class="topic-answer"><div class="topic-answer-header"><h4></h4><span></span></div><p></p></div>');
                    aDiv.find('h4').text(answer.author.firstName + " " + answer.author.lastName);
                    aDiv.find('span').data('date', answer.date);
                    aDiv.find('span').text(displayDate(new Date(answer.date)));
                    aDiv.find('p').text(answer.content);

                    $(this).parents('.topic').find('.topic-answers').prepend(aDiv);
                }

                $(this).parents('.topic').removeClass('answers-hidden');
                $(this).parents('.topic').find('i.new-answer-icon').fadeOut(300, function () {
                    $(this).remove();
                });

                var topic = $(this).parents('.topic')[0];
                var groupId;

                var topicsWithIcon = $('.topic').filter(function () {
                    if (this.isSameNode(topic))
                        return false;
                    return $(this).find('.new-answer-icon').length;
                });

                if (!$('.group-header').data('id')) {
                    groupId = $(this).parents('.topic').data('id');

                    topicsWithIcon = topicsWithIcon.filter(function () {
                        return $(this).data('id') == groupId;
                    });
                }
                else {
                    groupId = $('.group-header').data('id');
                }

                if (!topicsWithIcon.length) {
                    var groupLi = $('#groups-container li').filter(function () {
                        return $(this).data('id') == groupId;
                    });

                    groupLi.find('.new-answer-icon').remove();
                }
            });
    });

    $('.topic-footer .hide-answers').click(function () {
        $(this).parents('.topic').addClass('answers-hidden');

        $(this).parents('.topic').find('.topic-answers .topic-answer:nth-last-child(n+2)').remove();
    });

    $('.topic form textarea').on('input', function () {
        this.style.height = "1px";
        this.style.height = (this.scrollHeight + 3) + "px";
    });

    $('.add-attachments').click(function () {
        $(this).parent().find('input[type="file"]').trigger('click');
    })

    function onAddedAttachmentClick() {
        const dataTransfer = $(this).parents('.topic-answer-form').find('input[type="file"]').data('dataTransfer');
        const id = $(this).data('id');
        var spans = $(this).parent().find('span');
        for (var i = id; i < spans.length; i++) {
            $(spans[i]).data('id', i);
        }
        dataTransfer.items.remove(id - 1);
        $(this).remove();
    };

    $('.topic input[type="file"]').change(function () {
        const attachments = $(this).parents('.topic-answer-form').find('.attachments');
        var id = attachments.find('span:nth-last-child(1)').data('id');
        var dataTransfer = $(this).data('dataTransfer');
        if (!dataTransfer) {
            dataTransfer = new DataTransfer();
            $(this).data('dataTransfer', dataTransfer);
        }
        if (!id)
            id = 0;
        [].forEach.call(this.files, function (file) {
            dataTransfer.items.add(file);
            id++;
            var span = $('<span>');
            if (file.name.length < 15) {
                span.text(file.name);
            }
            else {
                span.text(file.name.substring(0, 14) + '...');
            }
            span.data('id', id);
            span.click(onAddedAttachmentClick)
            attachments.append(span);
        });
    });

    $('.topic-answer-form').submit(function (e) {
        e.preventDefault();

        var content = $(this).find('textarea').val();
        if (content == null || content == "")
            return false;

        var url = $(this).attr('action');
        var data = $(this).serialize();

        $(this).find('textarea').val("");

        $.post(url, data).fail(function (response) {
            alert('something went wrong, please try again');
        });

        return false;
    });


    $('.topic').hover(function () {
        if (!$('.group-header').data('id'))
            return;

        var icon = $(this).find('.new-topic-icon');

        if (!icon.length)
            return;

        icon.fadeOut(300, function () {
            $(this).remove();
        });

        var topic = this;

        var topicsWithIcon = $('.topic').filter(function () {
            if (this.isSameNode(topic))
                return false;

            if ($(this).attr('id') == 'future-topic')
                return false;


            return $(this).find('.new-topic-icon').length;
        });

        if (topicsWithIcon.length)
            return;

        var groupId = $('.group-header').data('id');

        var groupLi = $('#groups-container li').filter(function () {
            return $(this).data('id') == groupId;
        });

        groupLi.find('.new-topic-icon').remove();
        
    })
});

let connection = new signalR.HubConnectionBuilder()
    .withUrl('/hubs/groups')
    .build();

$(document).ready(function () {
    connection.on("TopicCreated", function (groupId, topicId, userId) {

        if ($('.group-header').length && $('.group-header').data('user-id') == userId)
            return;

        var groupLi = $('#groups-container li').filter(function () {
            return $(this).data('id') == groupId;
        });

        if (groupLi.find('.new-topic-icon').length)
            return;

        var icon = $('<i class="fas fa-exclamation-circle new-topic-icon"></i>');
        groupLi.find('a').after(icon);
    });

    connection.on('AnswerCreated', function (groupId, topicId, answerId, userId) {

        if ($('.group-header').length && $('.group-header').data('user-id') == userId)
            return;

        var groupLi = $('#groups-container li').filter(function () {
            return $(this).data('id') == groupId;
        });

        if (groupLi.find('.new-answer-icon').length)
            return;

        var icon = $('<i class="fas fa-exclamation-circle new-answer-icon"></i>');
        groupLi.append(icon);
    });

    if ($('#groups-container').length)
        connection.start();
});

function displayDate(date) {
    var difference = Date.now() - date;

    if (difference < 1000 * 60)
        return "now";

    var mins = (difference / 1000 / 60).toFixed();
    if (mins < 60)
        return mins + " min ago";

    var hours = (mins / 60).toFixed();
    if (hours < 24)
        return hours + " hours ago";

    var days = (hours / 24).toFixed();
    if (days < 7)
        return days + " days ago";

    var month = date.getMonth() + 1;
    var day = date.getDate();
    var year = date.getFullYear();

    return day + "." + month + "." + year;

}

$(document).ready(function () {
    function updateDate() {
        $('span').filter(function () {
            return $(this).data('date') !== undefined;
        }).each(function (ind, item) {
            var date = $(item).data('date');
            $(item).text(displayDate(new Date(date)));
        });
    }

    updateDate();

    window.setInterval(updateDate, 30000);
});



