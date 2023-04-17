// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification


//$(function () {
/*    
    var PlaceHolderElement = $('#PlaceHolderForModal');
    console.log("clicked", PlaceHolderElement);
    $('button[data-bs-toggle="ajax-modal"]').click(function (event) {
        var url = $(this).data('bs-url');
       // console.log(event);
       // console.log("Url:", url);
        $.get(url).done(function (data) {
            PlaceHolderElement.html(data);
            PlaceHolderElement.find('.modal').modal('show');
        })
    })

    PlaceHolderElement.on('click', '[data-bs-save="modal"]', function (event) {
       // console.log("event:",event);
        var form = $(this).parents('.modal').find('form');
        //console.log("form:", form);
        var actionUrl = form.attr('action');
        var sendData = form.serialize();
        console.log("save button clicked");
        console.log("actionUrl: ", actionUrl);
        console.log("sendData:", sendData);

        $.post(actionUrl, sendData).done(function (data) {
           // console.log("data:", data);
                PlaceHolderElement.find('.modal').modal('.hide');
            })
    })
})*/