namespace Paybills.API.Application.Controllers
{
    public class Consts
    {
        public const string VerificationEmail = 
            """
            <!doctype html> 
                <html lang="en">
                <head>
                    <meta charset="utf-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1">
                    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-QWTKZyjpPEjISv5WaRU9OFeRpok6YctnYmDr5pNlyT2bRjXh0JMhjY6hW+ALEwIH" crossorigin="anonymous">
                </head>
                <body>
                    <div class="container">
                        <div class="row">
                            <div class="container shadow p-3 mb-5 mt-2 bg-body-tertiary rounded col-lg-8 col-12">
                                    <div class="title">
                                        <span class="badge text-bg-primary fs-4 w-100 p-3">Welcome to Billminder</span>
                                    </div>
                                    <div class="email mt-3">
                                            <p>
                                                Hello <span class="fw-bold">{username}</span>! Your email has been configured on <a href="https://billminder.com.br">billminder.com.br</a>.
                                            </p>
                                            <p>Please click this link to validate: <a href="https://billminder.com.br/validate?email=<email>&emailToken=<email-token>">Confirm email</a></p>
                                    </div>
                                    <div class="footer">
                                        <p class="fst-italic">The activation link will expire in 24 hours</p>
                                    </div>
                            </div>
                        </div>
                    </div>
                </body>
                </html>
            """;
    }
}