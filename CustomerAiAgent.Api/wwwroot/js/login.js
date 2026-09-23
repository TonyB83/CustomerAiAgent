async function login()
{

    const username =
        document.getElementById("username").value;

    const password =
        document.getElementById("password").value;

    const error =
        document.getElementById("error");

    error.textContent = "";

    if (!username || !password)
    {

        error.textContent =
            "Veuillez saisir vos identifiants.";

        return;
    }

    try
    {

        const response = await fetch(
            "/api/auth/login",
            {
        method: "POST",

                headers:
            {
                "Content-Type": "application/json"
                },

                body: JSON.stringify({
username: username,
                    password: password
                })
            }
        );

if (!response.ok)
{

    error.textContent =
        "Utilisateur ou mot de passe incorrect.";

    return;
}

const data =
    await response.json();

// Stockage du JWT
sessionStorage.setItem(
    "jwt",
    data.access_token
);

// Redirection vers Chat
window.location.href = "/Chat";

    }
    catch (e) {

    console.error(e);

    error.textContent =
        "Impossible de contacter le serveur.";
}
}