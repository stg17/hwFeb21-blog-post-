$(() => {

    $("input").on("input", function () {
        validateButton();
    }) 

    $("textarea").on("input", function () {
        validateButton();
    })

    function validateButton() {
        const name = $("#commenterName").val().trim();
        const text = $("#text").val().trim();
        const validate = name && text;
        $("#submit").prop('disabled', !validate);
    }

})