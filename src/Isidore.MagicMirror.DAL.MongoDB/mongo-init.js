db.createUser(
    {
        user: "mirror_user",
        pwd: "eQrqUs1BbXx2",
        roles: [
            {
                role: "readWrite",
                db: "magicmirror"
            }
        ]
    }
);

