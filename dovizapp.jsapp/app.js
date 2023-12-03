// ** Asynchronous Method - IIFE (Immediately Invoked Function Expression)
(() => {
    var apiButton = document.querySelector("#apiButton");
    var tableContainer = document.querySelector("#tableContainer");
    var alertMessage = document.querySelector("#errorMessage")

    tableContainer.style.display = "none";
    alertMessage.style.display = "none"; 
    

    apiButton.addEventListener("click", () => {
        tableContainer.style.display = "block";
        login();
    });
})();


// ** Uygulama içerisindeki Login bilgileri ve bizlere dönen Token bilgisi, Session veya Local Storage'e aktarılarak dinamik bir şekilde uygulamaya aktarılabilirdi.
// ** Ancak bu işlem, uygulamayı daha da uzatabileceği için bu işlemler static bir şekilde yapılmıştır.
const login = async () => {
    var user = {
        UserName: "atakann_alkan",
        Password: "deneme1"
    }

    fetch("http://localhost:4000/api/auth/login", { method: "POST", headers: {"Content-Type": "application/json"}, body: JSON.stringify(user) })
    .then(response => response.text())
    .then(result => getCurrencies(result))
    .catch(error => displayErrorMessage(error));
}

const getCurrencies = async (token) => {
    fetch("http://localhost:4000/api/currency", { method: "GET", headers: {"Content-Type": "application/json", "Authorization": `Bearer ${token}`} })
    .then(response => response.text()) // "json" yerine "text" kullanıyoruz çünkü, bize "json" değil bir string yani "text" tipinde bir değer dönüyor.
    .then(result => setCurrencies(JSON.parse(result)))
    .catch(error => displayErrorMessage(error));
}



// ** OTHER OPERATIONS
const setCurrencies = (currencies) => {
    const container = document.querySelector("#currencies");

    container.innerHTML = ""; // Eski kayıtlar var ise Tablonun Body'sini boşaltıp, sonrasında yeni kayıtları ekliyorum.
    currencies.forEach(currency => {
        container.innerHTML += `
            <tr>
                <td>${currency.currencyId}</td>
                <td>${currency.currencyCode}</td>
                <td>${currency.name}</td>
                <td>${currency.unit}</td>
                <td>${currency.forexBuying}</td>
                <td>${currency.forexSelling}</td>
                <td>${currency.banknoteBuying}</td>
                <td>${currency.banknoteSelling}</td>
            </tr>
        `;
    });
}

const displayErrorMessage = (message) => {
    var alertMessage = document.querySelector("#errorMessage");    
    alertMessage.style.display = "block"; 
    alertMessage.querySelector("span").innerHTML = message;
}
