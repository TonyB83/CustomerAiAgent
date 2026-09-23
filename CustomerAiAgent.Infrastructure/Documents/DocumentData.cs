namespace CustomerAiAgent.Infrastructure.AI.Documents;

public static class DocumentData
{
    public static List<Document> Documents { get; } =
    [
        new Document
        {
            Id = 1,
            FileName = "procedure-sav.txt",
            Title = "Procédure SAV",
            Content = """
                PROCÉDURE SAV

                Modification d'une adresse de livraison :

                Un client peut demander la modification de son
                adresse de livraison tant que la commande n'a pas
                été expédiée.

                Une fois la commande expédiée, l'adresse ne peut
                plus être modifiée directement.

                Dans ce cas, le client doit contacter le service SAV.

                Le conseiller doit vérifier l'identité du client
                avant toute modification.
                """
        },

        new Document
        {
            Id = 2,
            FileName = "conditions-remboursement.txt",
            Title = "Conditions de remboursement",
            Content = """
                CONDITIONS DE REMBOURSEMENT

                Le client peut demander un remboursement dans un
                délai de 14 jours après réception de la commande.

                Le produit doit être retourné dans son état d'origine.

                Les frais de retour peuvent être déduits du montant
                du remboursement selon les conditions applicables.
                """
        },

        new Document
        {
            Id = 3,
            FileName = "procedure-annulation.txt",
            Title = "Procédure d'annulation",
            Content = """
                PROCÉDURE D'ANNULATION

                Une commande peut être annulée tant qu'elle est
                dans le statut Pending.

                Une commande déjà expédiée ne peut normalement
                plus être annulée.

                Pour une demande exceptionnelle, le client doit
                contacter le service SAV.
                """
        },

        new Document
        {
            Id = 4,
            FileName = "procedure-livraison.txt",
            Title = "Procédure de livraison",
            Content = """
                PROCÉDURE DE LIVRAISON

                Les commandes sont préparées avant leur expédition.

                Lorsque la commande passe au statut Shipped,
                elle est considérée comme expédiée.

                Après expédition, le client reçoit une notification
                contenant les informations de livraison.
                """
        },

        new Document
        {
            Id = 5,
            FileName = "procedure-client.txt",
            Title = "Gestion des informations client",
            Content = """
                GESTION DES INFORMATIONS CLIENT

                Le client peut demander la modification de certaines
                informations personnelles.

                Toute modification doit être effectuée après
                vérification de l'identité du client.

                L'adresse email peut être modifiée depuis le compte
                client ou avec l'assistance du service SAV.
                """
        }
    ];
}