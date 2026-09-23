const token =
    sessionStorage.getItem("jwt");

// Pas de JWT => retour Login
if (!token) {

    window.location.href = "/Login";
}


async function ask() {

    const question =
        document.getElementById("question")
            .value;

    const answer =
        document.getElementById("answer");

    if (!question) {

        answer.textContent =
            "Veuillez saisir une question.";

        return;
    }

    answer.textContent =
        "Gemini réfléchit...";

    try {

        const response =
            await fetch(
                "/api/chat",
                {
                    method: "POST",

                    headers: {

                        "Content-Type":
                            "application/json",

                        "Authorization":
                            `Bearer ${token}`
                    },

                    body: JSON.stringify({
                        message: question
                    })
                });

        // JWT invalide ou expiré
        if (response.status === 401) {

            sessionStorage.removeItem("jwt");

            window.location.href =
                "/Login";

            return;
        }

        if (!response.ok) {

            answer.textContent =
                "Erreur serveur.";

            return;
        }

        const data =
            await response.json();

        answer.textContent =
            data.answer;

    }
    catch (error) {

        console.error(error);

        answer.textContent =
            "Erreur de communication avec le serveur.";
    }
}


function logout() {

    sessionStorage.removeItem("jwt");

    window.location.href = "/Login";
}