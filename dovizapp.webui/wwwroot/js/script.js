// Currency Delete
$(".currencyDeleteModalBtn").click(function (){
    var url = $(this).attr("data-target");
    console.log(url);

    $(".currencyHardDeleteButon").attr("href", url.toString());
});