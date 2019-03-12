CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);


DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190304103242_Initial') THEN
    CREATE TABLE "SwimmingPools" (
        "Id" serial NOT NULL,
        "ShortName" text NULL,
        "Name" text NULL,
        CONSTRAINT "PK_SwimmingPools" PRIMARY KEY ("Id")
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190304103242_Initial') THEN
    CREATE TABLE "Schedules" (
        "Id" serial NOT NULL,
        "Day" timestamp without time zone NOT NULL,
        "Time" character varying(5) NOT NULL,
        "Tracks" text[] NULL,
        "PoolId" integer NULL,
        CONSTRAINT "PK_Schedules" PRIMARY KEY ("Id"),
        CONSTRAINT "AK_Schedules_Day_Time" UNIQUE ("Day", "Time"),
        CONSTRAINT "FK_Schedules_SwimmingPools_PoolId" FOREIGN KEY ("PoolId") REFERENCES "SwimmingPools" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190304103242_Initial') THEN
    CREATE INDEX "IX_Schedules_PoolId" ON "Schedules" ("PoolId");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190304103242_Initial') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20190304103242_Initial', '2.2.0-rtm-35687');
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190312082029_Address') THEN
    ALTER TABLE "SwimmingPools" ADD "AddressId" integer NULL;
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190312082029_Address') THEN
    CREATE TABLE "Address" (
        "Id" serial NOT NULL,
        CONSTRAINT "PK_Address" PRIMARY KEY ("Id")
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190312082029_Address') THEN
    CREATE INDEX "IX_SwimmingPools_AddressId" ON "SwimmingPools" ("AddressId");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190312082029_Address') THEN
    ALTER TABLE "SwimmingPools" ADD CONSTRAINT "FK_SwimmingPools_Address_AddressId" FOREIGN KEY ("AddressId") REFERENCES "Address" ("Id") ON DELETE RESTRICT;
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190312082029_Address') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20190312082029_Address', '2.2.0-rtm-35687');
    END IF;
END $$;
