--
-- PostgreSQL database dump
--

\restrict GRIK32g67YKcZkqS2RrmhNgTOAhnhDTNljowt4O8OEnM1lYkCrTwaSRCnY92hKt

-- Dumped from database version 17.6
-- Dumped by pg_dump version 18.4

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: hangfire; Type: SCHEMA; Schema: -; Owner: postgres
--

CREATE SCHEMA hangfire;


ALTER SCHEMA hangfire OWNER TO postgres;

--
-- Name: public; Type: SCHEMA; Schema: -; Owner: pg_database_owner
--

CREATE SCHEMA public;


ALTER SCHEMA public OWNER TO pg_database_owner;

--
-- Name: SCHEMA public; Type: COMMENT; Schema: -; Owner: pg_database_owner
--

COMMENT ON SCHEMA public IS 'standard public schema';


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: aggregatedcounter; Type: TABLE; Schema: hangfire; Owner: postgres
--

CREATE TABLE hangfire.aggregatedcounter (
    id bigint NOT NULL,
    key text NOT NULL,
    value bigint NOT NULL,
    expireat timestamp with time zone
);


ALTER TABLE hangfire.aggregatedcounter OWNER TO postgres;

--
-- Name: aggregatedcounter_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: postgres
--

CREATE SEQUENCE hangfire.aggregatedcounter_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE hangfire.aggregatedcounter_id_seq OWNER TO postgres;

--
-- Name: aggregatedcounter_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: postgres
--

ALTER SEQUENCE hangfire.aggregatedcounter_id_seq OWNED BY hangfire.aggregatedcounter.id;


--
-- Name: counter; Type: TABLE; Schema: hangfire; Owner: postgres
--

CREATE TABLE hangfire.counter (
    id bigint NOT NULL,
    key text NOT NULL,
    value bigint NOT NULL,
    expireat timestamp with time zone
);


ALTER TABLE hangfire.counter OWNER TO postgres;

--
-- Name: counter_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: postgres
--

CREATE SEQUENCE hangfire.counter_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE hangfire.counter_id_seq OWNER TO postgres;

--
-- Name: counter_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: postgres
--

ALTER SEQUENCE hangfire.counter_id_seq OWNED BY hangfire.counter.id;


--
-- Name: hash; Type: TABLE; Schema: hangfire; Owner: postgres
--

CREATE TABLE hangfire.hash (
    id bigint NOT NULL,
    key text NOT NULL,
    field text NOT NULL,
    value text,
    expireat timestamp with time zone,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.hash OWNER TO postgres;

--
-- Name: hash_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: postgres
--

CREATE SEQUENCE hangfire.hash_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE hangfire.hash_id_seq OWNER TO postgres;

--
-- Name: hash_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: postgres
--

ALTER SEQUENCE hangfire.hash_id_seq OWNED BY hangfire.hash.id;


--
-- Name: job; Type: TABLE; Schema: hangfire; Owner: postgres
--

CREATE TABLE hangfire.job (
    id bigint NOT NULL,
    stateid bigint,
    statename text,
    invocationdata jsonb NOT NULL,
    arguments jsonb NOT NULL,
    createdat timestamp with time zone NOT NULL,
    expireat timestamp with time zone,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.job OWNER TO postgres;

--
-- Name: job_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: postgres
--

CREATE SEQUENCE hangfire.job_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE hangfire.job_id_seq OWNER TO postgres;

--
-- Name: job_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: postgres
--

ALTER SEQUENCE hangfire.job_id_seq OWNED BY hangfire.job.id;


--
-- Name: jobparameter; Type: TABLE; Schema: hangfire; Owner: postgres
--

CREATE TABLE hangfire.jobparameter (
    id bigint NOT NULL,
    jobid bigint NOT NULL,
    name text NOT NULL,
    value text,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.jobparameter OWNER TO postgres;

--
-- Name: jobparameter_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: postgres
--

CREATE SEQUENCE hangfire.jobparameter_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE hangfire.jobparameter_id_seq OWNER TO postgres;

--
-- Name: jobparameter_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: postgres
--

ALTER SEQUENCE hangfire.jobparameter_id_seq OWNED BY hangfire.jobparameter.id;


--
-- Name: jobqueue; Type: TABLE; Schema: hangfire; Owner: postgres
--

CREATE TABLE hangfire.jobqueue (
    id bigint NOT NULL,
    jobid bigint NOT NULL,
    queue text NOT NULL,
    fetchedat timestamp with time zone,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.jobqueue OWNER TO postgres;

--
-- Name: jobqueue_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: postgres
--

CREATE SEQUENCE hangfire.jobqueue_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE hangfire.jobqueue_id_seq OWNER TO postgres;

--
-- Name: jobqueue_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: postgres
--

ALTER SEQUENCE hangfire.jobqueue_id_seq OWNED BY hangfire.jobqueue.id;


--
-- Name: list; Type: TABLE; Schema: hangfire; Owner: postgres
--

CREATE TABLE hangfire.list (
    id bigint NOT NULL,
    key text NOT NULL,
    value text,
    expireat timestamp with time zone,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.list OWNER TO postgres;

--
-- Name: list_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: postgres
--

CREATE SEQUENCE hangfire.list_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE hangfire.list_id_seq OWNER TO postgres;

--
-- Name: list_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: postgres
--

ALTER SEQUENCE hangfire.list_id_seq OWNED BY hangfire.list.id;


--
-- Name: lock; Type: TABLE; Schema: hangfire; Owner: postgres
--

CREATE TABLE hangfire.lock (
    resource text NOT NULL,
    updatecount integer DEFAULT 0 NOT NULL,
    acquired timestamp with time zone
);


ALTER TABLE hangfire.lock OWNER TO postgres;

--
-- Name: schema; Type: TABLE; Schema: hangfire; Owner: postgres
--

CREATE TABLE hangfire.schema (
    version integer NOT NULL
);


ALTER TABLE hangfire.schema OWNER TO postgres;

--
-- Name: server; Type: TABLE; Schema: hangfire; Owner: postgres
--

CREATE TABLE hangfire.server (
    id text NOT NULL,
    data jsonb,
    lastheartbeat timestamp with time zone NOT NULL,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.server OWNER TO postgres;

--
-- Name: set; Type: TABLE; Schema: hangfire; Owner: postgres
--

CREATE TABLE hangfire.set (
    id bigint NOT NULL,
    key text NOT NULL,
    score double precision NOT NULL,
    value text NOT NULL,
    expireat timestamp with time zone,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.set OWNER TO postgres;

--
-- Name: set_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: postgres
--

CREATE SEQUENCE hangfire.set_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE hangfire.set_id_seq OWNER TO postgres;

--
-- Name: set_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: postgres
--

ALTER SEQUENCE hangfire.set_id_seq OWNED BY hangfire.set.id;


--
-- Name: state; Type: TABLE; Schema: hangfire; Owner: postgres
--

CREATE TABLE hangfire.state (
    id bigint NOT NULL,
    jobid bigint NOT NULL,
    name text NOT NULL,
    reason text,
    createdat timestamp with time zone NOT NULL,
    data jsonb,
    updatecount integer DEFAULT 0 NOT NULL
);


ALTER TABLE hangfire.state OWNER TO postgres;

--
-- Name: state_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: postgres
--

CREATE SEQUENCE hangfire.state_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE hangfire.state_id_seq OWNER TO postgres;

--
-- Name: state_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: postgres
--

ALTER SEQUENCE hangfire.state_id_seq OWNED BY hangfire.state.id;


--
-- Name: BookingDiscounts; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."BookingDiscounts" (
    "BookingDiscountId" integer NOT NULL,
    "BookingId" uuid NOT NULL,
    "Name" character varying(200) NOT NULL,
    "DiscountAmount" numeric(18,2) NOT NULL,
    "IsAutoApplied" boolean NOT NULL,
    "AppliedDate" timestamp without time zone NOT NULL,
    "PromotionId" integer,
    "LoyaltyTierId" integer,
    "LoyaltyTransactionId" integer
);


ALTER TABLE public."BookingDiscounts" OWNER TO postgres;

--
-- Name: BookingDiscounts_BookingDiscountId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."BookingDiscounts" ALTER COLUMN "BookingDiscountId" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."BookingDiscounts_BookingDiscountId_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: BookingHistories; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."BookingHistories" (
    "BookingHistoryId" uuid NOT NULL,
    "BookingId" uuid NOT NULL,
    "EventType" text NOT NULL,
    "Payload" text,
    "ActorId" uuid,
    "CreatedAt" timestamp without time zone NOT NULL
);


ALTER TABLE public."BookingHistories" OWNER TO postgres;

--
-- Name: BookingItems; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."BookingItems" (
    "BookingItemId" uuid NOT NULL,
    "BookingId" uuid NOT NULL,
    "ServiceId" uuid,
    "NailVariantId" integer,
    "Quantity" integer NOT NULL,
    "Price" numeric(18,2) NOT NULL,
    "Duration" integer NOT NULL,
    "CustomerNailRequestId" uuid,
    "ShapeMethodConfigId" integer
);


ALTER TABLE public."BookingItems" OWNER TO postgres;

--
-- Name: BookingProcedures; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."BookingProcedures" (
    "BookingProcedureId" uuid NOT NULL,
    "BookingItemId" uuid NOT NULL,
    "ProcedureId" uuid,
    "ProcedureName" character varying(200) NOT NULL,
    "Description" text,
    "StepOrder" integer NOT NULL,
    "Status" character varying(20) NOT NULL,
    "CompletedAt" timestamp without time zone,
    "CompletedById" uuid,
    "IsRequired" boolean DEFAULT true NOT NULL,
    "ActiveDuration" integer DEFAULT 0 NOT NULL,
    "ActualEndTime" timestamp without time zone,
    "ActualStartTime" timestamp without time zone,
    "AssignedArtistId" uuid,
    "CanOverlap" boolean DEFAULT false NOT NULL,
    "Duration" integer DEFAULT 0 NOT NULL,
    "EstimatedEndTime" interval,
    "EstimatedStartTime" interval,
    "PassiveDuration" integer DEFAULT 0 NOT NULL,
    "IsMainStep" boolean DEFAULT false NOT NULL,
    "TransitionBuffer" integer DEFAULT 0 NOT NULL
);


ALTER TABLE public."BookingProcedures" OWNER TO postgres;

--
-- Name: BookingRatings; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."BookingRatings" (
    "BookingRatingId" uuid DEFAULT gen_random_uuid() NOT NULL,
    "BookingId" uuid NOT NULL,
    "CustomerId" uuid NOT NULL,
    "OverallScore" integer NOT NULL,
    "Comment" character varying(1000),
    "ImageUrl" character varying(500),
    "ServiceQuality" integer,
    "Punctuality" integer,
    "Cleanliness" integer,
    "IsUpdated" boolean NOT NULL,
    "Status" character varying(20) DEFAULT 'Active'::character varying NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "UpdatedAt" timestamp without time zone,
    "DeletedAt" timestamp without time zone,
    CONSTRAINT "CK_BookingRating_Scores" CHECK (((("OverallScore" >= 1) AND ("OverallScore" <= 5)) AND (("ServiceQuality" IS NULL) OR (("ServiceQuality" >= 1) AND ("ServiceQuality" <= 5))) AND (("Punctuality" IS NULL) OR (("Punctuality" >= 1) AND ("Punctuality" <= 5))) AND (("Cleanliness" IS NULL) OR (("Cleanliness" >= 1) AND ("Cleanliness" <= 5)))))
);


ALTER TABLE public."BookingRatings" OWNER TO postgres;

--
-- Name: BookingWaitlists; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."BookingWaitlists" (
    "WailistId" uuid NOT NULL,
    "CustomerId" uuid NOT NULL,
    "SalonId" uuid NOT NULL,
    "PreferredNailArtistId" uuid,
    "RequesetedDate" timestamp without time zone NOT NULL,
    "RequestedStartTime" interval NOT NULL,
    "EstimatedDuration" integer NOT NULL,
    "Position" integer NOT NULL,
    "Status" character varying(20) NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "NotifiedAt" timestamp without time zone,
    "ExpiresAt" timestamp without time zone,
    "ConvertedBookingId" uuid
);


ALTER TABLE public."BookingWaitlists" OWNER TO postgres;

--
-- Name: Bookings; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Bookings" (
    "BookingId" uuid NOT NULL,
    "CustomerId" uuid NOT NULL,
    "SalonId" uuid NOT NULL,
    "NailArtistId" uuid,
    "BookingDate" timestamp without time zone NOT NULL,
    "StartTime" interval NOT NULL,
    "TotalPrice" numeric(18,2),
    "Status" character varying(20) NOT NULL,
    "Price" numeric(18,2),
    "TotalDuration" integer NOT NULL,
    "UpdatedAt" timestamp without time zone,
    "CheckInImageUrl" text,
    "CheckOutImagesUrl" text,
    "QRCode" text,
    "Discount" numeric(18,2),
    "IsRated" boolean DEFAULT false NOT NULL,
    "ActualCheckInTime" timestamp without time zone,
    "ActualStartTime" timestamp without time zone,
    "IsLateArrival" boolean DEFAULT false NOT NULL,
    "IsRefunded" boolean DEFAULT false NOT NULL,
    "ChairId" uuid,
    "AmountDue" numeric,
    "AmountPaid" numeric,
    "ProposedBookingDate" timestamp without time zone,
    "ProposedBy" text,
    "ProposedStartTime" interval,
    "RescheduleReason" text,
    "WarrantyForBookingId" uuid
);


ALTER TABLE public."Bookings" OWNER TO postgres;

--
-- Name: Categories; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Categories" (
    "CategoryId" integer NOT NULL,
    "Name" text NOT NULL,
    "CategoryTypeId" integer NOT NULL,
    "Status" text DEFAULT 'Active'::text NOT NULL
);


ALTER TABLE public."Categories" OWNER TO postgres;

--
-- Name: Categories_CategoryId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."Categories" ALTER COLUMN "CategoryId" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Categories_CategoryId_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: CategoryTypes; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."CategoryTypes" (
    "CategoryTypeId" integer NOT NULL,
    "Name" text NOT NULL,
    "Status" text DEFAULT 'Active'::text NOT NULL
);


ALTER TABLE public."CategoryTypes" OWNER TO postgres;

--
-- Name: CategoryTypes_CategoryTypeId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."CategoryTypes" ALTER COLUMN "CategoryTypeId" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."CategoryTypes_CategoryTypeId_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: Chairs; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Chairs" (
    "ChairId" uuid NOT NULL,
    "SalonId" uuid NOT NULL,
    "ChairName" character varying(100) NOT NULL,
    "Status" character varying(30) DEFAULT 'Active'::character varying NOT NULL
);


ALTER TABLE public."Chairs" OWNER TO postgres;

--
-- Name: Components; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Components" (
    "ComponentId" integer NOT NULL,
    "Name" text NOT NULL,
    "ImageUrl" text NOT NULL,
    "ComponentType" integer NOT NULL,
    "Price" numeric(18,2) NOT NULL,
    "Duration" integer,
    "Status" text DEFAULT 'Active'::text NOT NULL
);


ALTER TABLE public."Components" OWNER TO postgres;

--
-- Name: Components_ComponentId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."Components" ALTER COLUMN "ComponentId" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Components_ComponentId_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: CustomerComponents; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."CustomerComponents" (
    "CustomerComponentId" integer NOT NULL,
    "UserId" uuid NOT NULL,
    "Name" text NOT NULL,
    "ImageUrl" text NOT NULL,
    "ComponentType" integer NOT NULL,
    "Price" numeric(18,2),
    "CreatedAt" timestamp without time zone NOT NULL,
    "Status" text DEFAULT 'Active'::text NOT NULL
);


ALTER TABLE public."CustomerComponents" OWNER TO postgres;

--
-- Name: CustomerComponents_CustomerComponentId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."CustomerComponents" ALTER COLUMN "CustomerComponentId" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."CustomerComponents_CustomerComponentId_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: CustomerNailComponents; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."CustomerNailComponents" (
    "CustomerNailComponentId" integer NOT NULL,
    "CustomerNailId" integer NOT NULL,
    "ComponentId" integer,
    "CustomerComponentId" integer,
    "PosX" numeric(18,2) NOT NULL,
    "PosY" numeric(18,2) NOT NULL,
    "FingerIndex" integer NOT NULL,
    "ConfigJson" text NOT NULL,
    CONSTRAINT "CK_CustomerNailComponent_OneComponent" CHECK (((("ComponentId" IS NOT NULL) AND ("CustomerComponentId" IS NULL)) OR (("ComponentId" IS NULL) AND ("CustomerComponentId" IS NOT NULL))))
);


ALTER TABLE public."CustomerNailComponents" OWNER TO postgres;

--
-- Name: COLUMN "CustomerNailComponents"."FingerIndex"; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public."CustomerNailComponents"."FingerIndex" IS '-1 = whole hand, 0-9 = specific finger index';


--
-- Name: CustomerNailComponents_CustomerNailComponentId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."CustomerNailComponents" ALTER COLUMN "CustomerNailComponentId" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."CustomerNailComponents_CustomerNailComponentId_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: CustomerNailRequests; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."CustomerNailRequests" (
    "CustomerNailRequestId" uuid DEFAULT gen_random_uuid() NOT NULL,
    "CustomerNailId" integer NOT NULL,
    "SalonId" uuid NOT NULL,
    "Status" character varying(30) NOT NULL,
    "RejectReason" text,
    "ApprovedArtistId" uuid,
    "Price" numeric(18,2),
    "Duration" integer,
    "CreatedAt" timestamp without time zone NOT NULL,
    "UpdatedAt" timestamp without time zone,
    "IsCustomerRequest" boolean DEFAULT false NOT NULL
);


ALTER TABLE public."CustomerNailRequests" OWNER TO postgres;

--
-- Name: CustomerNails; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."CustomerNails" (
    "CustomerNailId" integer NOT NULL,
    "UserId" uuid NOT NULL,
    "Name" text NOT NULL,
    "ImageUrl" text NOT NULL,
    "NailShapeId" integer,
    "NailSurfaceId" integer,
    "Price" numeric(18,2),
    "CustomColor" text,
    "Duration" integer,
    "CreatedAt" timestamp without time zone NOT NULL,
    "Status" character varying(30) DEFAULT 'Active'::character varying NOT NULL
);


ALTER TABLE public."CustomerNails" OWNER TO postgres;

--
-- Name: CustomerNails_CustomerNailId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."CustomerNails" ALTER COLUMN "CustomerNailId" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."CustomerNails_CustomerNailId_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: CustomerQuizAnswers; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."CustomerQuizAnswers" (
    "CustomerQuizAnswerId" uuid NOT NULL,
    "CustomerId" uuid NOT NULL,
    "QuizQuestionId" uuid NOT NULL,
    "QuizOptionId" uuid NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL
);


ALTER TABLE public."CustomerQuizAnswers" OWNER TO postgres;

--
-- Name: Customers; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Customers" (
    "UserId" uuid NOT NULL,
    "LoyaltyPoint" integer DEFAULT 0 NOT NULL,
    "SkinTone" character varying(100) DEFAULT ''::character varying,
    "Occupation" character varying(250) DEFAULT ''::character varying,
    "NailCondition" character varying(500) DEFAULT ''::character varying,
    "LifetimePoints" integer DEFAULT 0 NOT NULL,
    "LoyaltyTierId" integer DEFAULT 1,
    "PreferredColorsJson" character varying(500) DEFAULT ''::character varying NOT NULL,
    "PreferredComplexity" character varying(50) DEFAULT ''::character varying NOT NULL,
    "PreferredNailShapeId" integer,
    "PreferredOccasionsJson" character varying(500) DEFAULT ''::character varying NOT NULL,
    "PreferredStylesJson" character varying(500) DEFAULT ''::character varying NOT NULL,
    "HandShape" character varying(100) DEFAULT ''::character varying,
    "SkinShade" character varying(100) DEFAULT ''::character varying
);


ALTER TABLE public."Customers" OWNER TO postgres;

--
-- Name: FavoriteNails; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."FavoriteNails" (
    "FavoriteNailId" integer NOT NULL,
    "UserId" uuid NOT NULL,
    "NailDesignId" integer,
    "NailVariantId" integer,
    "CreatedAt" timestamp without time zone NOT NULL,
    CONSTRAINT "CK_FavoriteNail_DesignOrVariant" CHECK ((("NailDesignId" IS NOT NULL) OR ("NailVariantId" IS NOT NULL)))
);


ALTER TABLE public."FavoriteNails" OWNER TO postgres;

--
-- Name: FavoriteNails_FavoriteNailId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."FavoriteNails" ALTER COLUMN "FavoriteNailId" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."FavoriteNails_FavoriteNailId_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: LoyaltyTiers; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."LoyaltyTiers" (
    "LoyaltyTierId" integer NOT NULL,
    "Name" text NOT NULL,
    "Description" text NOT NULL,
    "MinLifetimePoints" integer,
    "MaxLifetimePoints" integer,
    "DiscountRate" numeric NOT NULL,
    "ImageUrl" text,
    "BackgroundColor" text,
    "TextColor" text,
    "ColorJson" text,
    "Status" text DEFAULT 'Active'::text NOT NULL,
    "SortOrder" integer
);


ALTER TABLE public."LoyaltyTiers" OWNER TO postgres;

--
-- Name: LoyaltyTiers_LoyaltyTierId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."LoyaltyTiers" ALTER COLUMN "LoyaltyTierId" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."LoyaltyTiers_LoyaltyTierId_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: LoyaltyTransactions; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."LoyaltyTransactions" (
    "LoyaltyTransactionId" integer NOT NULL,
    "CustomerId" uuid NOT NULL,
    "BookingId" uuid,
    "Points" integer NOT NULL,
    "TransactionType" text DEFAULT 'Earned'::text NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "Description" text
);


ALTER TABLE public."LoyaltyTransactions" OWNER TO postgres;

--
-- Name: LoyaltyTransactions_LoyaltyTransactionId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."LoyaltyTransactions" ALTER COLUMN "LoyaltyTransactionId" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."LoyaltyTransactions_LoyaltyTransactionId_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: NailArtistBreaks; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."NailArtistBreaks" (
    "NailArtistBreakId" uuid NOT NULL,
    "NailArtistId" uuid NOT NULL,
    "BreakDate" timestamp without time zone NOT NULL,
    "StartTime" interval NOT NULL,
    "EndTime" interval NOT NULL,
    "Reason" character varying(500),
    "Status" character varying(30) DEFAULT 'Pending'::character varying NOT NULL,
    "RejectReason" text
);


ALTER TABLE public."NailArtistBreaks" OWNER TO postgres;

--
-- Name: NailArtistSkills; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."NailArtistSkills" (
    "NailArtistSkillId" uuid NOT NULL,
    "NailArtistId" uuid NOT NULL,
    "SkillTypeId" uuid NOT NULL,
    "Level" integer NOT NULL
);


ALTER TABLE public."NailArtistSkills" OWNER TO postgres;

--
-- Name: NailArtists; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."NailArtists" (
    "NailArtistId" uuid NOT NULL,
    "AccountId" uuid NOT NULL,
    "Status" text DEFAULT 'Active'::text NOT NULL,
    "ConcurrentCapacity" integer DEFAULT 0 NOT NULL
);


ALTER TABLE public."NailArtists" OWNER TO postgres;

--
-- Name: NailCategories; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."NailCategories" (
    "NailCategoryId" integer NOT NULL,
    "NailDesignId" integer NOT NULL,
    "CategoryId" integer NOT NULL
);


ALTER TABLE public."NailCategories" OWNER TO postgres;

--
-- Name: NailCategories_NailCategoryId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."NailCategories" ALTER COLUMN "NailCategoryId" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."NailCategories_NailCategoryId_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: NailComponents; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."NailComponents" (
    "NailComponentId" integer NOT NULL,
    "ComponentId" integer NOT NULL,
    "NailVariantId" integer NOT NULL,
    "PosX" numeric(18,2) NOT NULL,
    "PosY" numeric(18,2) NOT NULL,
    "FingerIndex" integer NOT NULL,
    "ConfigJson" text NOT NULL
);


ALTER TABLE public."NailComponents" OWNER TO postgres;

--
-- Name: COLUMN "NailComponents"."FingerIndex"; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public."NailComponents"."FingerIndex" IS '-1 = whole hand, 0-9 = specific finger index';


--
-- Name: NailComponents_NailComponentId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."NailComponents" ALTER COLUMN "NailComponentId" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."NailComponents_NailComponentId_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: NailDesigns; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."NailDesigns" (
    "NailDesignId" integer NOT NULL,
    "Name" text NOT NULL,
    "MinPrice" numeric(18,2) NOT NULL,
    "Description" text NOT NULL,
    "Status" text DEFAULT 'Active'::text NOT NULL,
    "MaxPrice" numeric(18,2) DEFAULT 0.0 NOT NULL,
    "ImageUrl" character varying(500) DEFAULT ''::character varying NOT NULL
);


ALTER TABLE public."NailDesigns" OWNER TO postgres;

--
-- Name: NailDesigns_NailDesignId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."NailDesigns" ALTER COLUMN "NailDesignId" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."NailDesigns_NailDesignId_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: NailProcedures; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."NailProcedures" (
    "NailProcedureId" uuid NOT NULL,
    "NailVariantId" integer,
    "ProcedureId" uuid,
    "StepOrder" integer NOT NULL,
    "Status" text DEFAULT 'Active'::text NOT NULL,
    "CustomerNailId" integer,
    "EstimatedMinutes" integer,
    "IsCustomStep" boolean DEFAULT false NOT NULL,
    "Name" text,
    "Note" text,
    "Price" numeric
);


ALTER TABLE public."NailProcedures" OWNER TO postgres;

--
-- Name: NailRequiredSkills; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."NailRequiredSkills" (
    "NailRequiredSkillId" uuid NOT NULL,
    "NailVariantId" integer NOT NULL,
    "SkillTypeId" uuid NOT NULL,
    "RequiredLevel" integer NOT NULL
);


ALTER TABLE public."NailRequiredSkills" OWNER TO postgres;

--
-- Name: NailShapes; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."NailShapes" (
    "NailShapeId" integer NOT NULL,
    "Name" text NOT NULL,
    "ImageUrl" text NOT NULL,
    "Status" text DEFAULT 'Active'::text NOT NULL
);


ALTER TABLE public."NailShapes" OWNER TO postgres;

--
-- Name: NailShapes_NailShapeId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."NailShapes" ALTER COLUMN "NailShapeId" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."NailShapes_NailShapeId_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: NailSurfaces; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."NailSurfaces" (
    "NailSurfaceId" integer NOT NULL,
    "Name" text NOT NULL,
    "ShaderParam" text NOT NULL,
    "Price" numeric(18,2) NOT NULL,
    "Duration" integer,
    "Status" text DEFAULT 'Active'::text NOT NULL,
    "FinishType" text DEFAULT ''::text NOT NULL,
    "HueOffset" real DEFAULT 0 NOT NULL,
    "LightnessOffset" real DEFAULT 0 NOT NULL,
    "SaturationOffset" real DEFAULT 0 NOT NULL
);


ALTER TABLE public."NailSurfaces" OWNER TO postgres;

--
-- Name: NailSurfaces_NailSurfaceId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."NailSurfaces" ALTER COLUMN "NailSurfaceId" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."NailSurfaces_NailSurfaceId_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: NailVariants; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."NailVariants" (
    "NailVariantId" integer NOT NULL,
    "Name" text NOT NULL,
    "NailShapeId" integer,
    "NailSurfaceId" integer,
    "NailDesignId" integer,
    "Price" numeric(18,2) NOT NULL,
    "Duration" integer,
    "ImageUrl" text DEFAULT ''::text NOT NULL,
    "ColorJson" text DEFAULT ''::text NOT NULL,
    "Status" text DEFAULT 'Active'::text NOT NULL
);


ALTER TABLE public."NailVariants" OWNER TO postgres;

--
-- Name: NailVariants_NailVariantId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."NailVariants" ALTER COLUMN "NailVariantId" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."NailVariants_NailVariantId_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: Procedures; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Procedures" (
    "ProcedureId" uuid NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Description" text,
    "Duration" integer,
    "Status" text DEFAULT 'Active'::text NOT NULL,
    "CreateAt" timestamp without time zone NOT NULL,
    "IsRequired" boolean DEFAULT true NOT NULL,
    "ActiveDuration" integer DEFAULT 0 NOT NULL,
    "CanOverlap" boolean DEFAULT false NOT NULL,
    "PassiveDuration" integer DEFAULT 0 NOT NULL,
    "IsMainStep" boolean DEFAULT false NOT NULL,
    "TransitionBuffer" integer DEFAULT 0 NOT NULL,
    "ProcedureType" integer DEFAULT 0 NOT NULL
);


ALTER TABLE public."Procedures" OWNER TO postgres;

--
-- Name: Promotions; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Promotions" (
    "PromotionId" integer NOT NULL,
    "Name" text NOT NULL,
    "Description" text NOT NULL,
    "Type" character varying(30) NOT NULL,
    "Scope" character varying(30) NOT NULL,
    "DiscountType" character varying(30) NOT NULL,
    "DiscountValue" numeric(18,2) NOT NULL,
    "CategoryId" integer,
    "CategoryTypeId" integer,
    "NailDesignId" integer,
    "StartDate" timestamp without time zone NOT NULL,
    "EndDate" timestamp without time zone,
    "Status" character varying(20) DEFAULT 'Active'::character varying NOT NULL,
    "IsSelectable" boolean NOT NULL,
    "UsageLimit" integer,
    "CurrentUsageCount" integer DEFAULT 0 NOT NULL,
    "UserLimit" integer,
    "ImageUrl" text DEFAULT ''::text NOT NULL,
    "Situation" text DEFAULT ''::text NOT NULL,
    "PointsRequired" integer
);


ALTER TABLE public."Promotions" OWNER TO postgres;

--
-- Name: Promotions_PromotionId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."Promotions" ALTER COLUMN "PromotionId" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Promotions_PromotionId_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: QuizOptions; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."QuizOptions" (
    "QuizOptionId" uuid NOT NULL,
    "QuizQuestionId" uuid NOT NULL,
    "OptionValue" character varying(100) NOT NULL,
    "Label" character varying(250) NOT NULL,
    "Description" character varying(500)
);


ALTER TABLE public."QuizOptions" OWNER TO postgres;

--
-- Name: QuizQuestions; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."QuizQuestions" (
    "QuizQuestionId" uuid NOT NULL,
    "QuestionText" character varying(500) NOT NULL,
    "Type" character varying(50) NOT NULL,
    "Category" character varying(100) NOT NULL,
    "IsActive" boolean NOT NULL
);


ALTER TABLE public."QuizQuestions" OWNER TO postgres;

--
-- Name: SalonOffDates; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."SalonOffDates" (
    "SalonOffDateId" uuid NOT NULL,
    "SalonId" uuid NOT NULL,
    "StartDate" timestamp without time zone NOT NULL,
    "EndDate" timestamp without time zone NOT NULL,
    "Description" character varying(250)
);


ALTER TABLE public."SalonOffDates" OWNER TO postgres;

--
-- Name: SalonOperatingHours; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."SalonOperatingHours" (
    "OperatingHourId" uuid NOT NULL,
    "SalonId" uuid NOT NULL,
    "DayOfWeek" integer NOT NULL,
    "OpenTime" interval NOT NULL,
    "CloseTime" interval NOT NULL,
    "IsClosed" boolean NOT NULL
);


ALTER TABLE public."SalonOperatingHours" OWNER TO postgres;

--
-- Name: Salons; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Salons" (
    "SalonId" uuid NOT NULL,
    "Name" text NOT NULL,
    "Address" text NOT NULL,
    "Phone" text NOT NULL,
    "Latitude" double precision NOT NULL,
    "Longitude" double precision NOT NULL,
    "Status" text DEFAULT 'Active'::text NOT NULL,
    "ImageUrl" text,
    "DepositConfig" numeric DEFAULT 0.0 NOT NULL
);


ALTER TABLE public."Salons" OWNER TO postgres;

--
-- Name: Schedules; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Schedules" (
    "ScheduleId" uuid NOT NULL,
    "NailArtistId" uuid NOT NULL,
    "WorkDate" timestamp without time zone NOT NULL,
    "ShiftStart" interval NOT NULL,
    "ShiftEnd" interval NOT NULL,
    "Status" text DEFAULT 'Active'::text NOT NULL
);


ALTER TABLE public."Schedules" OWNER TO postgres;

--
-- Name: Services; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Services" (
    "ServiceId" uuid NOT NULL,
    "Name" text NOT NULL,
    "Description" text,
    "Price" numeric(18,2) NOT NULL,
    "Duration" integer NOT NULL,
    "Status" text DEFAULT 'Active'::text NOT NULL,
    "CreateAt" timestamp without time zone NOT NULL
);


ALTER TABLE public."Services" OWNER TO postgres;

--
-- Name: ShapeMethodConfigs; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."ShapeMethodConfigs" (
    "ShapeMethodConfigId" integer NOT NULL,
    "NailShapeId" integer NOT NULL,
    "Name" text NOT NULL,
    "Price" numeric(18,2) NOT NULL,
    "Duration" integer NOT NULL,
    "Status" text DEFAULT 'Active'::text NOT NULL
);


ALTER TABLE public."ShapeMethodConfigs" OWNER TO postgres;

--
-- Name: ShapeMethodConfigs_ShapeMethodConfigId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."ShapeMethodConfigs" ALTER COLUMN "ShapeMethodConfigId" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."ShapeMethodConfigs_ShapeMethodConfigId_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: SkillTypes; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."SkillTypes" (
    "SkillTypeId" uuid NOT NULL,
    "Name" text NOT NULL,
    "Description" text NOT NULL,
    "Status" text DEFAULT 'Active'::text NOT NULL
);


ALTER TABLE public."SkillTypes" OWNER TO postgres;

--
-- Name: StaffTransfers; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."StaffTransfers" (
    "StaffTransferId" uuid NOT NULL,
    "NailArtistId" uuid NOT NULL,
    "FromSalonId" uuid NOT NULL,
    "ToSalonId" uuid NOT NULL,
    "StartDate" timestamp without time zone NOT NULL,
    "EndDate" timestamp without time zone NOT NULL,
    "Status" integer NOT NULL,
    "Reason" character varying(500),
    "CreatedBy" uuid,
    "CreatedAt" timestamp without time zone NOT NULL,
    "UpdatedAt" timestamp without time zone
);


ALTER TABLE public."StaffTransfers" OWNER TO postgres;

--
-- Name: Transactions; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Transactions" (
    "TransactionId" integer NOT NULL,
    "BookingId" uuid,
    "OrderCode" character varying(50) NOT NULL,
    "Amount" numeric(18,2) NOT NULL,
    "Reference" character varying(200),
    "PaymentLinkId" character varying(200),
    "CheckoutUrl" text NOT NULL,
    "QrCode" text NOT NULL,
    "Status" character varying(30) NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "PaidAt" timestamp without time zone,
    "ExpiresAt" timestamp without time zone NOT NULL,
    "WebhookPayload" text DEFAULT ''::text NOT NULL,
    "Policy" character varying(500) DEFAULT ''::character varying NOT NULL
);


ALTER TABLE public."Transactions" OWNER TO postgres;

--
-- Name: Transactions_TransactionId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."Transactions" ALTER COLUMN "TransactionId" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Transactions_TransactionId_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: UserPromotionUsages; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."UserPromotionUsages" (
    "UserPromotionUsageId" integer NOT NULL,
    "UserId" uuid NOT NULL,
    "PromotionId" integer NOT NULL,
    "UsageCount" integer NOT NULL,
    "LastUsedDate" timestamp without time zone NOT NULL,
    "ReceivedCount" integer
);


ALTER TABLE public."UserPromotionUsages" OWNER TO postgres;

--
-- Name: UserPromotionUsages_UserPromotionUsageId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."UserPromotionUsages" ALTER COLUMN "UserPromotionUsageId" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."UserPromotionUsages_UserPromotionUsageId_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: Users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Users" (
    "UserId" uuid NOT NULL,
    "Email" text NOT NULL,
    "Password" text NOT NULL,
    "Phone" text,
    "FirstName" text NOT NULL,
    "LastName" text NOT NULL,
    "AvatarUrl" text,
    "Status" text DEFAULT 'Active'::text NOT NULL,
    "Role" text DEFAULT ''::text NOT NULL,
    "SalonId" uuid,
    "CreatedAt" timestamp without time zone DEFAULT '-infinity'::timestamp without time zone NOT NULL
);


ALTER TABLE public."Users" OWNER TO postgres;

--
-- Name: WaitlistItems; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."WaitlistItems" (
    "WaitlistItemId" uuid NOT NULL,
    "WaitlistId" uuid NOT NULL,
    "NailVariantId" integer,
    "ServiceId" uuid,
    "CustomerNailId" integer,
    "Quantity" integer NOT NULL
);


ALTER TABLE public."WaitlistItems" OWNER TO postgres;

--
-- Name: WalkInQueues; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."WalkInQueues" (
    "QueueId" uuid NOT NULL,
    "SalonId" uuid NOT NULL,
    "CustomerId" uuid,
    "OriginalBookingId" uuid,
    "GuestName" text,
    "GuestPhone" text,
    "QueuePosition" integer NOT NULL,
    "Status" character varying(20) NOT NULL,
    "ArrivalTime" timestamp without time zone NOT NULL,
    "CalledTime" timestamp without time zone,
    "ServiceStartTime" timestamp without time zone,
    "AssignedNailArtistId" uuid,
    "RequestNote" text,
    "EstimatedWait" integer,
    "ChairId" uuid,
    "SelectedItemsJson" text
);


ALTER TABLE public."WalkInQueues" OWNER TO postgres;

--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO postgres;

--
-- Name: aggregatedcounter id; Type: DEFAULT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.aggregatedcounter ALTER COLUMN id SET DEFAULT nextval('hangfire.aggregatedcounter_id_seq'::regclass);


--
-- Name: counter id; Type: DEFAULT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.counter ALTER COLUMN id SET DEFAULT nextval('hangfire.counter_id_seq'::regclass);


--
-- Name: hash id; Type: DEFAULT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.hash ALTER COLUMN id SET DEFAULT nextval('hangfire.hash_id_seq'::regclass);


--
-- Name: job id; Type: DEFAULT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.job ALTER COLUMN id SET DEFAULT nextval('hangfire.job_id_seq'::regclass);


--
-- Name: jobparameter id; Type: DEFAULT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.jobparameter ALTER COLUMN id SET DEFAULT nextval('hangfire.jobparameter_id_seq'::regclass);


--
-- Name: jobqueue id; Type: DEFAULT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.jobqueue ALTER COLUMN id SET DEFAULT nextval('hangfire.jobqueue_id_seq'::regclass);


--
-- Name: list id; Type: DEFAULT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.list ALTER COLUMN id SET DEFAULT nextval('hangfire.list_id_seq'::regclass);


--
-- Name: set id; Type: DEFAULT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.set ALTER COLUMN id SET DEFAULT nextval('hangfire.set_id_seq'::regclass);


--
-- Name: state id; Type: DEFAULT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.state ALTER COLUMN id SET DEFAULT nextval('hangfire.state_id_seq'::regclass);


--
-- Data for Name: aggregatedcounter; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.aggregatedcounter (id, key, value, expireat) FROM stdin;
8127	stats:succeeded:2026-08-10	149	2026-09-10 23:50:10.412481+00
13256	stats:succeeded:2026-09-08-16	36	2026-09-09 16:55:23.712534+00
13142	stats:succeeded:2026-09-08-13	18	2026-09-09 13:55:17.950376+00
13106	stats:succeeded:2026-09-08-12	18	2026-09-09 12:55:09.24149+00
12921	stats:succeeded:2026-09-08-06	17	2026-09-09 06:50:12.831001+00
13214	stats:succeeded:2026-09-08-15	48	2026-09-09 15:56:31.633961+00
11546	stats:succeeded:2026-09-01	25	2026-10-01 12:30:13.754371+00
13068	stats:succeeded:2026-09-08-11	18	2026-09-09 11:55:08.436669+00
10290	stats:succeeded:2026-08-19	101	2026-09-19 18:30:07.040905+00
11150	stats:succeeded:2026-08-25	57	2026-09-25 21:30:04.24951+00
12410	stats:succeeded:2026-09-07	216	2026-10-07 18:35:06.014008+00
12849	stats:succeeded:2026-09-08-04	18	2026-09-09 04:55:13.907727+00
9889	stats:succeeded:2026-08-18	153	2026-09-18 23:50:07.05185+00
12887	stats:succeeded:2026-09-08-05	18	2026-09-09 05:55:13.7405+00
12830	stats:succeeded:2026-09-08-03	14	2026-09-09 03:55:12.380153+00
13032	stats:succeeded:2026-09-08-10	18	2026-09-09 10:55:26.200491+00
12998	stats:succeeded:2026-09-08-09	18	2026-09-09 09:55:14.653239+00
10795	stats:succeeded:2026-08-22	50	2026-09-22 22:10:07.356756+00
12956	stats:succeeded:2026-09-08-07	5	2026-09-09 07:55:14.210926+00
11000	stats:succeeded:2026-08-24	58	2026-09-24 14:20:16.199525+00
12962	stats:succeeded:2026-09-08-08	18	2026-09-09 08:55:12.986503+00
7432	stats:succeeded:2026-08-08	123	2026-09-08 23:20:07.464326+00
11609	stats:succeeded:2026-09-02	11	2026-10-02 12:00:13.70616+00
12085	stats:succeeded:2026-09-06	181	2026-10-06 23:35:17.455115+00
11502	stats:succeeded:2026-08-28	18	2026-09-28 07:20:11.438511+00
11410	stats:succeeded:2026-08-27	34	2026-09-27 16:40:11.485624+00
8636	stats:succeeded:2026-08-12	51	2026-09-12 16:20:15.162306+00
9517	stats:succeeded:2026-08-17	132	2026-09-17 23:10:16.703074+00
9373	stats:succeeded:2026-08-16	53	2026-09-16 18:50:19.431493+00
10560	stats:succeeded:2026-08-20	22	2026-09-20 19:30:17.381913+00
10931	stats:succeeded:2026-08-23	28	2026-09-23 15:10:08.025098+00
8781	stats:succeeded:2026-08-13	44	2026-09-13 19:20:13.495867+00
11314	stats:succeeded:2026-08-26	34	2026-09-26 08:40:03.283393+00
12812	stats:succeeded:2026-09-08-02	12	2026-09-09 02:40:22.585754+00
8906	stats:succeeded:2026-08-14	72	2026-09-14 22:20:13.365688+00
13328	stats:succeeded:2026-09-08-18	15	2026-09-09 18:45:12.454575+00
10617	stats:succeeded:2026-08-21	64	2026-09-21 16:20:15.014836+00
11634	stats:succeeded:2026-09-03	9	2026-10-03 10:40:05.445538+00
11651	stats:succeeded:2026-09-04	69	2026-10-04 16:30:20.820582+00
3	stats:succeeded	5162	\N
13354	stats:succeeded:2026-09-08-19	8	2026-09-09 19:30:24.316775+00
12811	stats:succeeded:2026-09-08	335	2026-10-08 19:30:23.316775+00
13290	stats:succeeded:2026-09-08-17	18	2026-09-09 17:55:20.052524+00
11846	stats:succeeded:2026-09-05	103	2026-10-05 21:20:13.865665+00
9108	stats:succeeded:2026-08-15	95	2026-09-15 22:30:08.172649+00
8554	stats:succeeded:2026-08-11	33	2026-09-11 19:50:05.242275+00
13178	stats:succeeded:2026-09-08-14	18	2026-09-09 14:55:14.596097+00
7787	stats:succeeded:2026-08-09	121	2026-09-09 23:50:16.596688+00
\.


--
-- Data for Name: counter; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.counter (id, key, value, expireat) FROM stdin;
15665	stats:succeeded:2026-09-08	1	2026-10-08 19:35:18.719384+00
15666	stats:succeeded:2026-09-08-19	1	2026-09-09 19:35:19.719384+00
15667	stats:succeeded	1	\N
\.


--
-- Data for Name: hash; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.hash (id, key, field, value, expireat, updatecount) FROM stdin;
1	recurring-job:cancel-late-bookings	Queue	default	\N	0
2	recurring-job:cancel-late-bookings	Cron	*/10 * * * *	\N	0
3	recurring-job:cancel-late-bookings	TimeZoneId	UTC	\N	0
4	recurring-job:cancel-late-bookings	Job	{"t":"Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application","m":"CancelLateBookingsAsync"}	\N	0
5	recurring-job:cancel-late-bookings	CreatedAt	1782915946568	\N	0
7	recurring-job:cancel-late-bookings	V	2	\N	0
8	recurring-job:clear-daily-waitlist	Queue	default	\N	0
9	recurring-job:clear-daily-waitlist	Cron	0 0 * * *	\N	0
10	recurring-job:clear-daily-waitlist	TimeZoneId	UTC	\N	0
11	recurring-job:clear-daily-waitlist	Job	{"t":"Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IWaitlistJobExecutor, Nailify.Capstone.Application","m":"ClearDailyWaitlistAsync"}	\N	0
12	recurring-job:clear-daily-waitlist	CreatedAt	1782915948431	\N	0
14	recurring-job:clear-daily-waitlist	V	2	\N	0
19	recurring-job:proactive-delay-check	Queue	default	\N	0
20	recurring-job:proactive-delay-check	Cron	*/5 * * * *	\N	0
21	recurring-job:proactive-delay-check	TimeZoneId	UTC	\N	0
22	recurring-job:proactive-delay-check	Job	{"t":"Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application","m":"CheckAndNotifyDelayAsync"}	\N	0
23	recurring-job:proactive-delay-check	CreatedAt	1788588671605	\N	0
25	recurring-job:proactive-delay-check	V	2	\N	0
17	recurring-job:clear-daily-waitlist	LastExecution	1788833640608	\N	0
13	recurring-job:clear-daily-waitlist	NextExecution	1788912000000	\N	0
18	recurring-job:clear-daily-waitlist	LastJobId	4835	\N	0
15	recurring-job:cancel-late-bookings	LastExecution	1788895814436	\N	0
6	recurring-job:cancel-late-bookings	NextExecution	1788896400000	\N	0
16	recurring-job:cancel-late-bookings	LastJobId	5168	\N	0
26	recurring-job:proactive-delay-check	LastExecution	1788896112894	\N	0
24	recurring-job:proactive-delay-check	NextExecution	1788896400000	\N	0
27	recurring-job:proactive-delay-check	LastJobId	5170	\N	0
28	recurring-job:proactive-delay-check	Error		\N	0
29	recurring-job:proactive-delay-check	RetryAttempt	0	\N	0
\.


--
-- Data for Name: job; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.job (id, stateid, statename, invocationdata, arguments, createdat, expireat, updatecount) FROM stdin;
4914	15668	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 07:50:11.081422+00	2026-09-09 07:50:16.928126+00	0
5155	16434	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 18:25:15.134289+00	2026-09-09 18:25:22.967136+00	0
4839	15442	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 02:25:12.477917+00	2026-09-09 02:25:16.939429+00	0
4932	15722	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 08:50:16.180922+00	2026-09-09 08:50:20.856955+00	0
5038	16035	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 14:35:04.611276+00	2026-09-09 14:35:09.340752+00	0
4949	15772	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 09:45:09.907559+00	2026-09-09 09:45:21.429355+00	0
4984	15873	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 11:35:13.432789+00	2026-09-09 11:35:18.239237+00	0
5020	15981	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 13:35:09.537203+00	2026-09-09 13:35:18.005402+00	0
5055	16091	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 15:15:02.492598+00	2026-09-09 15:15:09.253559+00	0
5074	16164	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 15:25:00.905519+00	2026-09-09 15:25:05.424484+00	0
5092	16224	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 15:55:02.869131+00	2026-09-09 15:55:08.475496+00	0
5132	16367	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 17:10:04.56348+00	2026-09-09 17:10:11.482008+00	0
1601	5564	Failed	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "SendBookingReminderEmailAsync", "Arguments": "[\\"\\\\\\"58fc44bd-2ed3-40e1-9fbc-c5966335e604\\\\\\"\\"]", "ParameterTypes": "[\\"System.Guid, mscorlib\\"]"}	["\\"58fc44bd-2ed3-40e1-9fbc-c5966335e604\\""]	2026-07-23 13:59:15.933998+00	\N	0
5003	15931	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 12:40:05.575388+00	2026-09-09 12:40:10.645952+00	0
4895	15612	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 06:00:15.380566+00	2026-09-09 06:00:20.176831+00	0
4896	15613	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 06:00:17.797697+00	2026-09-09 06:00:22.29003+00	0
5115	16314	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 16:10:04.376852+00	2026-09-09 16:10:09.852197+00	0
5057	16120	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"2d9e1bb53f544f98895aa307ec08728c\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"2d9e1bb53f544f98895aa307ec08728c\\""]	2026-09-08 15:16:24.52169+00	2026-09-09 15:21:35.788022+00	0
5039	16039	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 14:40:10.431963+00	2026-09-09 14:40:15.055316+00	0
5084	16210	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"0f9dcf2d47364ac2ad2fd780f7e451b5\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"0f9dcf2d47364ac2ad2fd780f7e451b5\\""]	2026-09-08 15:49:34.918106+00	2026-09-09 15:54:45.095296+00	0
5040	16041	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 14:40:13.079458+00	2026-09-09 14:40:17.709519+00	0
4897	15616	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 06:05:07.098582+00	2026-09-09 06:05:11.90615+00	0
4899	15622	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 06:10:15.185373+00	2026-09-09 06:10:20.305296+00	0
4945	15759	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 09:30:12.093327+00	2026-09-09 09:30:19.325264+00	0
5114	16312	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 16:10:01.82365+00	2026-09-09 16:10:06.373513+00	0
625	2117	Failed	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "SendBookingReminderEmailAsync", "Arguments": "[\\"\\\\\\"5f2d06a2-6ebb-4f43-bee7-efd9ec40fd62\\\\\\"\\"]", "ParameterTypes": "[\\"System.Guid, mscorlib\\"]"}	["\\"5f2d06a2-6ebb-4f43-bee7-efd9ec40fd62\\""]	2026-07-11 06:11:31.686636+00	\N	0
4944	15760	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 09:30:06.295165+00	2026-09-09 09:30:18.704206+00	0
5004	15933	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 12:40:08.256057+00	2026-09-09 12:40:13.305342+00	0
4915	15670	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 07:50:15.540748+00	2026-09-09 07:50:21.266847+00	0
4933	15724	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 08:50:18.742557+00	2026-09-09 08:50:23.961285+00	0
4898	15620	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 06:10:12.504564+00	2026-09-09 06:10:17.328682+00	0
5133	16368	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 17:10:07.410491+00	2026-09-09 17:10:14.448152+00	0
4840	15446	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 02:30:01.807621+00	2026-09-09 02:30:06.529056+00	0
5093	16247	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"7cdb6bbb016349c49b589103a60e85c6\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"7cdb6bbb016349c49b589103a60e85c6\\""]	2026-09-08 15:55:27.503361+00	2026-09-09 16:00:32.139505+00	0
4950	15777	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 09:50:07.168233+00	2026-09-09 09:50:17.069852+00	0
5041	16044	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 14:45:02.44661+00	2026-09-09 14:45:06.969602+00	0
4841	15448	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 02:30:04.518904+00	2026-09-09 02:30:09.377487+00	0
5056	16118	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"2d9e1bb53f544f98895aa307ec08728c\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"2d9e1bb53f544f98895aa307ec08728c\\""]	2026-09-08 15:16:23.563274+00	2026-09-09 15:21:32.555944+00	0
5134	16371	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 17:15:04.948086+00	2026-09-09 17:15:12.433046+00	0
5021	15986	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 13:40:01.63454+00	2026-09-09 13:40:06.529299+00	0
5022	15987	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 13:40:04.095842+00	2026-09-09 13:40:08.92904+00	0
5060	16131	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"2d9e1bb53f544f98895aa307ec08728c\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"2d9e1bb53f544f98895aa307ec08728c\\""]	2026-09-08 15:16:27.302881+00	2026-09-09 15:21:46.921299+00	0
5068	16153	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"c695ec5628b94a4fa653ea6031803e73\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"c695ec5628b94a4fa653ea6031803e73\\""]	2026-09-08 15:16:59.265696+00	2026-09-09 15:22:26.281911+00	0
4843	15455	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 02:40:15.593182+00	2026-09-09 02:40:20.103149+00	0
5135	16375	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 17:20:04.176442+00	2026-09-09 17:20:09.86265+00	0
5136	16377	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 17:20:07.277964+00	2026-09-09 17:20:15.036597+00	0
5149	16416	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 18:05:10.255923+00	2026-09-09 18:05:17.12652+00	0
4858	15499	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 03:55:07.765066+00	2026-09-09 03:55:12.380153+00	0
5075	16168	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 15:30:06.726553+00	2026-09-09 15:30:13.074612+00	0
5156	16438	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 18:30:09.703538+00	2026-09-09 18:30:15.141421+00	0
5116	16317	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 16:15:07.875761+00	2026-09-09 16:15:15.484732+00	0
5137	16380	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 17:25:06.368367+00	2026-09-09 17:25:12.614874+00	0
5005	15936	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 12:45:10.450669+00	2026-09-09 12:45:20.110476+00	0
5023	15990	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 13:45:02.696844+00	2026-09-09 13:45:09.580359+00	0
4834	15429	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 02:14:04.142226+00	2026-09-09 02:14:13.89722+00	0
4850	15475	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 03:25:05.046936+00	2026-09-09 03:25:11.691278+00	0
5119	16326	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 16:25:08.612164+00	2026-09-09 16:25:14.582307+00	0
4916	15673	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 07:55:08.937624+00	2026-09-09 07:55:14.210926+00	0
4842	15451	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 02:35:09.689112+00	2026-09-09 02:35:14.14832+00	0
4860	15505	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 04:00:14.873363+00	2026-09-09 04:00:19.419999+00	0
4951	15778	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 09:50:11.263719+00	2026-09-09 09:50:18.875509+00	0
5157	16440	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 18:30:12.812857+00	2026-09-09 18:30:20.053967+00	0
5042	16049	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 14:50:07.055547+00	2026-09-09 14:50:12.643548+00	0
4972	15837	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 10:55:12.709806+00	2026-09-09 10:55:26.200491+00	0
5043	16050	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 14:50:09.60261+00	2026-09-09 14:50:14.201021+00	0
4869	15532	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 04:30:08.780723+00	2026-09-09 04:30:13.4074+00	0
5094	16249	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"7cdb6bbb016349c49b589103a60e85c6\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"7cdb6bbb016349c49b589103a60e85c6\\""]	2026-09-08 15:55:29.072482+00	2026-09-09 16:00:35.269016+00	0
4928	15709	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 08:35:05.108705+00	2026-09-09 08:35:10.193742+00	0
5098	16269	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"4040132cc34043719bae120dba2d99d0\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"4040132cc34043719bae120dba2d99d0\\""]	2026-09-08 15:55:58.1793+00	2026-09-09 16:01:10.017155+00	0
4930	15715	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 08:40:16.33735+00	2026-09-09 08:40:22.158171+00	0
5048	16073	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"a22081a2980d48b39281fd1ed7e29dde\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"a22081a2980d48b39281fd1ed7e29dde\\""]	2026-09-08 15:01:14.313432+00	2026-09-09 15:06:31.602166+00	0
4879	15562	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 05:05:06.269847+00	2026-09-09 05:05:12.24159+00	0
1603	5560	Failed	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "SendBookingReminderEmailAsync", "Arguments": "[\\"\\\\\\"58fc44bd-2ed3-40e1-9fbc-c5966335e604\\\\\\"\\"]", "ParameterTypes": "[\\"System.Guid, mscorlib\\"]"}	["\\"58fc44bd-2ed3-40e1-9fbc-c5966335e604\\""]	2026-07-23 13:59:23.288623+00	\N	0
4900	15625	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 06:15:05.266539+00	2026-09-09 06:15:09.74231+00	0
4844	15457	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 02:40:18.013873+00	2026-09-09 02:40:22.585754+00	0
5138	16384	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 17:30:02.796219+00	2026-09-09 17:30:10.717721+00	0
5117	16321	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 16:20:08.113357+00	2026-09-09 16:20:14.68329+00	0
4952	15781	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 09:55:10.074171+00	2026-09-09 09:55:14.653239+00	0
5024	15995	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 13:50:02.662207+00	2026-09-09 13:50:08.409123+00	0
4986	15879	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 11:40:05.261369+00	2026-09-09 11:40:09.766868+00	0
5044	16053	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 14:55:07.870956+00	2026-09-09 14:55:14.596097+00	0
5076	16170	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 15:30:11.091478+00	2026-09-09 15:30:17.641717+00	0
4917	15678	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 08:00:13.931445+00	2026-09-09 08:00:19.834642+00	0
4960	15804	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 10:20:02.38949+00	2026-09-09 10:20:07.180984+00	0
5006	15941	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 12:50:11.410018+00	2026-09-09 12:50:18.025718+00	0
5158	16443	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 18:35:06.736243+00	2026-09-09 18:35:12.50167+00	0
4979	15859	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 11:20:10.173517+00	2026-09-09 11:20:19.848816+00	0
4851	15480	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 03:37:31.209195+00	2026-09-09 03:37:38.272214+00	0
4921	15688	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 08:10:14.541789+00	2026-09-09 08:10:19.345909+00	0
4837	15437	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 02:20:04.441758+00	2026-09-09 02:20:09.126932+00	0
5078	16177	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 15:40:12.052927+00	2026-09-09 15:40:17.241082+00	0
4838	15439	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 02:20:07.165027+00	2026-09-09 02:20:12.055215+00	0
4854	15487	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 03:40:19.333101+00	2026-09-09 03:40:24.475813+00	0
4922	15691	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 08:15:03.814945+00	2026-09-09 08:15:08.565823+00	0
4856	15494	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 03:50:15.681591+00	2026-09-09 03:50:20.390572+00	0
4967	15824	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 10:40:03.377353+00	2026-09-09 10:40:15.456211+00	0
4934	15727	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 08:55:08.125376+00	2026-09-09 08:55:12.986503+00	0
4985	15877	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 11:40:02.851769+00	2026-09-09 11:40:07.322279+00	0
4918	15679	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 08:00:16.360381+00	2026-09-09 08:00:20.945199+00	0
4901	15629	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 06:20:10.719125+00	2026-09-09 06:20:15.373674+00	0
1602	5559	Failed	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "SendBookingReminderEmailAsync", "Arguments": "[\\"\\\\\\"58fc44bd-2ed3-40e1-9fbc-c5966335e604\\\\\\"\\"]", "ParameterTypes": "[\\"System.Guid, mscorlib\\"]"}	["\\"58fc44bd-2ed3-40e1-9fbc-c5966335e604\\""]	2026-07-23 13:59:22.723174+00	\N	0
4980	15861	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 11:20:15.886617+00	2026-09-09 11:20:24.330372+00	0
5139	16386	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 17:30:06.932872+00	2026-09-09 17:30:14.497909+00	0
5077	16173	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 15:35:09.842249+00	2026-09-09 15:35:16.879799+00	0
5118	16323	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 16:20:10.964476+00	2026-09-09 16:20:18.138074+00	0
5025	15996	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 13:50:05.136382+00	2026-09-09 13:50:10.829208+00	0
5007	15942	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 12:50:14.402675+00	2026-09-09 12:50:20.818594+00	0
5122	16335	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 16:35:08.603446+00	2026-09-09 16:35:14.613695+00	0
4996	15909	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 12:15:02.34496+00	2026-09-09 12:15:06.87571+00	0
4981	15864	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 11:25:11.477629+00	2026-09-09 11:25:20.745318+00	0
5096	16265	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"4040132cc34043719bae120dba2d99d0\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"4040132cc34043719bae120dba2d99d0\\""]	2026-09-08 15:55:55.670727+00	2026-09-09 16:01:06.056318+00	0
5045	16057	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 15:00:02.958274+00	2026-09-09 15:00:07.914066+00	0
4936	15733	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 09:00:17.661338+00	2026-09-09 09:00:23.293262+00	0
5046	16059	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 15:00:06.060937+00	2026-09-09 15:00:11.518603+00	0
5085	16214	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"0f9dcf2d47364ac2ad2fd780f7e451b5\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"0f9dcf2d47364ac2ad2fd780f7e451b5\\""]	2026-09-08 15:49:35.897836+00	2026-09-09 15:54:47.334428+00	0
5142	16395	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 17:40:05.972917+00	2026-09-09 17:40:12.867079+00	0
5124	16341	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 16:40:06.492529+00	2026-09-09 16:40:14.499823+00	0
5144	16403	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 17:50:08.405526+00	2026-09-09 17:50:16.253115+00	0
4859	15503	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 04:00:12.460048+00	2026-09-09 04:00:16.939535+00	0
4968	15825	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 10:40:09.225876+00	2026-09-09 10:40:19.140115+00	0
4935	15731	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 09:00:15.095698+00	2026-09-09 09:00:19.630737+00	0
4870	15535	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 04:35:13.699112+00	2026-09-09 04:35:18.181874+00	0
4953	15785	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 10:00:13.728852+00	2026-09-09 10:00:21.412388+00	0
5140	16389	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 17:35:05.585503+00	2026-09-09 17:35:11.532412+00	0
5079	16179	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 15:40:17.471521+00	2026-09-09 15:40:22.148992+00	0
4902	15631	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 06:20:13.134278+00	2026-09-09 06:20:17.688482+00	0
4919	15682	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 08:05:05.583152+00	2026-09-09 08:05:10.302019+00	0
4845	15462	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 03:13:24.495494+00	2026-09-09 03:13:40.17616+00	0
5026	15999	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 13:55:11.703653+00	2026-09-09 13:55:17.950376+00	0
4982	15869	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 11:30:03.106034+00	2026-09-09 11:30:11.602186+00	0
4946	15763	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 09:35:04.664508+00	2026-09-09 09:35:15.380887+00	0
4846	15463	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 03:13:28.095627+00	2026-09-09 03:13:40.239482+00	0
5008	15945	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 12:55:04.400541+00	2026-09-09 12:55:09.24149+00	0
4876	15553	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 04:55:09.442745+00	2026-09-09 04:55:13.907727+00	0
4920	15686	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 08:10:11.831184+00	2026-09-09 08:10:16.491346+00	0
4852	15481	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 03:37:33.800157+00	2026-09-09 03:37:43.068323+00	0
5143	16398	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 17:45:05.251855+00	2026-09-09 17:45:13.299013+00	0
4912	15664	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 07:46:02.594935+00	2026-09-09 07:46:14.24136+00	0
5145	16404	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 17:50:12.593491+00	2026-09-09 17:50:19.345643+00	0
4913	15663	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 07:46:06.197537+00	2026-09-09 07:46:14.169298+00	0
4931	15718	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 08:45:08.664608+00	2026-09-09 08:45:14.007376+00	0
5141	16393	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 17:40:02.071869+00	2026-09-09 17:40:08.069959+00	0
4853	15485	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 03:40:16.769905+00	2026-09-09 03:40:21.716511+00	0
4847	15466	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 03:15:07.820836+00	2026-09-09 03:15:12.463252+00	0
4987	15882	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 11:45:09.701652+00	2026-09-09 11:45:14.160844+00	0
5027	16003	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 14:00:02.868581+00	2026-09-09 14:00:07.45035+00	0
4954	15787	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 10:00:19.587514+00	2026-09-09 10:00:26.658783+00	0
4903	15634	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 06:25:03.436017+00	2026-09-09 06:25:07.984392+00	0
4861	15508	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 04:05:03.942466+00	2026-09-09 04:05:08.602682+00	0
4855	15490	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 03:45:09.096448+00	2026-09-09 03:45:13.754612+00	0
4937	15736	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 09:05:07.49907+00	2026-09-09 09:05:12.155284+00	0
5062	16136	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"ca7665ee71e547f98903bc1743ab33ba\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"ca7665ee71e547f98903bc1743ab33ba\\""]	2026-09-08 15:16:29.121424+00	2026-09-09 15:21:51.407785+00	0
5009	15949	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 13:00:10.950553+00	2026-09-09 13:00:19.633943+00	0
4991	15895	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 12:00:01.841373+00	2026-09-09 12:00:06.352148+00	0
5150	16421	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 18:10:52.459382+00	2026-09-09 18:11:01.811306+00	0
5151	16422	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 18:10:56.846531+00	2026-09-09 18:11:06.831982+00	0
4862	15512	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 04:10:09.899608+00	2026-09-09 04:10:14.667678+00	0
4909	15652	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 06:45:13.975858+00	2026-09-09 06:45:18.772487+00	0
4864	15517	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 04:15:01.730088+00	2026-09-09 04:15:06.331665+00	0
4871	15539	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 04:40:02.424227+00	2026-09-09 04:40:06.919994+00	0
4873	15544	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 04:45:11.81958+00	2026-09-09 04:45:16.497989+00	0
4874	15548	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 04:50:02.219466+00	2026-09-09 04:50:06.971253+00	0
4877	15557	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 05:00:13.858797+00	2026-09-09 05:00:18.335448+00	0
5159	16447	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 18:40:03.128633+00	2026-09-09 18:40:09.405172+00	0
4880	15567	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 05:10:12.427486+00	2026-09-09 05:10:18.06927+00	0
4857	15496	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 03:50:18.416833+00	2026-09-09 03:50:23.095909+00	0
4881	15568	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 05:10:14.834984+00	2026-09-09 05:10:19.671564+00	0
4863	15514	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 04:10:12.465202+00	2026-09-09 04:10:17.201765+00	0
4848	15470	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 03:20:12.966166+00	2026-09-09 03:20:17.450382+00	0
4883	15575	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 05:20:09.444395+00	2026-09-09 05:20:14.261249+00	0
4938	15741	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 09:10:13.289389+00	2026-09-09 09:10:20.92756+00	0
4849	15472	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 03:20:15.383576+00	2026-09-09 03:20:20.905566+00	0
4884	15577	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 05:20:12.131409+00	2026-09-09 05:20:17.750706+00	0
4833	15427	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 02:14:01.531694+00	2026-09-09 02:14:08.884536+00	0
4865	15521	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 04:20:06.82822+00	2026-09-09 04:20:11.358218+00	0
4885	15580	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 05:25:02.139041+00	2026-09-09 05:25:06.955051+00	0
4866	15523	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 04:20:09.257083+00	2026-09-09 04:20:13.939933+00	0
4835	15430	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IWaitlistJobExecutor, Nailify.Capstone.Application", "Method": "ClearDailyWaitlistAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 02:14:06.689608+00	2026-09-09 02:14:14.095682+00	0
4887	15586	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 05:30:09.299535+00	2026-09-09 05:30:14.620731+00	0
4872	15541	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 04:40:04.834124+00	2026-09-09 04:40:09.289268+00	0
4886	15584	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 05:30:06.890172+00	2026-09-09 05:30:11.33574+00	0
4836	15433	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 02:15:14.423379+00	2026-09-09 02:15:19.198112+00	0
4878	15559	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 05:00:16.27651+00	2026-09-09 05:00:21.888952+00	0
4888	15589	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 05:35:16.050762+00	2026-09-09 05:35:20.652572+00	0
4889	15593	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 05:40:05.342725+00	2026-09-09 05:40:09.954551+00	0
4890	15595	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 05:40:07.905127+00	2026-09-09 05:40:12.67779+00	0
4891	15598	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 05:45:12.530442+00	2026-09-09 05:45:16.988725+00	0
4892	15602	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 05:50:01.144652+00	2026-09-09 05:50:05.605601+00	0
4969	15828	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 10:45:13.065158+00	2026-09-09 10:45:27.861919+00	0
4955	15790	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 10:05:07.366235+00	2026-09-09 10:05:12.167792+00	0
4923	15695	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 08:20:09.465366+00	2026-09-09 08:20:14.069941+00	0
4962	15807	Scheduled	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "SendBookingReminderEmailAsync", "Arguments": "[\\"\\\\\\"05d91038-03d5-4743-80d9-34020b38e94a\\\\\\"\\"]", "ParameterTypes": "[\\"System.Guid, mscorlib\\"]"}	["\\"05d91038-03d5-4743-80d9-34020b38e94a\\""]	2026-09-08 10:24:10.102515+00	\N	0
4939	15742	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 09:10:16.097664+00	2026-09-09 09:10:21.612525+00	0
4989	15888	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 11:50:17.358343+00	2026-09-09 11:50:24.67205+00	0
4924	15697	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 08:20:12.021556+00	2026-09-09 08:20:16.755374+00	0
4867	15526	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 04:25:16.687183+00	2026-09-09 04:25:21.38543+00	0
4882	15571	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 05:15:04.095685+00	2026-09-09 05:15:08.653599+00	0
4904	15638	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 06:30:08.393843+00	2026-09-09 06:30:13.073085+00	0
4868	15530	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 04:30:06.363293+00	2026-09-09 04:30:10.873083+00	0
5010	15951	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 13:00:16.652923+00	2026-09-09 13:00:27.034314+00	0
4963	15810	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 10:25:01.092828+00	2026-09-09 10:25:10.841931+00	0
5028	16005	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 14:00:05.29416+00	2026-09-09 14:00:09.951088+00	0
4905	15640	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 06:30:11.080613+00	2026-09-09 06:30:15.860435+00	0
4988	15887	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 11:50:13.687413+00	2026-09-09 11:50:21.789911+00	0
4947	15767	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 09:40:03.857949+00	2026-09-09 09:40:15.382537+00	0
4973	15842	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 11:00:03.465549+00	2026-09-09 11:00:18.350741+00	0
4875	15550	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 04:50:04.633811+00	2026-09-09 04:50:09.293502+00	0
4974	15843	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 11:00:11.04253+00	2026-09-09 11:00:24.77831+00	0
4948	15769	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 09:40:09.352063+00	2026-09-09 09:40:22.085439+00	0
4983	15870	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 11:30:07.210142+00	2026-09-09 11:30:14.861248+00	0
4997	15913	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 12:20:12.553196+00	2026-09-09 12:20:20.729541+00	0
4998	15915	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 12:20:16.832353+00	2026-09-09 12:20:23.577888+00	0
5080	16182	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 15:45:15.269769+00	2026-09-09 15:45:20.267231+00	0
5091	16233	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"a499416af8dd4510bb44e42c542659d2\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"a499416af8dd4510bb44e42c542659d2\\""]	2026-09-08 15:51:24.201866+00	2026-09-09 15:56:31.633961+00	0
5121	16332	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 16:30:09.096412+00	2026-09-09 16:30:17.529685+00	0
5011	15954	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 13:05:07.698763+00	2026-09-09 13:05:12.552436+00	0
5029	16008	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 14:05:04.84086+00	2026-09-09 14:05:11.399525+00	0
5050	16081	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"0bf0cd4faee9432488cb2838fe8c2ee7\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"0bf0cd4faee9432488cb2838fe8c2ee7\\""]	2026-09-08 15:01:51.110067+00	2026-09-09 15:07:08.076641+00	0
4990	15891	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 11:55:02.122463+00	2026-09-09 11:55:08.436669+00	0
5058	16125	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"2d9e1bb53f544f98895aa307ec08728c\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"2d9e1bb53f544f98895aa307ec08728c\\""]	2026-09-08 15:16:25.545531+00	2026-09-09 15:21:40.104319+00	0
5090	16196	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 15:50:08.50091+00	2026-09-09 15:50:13.39268+00	0
5146	16407	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 17:55:12.885565+00	2026-09-09 17:55:20.052524+00	0
4970	15832	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 10:50:02.673573+00	2026-09-09 10:50:08.302485+00	0
4956	15794	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 10:10:06.936836+00	2026-09-09 10:10:19.317151+00	0
5089	16194	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 15:50:06.086701+00	2026-09-09 15:50:10.612365+00	0
4999	15918	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 12:25:09.501923+00	2026-09-09 12:25:19.085749+00	0
5064	16140	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"ca7665ee71e547f98903bc1743ab33ba\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"ca7665ee71e547f98903bc1743ab33ba\\""]	2026-09-08 15:16:30.842194+00	2026-09-09 15:21:55.233286+00	0
4971	15834	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 10:50:06.798148+00	2026-09-09 10:50:12.621487+00	0
5152	16425	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 18:15:10.134465+00	2026-09-09 18:15:17.885239+00	0
5101	16282	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"ac66bf0074234ccdac844bec2e15c94d\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"ac66bf0074234ccdac844bec2e15c94d\\""]	2026-09-08 16:00:10.067151+00	2026-09-09 16:05:23.369163+00	0
5069	16158	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"c695ec5628b94a4fa653ea6031803e73\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"c695ec5628b94a4fa653ea6031803e73\\""]	2026-09-08 15:17:00.118509+00	2026-09-09 15:22:30.628824+00	0
5070	16159	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"c695ec5628b94a4fa653ea6031803e73\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"c695ec5628b94a4fa653ea6031803e73\\""]	2026-09-08 15:17:01.07905+00	2026-09-09 15:22:32.516551+00	0
5082	16206	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"0f9dcf2d47364ac2ad2fd780f7e451b5\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"0f9dcf2d47364ac2ad2fd780f7e451b5\\""]	2026-09-08 15:49:33.213356+00	2026-09-09 15:54:42.104601+00	0
5088	16221	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"d7faf131b26d4c2791820192762f52d0\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"d7faf131b26d4c2791820192762f52d0\\""]	2026-09-08 15:49:38.623436+00	2026-09-09 15:54:53.498173+00	0
5120	16330	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 16:30:04.976485+00	2026-09-09 16:30:11.644072+00	0
4957	15796	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 10:10:13.216789+00	2026-09-09 10:10:24.897011+00	0
4992	15897	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 12:00:04.294659+00	2026-09-09 12:00:09.864634+00	0
5081	16203	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"0f9dcf2d47364ac2ad2fd780f7e451b5\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"0f9dcf2d47364ac2ad2fd780f7e451b5\\""]	2026-09-08 15:49:31.896367+00	2026-09-09 15:54:41.010448+00	0
4975	15846	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 11:05:02.74446+00	2026-09-09 11:05:15.212317+00	0
5052	16067	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 15:05:13.988901+00	2026-09-09 15:05:18.509643+00	0
5032	16017	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 14:15:07.51701+00	2026-09-09 14:15:12.558283+00	0
5030	16013	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 14:10:14.395567+00	2026-09-09 14:10:20.310936+00	0
5031	16014	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 14:10:16.962218+00	2026-09-09 14:10:22.388346+00	0
5001	15924	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 12:30:05.797324+00	2026-09-09 12:30:10.545273+00	0
5103	16242	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 16:00:14.353432+00	2026-09-09 16:00:21.649984+00	0
5012	15959	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 13:10:06.171384+00	2026-09-09 13:10:12.870599+00	0
5047	16072	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"a22081a2980d48b39281fd1ed7e29dde\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"a22081a2980d48b39281fd1ed7e29dde\\""]	2026-09-08 15:01:11.6251+00	2026-09-09 15:06:29.853138+00	0
5013	15960	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 13:10:09.880516+00	2026-09-09 13:10:15.771715+00	0
5000	15923	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 12:30:03.373915+00	2026-09-09 12:30:08.181949+00	0
5051	16082	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"0bf0cd4faee9432488cb2838fe8c2ee7\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"0bf0cd4faee9432488cb2838fe8c2ee7\\""]	2026-09-08 15:01:52.187289+00	2026-09-09 15:07:09.924026+00	0
4964	15814	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 10:30:09.596624+00	2026-09-09 10:30:21.273655+00	0
5014	15963	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 13:15:04.143889+00	2026-09-09 13:15:09.641887+00	0
5035	16026	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 14:25:06.871682+00	2026-09-09 14:25:11.670991+00	0
4965	15816	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 10:30:15.589741+00	2026-09-09 10:30:27.673352+00	0
5017	15972	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 13:25:08.161615+00	2026-09-09 13:25:15.209683+00	0
5036	16030	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 14:30:12.716056+00	2026-09-09 14:30:17.372757+00	0
5018	15976	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 13:30:05.361762+00	2026-09-09 13:30:10.946799+00	0
5037	16032	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 14:30:15.398616+00	2026-09-09 14:30:20.064114+00	0
5019	15978	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 13:30:09.33615+00	2026-09-09 13:30:15.894831+00	0
5160	16449	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 18:40:07.040608+00	2026-09-09 18:40:13.134009+00	0
4906	15643	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 06:35:15.985957+00	2026-09-09 06:35:20.451762+00	0
4925	15700	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 08:25:03.548499+00	2026-09-09 08:25:08.243283+00	0
4940	15745	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 09:15:05.809402+00	2026-09-09 09:15:11.221714+00	0
5102	16241	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 16:00:11.082629+00	2026-09-09 16:00:21.570535+00	0
4893	15604	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 05:50:03.561392+00	2026-09-09 05:50:08.016723+00	0
5049	16078	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"0bf0cd4faee9432488cb2838fe8c2ee7\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"0bf0cd4faee9432488cb2838fe8c2ee7\\""]	2026-09-08 15:01:49.775963+00	2026-09-09 15:07:06.038765+00	0
4958	15799	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 10:15:07.080627+00	2026-09-09 10:15:16.115041+00	0
4976	15850	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 11:10:01.678883+00	2026-09-09 11:10:06.16551+00	0
4941	15750	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 09:20:11.040682+00	2026-09-09 09:20:16.438716+00	0
4993	15900	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 12:05:14.92982+00	2026-09-09 12:05:22.52563+00	0
5015	15967	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 13:20:01.706168+00	2026-09-09 13:20:07.150051+00	0
4966	15819	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 10:35:15.350642+00	2026-09-09 10:35:27.360219+00	0
5033	16021	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 14:20:13.779647+00	2026-09-09 14:20:18.698621+00	0
5002	15927	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 12:35:15.808693+00	2026-09-09 12:35:20.488849+00	0
5016	15969	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 13:20:04.129999+00	2026-09-09 13:20:10.101755+00	0
5034	16023	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 14:20:16.356064+00	2026-09-09 14:20:22.919317+00	0
5061	16133	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"2d9e1bb53f544f98895aa307ec08728c\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"2d9e1bb53f544f98895aa307ec08728c\\""]	2026-09-08 15:16:28.152639+00	2026-09-09 15:21:48.636705+00	0
5087	16220	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"d7faf131b26d4c2791820192762f52d0\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"d7faf131b26d4c2791820192762f52d0\\""]	2026-09-08 15:49:37.763587+00	2026-09-09 15:54:51.389226+00	0
5063	16139	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"ca7665ee71e547f98903bc1743ab33ba\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"ca7665ee71e547f98903bc1743ab33ba\\""]	2026-09-08 15:16:29.98059+00	2026-09-09 15:21:54.172293+00	0
5066	16147	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"c695ec5628b94a4fa653ea6031803e73\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"c695ec5628b94a4fa653ea6031803e73\\""]	2026-09-08 15:16:57.563154+00	2026-09-09 15:22:20.219572+00	0
5065	16145	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"c695ec5628b94a4fa653ea6031803e73\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"c695ec5628b94a4fa653ea6031803e73\\""]	2026-09-08 15:16:56.539748+00	2026-09-09 15:22:17.666326+00	0
5071	16161	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"c695ec5628b94a4fa653ea6031803e73\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"c695ec5628b94a4fa653ea6031803e73\\""]	2026-09-08 15:17:01.932862+00	2026-09-09 15:22:36.791515+00	0
4959	15800	Scheduled	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "SendBookingReminderEmailAsync", "Arguments": "[\\"\\\\\\"05d91038-03d5-4743-80d9-34020b38e94a\\\\\\"\\"]", "ParameterTypes": "[\\"System.Guid, mscorlib\\"]"}	["\\"05d91038-03d5-4743-80d9-34020b38e94a\\""]	2026-09-08 10:19:48.518517+00	\N	0
4961	15806	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 10:20:04.801493+00	2026-09-09 10:20:14.969618+00	0
4894	15607	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 05:55:08.957858+00	2026-09-09 05:55:13.7405+00	0
4942	15751	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 09:20:13.455829+00	2026-09-09 09:20:17.906628+00	0
4978	15855	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 11:15:09.490629+00	2026-09-09 11:15:14.201956+00	0
5123	16339	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 16:40:03.16129+00	2026-09-09 16:40:09.056967+00	0
4907	15647	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 06:40:05.212114+00	2026-09-09 06:40:09.79013+00	0
4977	15852	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 11:10:04.109234+00	2026-09-09 11:10:11.399883+00	0
4926	15704	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 08:30:08.992738+00	2026-09-09 08:30:13.728577+00	0
4908	15649	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 06:40:07.776776+00	2026-09-09 06:40:12.553493+00	0
4927	15706	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 08:30:11.402892+00	2026-09-09 08:30:16.073442+00	0
5083	16209	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"0f9dcf2d47364ac2ad2fd780f7e451b5\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"0f9dcf2d47364ac2ad2fd780f7e451b5\\""]	2026-09-08 15:49:34.064839+00	2026-09-09 15:54:43.305211+00	0
4943	15754	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 09:25:04.563924+00	2026-09-09 09:25:09.300171+00	0
4910	15657	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 06:50:05.797613+00	2026-09-09 06:50:10.620584+00	0
4994	15905	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 12:10:05.409544+00	2026-09-09 12:10:14.314932+00	0
4911	15658	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 06:50:08.217176+00	2026-09-09 06:50:12.831001+00	0
4929	15713	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 08:40:13.926918+00	2026-09-09 08:40:18.5306+00	0
5053	16086	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 15:10:07.093135+00	2026-09-09 15:10:13.325991+00	0
4995	15906	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 12:10:09.510996+00	2026-09-09 12:10:16.944986+00	0
5054	16088	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 15:10:10.341004+00	2026-09-09 15:10:16.217242+00	0
5072	16111	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 15:20:10.818827+00	2026-09-09 15:20:18.886098+00	0
5073	16113	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 15:20:15.027702+00	2026-09-09 15:20:24.216674+00	0
5059	16127	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"2d9e1bb53f544f98895aa307ec08728c\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"2d9e1bb53f544f98895aa307ec08728c\\""]	2026-09-08 15:16:26.451865+00	2026-09-09 15:21:43.001677+00	0
5067	16151	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"c695ec5628b94a4fa653ea6031803e73\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"c695ec5628b94a4fa653ea6031803e73\\""]	2026-09-08 15:16:58.413949+00	2026-09-09 15:22:24.148537+00	0
5086	16217	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"0f9dcf2d47364ac2ad2fd780f7e451b5\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"0f9dcf2d47364ac2ad2fd780f7e451b5\\""]	2026-09-08 15:49:36.754216+00	2026-09-09 15:54:49.346784+00	0
5147	16411	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 18:00:11.361846+00	2026-09-09 18:00:16.865057+00	0
5125	16344	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 16:45:01.238603+00	2026-09-09 16:45:07.205336+00	0
5161	16452	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 18:45:06.358423+00	2026-09-09 18:45:12.454575+00	0
5154	16431	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 18:20:12.295652+00	2026-09-09 18:20:18.836948+00	0
5126	16348	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 16:50:12.61644+00	2026-09-09 16:50:19.880649+00	0
5127	16350	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 16:50:16.520595+00	2026-09-09 16:50:25.341743+00	0
5095	16261	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"4040132cc34043719bae120dba2d99d0\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"4040132cc34043719bae120dba2d99d0\\""]	2026-09-08 15:55:53.893083+00	2026-09-09 16:01:04.013503+00	0
5099	16272	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"024f0b48d6e84cc89e10af56a1007b3a\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"024f0b48d6e84cc89e10af56a1007b3a\\""]	2026-09-08 15:59:21.207365+00	2026-09-09 16:04:27.994794+00	0
5097	16268	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"4040132cc34043719bae120dba2d99d0\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"4040132cc34043719bae120dba2d99d0\\""]	2026-09-08 15:55:57.274693+00	2026-09-09 16:01:09.149217+00	0
5100	16275	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"c98d8e2c46f54bbb9f3c080f4a84d3a1\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"c98d8e2c46f54bbb9f3c080f4a84d3a1\\""]	2026-09-08 15:59:53.413645+00	2026-09-09 16:05:05.070806+00	0
5113	16278	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 16:05:08.989877+00	2026-09-09 16:05:16.100388+00	0
5105	16288	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"64e12eaf711c4a11942fdf9f4eca429b\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"64e12eaf711c4a11942fdf9f4eca429b\\""]	2026-09-08 16:00:57.857106+00	2026-09-09 16:06:04.547855+00	0
5104	16284	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"e2760ccc45b2409282aad1cd4d6d0067\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"e2760ccc45b2409282aad1cd4d6d0067\\""]	2026-09-08 16:00:21.865058+00	2026-09-09 16:05:25.95478+00	0
5106	16291	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"64e12eaf711c4a11942fdf9f4eca429b\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"64e12eaf711c4a11942fdf9f4eca429b\\""]	2026-09-08 16:00:59.830587+00	2026-09-09 16:06:09.822337+00	0
5108	16297	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"64e12eaf711c4a11942fdf9f4eca429b\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"64e12eaf711c4a11942fdf9f4eca429b\\""]	2026-09-08 16:01:01.704471+00	2026-09-09 16:06:15.949348+00	0
5112	16308	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"26e24d4d5dbd47059c517523851c9477\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"26e24d4d5dbd47059c517523851c9477\\""]	2026-09-08 16:01:05.307717+00	2026-09-09 16:06:29.909154+00	0
5107	16295	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"64e12eaf711c4a11942fdf9f4eca429b\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"64e12eaf711c4a11942fdf9f4eca429b\\""]	2026-09-08 16:01:00.683907+00	2026-09-09 16:06:13.490506+00	0
5110	16304	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"26e24d4d5dbd47059c517523851c9477\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"26e24d4d5dbd47059c517523851c9477\\""]	2026-09-08 16:01:03.595609+00	2026-09-09 16:06:23.27816+00	0
5111	16307	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"26e24d4d5dbd47059c517523851c9477\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"26e24d4d5dbd47059c517523851c9477\\""]	2026-09-08 16:01:04.453715+00	2026-09-09 16:06:26.41748+00	0
5148	16413	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 18:00:14.473781+00	2026-09-09 18:00:20.194297+00	0
5109	16300	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.ISlotHoldService, Nailify.Capstone.Application", "Method": "ReleaseHoldAndNotifyWaitersAsync", "Arguments": "[\\"\\\\\\"64e12eaf711c4a11942fdf9f4eca429b\\\\\\"\\"]", "ParameterTypes": "[\\"System.String\\"]"}	["\\"64e12eaf711c4a11942fdf9f4eca429b\\""]	2026-09-08 16:01:02.577294+00	2026-09-09 16:06:20.101661+00	0
5153	16430	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 18:20:09.442701+00	2026-09-09 18:20:16.583901+00	0
5128	16353	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 16:55:16.430359+00	2026-09-09 16:55:23.712534+00	0
5162	16457	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 19:10:19.928428+00	2026-09-09 19:10:28.392184+00	0
5129	16357	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 17:00:16.990089+00	2026-09-09 17:00:22.903826+00	0
5163	16458	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 19:10:23.464895+00	2026-09-09 19:10:32.311215+00	0
5164	16461	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 19:15:06.421532+00	2026-09-09 19:15:12.244844+00	0
5130	16359	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 17:00:20.280093+00	2026-09-09 17:00:27.053141+00	0
5131	16362	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 17:05:02.49472+00	2026-09-09 17:05:10.692438+00	0
5165	16465	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 19:20:03.189163+00	2026-09-09 19:20:11.87089+00	0
5166	16467	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 19:20:07.31948+00	2026-09-09 19:20:15.077707+00	0
5167	16470	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 19:25:05.176642+00	2026-09-09 19:25:12.447409+00	0
5168	16474	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingJobExecutor, Nailify.Capstone.Application", "Method": "CancelLateBookingsAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 19:30:15.525538+00	2026-09-09 19:30:21.016329+00	0
5170	16479	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 19:35:13.984328+00	2026-09-09 19:35:19.719384+00	0
5169	16476	Succeeded	{"Type": "Nailify.Capstone.Application.Interfaces.ServiceInterfaces.IBookingSchedulingService, Nailify.Capstone.Application", "Method": "CheckAndNotifyDelayAsync", "Arguments": "[]", "ParameterTypes": "[]"}	[]	2026-09-08 19:30:18.626972+00	2026-09-09 19:30:24.316775+00	0
\.


--
-- Data for Name: jobparameter; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.jobparameter (id, jobid, name, value, updatecount) FROM stdin;
14092	4836	RecurringJobId	"proactive-delay-check"	0
14093	4836	Time	1788833713	0
14094	4836	CurrentCulture	""	0
14128	4848	RecurringJobId	"cancel-late-bookings"	0
14129	4848	Time	1788837612	0
14130	4848	CurrentCulture	""	0
14168	4861	Time	1788840303	0
14169	4861	CurrentCulture	""	0
14170	4862	RecurringJobId	"cancel-late-bookings"	0
14171	4862	Time	1788840608	0
14172	4862	CurrentCulture	""	0
14176	4864	RecurringJobId	"proactive-delay-check"	0
14177	4864	Time	1788840900	0
14416	4944	RecurringJobId	"cancel-late-bookings"	0
14417	4944	Time	1788859804	0
14178	4864	CurrentCulture	""	0
14197	4871	RecurringJobId	"cancel-late-bookings"	0
14198	4871	Time	1788842401	0
14199	4871	CurrentCulture	""	0
14203	4873	RecurringJobId	"proactive-delay-check"	0
14204	4873	Time	1788842710	0
14205	4873	CurrentCulture	""	0
14206	4874	RecurringJobId	"cancel-late-bookings"	0
4767	1601	RetryCount	10	0
14207	4874	Time	1788843001	0
14208	4874	CurrentCulture	""	0
14257	4891	RecurringJobId	"proactive-delay-check"	0
14258	4891	Time	1788846311	0
14259	4891	CurrentCulture	""	0
14418	4944	CurrentCulture	"en-US"	0
14419	4945	RecurringJobId	"proactive-delay-check"	0
14420	4945	Time	1788859804	0
14421	4945	CurrentCulture	"en-US"	0
14458	4958	RecurringJobId	"proactive-delay-check"	0
14459	4958	Time	1788862505	0
14460	4958	CurrentCulture	""	0
14481	4967	RecurringJobId	"cancel-late-bookings"	0
14482	4967	Time	1788864001	0
14483	4967	CurrentCulture	"en-US"	0
14484	4968	RecurringJobId	"proactive-delay-check"	0
14485	4968	Time	1788864001	0
14486	4968	CurrentCulture	"en-US"	0
14496	4972	RecurringJobId	"proactive-delay-check"	0
14497	4972	Time	1788864910	0
14498	4972	CurrentCulture	"en-US"	0
14511	4977	RecurringJobId	"proactive-delay-check"	0
14512	4977	Time	1788865800	0
14513	4977	CurrentCulture	""	0
14514	4978	RecurringJobId	"proactive-delay-check"	0
14515	4978	Time	1788866108	0
14516	4978	CurrentCulture	""	0
14652	5024	RecurringJobId	"cancel-late-bookings"	0
14653	5024	Time	1788875401	0
14654	5024	CurrentCulture	""	0
14278	4898	RecurringJobId	"cancel-late-bookings"	0
4688	1601	CurrentCulture	""	0
14279	4898	Time	1788847811	0
14280	4898	CurrentCulture	""	0
14281	4899	RecurringJobId	"proactive-delay-check"	0
14282	4899	Time	1788847811	0
14283	4899	CurrentCulture	""	0
14284	4900	RecurringJobId	"proactive-delay-check"	0
14285	4900	Time	1788848104	0
14286	4900	CurrentCulture	""	0
14304	4906	CurrentCulture	""	0
14559	4993	RecurringJobId	"proactive-delay-check"	0
14560	4993	Time	1788869113	0
14561	4993	CurrentCulture	""	0
14599	5006	Time	1788871810	0
14600	5006	CurrentCulture	""	0
14601	5007	RecurringJobId	"proactive-delay-check"	0
14602	5007	Time	1788871810	0
14603	5007	CurrentCulture	""	0
14607	5009	RecurringJobId	"cancel-late-bookings"	0
14608	5009	Time	1788872409	0
14609	5009	CurrentCulture	""	0
14610	5010	RecurringJobId	"proactive-delay-check"	0
14611	5010	Time	1788872409	0
14612	5010	CurrentCulture	""	0
14095	4837	RecurringJobId	"cancel-late-bookings"	0
14096	4837	Time	1788834003	0
14097	4837	CurrentCulture	""	0
14098	4838	RecurringJobId	"proactive-delay-check"	0
14099	4838	Time	1788834003	0
14100	4838	CurrentCulture	""	0
14101	4839	RecurringJobId	"proactive-delay-check"	0
14102	4839	Time	1788834311	0
14103	4839	CurrentCulture	""	0
14134	4850	RecurringJobId	"proactive-delay-check"	0
14135	4850	Time	1788837904	0
14136	4850	CurrentCulture	""	0
14173	4863	RecurringJobId	"proactive-delay-check"	0
14174	4863	Time	1788840608	0
14175	4863	CurrentCulture	""	0
14179	4865	RecurringJobId	"cancel-late-bookings"	0
14180	4865	Time	1788841205	0
14181	4865	CurrentCulture	""	0
14182	4866	RecurringJobId	"proactive-delay-check"	0
4768	1602	RetryCount	10	0
14183	4866	Time	1788841205	0
14184	4866	CurrentCulture	""	0
14209	4875	RecurringJobId	"proactive-delay-check"	0
14422	4946	RecurringJobId	"proactive-delay-check"	0
14423	4946	Time	1788860102	0
14424	4946	CurrentCulture	""	0
14461	4959	CurrentCulture	"en-US"	0
14523	4981	RecurringJobId	"proactive-delay-check"	0
14524	4981	Time	1788866709	0
14525	4981	CurrentCulture	""	0
14562	4994	RecurringJobId	"cancel-late-bookings"	0
14563	4994	Time	1788869403	0
14564	4994	CurrentCulture	""	0
14565	4995	RecurringJobId	"proactive-delay-check"	0
14566	4995	Time	1788869403	0
14655	5025	RecurringJobId	"proactive-delay-check"	0
14656	5025	Time	1788875401	0
14657	5025	CurrentCulture	""	0
14661	5027	RecurringJobId	"cancel-late-bookings"	0
14662	5027	Time	1788876002	0
14663	5027	CurrentCulture	""	0
14664	5028	RecurringJobId	"proactive-delay-check"	0
14665	5028	Time	1788876002	0
14666	5028	CurrentCulture	""	0
14696	5038	CurrentCulture	""	0
14697	5039	RecurringJobId	"cancel-late-bookings"	0
14698	5039	Time	1788878409	0
14699	5039	CurrentCulture	""	0
14700	5040	RecurringJobId	"proactive-delay-check"	0
14701	5040	Time	1788878409	0
14702	5040	CurrentCulture	""	0
14706	5042	RecurringJobId	"cancel-late-bookings"	0
14210	4875	Time	1788843001	0
4690	1603	CurrentCulture	""	0
14211	4875	CurrentCulture	""	0
14260	4892	RecurringJobId	"cancel-late-bookings"	0
14261	4892	Time	1788846600	0
14567	4995	CurrentCulture	""	0
14604	5008	RecurringJobId	"proactive-delay-check"	0
14707	5042	Time	1788879006	0
14708	5042	CurrentCulture	""	0
14709	5043	RecurringJobId	"proactive-delay-check"	0
14710	5043	Time	1788879006	0
14711	5043	CurrentCulture	""	0
14722	5048	CurrentCulture	""	0
14723	5049	CurrentCulture	""	0
14724	5050	CurrentCulture	""	0
14725	5051	CurrentCulture	""	0
14726	5052	RecurringJobId	"proactive-delay-check"	0
14727	5052	Time	1788879913	0
14728	5052	CurrentCulture	""	0
14740	5058	CurrentCulture	""	0
14742	5060	CurrentCulture	""	0
14753	5071	CurrentCulture	""	0
14754	5072	RecurringJobId	"cancel-late-bookings"	0
14755	5072	Time	1788880809	0
14756	5072	CurrentCulture	""	0
14757	5073	RecurringJobId	"proactive-delay-check"	0
14605	5008	Time	1788872103	0
14606	5008	CurrentCulture	""	0
14758	5073	Time	1788880809	0
14759	5073	CurrentCulture	""	0
14780	5080	RecurringJobId	"proactive-delay-check"	0
14781	5080	Time	1788882313	0
14782	5080	CurrentCulture	"en-001"	0
14783	5080	CurrentUICulture	"en-US"	0
14791	5088	CurrentCulture	""	0
14805	5096	CurrentCulture	""	0
14808	5099	CurrentCulture	""	0
14809	5100	CurrentCulture	""	0
14810	5101	CurrentCulture	""	0
14817	5104	CurrentCulture	""	0
14819	5106	CurrentCulture	""	0
14820	5107	CurrentCulture	""	0
14822	5109	CurrentCulture	""	0
14823	5110	CurrentCulture	""	0
14825	5112	CurrentCulture	""	0
14826	5113	RecurringJobId	"proactive-delay-check"	0
14827	5113	Time	1788883507	0
14828	5113	CurrentCulture	""	0
14829	5114	RecurringJobId	"cancel-late-bookings"	0
14830	5114	Time	1788883800	0
14831	5114	CurrentCulture	""	0
14832	5115	RecurringJobId	"proactive-delay-check"	0
14833	5115	Time	1788883800	0
14834	5115	CurrentCulture	""	0
14835	5116	RecurringJobId	"proactive-delay-check"	0
14836	5116	Time	1788884106	0
1839	625	RetryCount	10	0
14104	4840	RecurringJobId	"cancel-late-bookings"	0
14105	4840	Time	1788834600	0
14106	4840	CurrentCulture	""	0
14107	4841	RecurringJobId	"proactive-delay-check"	0
14108	4841	Time	1788834600	0
14109	4841	CurrentCulture	""	0
14113	4843	RecurringJobId	"cancel-late-bookings"	0
14114	4843	Time	1788835214	0
14115	4843	CurrentCulture	""	0
14116	4844	RecurringJobId	"proactive-delay-check"	0
14117	4844	Time	1788835214	0
14118	4844	CurrentCulture	""	0
14137	4851	RecurringJobId	"cancel-late-bookings"	0
14138	4851	Time	1788838650	0
14139	4851	CurrentCulture	""	0
14146	4854	RecurringJobId	"proactive-delay-check"	0
14147	4854	Time	1788838815	0
14148	4854	CurrentCulture	""	0
14262	4892	CurrentCulture	""	0
14425	4947	RecurringJobId	"cancel-late-bookings"	0
14426	4947	Time	1788860401	0
14427	4947	CurrentCulture	""	0
14428	4948	RecurringJobId	"proactive-delay-check"	0
14429	4948	Time	1788860401	0
4769	1603	RetryCount	10	0
14430	4948	CurrentCulture	""	0
14434	4950	RecurringJobId	"cancel-late-bookings"	0
14435	4950	Time	1788861005	0
14436	4950	CurrentCulture	""	0
14437	4951	RecurringJobId	"proactive-delay-check"	0
14438	4951	Time	1788861005	0
14439	4951	CurrentCulture	""	0
14462	4960	RecurringJobId	"cancel-late-bookings"	0
14463	4960	Time	1788862801	0
14464	4960	CurrentCulture	""	0
14465	4961	RecurringJobId	"proactive-delay-check"	0
14466	4961	Time	1788862801	0
14467	4961	CurrentCulture	""	0
14469	4963	RecurringJobId	"proactive-delay-check"	0
14658	5026	RecurringJobId	"proactive-delay-check"	0
14659	5026	Time	1788875710	0
14660	5026	CurrentCulture	""	0
14712	5044	RecurringJobId	"proactive-delay-check"	0
14713	5044	Time	1788879306	0
14714	5044	CurrentCulture	""	0
14729	5053	RecurringJobId	"cancel-late-bookings"	0
14730	5053	Time	1788880205	0
14731	5053	CurrentCulture	""	0
14732	5054	RecurringJobId	"proactive-delay-check"	0
14733	5054	Time	1788880205	0
14734	5054	CurrentCulture	""	0
14745	5063	CurrentCulture	""	0
14746	5064	CurrentCulture	""	0
14751	5069	CurrentCulture	""	0
14752	5070	CurrentCulture	""	0
14760	5074	RecurringJobId	"proactive-delay-check"	0
14761	5074	Time	1788881100	0
14762	5074	CurrentCulture	""	0
14784	5081	CurrentCulture	""	0
14787	5084	CurrentCulture	""	0
14788	5085	CurrentCulture	""	0
14789	5086	CurrentCulture	""	0
14795	5090	RecurringJobId	"proactive-delay-check"	0
14796	5090	Time	1788882605	0
14797	5090	CurrentCulture	""	0
14798	5091	CurrentCulture	""	0
14802	5093	CurrentCulture	""	0
14470	4963	Time	1788863100	0
14471	4963	CurrentCulture	""	0
14508	4976	RecurringJobId	"cancel-late-bookings"	0
14509	4976	Time	1788865800	0
14510	4976	CurrentCulture	""	0
14526	4982	RecurringJobId	"cancel-late-bookings"	0
14527	4982	Time	1788867001	0
14528	4982	CurrentCulture	""	0
14529	4983	RecurringJobId	"proactive-delay-check"	0
14530	4983	Time	1788867001	0
14531	4983	CurrentCulture	""	0
14568	4996	RecurringJobId	"proactive-delay-check"	0
14569	4996	Time	1788869701	0
14570	4996	CurrentCulture	""	0
14613	5011	RecurringJobId	"proactive-delay-check"	0
14614	5011	Time	1788872706	0
14615	5011	CurrentCulture	""	0
14803	5094	CurrentCulture	""	0
14804	5095	CurrentCulture	""	0
14806	5097	CurrentCulture	""	0
14807	5098	CurrentCulture	""	0
14811	5102	RecurringJobId	"proactive-delay-check"	0
14431	4949	RecurringJobId	"proactive-delay-check"	0
14432	4949	Time	1788860707	0
14433	4949	CurrentCulture	"en-US"	0
14468	4962	CurrentCulture	""	0
14532	4984	RecurringJobId	"proactive-delay-check"	0
14533	4984	Time	1788867312	0
14534	4984	CurrentCulture	""	0
14535	4985	RecurringJobId	"proactive-delay-check"	0
14536	4985	Time	1788867602	0
14537	4985	CurrentCulture	""	0
14556	4992	RecurringJobId	"proactive-delay-check"	0
14557	4992	Time	1788868800	0
14558	4992	CurrentCulture	""	0
14571	4997	RecurringJobId	"cancel-late-bookings"	0
14572	4997	Time	1788870011	0
14573	4997	CurrentCulture	""	0
14667	5029	RecurringJobId	"proactive-delay-check"	0
14668	5029	Time	1788876303	0
14669	5029	CurrentCulture	""	0
14715	5045	RecurringJobId	"cancel-late-bookings"	0
14716	5045	Time	1788879601	0
14717	5045	CurrentCulture	""	0
14718	5046	RecurringJobId	"proactive-delay-check"	0
14719	5046	Time	1788879601	0
14720	5046	CurrentCulture	""	0
14735	5055	RecurringJobId	"proactive-delay-check"	0
14736	5055	Time	1788880501	0
14737	5055	CurrentCulture	""	0
1838	625	CurrentCulture	""	0
14739	5057	CurrentCulture	""	0
14741	5059	CurrentCulture	""	0
14743	5061	CurrentCulture	""	0
14744	5062	CurrentCulture	""	0
14747	5065	CurrentCulture	""	0
14748	5066	CurrentCulture	""	0
14749	5067	CurrentCulture	""	0
14763	5075	RecurringJobId	"cancel-late-bookings"	0
14764	5075	Time	1788881405	0
14765	5075	CurrentCulture	""	0
14766	5076	RecurringJobId	"proactive-delay-check"	0
14767	5076	Time	1788881405	0
14768	5076	CurrentCulture	""	0
14772	5078	RecurringJobId	"cancel-late-bookings"	0
14773	5078	Time	1788882010	0
14774	5078	CurrentCulture	"en-001"	0
14775	5078	CurrentUICulture	"en-US"	0
14776	5079	RecurringJobId	"proactive-delay-check"	0
14777	5079	Time	1788882010	0
14778	5079	CurrentCulture	"en-001"	0
14779	5079	CurrentUICulture	"en-US"	0
14785	5082	CurrentCulture	""	0
14790	5087	CurrentCulture	""	0
4689	1602	CurrentCulture	""	0
14792	5089	RecurringJobId	"cancel-late-bookings"	0
14793	5089	Time	1788882605	0
14794	5089	CurrentCulture	""	0
14574	4998	RecurringJobId	"proactive-delay-check"	0
14575	4998	Time	1788870011	0
14576	4998	CurrentCulture	""	0
14616	5012	RecurringJobId	"cancel-late-bookings"	0
14617	5012	Time	1788873004	0
14618	5012	CurrentCulture	""	0
14619	5013	RecurringJobId	"proactive-delay-check"	0
14620	5013	Time	1788873004	0
14621	5013	CurrentCulture	""	0
14622	5014	RecurringJobId	"proactive-delay-check"	0
14623	5014	Time	1788873303	0
14624	5014	CurrentCulture	""	0
14631	5017	RecurringJobId	"proactive-delay-check"	0
14632	5017	Time	1788873906	0
14633	5017	CurrentCulture	""	0
14634	5018	RecurringJobId	"cancel-late-bookings"	0
14635	5018	Time	1788874203	0
14636	5018	CurrentCulture	""	0
14637	5019	RecurringJobId	"proactive-delay-check"	0
14638	5019	Time	1788874203	0
14639	5019	CurrentCulture	""	0
14110	4842	RecurringJobId	"proactive-delay-check"	0
14111	4842	Time	1788834908	0
14112	4842	CurrentCulture	""	0
14140	4852	RecurringJobId	"proactive-delay-check"	0
14141	4852	Time	1788838650	0
14142	4852	CurrentCulture	""	0
14185	4867	RecurringJobId	"proactive-delay-check"	0
14186	4867	Time	1788841515	0
14187	4867	CurrentCulture	""	0
14221	4879	RecurringJobId	"proactive-delay-check"	0
14222	4879	Time	1788843905	0
14223	4879	CurrentCulture	""	0
14224	4880	RecurringJobId	"cancel-late-bookings"	0
14225	4880	Time	1788844211	0
14226	4880	CurrentCulture	""	0
14227	4881	RecurringJobId	"proactive-delay-check"	0
14228	4881	Time	1788844211	0
14229	4881	CurrentCulture	""	0
14248	4888	RecurringJobId	"proactive-delay-check"	0
14249	4888	Time	1788845715	0
14250	4888	CurrentCulture	""	0
14251	4889	RecurringJobId	"cancel-late-bookings"	0
14252	4889	Time	1788846004	0
14253	4889	CurrentCulture	""	0
14254	4890	RecurringJobId	"proactive-delay-check"	0
14255	4890	Time	1788846004	0
14256	4890	CurrentCulture	""	0
14263	4893	RecurringJobId	"proactive-delay-check"	0
14264	4893	Time	1788846600	0
14265	4893	CurrentCulture	""	0
14266	4894	RecurringJobId	"proactive-delay-check"	0
14267	4894	Time	1788846908	0
14268	4894	CurrentCulture	""	0
14287	4901	RecurringJobId	"cancel-late-bookings"	0
14288	4901	Time	1788848409	0
14289	4901	CurrentCulture	""	0
14290	4902	RecurringJobId	"proactive-delay-check"	0
14440	4952	RecurringJobId	"proactive-delay-check"	0
14441	4952	Time	1788861309	0
14442	4952	CurrentCulture	""	0
14472	4964	RecurringJobId	"cancel-late-bookings"	0
14473	4964	Time	1788863407	0
14474	4964	CurrentCulture	"en-US"	0
14475	4965	RecurringJobId	"proactive-delay-check"	0
14476	4965	Time	1788863407	0
14477	4965	CurrentCulture	"en-US"	0
14538	4986	RecurringJobId	"cancel-late-bookings"	0
14539	4986	Time	1788867602	0
14540	4986	CurrentCulture	""	0
14577	4999	RecurringJobId	"proactive-delay-check"	0
14578	4999	Time	1788870307	0
14579	4999	CurrentCulture	""	0
14625	5015	RecurringJobId	"cancel-late-bookings"	0
14626	5015	Time	1788873600	0
14627	5015	CurrentCulture	""	0
14628	5016	RecurringJobId	"proactive-delay-check"	0
14629	5016	Time	1788873600	0
14630	5016	CurrentCulture	""	0
14643	5021	RecurringJobId	"cancel-late-bookings"	0
14644	5021	Time	1788874800	0
14645	5021	CurrentCulture	""	0
14646	5022	RecurringJobId	"proactive-delay-check"	0
14647	5022	Time	1788874800	0
14648	5022	CurrentCulture	""	0
14670	5030	RecurringJobId	"cancel-late-bookings"	0
14671	5030	Time	1788876613	0
14672	5030	CurrentCulture	""	0
14673	5031	RecurringJobId	"proactive-delay-check"	0
14674	5031	Time	1788876613	0
14675	5031	CurrentCulture	""	0
14676	5032	RecurringJobId	"proactive-delay-check"	0
14677	5032	Time	1788876906	0
14678	5032	CurrentCulture	""	0
14685	5035	RecurringJobId	"proactive-delay-check"	0
14686	5035	Time	1788877505	0
14687	5035	CurrentCulture	""	0
14688	5036	RecurringJobId	"cancel-late-bookings"	0
14291	4902	Time	1788848409	0
14292	4902	CurrentCulture	""	0
14296	4904	RecurringJobId	"cancel-late-bookings"	0
14297	4904	Time	1788849007	0
14298	4904	CurrentCulture	""	0
14299	4905	RecurringJobId	"proactive-delay-check"	0
14300	4905	Time	1788849007	0
14301	4905	CurrentCulture	""	0
14305	4907	RecurringJobId	"cancel-late-bookings"	0
14306	4907	Time	1788849604	0
14307	4907	CurrentCulture	""	0
14308	4908	RecurringJobId	"proactive-delay-check"	0
14309	4908	Time	1788849604	0
14310	4908	CurrentCulture	""	0
14311	4909	RecurringJobId	"proactive-delay-check"	0
14312	4909	Time	1788849913	0
14313	4909	CurrentCulture	""	0
14314	4910	RecurringJobId	"cancel-late-bookings"	0
14315	4910	Time	1788850204	0
14316	4910	CurrentCulture	""	0
14317	4911	RecurringJobId	"proactive-delay-check"	0
14318	4911	Time	1788850204	0
14319	4911	CurrentCulture	""	0
14320	4912	RecurringJobId	"proactive-delay-check"	0
14321	4912	Time	1788853560	0
14119	4845	RecurringJobId	"proactive-delay-check"	0
14120	4845	Time	1788837203	0
14121	4845	CurrentCulture	""	0
14143	4853	RecurringJobId	"cancel-late-bookings"	0
14144	4853	Time	1788838815	0
14145	4853	CurrentCulture	""	0
14149	4855	RecurringJobId	"proactive-delay-check"	0
14150	4855	Time	1788839108	0
14151	4855	CurrentCulture	""	0
14158	4858	RecurringJobId	"proactive-delay-check"	0
14159	4858	Time	1788839706	0
14160	4858	CurrentCulture	""	0
14161	4859	RecurringJobId	"cancel-late-bookings"	0
14162	4859	Time	1788840011	0
14163	4859	CurrentCulture	""	0
14164	4860	RecurringJobId	"proactive-delay-check"	0
14165	4860	Time	1788840011	0
14166	4860	CurrentCulture	""	0
14188	4868	RecurringJobId	"cancel-late-bookings"	0
14189	4868	Time	1788841805	0
14190	4868	CurrentCulture	""	0
14191	4869	RecurringJobId	"proactive-delay-check"	0
14192	4869	Time	1788841805	0
14193	4869	CurrentCulture	""	0
14200	4872	RecurringJobId	"proactive-delay-check"	0
14201	4872	Time	1788842401	0
14202	4872	CurrentCulture	""	0
14230	4882	RecurringJobId	"proactive-delay-check"	0
14231	4882	Time	1788844503	0
14232	4882	CurrentCulture	""	0
14443	4953	RecurringJobId	"cancel-late-bookings"	0
14444	4953	Time	1788861611	0
14445	4953	CurrentCulture	""	0
14446	4954	RecurringJobId	"proactive-delay-check"	0
14447	4954	Time	1788861611	0
14448	4954	CurrentCulture	""	0
14478	4966	RecurringJobId	"proactive-delay-check"	0
14479	4966	Time	1788863713	0
14480	4966	CurrentCulture	"en-US"	0
14487	4969	RecurringJobId	"proactive-delay-check"	0
14488	4969	Time	1788864310	0
14489	4969	CurrentCulture	"en-US"	0
14499	4973	RecurringJobId	"cancel-late-bookings"	0
14500	4973	Time	1788865201	0
14501	4973	CurrentCulture	"en-US"	0
14502	4974	RecurringJobId	"proactive-delay-check"	0
14503	4974	Time	1788865201	0
14504	4974	CurrentCulture	"en-US"	0
14541	4987	RecurringJobId	"proactive-delay-check"	0
14542	4987	Time	1788867908	0
14543	4987	CurrentCulture	""	0
14553	4991	RecurringJobId	"cancel-late-bookings"	0
14554	4991	Time	1788868800	0
14555	4991	CurrentCulture	""	0
14580	5000	RecurringJobId	"cancel-late-bookings"	0
14581	5000	Time	1788870602	0
14582	5000	CurrentCulture	""	0
14583	5001	RecurringJobId	"proactive-delay-check"	0
14584	5001	Time	1788870602	0
14585	5001	CurrentCulture	""	0
14586	5002	RecurringJobId	"proactive-delay-check"	0
14587	5002	Time	1788870914	0
14588	5002	CurrentCulture	""	0
14589	5003	RecurringJobId	"cancel-late-bookings"	0
14590	5003	Time	1788871204	0
14591	5003	CurrentCulture	""	0
14592	5004	RecurringJobId	"proactive-delay-check"	0
14593	5004	Time	1788871204	0
14594	5004	CurrentCulture	""	0
14640	5020	RecurringJobId	"proactive-delay-check"	0
14641	5020	Time	1788874507	0
14642	5020	CurrentCulture	""	0
14679	5033	RecurringJobId	"cancel-late-bookings"	0
14680	5033	Time	1788877212	0
14681	5033	CurrentCulture	""	0
14682	5034	RecurringJobId	"proactive-delay-check"	0
14683	5034	Time	1788877212	0
14684	5034	CurrentCulture	""	0
14694	5038	RecurringJobId	"proactive-delay-check"	0
14695	5038	Time	1788878103	0
14083	4833	RecurringJobId	"cancel-late-bookings"	0
14084	4833	Time	1788833640	0
14085	4833	CurrentCulture	""	0
14233	4883	RecurringJobId	"cancel-late-bookings"	0
14234	4883	Time	1788844808	0
14235	4883	CurrentCulture	""	0
14236	4884	RecurringJobId	"proactive-delay-check"	0
14237	4884	Time	1788844808	0
14238	4884	CurrentCulture	""	0
14122	4846	RecurringJobId	"cancel-late-bookings"	0
14123	4846	Time	1788837203	0
14124	4846	CurrentCulture	""	0
14152	4856	RecurringJobId	"cancel-late-bookings"	0
14153	4856	Time	1788839414	0
14154	4856	CurrentCulture	""	0
14155	4857	RecurringJobId	"proactive-delay-check"	0
14156	4857	Time	1788839414	0
14157	4857	CurrentCulture	""	0
14194	4870	RecurringJobId	"proactive-delay-check"	0
14195	4870	Time	1788842112	0
14196	4870	CurrentCulture	""	0
14212	4876	RecurringJobId	"proactive-delay-check"	0
14213	4876	Time	1788843308	0
14214	4876	CurrentCulture	""	0
14215	4877	RecurringJobId	"cancel-late-bookings"	0
14216	4877	Time	1788843613	0
14217	4877	CurrentCulture	""	0
14218	4878	RecurringJobId	"proactive-delay-check"	0
14219	4878	Time	1788843613	0
14220	4878	CurrentCulture	""	0
14239	4885	RecurringJobId	"proactive-delay-check"	0
14240	4885	Time	1788845101	0
14241	4885	CurrentCulture	""	0
14242	4886	RecurringJobId	"cancel-late-bookings"	0
14243	4886	Time	1788845406	0
14244	4886	CurrentCulture	""	0
14245	4887	RecurringJobId	"proactive-delay-check"	0
14246	4887	Time	1788845406	0
14247	4887	CurrentCulture	""	0
14269	4895	RecurringJobId	"cancel-late-bookings"	0
14270	4895	Time	1788847214	0
14271	4895	CurrentCulture	""	0
14272	4896	RecurringJobId	"proactive-delay-check"	0
14273	4896	Time	1788847214	0
14274	4896	CurrentCulture	""	0
14275	4897	RecurringJobId	"proactive-delay-check"	0
14276	4897	Time	1788847506	0
14277	4897	CurrentCulture	""	0
14293	4903	RecurringJobId	"proactive-delay-check"	0
14294	4903	Time	1788848702	0
14295	4903	CurrentCulture	""	0
14302	4906	RecurringJobId	"proactive-delay-check"	0
14303	4906	Time	1788849315	0
14449	4955	RecurringJobId	"proactive-delay-check"	0
14450	4955	Time	1788861906	0
14451	4955	CurrentCulture	""	0
14490	4970	RecurringJobId	"cancel-late-bookings"	0
14491	4970	Time	1788864601	0
14492	4970	CurrentCulture	""	0
14493	4971	RecurringJobId	"proactive-delay-check"	0
14494	4971	Time	1788864601	0
14495	4971	CurrentCulture	""	0
14517	4979	RecurringJobId	"cancel-late-bookings"	0
14518	4979	Time	1788866407	0
14519	4979	CurrentCulture	""	0
14520	4980	RecurringJobId	"proactive-delay-check"	0
14521	4980	Time	1788866407	0
14522	4980	CurrentCulture	""	0
14544	4988	RecurringJobId	"cancel-late-bookings"	0
14545	4988	Time	1788868212	0
14546	4988	CurrentCulture	""	0
14547	4989	RecurringJobId	"proactive-delay-check"	0
14548	4989	Time	1788868212	0
14549	4989	CurrentCulture	""	0
14595	5005	RecurringJobId	"proactive-delay-check"	0
14596	5005	Time	1788871508	0
14597	5005	CurrentCulture	""	0
14649	5023	RecurringJobId	"proactive-delay-check"	0
14650	5023	Time	1788875101	0
14651	5023	CurrentCulture	""	0
14689	5036	Time	1788877811	0
14086	4834	RecurringJobId	"proactive-delay-check"	0
14087	4834	Time	1788833640	0
14088	4834	CurrentCulture	""	0
14690	5036	CurrentCulture	""	0
14691	5037	RecurringJobId	"proactive-delay-check"	0
14692	5037	Time	1788877811	0
14693	5037	CurrentCulture	""	0
14703	5041	RecurringJobId	"proactive-delay-check"	0
14704	5041	Time	1788878701	0
14705	5041	CurrentCulture	""	0
14721	5047	CurrentCulture	""	0
14738	5056	CurrentCulture	""	0
14750	5068	CurrentCulture	""	0
14769	5077	RecurringJobId	"proactive-delay-check"	0
14770	5077	Time	1788881708	0
14771	5077	CurrentCulture	""	0
14786	5083	CurrentCulture	""	0
14799	5092	RecurringJobId	"proactive-delay-check"	0
14800	5092	Time	1788882901	0
14801	5092	CurrentCulture	""	0
14812	5102	Time	1788883209	0
14813	5102	CurrentCulture	""	0
14814	5103	RecurringJobId	"cancel-late-bookings"	0
14815	5103	Time	1788883209	0
14816	5103	CurrentCulture	""	0
14818	5105	CurrentCulture	""	0
14821	5108	CurrentCulture	""	0
14824	5111	CurrentCulture	""	0
14837	5116	CurrentCulture	""	0
14838	5117	RecurringJobId	"cancel-late-bookings"	0
14839	5117	Time	1788884407	0
14840	5117	CurrentCulture	""	0
14841	5118	RecurringJobId	"proactive-delay-check"	0
14842	5118	Time	1788884407	0
14843	5118	CurrentCulture	""	0
14844	5119	RecurringJobId	"proactive-delay-check"	0
14845	5119	Time	1788884707	0
14846	5119	CurrentCulture	""	0
14847	5120	RecurringJobId	"cancel-late-bookings"	0
14848	5120	Time	1788885003	0
14849	5120	CurrentCulture	""	0
14850	5121	RecurringJobId	"proactive-delay-check"	0
14851	5121	Time	1788885003	0
14852	5121	CurrentCulture	""	0
14853	5122	RecurringJobId	"proactive-delay-check"	0
14854	5122	Time	1788885307	0
14855	5122	CurrentCulture	""	0
14856	5123	RecurringJobId	"cancel-late-bookings"	0
14857	5123	Time	1788885601	0
14858	5123	CurrentCulture	""	0
14859	5124	RecurringJobId	"proactive-delay-check"	0
14860	5124	Time	1788885601	0
14861	5124	CurrentCulture	""	0
14862	5125	RecurringJobId	"proactive-delay-check"	0
14863	5125	Time	1788885900	0
14864	5125	CurrentCulture	""	0
14865	5126	RecurringJobId	"cancel-late-bookings"	0
14089	4835	RecurringJobId	"clear-daily-waitlist"	0
14090	4835	Time	1788833640	0
14091	4835	CurrentCulture	""	0
14125	4847	RecurringJobId	"proactive-delay-check"	0
14126	4847	Time	1788837306	0
14127	4847	CurrentCulture	""	0
14131	4849	RecurringJobId	"proactive-delay-check"	0
14132	4849	Time	1788837612	0
14133	4849	CurrentCulture	""	0
14167	4861	RecurringJobId	"proactive-delay-check"	0
14866	5126	Time	1788886211	0
14867	5126	CurrentCulture	""	0
14868	5127	RecurringJobId	"proactive-delay-check"	0
14869	5127	Time	1788886211	0
14870	5127	CurrentCulture	""	0
14871	5128	RecurringJobId	"proactive-delay-check"	0
14872	5128	Time	1788886515	0
14873	5128	CurrentCulture	""	0
14874	5129	RecurringJobId	"cancel-late-bookings"	0
14875	5129	Time	1788886815	0
14876	5129	CurrentCulture	""	0
14877	5130	RecurringJobId	"proactive-delay-check"	0
14878	5130	Time	1788886815	0
14879	5130	CurrentCulture	""	0
14880	5131	RecurringJobId	"proactive-delay-check"	0
14881	5131	Time	1788887100	0
14882	5131	CurrentCulture	""	0
14883	5132	RecurringJobId	"cancel-late-bookings"	0
14884	5132	Time	1788887403	0
14885	5132	CurrentCulture	""	0
14886	5133	RecurringJobId	"proactive-delay-check"	0
14887	5133	Time	1788887403	0
14888	5133	CurrentCulture	""	0
14889	5134	RecurringJobId	"proactive-delay-check"	0
14890	5134	Time	1788887703	0
14891	5134	CurrentCulture	""	0
14892	5135	RecurringJobId	"cancel-late-bookings"	0
14893	5135	Time	1788888003	0
14894	5135	CurrentCulture	""	0
14895	5136	RecurringJobId	"proactive-delay-check"	0
14896	5136	Time	1788888003	0
14897	5136	CurrentCulture	""	0
14898	5137	RecurringJobId	"proactive-delay-check"	0
14899	5137	Time	1788888305	0
14900	5137	CurrentCulture	""	0
14901	5138	RecurringJobId	"cancel-late-bookings"	0
14902	5138	Time	1788888601	0
14903	5138	CurrentCulture	""	0
14904	5139	RecurringJobId	"proactive-delay-check"	0
14322	4912	CurrentCulture	""	0
14323	4913	RecurringJobId	"cancel-late-bookings"	0
14324	4913	Time	1788853560	0
14325	4913	CurrentCulture	""	0
14326	4914	RecurringJobId	"proactive-delay-check"	0
14327	4914	Time	1788853809	0
14328	4914	CurrentCulture	""	0
14329	4915	RecurringJobId	"cancel-late-bookings"	0
14330	4915	Time	1788853809	0
14331	4915	CurrentCulture	""	0
14332	4916	RecurringJobId	"proactive-delay-check"	0
14333	4916	Time	1788854108	0
14334	4916	CurrentCulture	""	0
14335	4917	RecurringJobId	"cancel-late-bookings"	0
14336	4917	Time	1788854413	0
14337	4917	CurrentCulture	""	0
14338	4918	RecurringJobId	"proactive-delay-check"	0
14339	4918	Time	1788854413	0
14340	4918	CurrentCulture	""	0
14341	4919	RecurringJobId	"proactive-delay-check"	0
14342	4919	Time	1788854704	0
14343	4919	CurrentCulture	""	0
14344	4920	RecurringJobId	"cancel-late-bookings"	0
14345	4920	Time	1788855010	0
14346	4920	CurrentCulture	""	0
14347	4921	RecurringJobId	"proactive-delay-check"	0
14348	4921	Time	1788855010	0
14349	4921	CurrentCulture	""	0
14350	4922	RecurringJobId	"proactive-delay-check"	0
14351	4922	Time	1788855302	0
14352	4922	CurrentCulture	""	0
14353	4923	RecurringJobId	"cancel-late-bookings"	0
14354	4923	Time	1788855608	0
14355	4923	CurrentCulture	""	0
14356	4924	RecurringJobId	"proactive-delay-check"	0
14357	4924	Time	1788855608	0
14358	4924	CurrentCulture	""	0
14359	4925	RecurringJobId	"proactive-delay-check"	0
14360	4925	Time	1788855902	0
14361	4925	CurrentCulture	""	0
14362	4926	RecurringJobId	"cancel-late-bookings"	0
14363	4926	Time	1788856208	0
14364	4926	CurrentCulture	""	0
14365	4927	RecurringJobId	"proactive-delay-check"	0
14366	4927	Time	1788856208	0
14367	4927	CurrentCulture	""	0
14368	4928	RecurringJobId	"proactive-delay-check"	0
14369	4928	Time	1788856504	0
14370	4928	CurrentCulture	""	0
14371	4929	RecurringJobId	"cancel-late-bookings"	0
14372	4929	Time	1788856813	0
14373	4929	CurrentCulture	""	0
14374	4930	RecurringJobId	"proactive-delay-check"	0
14375	4930	Time	1788856813	0
14376	4930	CurrentCulture	""	0
14377	4931	RecurringJobId	"proactive-delay-check"	0
14378	4931	Time	1788857107	0
14379	4931	CurrentCulture	""	0
14380	4932	RecurringJobId	"proactive-delay-check"	0
14381	4932	Time	1788857415	0
14382	4932	CurrentCulture	""	0
14383	4933	RecurringJobId	"cancel-late-bookings"	0
14384	4933	Time	1788857415	0
14385	4933	CurrentCulture	""	0
14386	4934	RecurringJobId	"proactive-delay-check"	0
14387	4934	Time	1788857707	0
14388	4934	CurrentCulture	""	0
14389	4935	RecurringJobId	"cancel-late-bookings"	0
14390	4935	Time	1788858014	0
14391	4935	CurrentCulture	""	0
14392	4936	RecurringJobId	"proactive-delay-check"	0
14393	4936	Time	1788858014	0
14394	4936	CurrentCulture	""	0
14395	4937	RecurringJobId	"proactive-delay-check"	0
14396	4937	Time	1788858306	0
14397	4937	CurrentCulture	""	0
14398	4938	RecurringJobId	"cancel-late-bookings"	0
14399	4938	Time	1788858612	0
14400	4938	CurrentCulture	""	0
14401	4939	RecurringJobId	"proactive-delay-check"	0
14402	4939	Time	1788858612	0
14403	4939	CurrentCulture	""	0
14404	4940	RecurringJobId	"proactive-delay-check"	0
14405	4940	Time	1788858904	0
14406	4940	CurrentCulture	""	0
14407	4941	RecurringJobId	"cancel-late-bookings"	0
14408	4941	Time	1788859210	0
14409	4941	CurrentCulture	""	0
14410	4942	RecurringJobId	"proactive-delay-check"	0
14411	4942	Time	1788859210	0
14412	4942	CurrentCulture	""	0
14413	4943	RecurringJobId	"proactive-delay-check"	0
14414	4943	Time	1788859503	0
14415	4943	CurrentCulture	""	0
14452	4956	RecurringJobId	"cancel-late-bookings"	0
14453	4956	Time	1788862204	0
14454	4956	CurrentCulture	"en-US"	0
14455	4957	RecurringJobId	"proactive-delay-check"	0
14456	4957	Time	1788862204	0
14457	4957	CurrentCulture	"en-US"	0
14505	4975	RecurringJobId	"proactive-delay-check"	0
14506	4975	Time	1788865500	0
14507	4975	CurrentCulture	"en-US"	0
14550	4990	RecurringJobId	"proactive-delay-check"	0
14551	4990	Time	1788868501	0
14552	4990	CurrentCulture	""	0
14598	5006	RecurringJobId	"cancel-late-bookings"	0
14905	5139	Time	1788888601	0
14906	5139	CurrentCulture	""	0
14907	5140	RecurringJobId	"proactive-delay-check"	0
14908	5140	Time	1788888904	0
14909	5140	CurrentCulture	""	0
14910	5141	RecurringJobId	"cancel-late-bookings"	0
14911	5141	Time	1788889200	0
14912	5141	CurrentCulture	""	0
14913	5142	RecurringJobId	"proactive-delay-check"	0
14914	5142	Time	1788889200	0
14915	5142	CurrentCulture	""	0
14916	5143	RecurringJobId	"proactive-delay-check"	0
14917	5143	Time	1788889503	0
14918	5143	CurrentCulture	""	0
14919	5144	RecurringJobId	"cancel-late-bookings"	0
14920	5144	Time	1788889806	0
14921	5144	CurrentCulture	""	0
14922	5145	RecurringJobId	"proactive-delay-check"	0
14923	5145	Time	1788889806	0
14924	5145	CurrentCulture	""	0
14925	5146	RecurringJobId	"proactive-delay-check"	0
14926	5146	Time	1788890111	0
14927	5146	CurrentCulture	""	0
14928	5147	RecurringJobId	"proactive-delay-check"	0
14929	5147	Time	1788890410	0
14930	5147	CurrentCulture	""	0
14931	5148	RecurringJobId	"cancel-late-bookings"	0
14932	5148	Time	1788890410	0
14933	5148	CurrentCulture	""	0
14934	5149	RecurringJobId	"proactive-delay-check"	0
14935	5149	Time	1788890709	0
14936	5149	CurrentCulture	""	0
14937	5150	RecurringJobId	"cancel-late-bookings"	0
14938	5150	Time	1788891050	0
14939	5150	CurrentCulture	""	0
14940	5151	RecurringJobId	"proactive-delay-check"	0
14941	5151	Time	1788891050	0
14942	5151	CurrentCulture	""	0
14943	5152	RecurringJobId	"proactive-delay-check"	0
14944	5152	Time	1788891308	0
14945	5152	CurrentCulture	""	0
14946	5153	RecurringJobId	"cancel-late-bookings"	0
14947	5153	Time	1788891608	0
14948	5153	CurrentCulture	""	0
14949	5154	RecurringJobId	"proactive-delay-check"	0
14950	5154	Time	1788891608	0
14951	5154	CurrentCulture	""	0
14952	5155	RecurringJobId	"proactive-delay-check"	0
14953	5155	Time	1788891913	0
14954	5155	CurrentCulture	""	0
14955	5156	RecurringJobId	"cancel-late-bookings"	0
14956	5156	Time	1788892208	0
14957	5156	CurrentCulture	""	0
14958	5157	RecurringJobId	"proactive-delay-check"	0
14959	5157	Time	1788892208	0
14960	5157	CurrentCulture	""	0
14961	5158	RecurringJobId	"proactive-delay-check"	0
14962	5158	Time	1788892505	0
14963	5158	CurrentCulture	""	0
14964	5159	RecurringJobId	"cancel-late-bookings"	0
14965	5159	Time	1788892801	0
14966	5159	CurrentCulture	""	0
14967	5160	RecurringJobId	"proactive-delay-check"	0
14968	5160	Time	1788892801	0
14969	5160	CurrentCulture	""	0
14970	5161	RecurringJobId	"proactive-delay-check"	0
14971	5161	Time	1788893105	0
14972	5161	CurrentCulture	""	0
14973	5162	RecurringJobId	"cancel-late-bookings"	0
14974	5162	Time	1788894618	0
14975	5162	CurrentCulture	""	0
14976	5163	RecurringJobId	"proactive-delay-check"	0
14977	5163	Time	1788894618	0
14978	5163	CurrentCulture	""	0
14979	5164	RecurringJobId	"proactive-delay-check"	0
14980	5164	Time	1788894905	0
14981	5164	CurrentCulture	""	0
14982	5165	RecurringJobId	"cancel-late-bookings"	0
14983	5165	Time	1788895201	0
14984	5165	CurrentCulture	""	0
14985	5166	RecurringJobId	"proactive-delay-check"	0
14986	5166	Time	1788895201	0
14987	5166	CurrentCulture	""	0
14988	5167	RecurringJobId	"proactive-delay-check"	0
14989	5167	Time	1788895504	0
14990	5167	CurrentCulture	""	0
14991	5168	RecurringJobId	"cancel-late-bookings"	0
14992	5168	Time	1788895814	0
14993	5168	CurrentCulture	""	0
14994	5169	RecurringJobId	"proactive-delay-check"	0
14995	5169	Time	1788895814	0
14996	5169	CurrentCulture	""	0
14997	5170	RecurringJobId	"proactive-delay-check"	0
14998	5170	Time	1788896112	0
14999	5170	CurrentCulture	""	0
\.


--
-- Data for Name: jobqueue; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.jobqueue (id, jobid, queue, fetchedat, updatecount) FROM stdin;
\.


--
-- Data for Name: list; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.list (id, key, value, expireat, updatecount) FROM stdin;
\.


--
-- Data for Name: lock; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.lock (resource, updatecount, acquired) FROM stdin;
\.


--
-- Data for Name: schema; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.schema (version) FROM stdin;
23
\.


--
-- Data for Name: server; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.server (id, data, lastheartbeat, updatecount) FROM stdin;
\.


--
-- Data for Name: set; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.set (id, key, score, value, expireat, updatecount) FROM stdin;
2	recurring-jobs	1788912000	clear-daily-waitlist	\N	0
1	recurring-jobs	1788896400	cancel-late-bookings	\N	0
4613	recurring-jobs	1788896400	proactive-delay-check	\N	0
5347	schedule	1788916500	4959	\N	0
5350	schedule	1788916500	4962	\N	0
\.


--
-- Data for Name: state; Type: TABLE DATA; Schema: hangfire; Owner: postgres
--

COPY hangfire.state (id, jobid, name, reason, createdat, data, updatecount) FROM stdin;
15443	4840	Enqueued	Triggered by recurring job scheduler	2026-09-08 02:30:02.28862+00	{"Queue": "default", "EnqueuedAt": "1788834602288"}	0
15450	4842	Processing	\N	2026-09-08 02:35:12.239523+00	{"ServerId": "nailify background server:1:a0292a93-5fe1-4130-9485-2ea5702666b1", "WorkerId": "ad02d99d-dc93-48c5-9b47-789ecbfb2ff5", "StartedAt": "1788834911535"}	0
15451	4842	Succeeded	\N	2026-09-08 02:35:14.363944+00	{"Latency": "3117", "SucceededAt": "1788834913232", "PerformanceDuration": "425"}	0
1961	625	Scheduled	\N	2026-07-11 06:11:31.974371+00	{"EnqueueAt": "1783750500000", "ScheduledAt": "1783750291685"}	0
15452	4843	Enqueued	Triggered by recurring job scheduler	2026-09-08 02:40:16.024425+00	{"Queue": "default", "EnqueuedAt": "1788835216024"}	0
15991	5024	Enqueued	Triggered by recurring job scheduler	2026-09-08 13:50:03.130585+00	{"Queue": "default", "EnqueuedAt": "1788875403123"}	0
15992	5025	Enqueued	Triggered by recurring job scheduler	2026-09-08 13:50:05.564692+00	{"Queue": "default", "EnqueuedAt": "1788875405564"}	0
15994	5025	Processing	\N	2026-09-08 13:50:08.075922+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "54ad23b1-3311-415d-91c9-a1c465c0f4ed", "StartedAt": "1788875406982"}	0
1962	625	Enqueued	Triggered by DelayedJobScheduler	2026-07-11 06:15:09.846654+00	{"Queue": "default", "EnqueuedAt": "1783750509175"}	0
1964	625	Failed	An exception occurred during performance of the job.	2026-07-11 06:17:12.700835+00	{"FailedAt": "1783750631989", "ServerId": "nailify background server:1:20837eab-63ae-4ef8-971f-b6a57f9583eb", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at System.RuntimeMethodHandle.InvokeMethod(Object target, Void** arguments, Signature sig, Boolean isConstructor)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
1965	625	Scheduled	Retry attempt 1 of 10: The operation has timed out.	2026-07-11 06:17:12.975133+00	{"EnqueueAt": "1783750671667", "ScheduledAt": "1783750632667"}	0
15996	5025	Succeeded	\N	2026-09-08 13:50:11.162652+00	{"Latency": "3862", "SucceededAt": "1788875409538", "PerformanceDuration": "540"}	0
15454	4844	Enqueued	Triggered by recurring job scheduler	2026-09-08 02:40:18.440245+00	{"Queue": "default", "EnqueuedAt": "1788835218440"}	0
15455	4843	Succeeded	\N	2026-09-08 02:40:20.319176+00	{"Latency": "3148", "SucceededAt": "1788835219186", "PerformanceDuration": "444"}	0
15719	4932	Enqueued	Triggered by recurring job scheduler	2026-09-08 08:50:16.636539+00	{"Queue": "default", "EnqueuedAt": "1788857416636"}	0
15720	4932	Processing	\N	2026-09-08 08:50:18.865289+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788857418150"}	0
15721	4933	Enqueued	Triggered by recurring job scheduler	2026-09-08 08:50:19.195385+00	{"Queue": "default", "EnqueuedAt": "1788857419195"}	0
5513	1601	Processing	\N	2026-07-24 06:46:18.529109+00	{"ServerId": "nailify background server:1:05495341-665e-41b1-a8ea-fb7b6cd0dbff", "WorkerId": "e8788d92-5d22-4e15-9484-c24b32c78221", "StartedAt": "1784875578057"}	0
15722	4932	Succeeded	\N	2026-09-08 08:50:21.068884+00	{"Latency": "3253", "SucceededAt": "1788857419934", "PerformanceDuration": "499"}	0
15723	4933	Processing	\N	2026-09-08 08:50:21.725351+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "a7f4d136-bee6-48c7-a7b1-7dcb7bf0ce5c", "StartedAt": "1788857420766"}	0
15724	4933	Succeeded	\N	2026-09-08 08:50:24.181346+00	{"Latency": "3763", "SucceededAt": "1788857422979", "PerformanceDuration": "472"}	0
15729	4935	Processing	\N	2026-09-08 09:00:17.715543+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788858017010"}	0
16000	5027	Enqueued	Triggered by recurring job scheduler	2026-09-08 14:00:03.306406+00	{"Queue": "default", "EnqueuedAt": "1788876003306"}	0
5557	1601	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 08:38:46.651622+00	{"Queue": "default", "EnqueuedAt": "1784882325993"}	0
16002	5028	Enqueued	Triggered by recurring job scheduler	2026-09-08 14:00:05.718786+00	{"Queue": "default", "EnqueuedAt": "1788876005718"}	0
1966	625	Enqueued	Triggered by DelayedJobScheduler	2026-07-11 06:18:04.295594+00	{"Queue": "default", "EnqueuedAt": "1783750683631"}	0
15547	4875	Enqueued	Triggered by recurring job scheduler	2026-09-08 04:50:05.069019+00	{"Queue": "default", "EnqueuedAt": "1788843005068"}	0
1974	625	Processing	\N	2026-07-11 06:20:58.28387+00	{"ServerId": "nailify background server:1:20837eab-63ae-4ef8-971f-b6a57f9583eb", "WorkerId": "fbad532a-1d11-4982-b562-6ce3de59b2d4", "StartedAt": "1783750857810"}	0
1981	625	Enqueued	Triggered by DelayedJobScheduler	2026-07-11 06:28:47.632613+00	{"Queue": "default", "EnqueuedAt": "1783751326969"}	0
15549	4875	Processing	\N	2026-09-08 04:50:07.282708+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788843006485"}	0
15555	4877	Processing	\N	2026-09-08 05:00:16.421505+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "f953e7fe-c9a8-4ac8-834a-0fcf3d7e9142", "StartedAt": "1788843615709"}	0
15560	4879	Enqueued	Triggered by recurring job scheduler	2026-09-08 05:05:06.760143+00	{"Queue": "default", "EnqueuedAt": "1788843906760"}	0
15561	4879	Processing	\N	2026-09-08 05:05:10.102392+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "f953e7fe-c9a8-4ac8-834a-0fcf3d7e9142", "StartedAt": "1788843909312"}	0
15562	4879	Succeeded	\N	2026-09-08 05:05:12.47651+00	{"Latency": "4463", "SucceededAt": "1788843911213", "PerformanceDuration": "480"}	0
15993	5024	Processing	\N	2026-09-08 13:50:05.711363+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "5bb4a812-7c29-4b27-a144-6d5dc2bb42f5", "StartedAt": "1788875404681"}	0
15995	5024	Succeeded	\N	2026-09-08 13:50:08.721507+00	{"Latency": "3903", "SucceededAt": "1788875407249", "PerformanceDuration": "683"}	0
16001	5027	Processing	\N	2026-09-08 14:00:05.466898+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "5bb4a812-7c29-4b27-a144-6d5dc2bb42f5", "StartedAt": "1788876004756"}	0
16104	5068	Scheduled	\N	2026-09-08 15:16:59.741108+00	{"EnqueueAt": "1788880919265", "ScheduledAt": "1788880619265"}	0
16165	5075	Enqueued	Triggered by recurring job scheduler	2026-09-08 15:30:07.49927+00	{"Queue": "default", "EnqueuedAt": "1788881407499"}	0
5516	1602	Failed	An exception occurred during performance of the job.	2026-07-24 06:47:16.020076+00	{"FailedAt": "1784875635355", "ServerId": "nailify background server:1:05495341-665e-41b1-a8ea-fb7b6cd0dbff", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
5517	1602	Scheduled	Retry attempt 10 of 10: The operation has timed out.	2026-07-24 06:47:16.209496+00	{"EnqueueAt": "1784882222019", "ScheduledAt": "1784875636019"}	0
16167	5076	Enqueued	Triggered by recurring job scheduler	2026-09-08 15:30:11.861564+00	{"Queue": "default", "EnqueuedAt": "1788881411861"}	0
16172	5077	Processing	\N	2026-09-08 15:35:13.524377+00	{"ServerId": "nailify background server:1:72e9e6cb-dd7c-4ec2-a0bc-32fa62e228f3", "WorkerId": "d8bb937d-15f2-45b4-8a25-4026051ce0e1", "StartedAt": "1788881712234"}	0
16173	5077	Succeeded	\N	2026-09-08 15:35:17.250603+00	{"Latency": "4722", "SucceededAt": "1788881715185", "PerformanceDuration": "621"}	0
16174	5078	Enqueued	Triggered by recurring job scheduler	2026-09-08 15:40:13.070538+00	{"Queue": "default", "EnqueuedAt": "1788882013046"}	0
16176	5079	Enqueued	Triggered by recurring job scheduler	2026-09-08 15:40:18.420287+00	{"Queue": "default", "EnqueuedAt": "1788882018419"}	0
15444	4840	Processing	\N	2026-09-08 02:30:04.586864+00	{"ServerId": "nailify background server:1:a0292a93-5fe1-4130-9485-2ea5702666b1", "WorkerId": "ad02d99d-dc93-48c5-9b47-789ecbfb2ff5", "StartedAt": "1788834603879"}	0
15725	4934	Enqueued	Triggered by recurring job scheduler	2026-09-08 08:55:08.588201+00	{"Queue": "default", "EnqueuedAt": "1788857708588"}	0
5559	1602	Failed	An exception occurred during performance of the job.	2026-07-24 08:39:12.414075+00	{"FailedAt": "1784882351661", "ServerId": "nailify background server:1:05495341-665e-41b1-a8ea-fb7b6cd0dbff", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
15726	4934	Processing	\N	2026-09-08 08:55:10.906903+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788857710109"}	0
15727	4934	Succeeded	\N	2026-09-08 08:55:13.213385+00	{"Latency": "3419", "SucceededAt": "1788857711971", "PerformanceDuration": "425"}	0
15728	4935	Enqueued	Triggered by recurring job scheduler	2026-09-08 09:00:15.525208+00	{"Queue": "default", "EnqueuedAt": "1788858015525"}	0
16221	5088	Succeeded	\N	2026-09-08 15:54:53.729782+00	{"Latency": "313557", "SucceededAt": "1788882892472", "PerformanceDuration": "291"}	0
15997	5026	Enqueued	Triggered by recurring job scheduler	2026-09-08 13:55:12.46771+00	{"Queue": "default", "EnqueuedAt": "1788875712467"}	0
15445	4841	Enqueued	Triggered by recurring job scheduler	2026-09-08 02:30:04.99697+00	{"Queue": "default", "EnqueuedAt": "1788834604996"}	0
15446	4840	Succeeded	\N	2026-09-08 02:30:06.74579+00	{"Latency": "3347", "SucceededAt": "1788834605614", "PerformanceDuration": "459"}	0
15447	4841	Processing	\N	2026-09-08 02:30:07.373153+00	{"ServerId": "nailify background server:1:a0292a93-5fe1-4130-9485-2ea5702666b1", "WorkerId": "d167b873-3f4b-4b1d-bfc9-a25631755035", "StartedAt": "1788834606586"}	0
15448	4841	Succeeded	\N	2026-09-08 02:30:09.594139+00	{"Latency": "3485", "SucceededAt": "1788834608462", "PerformanceDuration": "459"}	0
15449	4842	Enqueued	Triggered by recurring job scheduler	2026-09-08 02:35:10.117648+00	{"Queue": "default", "EnqueuedAt": "1788834910117"}	0
15453	4843	Processing	\N	2026-09-08 02:40:18.171139+00	{"ServerId": "nailify background server:1:a0292a93-5fe1-4130-9485-2ea5702666b1", "WorkerId": "ad02d99d-dc93-48c5-9b47-789ecbfb2ff5", "StartedAt": "1788835217463"}	0
15548	4874	Succeeded	\N	2026-09-08 04:50:07.198417+00	{"Latency": "3290", "SucceededAt": "1788843005990", "PerformanceDuration": "480"}	0
15730	4936	Enqueued	Triggered by recurring job scheduler	2026-09-08 09:00:18.127966+00	{"Queue": "default", "EnqueuedAt": "1788858018127"}	0
15731	4935	Succeeded	\N	2026-09-08 09:00:19.841942+00	{"Latency": "3192", "SucceededAt": "1788858018712", "PerformanceDuration": "424"}	0
1963	625	Processing	\N	2026-07-11 06:15:10.901529+00	{"ServerId": "nailify background server:1:20837eab-63ae-4ef8-971f-b6a57f9583eb", "WorkerId": "dea63b16-3343-45d6-84b2-bb1355fa88ab", "StartedAt": "1783750510423"}	0
15801	4960	Enqueued	Triggered by recurring job scheduler	2026-09-08 10:20:02.822945+00	{"Queue": "default", "EnqueuedAt": "1788862802822"}	0
15802	4960	Processing	\N	2026-09-08 10:20:05.074637+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788862804271"}	0
15808	4963	Enqueued	Triggered by recurring job scheduler	2026-09-08 10:25:01.524946+00	{"Queue": "default", "EnqueuedAt": "1788863101524"}	0
15833	4971	Processing	\N	2026-09-08 10:50:10.484797+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "a7f4d136-bee6-48c7-a7b1-7dcb7bf0ce5c", "StartedAt": "1788864609685"}	0
15834	4971	Succeeded	\N	2026-09-08 10:50:12.853868+00	{"Latency": "4326", "SucceededAt": "1788864611593", "PerformanceDuration": "469"}	0
15847	4976	Enqueued	Triggered by recurring job scheduler	2026-09-08 11:10:02.122633+00	{"Queue": "default", "EnqueuedAt": "1788865802122"}	0
15550	4875	Succeeded	\N	2026-09-08 04:50:09.510378+00	{"Latency": "3288", "SucceededAt": "1788843008378", "PerformanceDuration": "456"}	0
1967	625	Processing	\N	2026-07-11 06:18:05.332296+00	{"ServerId": "nailify background server:1:20837eab-63ae-4ef8-971f-b6a57f9583eb", "WorkerId": "8636baf3-be15-420d-a1a5-e143aeedb6be", "StartedAt": "1783750684863"}	0
1973	625	Enqueued	Triggered by DelayedJobScheduler	2026-07-11 06:20:57.243422+00	{"Queue": "default", "EnqueuedAt": "1783750856588"}	0
1975	625	Failed	An exception occurred during performance of the job.	2026-07-11 06:22:59.703957+00	{"FailedAt": "1783750979045", "ServerId": "nailify background server:1:20837eab-63ae-4ef8-971f-b6a57f9583eb", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
1976	625	Scheduled	Retry attempt 3 of 10: The operation has timed out.	2026-07-11 06:22:59.892428+00	{"EnqueueAt": "1783751061703", "ScheduledAt": "1783750979703"}	0
1977	625	Enqueued	Triggered by DelayedJobScheduler	2026-07-11 06:24:36.835192+00	{"Queue": "default", "EnqueuedAt": "1783751076180"}	0
1978	625	Processing	\N	2026-07-11 06:24:37.870048+00	{"ServerId": "nailify background server:1:20837eab-63ae-4ef8-971f-b6a57f9583eb", "WorkerId": "8636baf3-be15-420d-a1a5-e143aeedb6be", "StartedAt": "1783751077401"}	0
15551	4876	Enqueued	Triggered by recurring job scheduler	2026-09-08 04:55:09.873184+00	{"Queue": "default", "EnqueuedAt": "1788843309873"}	0
15552	4876	Processing	\N	2026-09-08 04:55:11.999133+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "f953e7fe-c9a8-4ac8-834a-0fcf3d7e9142", "StartedAt": "1788843311292"}	0
15553	4876	Succeeded	\N	2026-09-08 04:55:14.125367+00	{"Latency": "3123", "SucceededAt": "1788843312994", "PerformanceDuration": "428"}	0
15998	5026	Processing	\N	2026-09-08 13:55:15.609102+00	{"ServerId": "nailify background server:1:cc435256-039f-4352-ad49-91c860e852fd", "WorkerId": "911ea8c4-edf6-46a7-b556-d294ca960732", "StartedAt": "1788875714764"}	0
15999	5026	Succeeded	\N	2026-09-08 13:55:18.271072+00	{"Latency": "4595", "SucceededAt": "1788875716809", "PerformanceDuration": "510"}	0
16008	5029	Succeeded	\N	2026-09-08 14:05:11.759137+00	{"Latency": "4180", "SucceededAt": "1788876309710", "PerformanceDuration": "689"}	0
5472	1603	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 05:30:03.642892+00	{"Queue": "default", "EnqueuedAt": "1784871002981"}	0
16108	5072	Enqueued	Triggered by recurring job scheduler	2026-09-08 15:20:11.585037+00	{"Queue": "default", "EnqueuedAt": "1788880811584"}	0
16110	5073	Enqueued	Triggered by recurring job scheduler	2026-09-08 15:20:15.761321+00	{"Queue": "default", "EnqueuedAt": "1788880815761"}	0
16118	5056	Succeeded	\N	2026-09-08 15:21:32.897576+00	{"Latency": "306857", "SucceededAt": "1788880890932", "PerformanceDuration": "511"}	0
15456	4844	Processing	\N	2026-09-08 02:40:20.627446+00	{"ServerId": "nailify background server:1:a0292a93-5fe1-4130-9485-2ea5702666b1", "WorkerId": "d167b873-3f4b-4b1d-bfc9-a25631755035", "StartedAt": "1788835219876"}	0
15457	4844	Succeeded	\N	2026-09-08 02:40:22.802062+00	{"Latency": "3218", "SucceededAt": "1788835221668", "PerformanceDuration": "436"}	0
15554	4877	Enqueued	Triggered by recurring job scheduler	2026-09-08 05:00:14.286478+00	{"Queue": "default", "EnqueuedAt": "1788843614286"}	0
5477	1603	Failed	An exception occurred during performance of the job.	2026-07-24 05:32:06.088897+00	{"FailedAt": "1784871125430", "ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
5478	1603	Scheduled	Retry attempt 9 of 10: The operation has timed out.	2026-07-24 05:32:06.280028+00	{"EnqueueAt": "1784875498088", "ScheduledAt": "1784871126088"}	0
5479	1602	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 05:32:09.833229+00	{"Queue": "default", "EnqueuedAt": "1784871129175"}	0
15556	4878	Enqueued	Triggered by recurring job scheduler	2026-09-08 05:00:16.701862+00	{"Queue": "default", "EnqueuedAt": "1788843616701"}	0
15557	4877	Succeeded	\N	2026-09-08 05:00:18.552216+00	{"Latency": "3130", "SucceededAt": "1788843617417", "PerformanceDuration": "427"}	0
15732	4936	Processing	\N	2026-09-08 09:00:21.32551+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "a7f4d136-bee6-48c7-a7b1-7dcb7bf0ce5c", "StartedAt": "1788858020577"}	0
15733	4936	Succeeded	\N	2026-09-08 09:00:23.503415+00	{"Latency": "4267", "SucceededAt": "1788858022375", "PerformanceDuration": "447"}	0
15741	4938	Succeeded	\N	2026-09-08 09:10:21.174414+00	{"Latency": "5935", "SucceededAt": "1788858619718", "PerformanceDuration": "493"}	0
15743	4940	Enqueued	Triggered by recurring job scheduler	2026-09-08 09:15:06.235618+00	{"Queue": "default", "EnqueuedAt": "1788858906235"}	0
16256	5108	Scheduled	\N	2026-09-08 16:01:02.13016+00	{"EnqueueAt": "1788883561704", "ScheduledAt": "1788883261704"}	0
1979	625	Failed	An exception occurred during performance of the job.	2026-07-11 06:26:39.286331+00	{"FailedAt": "1783751198624", "ServerId": "nailify background server:1:20837eab-63ae-4ef8-971f-b6a57f9583eb", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
1980	625	Scheduled	Retry attempt 4 of 10: The operation has timed out.	2026-07-11 06:26:39.477272+00	{"EnqueueAt": "1783751315285", "ScheduledAt": "1783751199285"}	0
1982	625	Processing	\N	2026-07-11 06:28:48.675302+00	{"ServerId": "nailify background server:1:20837eab-63ae-4ef8-971f-b6a57f9583eb", "WorkerId": "fbad532a-1d11-4982-b562-6ce3de59b2d4", "StartedAt": "1783751328204"}	0
15734	4937	Enqueued	Triggered by recurring job scheduler	2026-09-08 09:05:07.972549+00	{"Queue": "default", "EnqueuedAt": "1788858307972"}	0
15735	4937	Processing	\N	2026-09-08 09:05:10.208609+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788858309502"}	0
15736	4937	Succeeded	\N	2026-09-08 09:05:12.365433+00	{"Latency": "3276", "SucceededAt": "1788858311236", "PerformanceDuration": "460"}	0
5480	1602	Processing	\N	2026-07-24 05:32:10.861306+00	{"ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "WorkerId": "2934f2ee-fb4d-40c0-a358-a99847227519", "StartedAt": "1784871130394"}	0
5485	1601	Failed	An exception occurred during performance of the job.	2026-07-24 05:36:19.335091+00	{"FailedAt": "1784871378686", "ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
15737	4938	Enqueued	Triggered by recurring job scheduler	2026-09-08 09:10:13.76494+00	{"Queue": "default", "EnqueuedAt": "1788858613764"}	0
15738	4939	Enqueued	Triggered by recurring job scheduler	2026-09-08 09:10:16.570107+00	{"Queue": "default", "EnqueuedAt": "1788858616569"}	0
16003	5027	Succeeded	\N	2026-09-08 14:00:07.664948+00	{"Latency": "3178", "SucceededAt": "1788876006522", "PerformanceDuration": "474"}	0
15458	4845	Enqueued	Triggered by recurring job scheduler	2026-09-08 03:13:25.198621+00	{"Queue": "default", "EnqueuedAt": "1788837205095"}	0
15461	4846	Processing	\N	2026-09-08 03:13:31.60141+00	{"ServerId": "nailify background server:1:74d36720-dfd4-416f-b17a-72a748859de2", "WorkerId": "87fe26d0-774b-40f7-bc8b-14f496865634", "StartedAt": "1788837210695"}	0
15462	4845	Succeeded	\N	2026-09-08 03:13:40.449822+00	{"Latency": "4603", "SucceededAt": "1788837219100", "PerformanceDuration": "10002"}	0
15467	4848	Enqueued	Triggered by recurring job scheduler	2026-09-08 03:20:13.396899+00	{"Queue": "default", "EnqueuedAt": "1788837613396"}	0
15468	4848	Processing	\N	2026-09-08 03:20:15.524722+00	{"ServerId": "nailify background server:1:74d36720-dfd4-416f-b17a-72a748859de2", "WorkerId": "87fe26d0-774b-40f7-bc8b-14f496865634", "StartedAt": "1788837614816"}	0
15469	4849	Enqueued	Triggered by recurring job scheduler	2026-09-08 03:20:15.81082+00	{"Queue": "default", "EnqueuedAt": "1788837615810"}	0
15739	4938	Processing	\N	2026-09-08 09:10:18.459781+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788858617533"}	0
15740	4939	Processing	\N	2026-09-08 09:10:19.669+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "a7f4d136-bee6-48c7-a7b1-7dcb7bf0ce5c", "StartedAt": "1788858618962"}	0
5486	1601	Scheduled	Retry attempt 9 of 10: The operation has timed out.	2026-07-24 05:36:19.520701+00	{"EnqueueAt": "1784875562334", "ScheduledAt": "1784871379334"}	0
15742	4939	Succeeded	\N	2026-09-08 09:10:21.822766+00	{"Latency": "4137", "SucceededAt": "1788858620692", "PerformanceDuration": "456"}	0
15745	4940	Succeeded	\N	2026-09-08 09:15:11.432961+00	{"Latency": "4049", "SucceededAt": "1788858910302", "PerformanceDuration": "443"}	0
15748	4941	Processing	\N	2026-09-08 09:20:14.438659+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788859213735"}	0
15803	4961	Enqueued	Triggered by recurring job scheduler	2026-09-08 10:20:05.227055+00	{"Queue": "default", "EnqueuedAt": "1788862805226"}	0
15804	4960	Succeeded	\N	2026-09-08 10:20:07.415017+00	{"Latency": "3321", "SucceededAt": "1788862806155", "PerformanceDuration": "445"}	0
15885	4988	Processing	\N	2026-09-08 11:50:18.287423+00	{"ServerId": "nailify background server:1:b18e0b9c-1bd7-43d3-b97e-5555ea110c7c", "WorkerId": "7827896a-bbf0-48ff-b93b-68216c3a73fc", "StartedAt": "1788868216730"}	0
15886	4989	Processing	\N	2026-09-08 11:50:21.292985+00	{"ServerId": "nailify background server:1:b18e0b9c-1bd7-43d3-b97e-5555ea110c7c", "WorkerId": "29bf4934-95ab-4c8f-8b86-ad0a16221edf", "StartedAt": "1788868219852"}	0
2010	625	Failed	An exception occurred during performance of the job.	2026-07-11 07:18:41.386604+00	{"FailedAt": "1783754320683", "ServerId": "nailify background server:1:f11c7cbf-56a8-46d7-a565-e1bd4c71eb9a", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
2011	625	Scheduled	Retry attempt 8 of 10: The operation has timed out.	2026-07-11 07:18:41.654531+00	{"EnqueueAt": "1783756945353", "ScheduledAt": "1783754321353"}	0
15459	4845	Processing	\N	2026-09-08 03:13:28.397188+00	{"ServerId": "nailify background server:1:74d36720-dfd4-416f-b17a-72a748859de2", "WorkerId": "eebbaa8b-753d-4f40-b635-63b334ff6abe", "StartedAt": "1788837207396"}	0
16004	5028	Processing	\N	2026-09-08 14:00:07.956829+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "54ad23b1-3311-415d-91c9-a1c465c0f4ed", "StartedAt": "1788876007170"}	0
16005	5028	Succeeded	\N	2026-09-08 14:00:10.165652+00	{"Latency": "3294", "SucceededAt": "1788876009034", "PerformanceDuration": "445"}	0
15464	4847	Enqueued	Triggered by recurring job scheduler	2026-09-08 03:15:08.279612+00	{"Queue": "default", "EnqueuedAt": "1788837308279"}	0
15465	4847	Processing	\N	2026-09-08 03:15:10.488953+00	{"ServerId": "nailify background server:1:74d36720-dfd4-416f-b17a-72a748859de2", "WorkerId": "eebbaa8b-753d-4f40-b635-63b334ff6abe", "StartedAt": "1788837309778"}	0
15466	4847	Succeeded	\N	2026-09-08 03:15:12.674248+00	{"Latency": "3238", "SucceededAt": "1788837311545", "PerformanceDuration": "485"}	0
1986	625	Failed	An exception occurred during performance of the job.	2026-07-11 06:30:50.098437+00	{"FailedAt": "1783751449435", "ServerId": "nailify background server:1:20837eab-63ae-4ef8-971f-b6a57f9583eb", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
1987	625	Scheduled	Retry attempt 5 of 10: The operation has timed out.	2026-07-11 06:30:50.288636+00	{"EnqueueAt": "1783751736097", "ScheduledAt": "1783751450097"}	0
1990	625	Failed	An exception occurred during performance of the job.	2026-07-11 06:37:53.310486+00	{"FailedAt": "1783751872650", "ServerId": "nailify background server:1:20837eab-63ae-4ef8-971f-b6a57f9583eb", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
1991	625	Scheduled	Retry attempt 6 of 10: The operation has timed out.	2026-07-11 06:37:53.499886+00	{"EnqueueAt": "1783752675309", "ScheduledAt": "1783751873309"}	0
5474	1603	Processing	\N	2026-07-24 05:30:04.677085+00	{"ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "WorkerId": "9ae7688f-8df2-4017-9893-0e4e5fd1b009", "StartedAt": "1784871004210"}	0
2008	625	Enqueued	Triggered by DelayedJobScheduler	2026-07-11 07:16:36.782871+00	{"Queue": "default", "EnqueuedAt": "1783754196110"}	0
15558	4878	Processing	\N	2026-09-08 05:00:19.836508+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788843619128"}	0
15559	4878	Succeeded	\N	2026-09-08 05:00:22.123184+00	{"Latency": "4128", "SucceededAt": "1788843620848", "PerformanceDuration": "444"}	0
15564	4881	Enqueued	Triggered by recurring job scheduler	2026-09-08 05:10:15.259294+00	{"Queue": "default", "EnqueuedAt": "1788844215259"}	0
15641	4906	Enqueued	Triggered by recurring job scheduler	2026-09-08 06:35:16.415229+00	{"Queue": "default", "EnqueuedAt": "1788849316415"}	0
15642	4906	Processing	\N	2026-09-08 06:35:18.543205+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788849317835"}	0
15643	4906	Succeeded	\N	2026-09-08 06:35:20.672064+00	{"Latency": "3123", "SucceededAt": "1788849319537", "PerformanceDuration": "427"}	0
15645	4907	Processing	\N	2026-09-08 06:40:07.863172+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788849607157"}	0
5483	1601	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 05:34:16.000026+00	{"Queue": "default", "EnqueuedAt": "1784871255349"}	0
15646	4908	Enqueued	Triggered by recurring job scheduler	2026-09-08 06:40:08.229034+00	{"Queue": "default", "EnqueuedAt": "1788849608228"}	0
15693	4923	Processing	\N	2026-09-08 08:20:12.110546+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "a7f4d136-bee6-48c7-a7b1-7dcb7bf0ce5c", "StartedAt": "1788855611406"}	0
15744	4940	Processing	\N	2026-09-08 09:15:09.292397+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788858908586"}	0
15746	4941	Enqueued	Triggered by recurring job scheduler	2026-09-08 09:20:11.471818+00	{"Queue": "default", "EnqueuedAt": "1788859211471"}	0
15747	4942	Enqueued	Triggered by recurring job scheduler	2026-09-08 09:20:13.884764+00	{"Queue": "default", "EnqueuedAt": "1788859213884"}	0
15751	4942	Succeeded	\N	2026-09-08 09:20:18.117631+00	{"Latency": "3111", "SucceededAt": "1788859216991", "PerformanceDuration": "424"}	0
2113	625	Enqueued	Triggered by DelayedJobScheduler	2026-07-11 11:16:12.866389+00	{"Queue": "default", "EnqueuedAt": "1783768571879"}	0
16006	5029	Enqueued	Triggered by recurring job scheduler	2026-09-08 14:05:05.399037+00	{"Queue": "default", "EnqueuedAt": "1788876305398"}	0
15460	4846	Enqueued	Triggered by recurring job scheduler	2026-09-08 03:13:28.653284+00	{"Queue": "default", "EnqueuedAt": "1788837208653"}	0
15463	4846	Succeeded	\N	2026-09-08 03:13:40.495014+00	{"Latency": "4302", "SucceededAt": "1788837219099", "PerformanceDuration": "6701"}	0
15563	4880	Enqueued	Triggered by recurring job scheduler	2026-09-08 05:10:12.852776+00	{"Queue": "default", "EnqueuedAt": "1788844212852"}	0
15565	4880	Processing	\N	2026-09-08 05:10:16.02157+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788844215271"}	0
5481	1602	Failed	An exception occurred during performance of the job.	2026-07-24 05:34:12.271089+00	{"FailedAt": "1784871251613", "ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
15566	4881	Processing	\N	2026-09-08 05:10:17.512963+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "f953e7fe-c9a8-4ac8-834a-0fcf3d7e9142", "StartedAt": "1788844216717"}	0
5482	1602	Scheduled	Retry attempt 9 of 10: The operation has timed out.	2026-07-24 05:34:12.461466+00	{"EnqueueAt": "1784875498270", "ScheduledAt": "1784871252270"}	0
1988	625	Enqueued	Triggered by DelayedJobScheduler	2026-07-11 06:35:50.798065+00	{"Queue": "default", "EnqueuedAt": "1783751750134"}	0
5518	1601	Failed	An exception occurred during performance of the job.	2026-07-24 06:48:19.951418+00	{"FailedAt": "1784875699296", "ServerId": "nailify background server:1:05495341-665e-41b1-a8ea-fb7b6cd0dbff", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
15567	4880	Succeeded	\N	2026-09-08 05:10:18.293839+00	{"Latency": "4195", "SucceededAt": "1788844217093", "PerformanceDuration": "471"}	0
15573	4883	Processing	\N	2026-09-08 05:20:12.237321+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788844811486"}	0
15574	4884	Enqueued	Triggered by recurring job scheduler	2026-09-08 05:20:12.605149+00	{"Queue": "default", "EnqueuedAt": "1788844812605"}	0
15575	4883	Succeeded	\N	2026-09-08 05:20:14.496151+00	{"Latency": "3397", "SucceededAt": "1788844813295", "PerformanceDuration": "454"}	0
15576	4884	Processing	\N	2026-09-08 05:20:15.714643+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "f953e7fe-c9a8-4ac8-834a-0fcf3d7e9142", "StartedAt": "1788844815005"}	0
5519	1601	Scheduled	Retry attempt 10 of 10: The operation has timed out.	2026-07-24 06:48:20.138613+00	{"EnqueueAt": "1784882315950", "ScheduledAt": "1784875699950"}	0
15577	4884	Succeeded	\N	2026-09-08 05:20:17.995431+00	{"Latency": "4149", "SucceededAt": "1788844816731", "PerformanceDuration": "450"}	0
15749	4942	Processing	\N	2026-09-08 09:20:16.00173+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "a7f4d136-bee6-48c7-a7b1-7dcb7bf0ce5c", "StartedAt": "1788859215298"}	0
1989	625	Processing	\N	2026-07-11 06:35:51.854361+00	{"ServerId": "nailify background server:1:20837eab-63ae-4ef8-971f-b6a57f9583eb", "WorkerId": "fbad532a-1d11-4982-b562-6ce3de59b2d4", "StartedAt": "1783751751386"}	0
15750	4941	Succeeded	\N	2026-09-08 09:20:16.661744+00	{"Latency": "3962", "SucceededAt": "1788859215457", "PerformanceDuration": "455"}	0
2009	625	Processing	\N	2026-07-11 07:16:37.832045+00	{"ServerId": "nailify background server:1:f11c7cbf-56a8-46d7-a565-e1bd4c71eb9a", "WorkerId": "4cdc5996-1a83-4d4a-bcdd-0540f10efdf1", "StartedAt": "1783754197356"}	0
15752	4943	Enqueued	Triggered by recurring job scheduler	2026-09-08 09:25:05.016648+00	{"Queue": "default", "EnqueuedAt": "1788859505016"}	0
15753	4943	Processing	\N	2026-09-08 09:25:07.270636+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788859506520"}	0
15754	4943	Succeeded	\N	2026-09-08 09:25:09.522057+00	{"Latency": "3307", "SucceededAt": "1788859508322", "PerformanceDuration": "451"}	0
2114	625	Processing	\N	2026-07-11 11:16:13.919533+00	{"ServerId": "nailify background server:1:012df177-73c2-424d-8a6e-e0b60dfa00b7", "WorkerId": "80dbf479-c1e1-4195-b1a5-6c9850f9dbdf", "StartedAt": "1783768573442"}	0
15470	4848	Succeeded	\N	2026-09-08 03:20:17.661345+00	{"Latency": "3126", "SucceededAt": "1788837616520", "PerformanceDuration": "428"}	0
15568	4881	Succeeded	\N	2026-09-08 05:10:19.906538+00	{"Latency": "3320", "SucceededAt": "1788844218633", "PerformanceDuration": "477"}	0
16007	5029	Processing	\N	2026-09-08 14:05:08.292883+00	{"ServerId": "nailify background server:1:cc435256-039f-4352-ad49-91c860e852fd", "WorkerId": "2619ce79-edfe-4df2-bada-97d6aa443c17", "StartedAt": "1788876307385"}	0
16109	5072	Processing	\N	2026-09-08 15:20:15.362827+00	{"ServerId": "nailify background server:1:72e9e6cb-dd7c-4ec2-a0bc-32fa62e228f3", "WorkerId": "d8bb937d-15f2-45b4-8a25-4026051ce0e1", "StartedAt": "1788880814075"}	0
5484	1601	Processing	\N	2026-07-24 05:34:17.933196+00	{"ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "WorkerId": "0cfcd5db-6ca3-4857-93f2-7a4d33aa325b", "StartedAt": "1784871257468"}	0
15569	4882	Enqueued	Triggered by recurring job scheduler	2026-09-08 05:15:04.531354+00	{"Queue": "default", "EnqueuedAt": "1788844504531"}	0
15570	4882	Processing	\N	2026-09-08 05:15:06.704181+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788844505951"}	0
15755	4944	Enqueued	Triggered by recurring job scheduler	2026-09-08 09:30:07.36339+00	{"Queue": "default", "EnqueuedAt": "1788859807337"}	0
15757	4945	Enqueued	Triggered by recurring job scheduler	2026-09-08 09:30:13.141204+00	{"Queue": "default", "EnqueuedAt": "1788859813140"}	0
15571	4882	Succeeded	\N	2026-09-08 05:15:08.875419+00	{"Latency": "3211", "SucceededAt": "1788844507742", "PerformanceDuration": "435"}	0
15572	4883	Enqueued	Triggered by recurring job scheduler	2026-09-08 05:20:09.91989+00	{"Queue": "default", "EnqueuedAt": "1788844809919"}	0
15805	4961	Processing	\N	2026-09-08 10:20:08.391902+00	{"ServerId": "nailify background server:3512:b809f0f4-ed72-4bdd-a858-9934eac117e1", "WorkerId": "a202a7d6-6289-435e-ab34-43b4c6541743", "StartedAt": "1788862806318"}	0
15887	4988	Succeeded	\N	2026-09-08 11:50:22.118815+00	{"Latency": "5664", "SucceededAt": "1788868220029", "PerformanceDuration": "677"}	0
15935	5005	Processing	\N	2026-09-08 12:45:15.90772+00	{"ServerId": "nailify background server:1:25ec5e43-897a-4d2b-be96-cc49f0a7d34b", "WorkerId": "2c170854-5a07-4905-a526-1940ef219eaa", "StartedAt": "1788871514384"}	0
15964	5015	Enqueued	Triggered by recurring job scheduler	2026-09-08 13:20:02.13914+00	{"Queue": "default", "EnqueuedAt": "1788873602139"}	0
15965	5015	Processing	\N	2026-09-08 13:20:04.532557+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788873603635"}	0
16111	5072	Succeeded	\N	2026-09-08 15:20:19.236411+00	{"Latency": "5584", "SucceededAt": "1788880817182", "PerformanceDuration": "779"}	0
15578	4885	Enqueued	Triggered by recurring job scheduler	2026-09-08 05:25:02.571077+00	{"Queue": "default", "EnqueuedAt": "1788845102570"}	0
2078	625	Enqueued	Triggered by DelayedJobScheduler	2026-07-11 09:16:07.08392+00	{"Queue": "default", "EnqueuedAt": "1783761366425"}	0
2080	625	Failed	An exception occurred during performance of the job.	2026-07-11 09:18:09.550751+00	{"FailedAt": "1783761488889", "ServerId": "nailify background server:1:f11c7cbf-56a8-46d7-a565-e1bd4c71eb9a", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
2081	625	Scheduled	Retry attempt 10 of 10: The operation has timed out.	2026-07-11 09:18:09.740193+00	{"EnqueueAt": "1783768355550", "ScheduledAt": "1783761489550"}	0
15974	5018	Processing	\N	2026-09-08 13:30:08.867112+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "a7f4d136-bee6-48c7-a7b1-7dcb7bf0ce5c", "StartedAt": "1788874208161"}	0
15976	5018	Succeeded	\N	2026-09-08 13:30:11.18222+00	{"Latency": "4070", "SucceededAt": "1788874209909", "PerformanceDuration": "476"}	0
15982	5021	Enqueued	Triggered by recurring job scheduler	2026-09-08 13:40:02.087599+00	{"Queue": "default", "EnqueuedAt": "1788874802087"}	0
15983	5021	Processing	\N	2026-09-08 13:40:04.406185+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788874803515"}	0
16114	5056	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 15:21:26.689424+00	{"Queue": "default", "EnqueuedAt": "1788880884883"}	0
16116	5057	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 15:21:29.776603+00	{"Queue": "default", "EnqueuedAt": "1788880887980"}	0
16120	5057	Succeeded	\N	2026-09-08 15:21:36.136851+00	{"Latency": "309052", "SucceededAt": "1788880894085", "PerformanceDuration": "511"}	0
16125	5058	Succeeded	\N	2026-09-08 15:21:40.453473+00	{"Latency": "312352", "SucceededAt": "1788880898403", "PerformanceDuration": "504"}	0
16131	5060	Succeeded	\N	2026-09-08 15:21:47.270413+00	{"Latency": "317480", "SucceededAt": "1788880905219", "PerformanceDuration": "435"}	0
16139	5063	Succeeded	\N	2026-09-08 15:21:54.521252+00	{"Latency": "321983", "SucceededAt": "1788880912470", "PerformanceDuration": "506"}	0
16166	5075	Processing	\N	2026-09-08 15:30:10.78367+00	{"ServerId": "nailify background server:1:72e9e6cb-dd7c-4ec2-a0bc-32fa62e228f3", "WorkerId": "4fe47fac-a840-4aed-934d-357db4c2910a", "StartedAt": "1788881409947"}	0
16168	5075	Succeeded	\N	2026-09-08 15:30:13.347359+00	{"Latency": "4726", "SucceededAt": "1788881411987", "PerformanceDuration": "534"}	0
15471	4849	Processing	\N	2026-09-08 03:20:18.962951+00	{"ServerId": "nailify background server:1:74d36720-dfd4-416f-b17a-72a748859de2", "WorkerId": "eebbaa8b-753d-4f40-b635-63b334ff6abe", "StartedAt": "1788837618225"}	0
15472	4849	Succeeded	\N	2026-09-08 03:20:21.117068+00	{"Latency": "4150", "SucceededAt": "1788837619984", "PerformanceDuration": "450"}	0
15473	4850	Enqueued	Triggered by recurring job scheduler	2026-09-08 03:25:05.477459+00	{"Queue": "default", "EnqueuedAt": "1788837905477"}	0
16009	5030	Enqueued	Triggered by recurring job scheduler	2026-09-08 14:10:14.85069+00	{"Queue": "default", "EnqueuedAt": "1788876614850"}	0
16011	5030	Processing	\N	2026-09-08 14:10:18.146101+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "5bb4a812-7c29-4b27-a144-6d5dc2bb42f5", "StartedAt": "1788876617350"}	0
15474	4850	Processing	\N	2026-09-08 03:25:09.772464+00	{"ServerId": "nailify background server:1:74d36720-dfd4-416f-b17a-72a748859de2", "WorkerId": "87fe26d0-774b-40f7-bc8b-14f496865634", "StartedAt": "1788837909065"}	0
15475	4850	Succeeded	\N	2026-09-08 03:25:11.901823+00	{"Latency": "5295", "SucceededAt": "1788837910770", "PerformanceDuration": "428"}	0
2117	625	Failed	An exception occurred during performance of the job.	2026-07-11 11:18:25.622373+00	{"FailedAt": "1783768704706", "ServerId": "nailify background server:1:012df177-73c2-424d-8a6e-e0b60dfa00b7", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at System.RuntimeMethodHandle.InvokeMethod(Object target, Void** arguments, Signature sig, Boolean isConstructor)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
15579	4885	Processing	\N	2026-09-08 05:25:04.834137+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788845104034"}	0
15580	4885	Succeeded	\N	2026-09-08 05:25:07.203258+00	{"Latency": "3337", "SucceededAt": "1788845105923", "PerformanceDuration": "447"}	0
15581	4886	Enqueued	Triggered by recurring job scheduler	2026-09-08 05:30:07.31557+00	{"Queue": "default", "EnqueuedAt": "1788845407315"}	0
15583	4887	Enqueued	Triggered by recurring job scheduler	2026-09-08 05:30:09.7248+00	{"Queue": "default", "EnqueuedAt": "1788845409724"}	0
15644	4907	Enqueued	Triggered by recurring job scheduler	2026-09-08 06:40:05.667166+00	{"Queue": "default", "EnqueuedAt": "1788849605667"}	0
15647	4907	Succeeded	\N	2026-09-08 06:40:10.010189+00	{"Latency": "3217", "SucceededAt": "1788849608881", "PerformanceDuration": "451"}	0
15648	4908	Processing	\N	2026-09-08 06:40:10.512942+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "f953e7fe-c9a8-4ac8-834a-0fcf3d7e9142", "StartedAt": "1788849609717"}	0
15649	4908	Succeeded	\N	2026-09-08 06:40:12.78169+00	{"Latency": "3372", "SucceededAt": "1788849611583", "PerformanceDuration": "433"}	0
5381	1602	Failed	An exception occurred during performance of the job.	2026-07-24 03:52:42.162498+00	{"FailedAt": "1784865161502", "ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
5382	1602	Scheduled	Retry attempt 3 of 10: The operation has timed out.	2026-07-24 03:52:42.353452+00	{"EnqueueAt": "1784865271161", "ScheduledAt": "1784865162161"}	0
5383	1601	Failed	An exception occurred during performance of the job.	2026-07-24 03:53:15.029756+00	{"FailedAt": "1784865194369", "ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
5384	1601	Scheduled	Retry attempt 3 of 10: The operation has timed out.	2026-07-24 03:53:15.21854+00	{"EnqueueAt": "1784865250028", "ScheduledAt": "1784865195028"}	0
15476	4851	Enqueued	Triggered by recurring job scheduler	2026-09-08 03:37:31.756692+00	{"Queue": "default", "EnqueuedAt": "1788838651746"}	0
16010	5031	Enqueued	Triggered by recurring job scheduler	2026-09-08 14:10:17.414822+00	{"Queue": "default", "EnqueuedAt": "1788876617414"}	0
15477	4851	Processing	\N	2026-09-08 03:37:34.084059+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788838653317"}	0
5347	1601	Processing	\N	2026-07-24 03:45:04.27043+00	{"ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "WorkerId": "21dd2d47-d166-4ac1-beb8-d2bbe9465a8e", "StartedAt": "1784864703808"}	0
5356	1603	Failed	An exception occurred during performance of the job.	2026-07-24 03:47:07.915226+00	{"FailedAt": "1784864827264", "ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
15756	4944	Processing	\N	2026-09-08 09:30:12.670651+00	{"ServerId": "nailify background server:12252:f83b6c45-cc61-4155-858e-f9e8b3a539ca", "WorkerId": "3517d9f0-e06a-4ecd-91b9-0605b9f25d50", "StartedAt": "1788859810866"}	0
15807	4962	Scheduled	\N	2026-09-08 10:24:10.531041+00	{"EnqueueAt": "1788916500000", "ScheduledAt": "1788863050097"}	0
16013	5030	Succeeded	\N	2026-09-08 14:10:20.542895+00	{"Latency": "4389", "SucceededAt": "1788876619269", "PerformanceDuration": "484"}	0
15582	4886	Processing	\N	2026-09-08 05:30:09.441628+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788845408733"}	0
15584	4886	Succeeded	\N	2026-09-08 05:30:11.567264+00	{"Latency": "3119", "SucceededAt": "1788845410435", "PerformanceDuration": "426"}	0
15585	4887	Processing	\N	2026-09-08 05:30:12.708898+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "f953e7fe-c9a8-4ac8-834a-0fcf3d7e9142", "StartedAt": "1788845412000"}	0
15587	4888	Enqueued	Triggered by recurring job scheduler	2026-09-08 05:35:16.483142+00	{"Queue": "default", "EnqueuedAt": "1788845716483"}	0
15590	4889	Enqueued	Triggered by recurring job scheduler	2026-09-08 05:40:05.799285+00	{"Queue": "default", "EnqueuedAt": "1788846005799"}	0
5357	1603	Scheduled	Retry attempt 1 of 10: The operation has timed out.	2026-07-24 03:47:08.102118+00	{"EnqueueAt": "1784864868914", "ScheduledAt": "1784864827914"}	0
5358	1602	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 03:47:27.217828+00	{"Queue": "default", "EnqueuedAt": "1784864846569"}	0
5360	1601	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 03:48:00.020365+00	{"Queue": "default", "EnqueuedAt": "1784864879371"}	0
5362	1603	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 03:48:01.134663+00	{"Queue": "default", "EnqueuedAt": "1784864880487"}	0
5363	1603	Processing	\N	2026-07-24 03:48:02.157862+00	{"ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "WorkerId": "21dd2d47-d166-4ac1-beb8-d2bbe9465a8e", "StartedAt": "1784864881694"}	0
15888	4989	Succeeded	\N	2026-09-08 11:50:24.988042+00	{"Latency": "5123", "SucceededAt": "1788868223147", "PerformanceDuration": "665"}	0
5374	1603	Processing	\N	2026-07-24 03:50:39.541444+00	{"ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "WorkerId": "9ae7688f-8df2-4017-9893-0e4e5fd1b009", "StartedAt": "1784865039074"}	0
5376	1602	Processing	\N	2026-07-24 03:50:40.748784+00	{"ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "WorkerId": "0cfcd5db-6ca3-4857-93f2-7a4d33aa325b", "StartedAt": "1784865040282"}	0
5378	1601	Processing	\N	2026-07-24 03:51:13.613342+00	{"ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "WorkerId": "2934f2ee-fb4d-40c0-a358-a99847227519", "StartedAt": "1784865073146"}	0
5379	1603	Failed	An exception occurred during performance of the job.	2026-07-24 03:52:41.057015+00	{"FailedAt": "1784865160401", "ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
5380	1603	Scheduled	Retry attempt 3 of 10: The operation has timed out.	2026-07-24 03:52:41.244906+00	{"EnqueueAt": "1784865228056", "ScheduledAt": "1784865161056"}	0
5386	1603	Processing	\N	2026-07-24 03:53:52.071171+00	{"ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "WorkerId": "21dd2d47-d166-4ac1-beb8-d2bbe9465a8e", "StartedAt": "1784865231603"}	0
5346	1601	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 03:45:03.228259+00	{"Queue": "default", "EnqueuedAt": "1784864702559"}	0
5348	1602	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 03:45:04.36228+00	{"Queue": "default", "EnqueuedAt": "1784864703711"}	0
5350	1603	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 03:45:05.479964+00	{"Queue": "default", "EnqueuedAt": "1784864704828"}	0
5351	1603	Processing	\N	2026-07-24 03:45:06.505663+00	{"ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "WorkerId": "9ae7688f-8df2-4017-9893-0e4e5fd1b009", "StartedAt": "1784864706041"}	0
5353	1602	Failed	An exception occurred during performance of the job.	2026-07-24 03:47:06.863428+00	{"FailedAt": "1784864826207", "ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
5355	1602	Scheduled	Retry attempt 1 of 10: The operation has timed out.	2026-07-24 03:47:07.051608+00	{"EnqueueAt": "1784864846862", "ScheduledAt": "1784864826862"}	0
15758	4945	Processing	\N	2026-09-08 09:30:17.383882+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "a7f4d136-bee6-48c7-a7b1-7dcb7bf0ce5c", "StartedAt": "1788859816664"}	0
16012	5031	Processing	\N	2026-09-08 14:10:20.468204+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "54ad23b1-3311-415d-91c9-a1c465c0f4ed", "StartedAt": "1788876619761"}	0
16014	5031	Succeeded	\N	2026-09-08 14:10:22.602166+00	{"Latency": "4073", "SucceededAt": "1788876621471", "PerformanceDuration": "435"}	0
1998	625	Enqueued	Triggered by DelayedJobScheduler	2026-07-11 06:51:17.272829+00	{"Queue": "default", "EnqueuedAt": "1783752676600"}	0
16015	5032	Enqueued	Triggered by recurring job scheduler	2026-09-08 14:15:08.001993+00	{"Queue": "default", "EnqueuedAt": "1788876908001"}	0
16019	5033	Processing	\N	2026-09-08 14:20:16.556636+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "54ad23b1-3311-415d-91c9-a1c465c0f4ed", "StartedAt": "1788877215763"}	0
5359	1602	Processing	\N	2026-07-24 03:47:28.242199+00	{"ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "WorkerId": "0cfcd5db-6ca3-4857-93f2-7a4d33aa325b", "StartedAt": "1784864847777"}	0
5367	1601	Failed	An exception occurred during performance of the job.	2026-07-24 03:50:02.468047+00	{"FailedAt": "1784865001813", "ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
5368	1601	Scheduled	Retry attempt 2 of 10: The operation has timed out.	2026-07-24 03:50:02.655193+00	{"EnqueueAt": "1784865064467", "ScheduledAt": "1784865002467"}	0
5373	1603	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 03:50:38.510628+00	{"Queue": "default", "EnqueuedAt": "1784865037859"}	0
5375	1602	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 03:50:39.630158+00	{"Queue": "default", "EnqueuedAt": "1784865038979"}	0
16022	5034	Processing	\N	2026-09-08 14:20:20.938184+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "5bb4a812-7c29-4b27-a144-6d5dc2bb42f5", "StartedAt": "1788877220182"}	0
16023	5034	Succeeded	\N	2026-09-08 14:20:23.132701+00	{"Latency": "5187", "SucceededAt": "1788877221998", "PerformanceDuration": "454"}	0
16112	5073	Processing	\N	2026-09-08 15:20:20.855331+00	{"ServerId": "nailify background server:1:72e9e6cb-dd7c-4ec2-a0bc-32fa62e228f3", "WorkerId": "4fe47fac-a840-4aed-934d-357db4c2910a", "StartedAt": "1788880819630"}	0
15810	4963	Succeeded	\N	2026-09-08 10:25:10.922863+00	{"Latency": "-25194862", "SucceededAt": "1788863107471", "PerformanceDuration": "1241"}	0
15815	4965	Processing	\N	2026-09-08 10:30:22.013681+00	{"ServerId": "nailify background server:3512:b809f0f4-ed72-4bdd-a858-9934eac117e1", "WorkerId": "a202a7d6-6289-435e-ab34-43b4c6541743", "StartedAt": "1788863420128"}	0
15889	4990	Enqueued	Triggered by recurring job scheduler	2026-09-08 11:55:02.694927+00	{"Queue": "default", "EnqueuedAt": "1788868502694"}	0
15891	4990	Succeeded	\N	2026-09-08 11:55:08.718896+00	{"Latency": "4585", "SucceededAt": "1788868507263", "PerformanceDuration": "555"}	0
15936	5005	Succeeded	\N	2026-09-08 12:45:20.565316+00	{"Latency": "6669", "SucceededAt": "1788871518112", "PerformanceDuration": "992"}	0
15967	5015	Succeeded	\N	2026-09-08 13:20:07.410392+00	{"Latency": "3688", "SucceededAt": "1788873605954", "PerformanceDuration": "559"}	0
5349	1602	Processing	\N	2026-07-24 03:45:05.39976+00	{"ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "WorkerId": "9ca1c88a-fb22-4a9d-995a-5a2190b2b1a6", "StartedAt": "1784864704924"}	0
5352	1601	Failed	An exception occurred during performance of the job.	2026-07-24 03:47:06.713688+00	{"FailedAt": "1784864826005", "ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
5354	1601	Scheduled	Retry attempt 1 of 10: The operation has timed out.	2026-07-24 03:47:06.910805+00	{"EnqueueAt": "1784864865684", "ScheduledAt": "1784864826684"}	0
1999	625	Processing	\N	2026-07-11 06:51:18.320705+00	{"ServerId": "nailify background server:1:2049592b-0bb0-4160-8375-d637f20ce5ac", "WorkerId": "6022abc8-9f13-43d9-813b-b5be766a3000", "StartedAt": "1783752677844"}	0
5361	1601	Processing	\N	2026-07-24 03:48:01.053601+00	{"ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "WorkerId": "2934f2ee-fb4d-40c0-a358-a99847227519", "StartedAt": "1784864880580"}	0
2030	625	Enqueued	Triggered by DelayedJobScheduler	2026-07-11 08:02:38.984927+00	{"Queue": "default", "EnqueuedAt": "1783756958319"}	0
2031	625	Processing	\N	2026-07-11 08:02:40.039614+00	{"ServerId": "nailify background server:1:f11c7cbf-56a8-46d7-a565-e1bd4c71eb9a", "WorkerId": "9a90ef69-013a-46ef-829e-fdf2f1f8eeaa", "StartedAt": "1783756959569"}	0
2032	625	Failed	An exception occurred during performance of the job.	2026-07-11 08:04:41.551777+00	{"FailedAt": "1783757080808", "ServerId": "nailify background server:1:f11c7cbf-56a8-46d7-a565-e1bd4c71eb9a", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
2033	625	Scheduled	Retry attempt 9 of 10: The operation has timed out.	2026-07-11 08:04:41.745447+00	{"EnqueueAt": "1783761363550", "ScheduledAt": "1783757081550"}	0
16016	5032	Processing	\N	2026-09-08 14:15:10.393545+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "54ad23b1-3311-415d-91c9-a1c465c0f4ed", "StartedAt": "1788876909597"}	0
16017	5032	Succeeded	\N	2026-09-08 14:15:12.790433+00	{"Latency": "3518", "SucceededAt": "1788876911517", "PerformanceDuration": "481"}	0
16018	5033	Enqueued	Triggered by recurring job scheduler	2026-09-08 14:20:14.235638+00	{"Queue": "default", "EnqueuedAt": "1788877214235"}	0
16020	5034	Enqueued	Triggered by recurring job scheduler	2026-09-08 14:20:16.811076+00	{"Queue": "default", "EnqueuedAt": "1788877216810"}	0
16021	5033	Succeeded	\N	2026-09-08 14:20:18.928596+00	{"Latency": "3409", "SucceededAt": "1788877217666", "PerformanceDuration": "477"}	0
15759	4945	Succeeded	\N	2026-09-08 09:30:19.538799+00	{"Latency": "5881", "SucceededAt": "1788859818403", "PerformanceDuration": "428"}	0
15780	4952	Processing	\N	2026-09-08 09:55:12.678914+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788861311964"}	0
15781	4952	Succeeded	\N	2026-09-08 09:55:14.869263+00	{"Latency": "3189", "SucceededAt": "1788861313737", "PerformanceDuration": "473"}	0
15788	4955	Enqueued	Triggered by recurring job scheduler	2026-09-08 10:05:07.872425+00	{"Queue": "default", "EnqueuedAt": "1788861907872"}	0
15789	4955	Processing	\N	2026-09-08 10:05:10.196914+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788861909492"}	0
15790	4955	Succeeded	\N	2026-09-08 10:05:12.382527+00	{"Latency": "3397", "SucceededAt": "1788861911247", "PerformanceDuration": "483"}	0
16113	5073	Succeeded	\N	2026-09-08 15:20:24.554496+00	{"Latency": "6810", "SucceededAt": "1788880822592", "PerformanceDuration": "754"}	0
5364	1602	Failed	An exception occurred during performance of the job.	2026-07-24 03:49:29.66263+00	{"FailedAt": "1784864968995", "ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
5365	1602	Scheduled	Retry attempt 2 of 10: The operation has timed out.	2026-07-24 03:49:29.853761+00	{"EnqueueAt": "1784865031662", "ScheduledAt": "1784864969662"}	0
5369	1603	Failed	An exception occurred during performance of the job.	2026-07-24 03:50:03.572624+00	{"FailedAt": "1784865002909", "ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
5371	1603	Scheduled	Retry attempt 2 of 10: The operation has timed out.	2026-07-24 03:50:03.761993+00	{"EnqueueAt": "1784865029572", "ScheduledAt": "1784865003572"}	0
5377	1601	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 03:51:12.573959+00	{"Queue": "default", "EnqueuedAt": "1784865071909"}	0
2000	625	Failed	An exception occurred during performance of the job.	2026-07-11 06:53:21.785078+00	{"FailedAt": "1783752801088", "ServerId": "nailify background server:1:2049592b-0bb0-4160-8375-d637f20ce5ac", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
15760	4944	Succeeded	\N	2026-09-08 09:30:18.752615+00	{"Latency": "-25192158", "SucceededAt": "1788859815223", "PerformanceDuration": "1087"}	0
15770	4949	Enqueued	Triggered by recurring job scheduler	2026-09-08 09:45:10.97854+00	{"Queue": "default", "EnqueuedAt": "1788860710977"}	0
16024	5035	Enqueued	Triggered by recurring job scheduler	2026-09-08 14:25:07.383249+00	{"Queue": "default", "EnqueuedAt": "1788877507383"}	0
16025	5035	Processing	\N	2026-09-08 14:25:09.729364+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "54ad23b1-3311-415d-91c9-a1c465c0f4ed", "StartedAt": "1788877509023"}	0
2001	625	Scheduled	Retry attempt 7 of 10: The operation has timed out.	2026-07-11 06:53:21.98134+00	{"EnqueueAt": "1783754189770", "ScheduledAt": "1783752801770"}	0
5387	1601	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 03:54:23.294039+00	{"Queue": "default", "EnqueuedAt": "1784865262636"}	0
16026	5035	Succeeded	\N	2026-09-08 14:25:11.881491+00	{"Latency": "3423", "SucceededAt": "1788877510753", "PerformanceDuration": "458"}	0
16027	5036	Enqueued	Triggered by recurring job scheduler	2026-09-08 14:30:13.194858+00	{"Queue": "default", "EnqueuedAt": "1788877813194"}	0
16170	5076	Succeeded	\N	2026-09-08 15:30:17.889272+00	{"Latency": "4826", "SucceededAt": "1788881416554", "PerformanceDuration": "636"}	0
2079	625	Processing	\N	2026-07-11 09:16:08.1271+00	{"ServerId": "nailify background server:1:f11c7cbf-56a8-46d7-a565-e1bd4c71eb9a", "WorkerId": "4fffb026-b2e7-4d99-90b9-9acdfafd0c42", "StartedAt": "1783761367656"}	0
5385	1603	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 03:53:50.470979+00	{"Queue": "default", "EnqueuedAt": "1784865229814"}	0
5388	1601	Processing	\N	2026-07-24 03:54:24.329998+00	{"ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "WorkerId": "9ca1c88a-fb22-4a9d-995a-5a2190b2b1a6", "StartedAt": "1784865263858"}	0
5390	1602	Processing	\N	2026-07-24 03:54:41.581534+00	{"ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "WorkerId": "9ae7688f-8df2-4017-9893-0e4e5fd1b009", "StartedAt": "1784865281111"}	0
5391	1603	Failed	An exception occurred during performance of the job.	2026-07-24 03:55:53.487495+00	{"FailedAt": "1784865352826", "ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
1968	625	Failed	An exception occurred during performance of the job.	2026-07-11 06:20:06.763485+00	{"FailedAt": "1783750806093", "ServerId": "nailify background server:1:20837eab-63ae-4ef8-971f-b6a57f9583eb", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
1969	625	Scheduled	Retry attempt 2 of 10: The operation has timed out.	2026-07-11 06:20:06.953552+00	{"EnqueueAt": "1783750848762", "ScheduledAt": "1783750806762"}	0
16028	5036	Processing	\N	2026-09-08 14:30:15.449392+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "54ad23b1-3311-415d-91c9-a1c465c0f4ed", "StartedAt": "1788877814738"}	0
15761	4946	Enqueued	Triggered by recurring job scheduler	2026-09-08 09:35:05.736292+00	{"Queue": "default", "EnqueuedAt": "1788860105725"}	0
5392	1603	Scheduled	Retry attempt 4 of 10: The operation has timed out.	2026-07-24 03:55:53.676782+00	{"EnqueueAt": "1784865461486", "ScheduledAt": "1784865353486"}	0
5393	1601	Failed	An exception occurred during performance of the job.	2026-07-24 03:56:25.749182+00	{"FailedAt": "1784865385087", "ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
5394	1601	Scheduled	Retry attempt 4 of 10: The operation has timed out.	2026-07-24 03:56:25.937731+00	{"EnqueueAt": "1784865573748", "ScheduledAt": "1784865385748"}	0
5395	1602	Failed	An exception occurred during performance of the job.	2026-07-24 03:56:43.056121+00	{"FailedAt": "1784865402396", "ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
5389	1602	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 03:54:40.546906+00	{"Queue": "default", "EnqueuedAt": "1784865279891"}	0
15478	4852	Enqueued	Triggered by recurring job scheduler	2026-09-08 03:37:34.228998+00	{"Queue": "default", "EnqueuedAt": "1788838654228"}	0
15479	4852	Processing	\N	2026-09-08 03:37:36.699303+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "f953e7fe-c9a8-4ac8-834a-0fcf3d7e9142", "StartedAt": "1788838655898"}	0
15762	4946	Processing	\N	2026-09-08 09:35:10.77575+00	{"ServerId": "nailify background server:1:7cf87f97-3d37-4c17-aebb-7fe4739b0337", "WorkerId": "6d7d773f-565b-44eb-9fd5-1dee50ab5f52", "StartedAt": "1788860109087"}	0
15763	4946	Succeeded	\N	2026-09-08 09:35:15.786658+00	{"Latency": "7421", "SucceededAt": "1788860113169", "PerformanceDuration": "1083"}	0
16029	5037	Enqueued	Triggered by recurring job scheduler	2026-09-08 14:30:15.870888+00	{"Queue": "default", "EnqueuedAt": "1788877815870"}	0
16030	5036	Succeeded	\N	2026-09-08 14:30:17.582996+00	{"Latency": "3307", "SucceededAt": "1788877816451", "PerformanceDuration": "428"}	0
15480	4851	Succeeded	\N	2026-09-08 03:37:38.598508+00	{"Latency": "3480", "SucceededAt": "1788838657200", "PerformanceDuration": "2511"}	0
15481	4852	Succeeded	\N	2026-09-08 03:37:43.340627+00	{"Latency": "3550", "SucceededAt": "1788838661999", "PerformanceDuration": "4648"}	0
15482	4853	Enqueued	Triggered by recurring job scheduler	2026-09-08 03:40:17.223839+00	{"Queue": "default", "EnqueuedAt": "1788838817223"}	0
15484	4854	Enqueued	Triggered by recurring job scheduler	2026-09-08 03:40:19.785691+00	{"Queue": "default", "EnqueuedAt": "1788838819785"}	0
15485	4853	Succeeded	\N	2026-09-08 03:40:21.929041+00	{"Latency": "3596", "SucceededAt": "1788838820797", "PerformanceDuration": "430"}	0
15764	4947	Enqueued	Triggered by recurring job scheduler	2026-09-08 09:40:04.869534+00	{"Queue": "default", "EnqueuedAt": "1788860404869"}	0
15765	4948	Enqueued	Triggered by recurring job scheduler	2026-09-08 09:40:10.341635+00	{"Queue": "default", "EnqueuedAt": "1788860410341"}	0
15773	4950	Enqueued	Triggered by recurring job scheduler	2026-09-08 09:50:07.894305+00	{"Queue": "default", "EnqueuedAt": "1788861007894"}	0
15774	4951	Enqueued	Triggered by recurring job scheduler	2026-09-08 09:50:11.985084+00	{"Queue": "default", "EnqueuedAt": "1788861011984"}	0
15778	4951	Succeeded	\N	2026-09-08 09:50:19.199682+00	{"Latency": "5283", "SucceededAt": "1788861017281", "PerformanceDuration": "734"}	0
15811	4964	Enqueued	Triggered by recurring job scheduler	2026-09-08 10:30:10.666526+00	{"Queue": "default", "EnqueuedAt": "1788863410666"}	0
15813	4965	Enqueued	Triggered by recurring job scheduler	2026-09-08 10:30:16.638867+00	{"Queue": "default", "EnqueuedAt": "1788863416638"}	0
15816	4965	Succeeded	\N	2026-09-08 10:30:27.593123+00	{"Latency": "-25191932", "SucceededAt": "1788863424894", "PerformanceDuration": "1237"}	0
15819	4966	Succeeded	\N	2026-09-08 10:35:27.437854+00	{"Latency": "-25191954", "SucceededAt": "1788863724592", "PerformanceDuration": "1197"}	0
15821	4967	Processing	\N	2026-09-08 10:40:09.788665+00	{"ServerId": "nailify background server:3512:b809f0f4-ed72-4bdd-a858-9934eac117e1", "WorkerId": "a202a7d6-6289-435e-ab34-43b4c6541743", "StartedAt": "1788864008067"}	0
15824	4967	Succeeded	\N	2026-09-08 10:40:15.464277+00	{"Latency": "-25192173", "SucceededAt": "1788864012430", "PerformanceDuration": "1226"}	0
15890	4990	Processing	\N	2026-09-08 11:55:05.77839+00	{"ServerId": "nailify background server:1:b18e0b9c-1bd7-43d3-b97e-5555ea110c7c", "WorkerId": "29bf4934-95ab-4c8f-8b86-ad0a16221edf", "StartedAt": "1788868504626"}	0
15937	5006	Enqueued	Triggered by recurring job scheduler	2026-09-08 12:50:11.91525+00	{"Queue": "default", "EnqueuedAt": "1788871811914"}	0
15938	5007	Enqueued	Triggered by recurring job scheduler	2026-09-08 12:50:14.954118+00	{"Queue": "default", "EnqueuedAt": "1788871814953"}	0
15942	5007	Succeeded	\N	2026-09-08 12:50:21.069216+00	{"Latency": "4547", "SucceededAt": "1788871819676", "PerformanceDuration": "726"}	0
15968	5016	Processing	\N	2026-09-08 13:20:08.037253+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "a7f4d136-bee6-48c7-a7b1-7dcb7bf0ce5c", "StartedAt": "1788873607330"}	0
15969	5016	Succeeded	\N	2026-09-08 13:20:10.333427+00	{"Latency": "4476", "SucceededAt": "1788873609073", "PerformanceDuration": "467"}	0
16031	5037	Processing	\N	2026-09-08 14:30:18.127874+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "5bb4a812-7c29-4b27-a144-6d5dc2bb42f5", "StartedAt": "1788877817416"}	0
16032	5037	Succeeded	\N	2026-09-08 14:30:20.273909+00	{"Latency": "3300", "SucceededAt": "1788877819141", "PerformanceDuration": "442"}	0
16033	5038	Enqueued	Triggered by recurring job scheduler	2026-09-08 14:35:05.098174+00	{"Queue": "default", "EnqueuedAt": "1788878105097"}	0
16034	5038	Processing	\N	2026-09-08 14:35:07.39063+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "54ad23b1-3311-415d-91c9-a1c465c0f4ed", "StartedAt": "1788878106682"}	0
16035	5038	Succeeded	\N	2026-09-08 14:35:09.550132+00	{"Latency": "3348", "SucceededAt": "1788878108418", "PerformanceDuration": "458"}	0
16036	5039	Enqueued	Triggered by recurring job scheduler	2026-09-08 14:40:10.90024+00	{"Queue": "default", "EnqueuedAt": "1788878410900"}	0
16043	5041	Processing	\N	2026-09-08 14:45:05.036204+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "54ad23b1-3311-415d-91c9-a1c465c0f4ed", "StartedAt": "1788878704329"}	0
16044	5041	Succeeded	\N	2026-09-08 14:45:07.177004+00	{"Latency": "3157", "SucceededAt": "1788878706048", "PerformanceDuration": "444"}	0
16045	5042	Enqueued	Triggered by recurring job scheduler	2026-09-08 14:50:07.507277+00	{"Queue": "default", "EnqueuedAt": "1788879007507"}	0
16046	5043	Enqueued	Triggered by recurring job scheduler	2026-09-08 14:50:10.05202+00	{"Queue": "default", "EnqueuedAt": "1788879010051"}	0
16050	5043	Succeeded	\N	2026-09-08 14:50:14.407981+00	{"Latency": "3229", "SucceededAt": "1788879013277", "PerformanceDuration": "445"}	0
16061	5048	Scheduled	\N	2026-09-08 15:01:14.766756+00	{"EnqueueAt": "1788879974312", "ScheduledAt": "1788879674312"}	0
16063	5050	Scheduled	\N	2026-09-08 15:01:51.537102+00	{"EnqueueAt": "1788880011109", "ScheduledAt": "1788879711109"}	0
5396	1602	Scheduled	Retry attempt 4 of 10: The operation has timed out.	2026-07-24 03:56:43.244382+00	{"EnqueueAt": "1784865519055", "ScheduledAt": "1784865403055"}	0
15766	4947	Processing	\N	2026-09-08 09:40:10.389264+00	{"ServerId": "nailify background server:1:7cf87f97-3d37-4c17-aebb-7fe4739b0337", "WorkerId": "6d7d773f-565b-44eb-9fd5-1dee50ab5f52", "StartedAt": "1788860408083"}	0
15767	4947	Succeeded	\N	2026-09-08 09:40:15.783306+00	{"Latency": "7929", "SucceededAt": "1788860413062", "PerformanceDuration": "1275"}	0
15820	4967	Enqueued	Triggered by recurring job scheduler	2026-09-08 10:40:04.464965+00	{"Queue": "default", "EnqueuedAt": "1788864004464"}	0
16037	5039	Processing	\N	2026-09-08 14:40:13.136492+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "54ad23b1-3311-415d-91c9-a1c465c0f4ed", "StartedAt": "1788878412429"}	0
16038	5040	Enqueued	Triggered by recurring job scheduler	2026-09-08 14:40:13.546892+00	{"Queue": "default", "EnqueuedAt": "1788878413546"}	0
15822	4968	Enqueued	Triggered by recurring job scheduler	2026-09-08 10:40:10.287591+00	{"Queue": "default", "EnqueuedAt": "1788864010287"}	0
16039	5039	Succeeded	\N	2026-09-08 14:40:15.263861+00	{"Latency": "3272", "SucceededAt": "1788878414131", "PerformanceDuration": "426"}	0
15828	4969	Succeeded	\N	2026-09-08 10:45:27.879078+00	{"Latency": "-25189300", "SucceededAt": "1788864324921", "PerformanceDuration": "1156"}	0
15835	4972	Enqueued	Triggered by recurring job scheduler	2026-09-08 10:55:13.858096+00	{"Queue": "default", "EnqueuedAt": "1788864913857"}	0
15837	4972	Succeeded	\N	2026-09-08 10:55:26.291374+00	{"Latency": "-25190852", "SucceededAt": "1788864923248", "PerformanceDuration": "1390"}	0
15851	4977	Processing	\N	2026-09-08 11:10:07.887115+00	{"ServerId": "nailify background server:1:7cf87f97-3d37-4c17-aebb-7fe4739b0337", "WorkerId": "dc17abe1-5707-46f0-87b8-8c4f5b930412", "StartedAt": "1788865806660"}	0
15852	4977	Succeeded	\N	2026-09-08 11:10:11.745289+00	{"Latency": "4769", "SucceededAt": "1788865809789", "PerformanceDuration": "910"}	0
15860	4980	Processing	\N	2026-09-08 11:20:21.451507+00	{"ServerId": "nailify background server:1:7cf87f97-3d37-4c17-aebb-7fe4739b0337", "WorkerId": "6d7d773f-565b-44eb-9fd5-1dee50ab5f52", "StartedAt": "1788866420506"}	0
15861	4980	Succeeded	\N	2026-09-08 11:20:24.668387+00	{"Latency": "6346", "SucceededAt": "1788866422893", "PerformanceDuration": "660"}	0
15863	4981	Processing	\N	2026-09-08 11:25:16.387017+00	{"ServerId": "nailify background server:1:7cf87f97-3d37-4c17-aebb-7fe4739b0337", "WorkerId": "dc17abe1-5707-46f0-87b8-8c4f5b930412", "StartedAt": "1788866714756"}	0
15868	4983	Processing	\N	2026-09-08 11:30:11.554248+00	{"ServerId": "nailify background server:1:7cf87f97-3d37-4c17-aebb-7fe4739b0337", "WorkerId": "6d7d773f-565b-44eb-9fd5-1dee50ab5f52", "StartedAt": "1788867010348"}	0
15870	4983	Succeeded	\N	2026-09-08 11:30:15.188464+00	{"Latency": "5310", "SucceededAt": "1788867013258", "PerformanceDuration": "737"}	0
15898	4993	Enqueued	Triggered by recurring job scheduler	2026-09-08 12:05:15.492018+00	{"Queue": "default", "EnqueuedAt": "1788869115491"}	0
15904	4995	Processing	\N	2026-09-08 12:10:13.337731+00	{"ServerId": "nailify background server:1:b18e0b9c-1bd7-43d3-b97e-5555ea110c7c", "WorkerId": "29bf4934-95ab-4c8f-8b86-ad0a16221edf", "StartedAt": "1788869412496"}	0
15917	4999	Processing	\N	2026-09-08 12:25:15.465182+00	{"ServerId": "nailify background server:1:b18e0b9c-1bd7-43d3-b97e-5555ea110c7c", "WorkerId": "29bf4934-95ab-4c8f-8b86-ad0a16221edf", "StartedAt": "1788870314188"}	0
15918	4999	Succeeded	\N	2026-09-08 12:25:19.438898+00	{"Latency": "6997", "SucceededAt": "1788870317410", "PerformanceDuration": "910"}	0
15939	5006	Processing	\N	2026-09-08 12:50:15.812291+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788871815026"}	0
15941	5006	Succeeded	\N	2026-09-08 12:50:18.267451+00	{"Latency": "5033", "SucceededAt": "1788871816912", "PerformanceDuration": "469"}	0
15977	5019	Processing	\N	2026-09-08 13:30:13.121473+00	{"ServerId": "nailify background server:1:cc435256-039f-4352-ad49-91c860e852fd", "WorkerId": "2619ce79-edfe-4df2-bada-97d6aa443c17", "StartedAt": "1788874212204"}	0
15989	5023	Processing	\N	2026-09-08 13:45:06.805151+00	{"ServerId": "nailify background server:1:cc435256-039f-4352-ad49-91c860e852fd", "WorkerId": "911ea8c4-edf6-46a7-b556-d294ca960732", "StartedAt": "1788875105639"}	0
16040	5040	Processing	\N	2026-09-08 14:40:15.781116+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "5bb4a812-7c29-4b27-a144-6d5dc2bb42f5", "StartedAt": "1788878415074"}	0
16041	5040	Succeeded	\N	2026-09-08 14:40:17.918013+00	{"Latency": "3267", "SucceededAt": "1788878416786", "PerformanceDuration": "439"}	0
16042	5041	Enqueued	Triggered by recurring job scheduler	2026-09-08 14:45:02.886436+00	{"Queue": "default", "EnqueuedAt": "1788878702886"}	0
16115	5056	Processing	\N	2026-09-08 15:21:29.450378+00	{"ServerId": "nailify background server:1:72e9e6cb-dd7c-4ec2-a0bc-32fa62e228f3", "WorkerId": "4fe47fac-a840-4aed-934d-357db4c2910a", "StartedAt": "1788880888240"}	0
16119	5058	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 15:21:34.22447+00	{"Queue": "default", "EnqueuedAt": "1788880892529"}	0
16122	5059	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 15:21:37.128849+00	{"Queue": "default", "EnqueuedAt": "1788880895435"}	0
16124	5060	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 15:21:40.033626+00	{"Queue": "default", "EnqueuedAt": "1788880898339"}	0
16126	5061	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 15:21:42.937984+00	{"Queue": "default", "EnqueuedAt": "1788880901244"}	0
16129	5062	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 15:21:45.843905+00	{"Queue": "default", "EnqueuedAt": "1788880904149"}	0
15483	4853	Processing	\N	2026-09-08 03:40:19.637734+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788838818730"}	0
5397	1603	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 03:57:49.018455+00	{"Queue": "default", "EnqueuedAt": "1784865468361"}	0
5399	1602	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 03:58:52.965992+00	{"Queue": "default", "EnqueuedAt": "1784865532309"}	0
5402	1601	Processing	\N	2026-07-24 03:59:42.360849+00	{"ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "WorkerId": "9ae7688f-8df2-4017-9893-0e4e5fd1b009", "StartedAt": "1784865581891"}	0
5403	1603	Failed	An exception occurred during performance of the job.	2026-07-24 03:59:51.471258+00	{"FailedAt": "1784865590813", "ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
5404	1603	Scheduled	Retry attempt 5 of 10: The operation has timed out.	2026-07-24 03:59:51.659151+00	{"EnqueueAt": "1784865952470", "ScheduledAt": "1784865591470"}	0
16047	5042	Processing	\N	2026-09-08 14:50:10.074497+00	{"ServerId": "nailify background server:1:78c55580-7c90-4616-b8de-490c53f2c84e", "WorkerId": "a46319ce-cd89-4b6b-ac82-37ee4342c363", "StartedAt": "1788879009005"}	0
15486	4854	Processing	\N	2026-09-08 03:40:22.19817+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "f953e7fe-c9a8-4ac8-834a-0fcf3d7e9142", "StartedAt": "1788838821290"}	0
15487	4854	Succeeded	\N	2026-09-08 03:40:24.688503+00	{"Latency": "3593", "SucceededAt": "1788838823498", "PerformanceDuration": "571"}	0
16049	5042	Succeeded	\N	2026-09-08 14:50:12.916713+00	{"Latency": "3844", "SucceededAt": "1788879011466", "PerformanceDuration": "565"}	0
15491	4856	Enqueued	Triggered by recurring job scheduler	2026-09-08 03:50:16.169273+00	{"Queue": "default", "EnqueuedAt": "1788839416169"}	0
15493	4857	Enqueued	Triggered by recurring job scheduler	2026-09-08 03:50:18.895722+00	{"Queue": "default", "EnqueuedAt": "1788839418895"}	0
15494	4856	Succeeded	\N	2026-09-08 03:50:20.603859+00	{"Latency": "3354", "SucceededAt": "1788839419470", "PerformanceDuration": "434"}	0
5416	1601	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 04:07:34.574314+00	{"Queue": "default", "EnqueuedAt": "1784866053919"}	0
5420	1602	Failed	An exception occurred during performance of the job.	2026-07-24 04:08:01.9381+00	{"FailedAt": "1784866081281", "ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
5421	1602	Scheduled	Retry attempt 6 of 10: The operation has timed out.	2026-07-24 04:08:02.125032+00	{"EnqueueAt": "1784866871937", "ScheduledAt": "1784866081937"}	0
15495	4857	Processing	\N	2026-09-08 03:50:21.163+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "f953e7fe-c9a8-4ac8-834a-0fcf3d7e9142", "StartedAt": "1788839420454"}	0
15496	4857	Succeeded	\N	2026-09-08 03:50:23.308381+00	{"Latency": "3313", "SucceededAt": "1788839422175", "PerformanceDuration": "444"}	0
16052	5044	Processing	\N	2026-09-08 14:55:11.44569+00	{"ServerId": "nailify background server:1:78c55580-7c90-4616-b8de-490c53f2c84e", "WorkerId": "60612e68-36ba-488c-8747-3ed3dc28cbd2", "StartedAt": "1788879310241"}	0
16053	5044	Succeeded	\N	2026-09-08 14:55:14.918086+00	{"Latency": "4537", "SucceededAt": "1788879312991", "PerformanceDuration": "583"}	0
16054	5045	Enqueued	Triggered by recurring job scheduler	2026-09-08 15:00:03.506501+00	{"Queue": "default", "EnqueuedAt": "1788879603506"}	0
16056	5046	Enqueued	Triggered by recurring job scheduler	2026-09-08 15:00:06.606332+00	{"Queue": "default", "EnqueuedAt": "1788879606606"}	0
16048	5043	Processing	\N	2026-09-08 14:50:12.256667+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "5bb4a812-7c29-4b27-a144-6d5dc2bb42f5", "StartedAt": "1788879011550"}	0
5398	1603	Processing	\N	2026-07-24 03:57:50.052269+00	{"ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "WorkerId": "21dd2d47-d166-4ac1-beb8-d2bbe9465a8e", "StartedAt": "1784865469584"}	0
16062	5049	Scheduled	\N	2026-09-08 15:01:50.20116+00	{"EnqueueAt": "1788880009775", "ScheduledAt": "1788879709775"}	0
16064	5051	Scheduled	\N	2026-09-08 15:01:52.635297+00	{"EnqueueAt": "1788880012187", "ScheduledAt": "1788879712187"}	0
16065	5052	Enqueued	Triggered by recurring job scheduler	2026-09-08 15:05:14.418597+00	{"Queue": "default", "EnqueuedAt": "1788879914418"}	0
15488	4855	Enqueued	Triggered by recurring job scheduler	2026-09-08 03:45:09.572088+00	{"Queue": "default", "EnqueuedAt": "1788839109571"}	0
15489	4855	Processing	\N	2026-09-08 03:45:11.821867+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788839111116"}	0
5410	1601	Failed	An exception occurred during performance of the job.	2026-07-24 04:01:43.789523+00	{"FailedAt": "1784865703129", "ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
5411	1601	Scheduled	Retry attempt 5 of 10: The operation has timed out.	2026-07-24 04:01:43.977019+00	{"EnqueueAt": "1784866044789", "ScheduledAt": "1784865703789"}	0
5412	1603	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 04:05:58.328428+00	{"Queue": "default", "EnqueuedAt": "1784865957664"}	0
5414	1602	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 04:05:59.472015+00	{"Queue": "default", "EnqueuedAt": "1784865958811"}	0
15490	4855	Succeeded	\N	2026-09-08 03:45:13.968722+00	{"Latency": "3291", "SucceededAt": "1788839112834", "PerformanceDuration": "445"}	0
15492	4856	Processing	\N	2026-09-08 03:50:18.458081+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788839417747"}	0
15497	4858	Enqueued	Triggered by recurring job scheduler	2026-09-08 03:55:08.193388+00	{"Queue": "default", "EnqueuedAt": "1788839708193"}	0
15500	4859	Enqueued	Triggered by recurring job scheduler	2026-09-08 04:00:12.886058+00	{"Queue": "default", "EnqueuedAt": "1788840012885"}	0
15502	4860	Enqueued	Triggered by recurring job scheduler	2026-09-08 04:00:15.303188+00	{"Queue": "default", "EnqueuedAt": "1788840015302"}	0
15508	4861	Succeeded	\N	2026-09-08 04:05:08.840132+00	{"Latency": "3204", "SucceededAt": "1788840307619", "PerformanceDuration": "472"}	0
15512	4862	Succeeded	\N	2026-09-08 04:10:14.892492+00	{"Latency": "3366", "SucceededAt": "1788840613694", "PerformanceDuration": "427"}	0
15768	4948	Processing	\N	2026-09-08 09:40:17.896452+00	{"ServerId": "nailify background server:1:7cf87f97-3d37-4c17-aebb-7fe4739b0337", "WorkerId": "dc17abe1-5707-46f0-87b8-8c4f5b930412", "StartedAt": "1788860416257"}	0
15769	4948	Succeeded	\N	2026-09-08 09:40:22.397377+00	{"Latency": "9764", "SucceededAt": "1788860420079", "PerformanceDuration": "962"}	0
15823	4968	Processing	\N	2026-09-08 10:40:15.553044+00	{"ServerId": "nailify background server:1:7cf87f97-3d37-4c17-aebb-7fe4739b0337", "WorkerId": "6d7d773f-565b-44eb-9fd5-1dee50ab5f52", "StartedAt": "1788864014272"}	0
15899	4993	Processing	\N	2026-09-08 12:05:18.777668+00	{"ServerId": "nailify background server:1:b18e0b9c-1bd7-43d3-b97e-5555ea110c7c", "WorkerId": "29bf4934-95ab-4c8f-8b86-ad0a16221edf", "StartedAt": "1788869117413"}	0
15900	4993	Succeeded	\N	2026-09-08 12:05:22.902047+00	{"Latency": "4995", "SucceededAt": "1788869120610", "PerformanceDuration": "685"}	0
15906	4995	Succeeded	\N	2026-09-08 12:10:17.370613+00	{"Latency": "4505", "SucceededAt": "1788869414731", "PerformanceDuration": "715"}	0
15911	4997	Processing	\N	2026-09-08 12:20:17.09928+00	{"ServerId": "nailify background server:1:b18e0b9c-1bd7-43d3-b97e-5555ea110c7c", "WorkerId": "7827896a-bbf0-48ff-b93b-68216c3a73fc", "StartedAt": "1788870015836"}	0
15913	4997	Succeeded	\N	2026-09-08 12:20:21.12541+00	{"Latency": "5604", "SucceededAt": "1788870018929", "PerformanceDuration": "772"}	0
15916	4999	Enqueued	Triggered by recurring job scheduler	2026-09-08 12:25:11.06161+00	{"Queue": "default", "EnqueuedAt": "1788870311061"}	0
15940	5007	Processing	\N	2026-09-08 12:50:17.98327+00	{"ServerId": "nailify background server:1:25ec5e43-897a-4d2b-be96-cc49f0a7d34b", "WorkerId": "2c170854-5a07-4905-a526-1940ef219eaa", "StartedAt": "1788871816777"}	0
15979	5020	Enqueued	Triggered by recurring job scheduler	2026-09-08 13:35:10.485478+00	{"Queue": "default", "EnqueuedAt": "1788874510485"}	0
16066	5052	Processing	\N	2026-09-08 15:05:16.568762+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "5bb4a812-7c29-4b27-a144-6d5dc2bb42f5", "StartedAt": "1788879915861"}	0
16067	5052	Succeeded	\N	2026-09-08 15:05:18.715802+00	{"Latency": "3147", "SucceededAt": "1788879917583", "PerformanceDuration": "447"}	0
16068	5047	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 15:06:26.367734+00	{"Queue": "default", "EnqueuedAt": "1788879985312"}	0
16069	5047	Processing	\N	2026-09-08 15:06:28.025145+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "54ad23b1-3311-415d-91c9-a1c465c0f4ed", "StartedAt": "1788879987251"}	0
16257	5109	Scheduled	\N	2026-09-08 16:01:03.00362+00	{"EnqueueAt": "1788883562577", "ScheduledAt": "1788883262577"}	0
5400	1602	Processing	\N	2026-07-24 03:58:54.55752+00	{"ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "WorkerId": "9ca1c88a-fb22-4a9d-995a-5a2190b2b1a6", "StartedAt": "1784865534092"}	0
5254	1602	Scheduled	\N	2026-07-23 13:59:23.005341+00	{"EnqueueAt": "1784864700000", "ScheduledAt": "1784815162722"}	0
15498	4858	Processing	\N	2026-09-08 03:55:10.465493+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788839709742"}	0
15499	4858	Succeeded	\N	2026-09-08 03:55:12.591832+00	{"Latency": "3269", "SucceededAt": "1788839711460", "PerformanceDuration": "426"}	0
15501	4859	Processing	\N	2026-09-08 04:00:15.01733+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788840014305"}	0
15504	4860	Processing	\N	2026-09-08 04:00:17.459978+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "f953e7fe-c9a8-4ac8-834a-0fcf3d7e9142", "StartedAt": "1788840016749"}	0
15586	4887	Succeeded	\N	2026-09-08 05:30:14.854452+00	{"Latency": "3974", "SucceededAt": "1788845413719", "PerformanceDuration": "444"}	0
15588	4888	Processing	\N	2026-09-08 05:35:18.638636+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788845717929"}	0
15771	4949	Processing	\N	2026-09-08 09:45:16.096621+00	{"ServerId": "nailify background server:12252:f83b6c45-cc61-4155-858e-f9e8b3a539ca", "WorkerId": "286cc9cb-2cc8-46c0-83be-b5ef65754cd4", "StartedAt": "1788860714498"}	0
5401	1601	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 03:59:41.331137+00	{"Queue": "default", "EnqueuedAt": "1784865580680"}	0
15772	4949	Succeeded	\N	2026-09-08 09:45:21.31557+00	{"Latency": "-25192436", "SucceededAt": "1788860718663", "PerformanceDuration": "1191"}	0
5408	1602	Failed	An exception occurred during performance of the job.	2026-07-24 04:00:55.960839+00	{"FailedAt": "1784865655308", "ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
5409	1602	Scheduled	Retry attempt 5 of 10: The operation has timed out.	2026-07-24 04:00:56.148168+00	{"EnqueueAt": "1784865956960", "ScheduledAt": "1784865655960"}	0
5417	1601	Processing	\N	2026-07-24 04:07:35.613392+00	{"ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "WorkerId": "0cfcd5db-6ca3-4857-93f2-7a4d33aa325b", "StartedAt": "1784866055140"}	0
5418	1603	Failed	An exception occurred during performance of the job.	2026-07-24 04:08:00.807392+00	{"FailedAt": "1784866080142", "ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
5419	1603	Scheduled	Retry attempt 6 of 10: The operation has timed out.	2026-07-24 04:08:00.997841+00	{"EnqueueAt": "1784866840806", "ScheduledAt": "1784866080806"}	0
15825	4968	Succeeded	\N	2026-09-08 10:40:19.493147+00	{"Latency": "7363", "SucceededAt": "1788864017460", "PerformanceDuration": "871"}	0
15901	4994	Enqueued	Triggered by recurring job scheduler	2026-09-08 12:10:06.135691+00	{"Queue": "default", "EnqueuedAt": "1788869406135"}	0
15903	4995	Enqueued	Triggered by recurring job scheduler	2026-09-08 12:10:10.234612+00	{"Queue": "default", "EnqueuedAt": "1788869410234"}	0
15943	5008	Enqueued	Triggered by recurring job scheduler	2026-09-08 12:55:04.877996+00	{"Queue": "default", "EnqueuedAt": "1788872104877"}	0
15944	5008	Processing	\N	2026-09-08 12:55:07.186965+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788872106409"}	0
15945	5008	Succeeded	\N	2026-09-08 12:55:09.464535+00	{"Latency": "3408", "SucceededAt": "1788872108269", "PerformanceDuration": "459"}	0
15980	5020	Processing	\N	2026-09-08 13:35:14.601203+00	{"ServerId": "nailify background server:1:cc435256-039f-4352-ad49-91c860e852fd", "WorkerId": "911ea8c4-edf6-46a7-b556-d294ca960732", "StartedAt": "1788874513291"}	0
15981	5020	Succeeded	\N	2026-09-08 13:35:18.364879+00	{"Latency": "6105", "SucceededAt": "1788874516312", "PerformanceDuration": "670"}	0
15988	5023	Enqueued	Triggered by recurring job scheduler	2026-09-08 13:45:03.421433+00	{"Queue": "default", "EnqueuedAt": "1788875103421"}	0
5413	1603	Processing	\N	2026-07-24 04:05:59.376849+00	{"ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "WorkerId": "9ca1c88a-fb22-4a9d-995a-5a2190b2b1a6", "StartedAt": "1784865958903"}	0
5253	1601	Scheduled	\N	2026-07-23 13:59:16.219403+00	{"EnqueueAt": "1784864700000", "ScheduledAt": "1784815155932"}	0
5415	1602	Processing	\N	2026-07-24 04:06:00.515736+00	{"ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "WorkerId": "9ae7688f-8df2-4017-9893-0e4e5fd1b009", "StartedAt": "1784865960042"}	0
16051	5044	Enqueued	Triggered by recurring job scheduler	2026-09-08 14:55:08.404555+00	{"Queue": "default", "EnqueuedAt": "1788879308402"}	0
16058	5046	Processing	\N	2026-09-08 15:00:09.226048+00	{"ServerId": "nailify background server:1:78c55580-7c90-4616-b8de-490c53f2c84e", "WorkerId": "a46319ce-cd89-4b6b-ac82-37ee4342c363", "StartedAt": "1788879608392"}	0
5436	1603	Failed	An exception occurred during performance of the job.	2026-07-24 04:22:54.173904+00	{"FailedAt": "1784866973509", "ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
5437	1603	Scheduled	Retry attempt 7 of 10: The operation has timed out.	2026-07-24 04:22:54.365045+00	{"EnqueueAt": "1784868418173", "ScheduledAt": "1784866974173"}	0
15775	4950	Processing	\N	2026-09-08 09:50:11.753097+00	{"ServerId": "nailify background server:12252:f83b6c45-cc61-4155-858e-f9e8b3a539ca", "WorkerId": "286cc9cb-2cc8-46c0-83be-b5ef65754cd4", "StartedAt": "1788861009828"}	0
15829	4970	Enqueued	Triggered by recurring job scheduler	2026-09-08 10:50:03.429823+00	{"Queue": "default", "EnqueuedAt": "1788864603429"}	0
5553	1602	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 08:37:09.857764+00	{"Queue": "default", "EnqueuedAt": "1784882229199"}	0
5564	1601	Failed	An exception occurred during performance of the job.	2026-07-24 08:40:49.191452+00	{"FailedAt": "1784882448439", "ServerId": "nailify background server:1:05495341-665e-41b1-a8ea-fb7b6cd0dbff", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
15831	4971	Enqueued	Triggered by recurring job scheduler	2026-09-08 10:50:07.51574+00	{"Queue": "default", "EnqueuedAt": "1788864607515"}	0
15856	4979	Enqueued	Triggered by recurring job scheduler	2026-09-08 11:20:11.316271+00	{"Queue": "default", "EnqueuedAt": "1788866411316"}	0
15858	4980	Enqueued	Triggered by recurring job scheduler	2026-09-08 11:20:16.939185+00	{"Queue": "default", "EnqueuedAt": "1788866416939"}	0
15862	4981	Enqueued	Triggered by recurring job scheduler	2026-09-08 11:25:12.246573+00	{"Queue": "default", "EnqueuedAt": "1788866712246"}	0
16059	5046	Succeeded	\N	2026-09-08 15:00:11.763398+00	{"Latency": "3834", "SucceededAt": "1788879610429", "PerformanceDuration": "534"}	0
16117	5057	Processing	\N	2026-09-08 15:21:32.55754+00	{"ServerId": "nailify background server:1:72e9e6cb-dd7c-4ec2-a0bc-32fa62e228f3", "WorkerId": "d8bb937d-15f2-45b4-8a25-4026051ce0e1", "StartedAt": "1788880891288"}	0
15864	4981	Succeeded	\N	2026-09-08 11:25:21.098028+00	{"Latency": "6231", "SucceededAt": "1788866719069", "PerformanceDuration": "1360"}	0
15866	4982	Processing	\N	2026-09-08 11:30:07.706586+00	{"ServerId": "nailify background server:1:7cf87f97-3d37-4c17-aebb-7fe4739b0337", "WorkerId": "dc17abe1-5707-46f0-87b8-8c4f5b930412", "StartedAt": "1788867006247"}	0
15869	4982	Succeeded	\N	2026-09-08 11:30:12.043874+00	{"Latency": "5771", "SucceededAt": "1788867009706", "PerformanceDuration": "829"}	0
15902	4994	Processing	\N	2026-09-08 12:10:10.022875+00	{"ServerId": "nailify background server:1:b18e0b9c-1bd7-43d3-b97e-5555ea110c7c", "WorkerId": "7827896a-bbf0-48ff-b93b-68216c3a73fc", "StartedAt": "1788869408401"}	0
5422	1601	Failed	An exception occurred during performance of the job.	2026-07-24 04:09:37.035094+00	{"FailedAt": "1784866176376", "ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
5255	1603	Scheduled	\N	2026-07-23 13:59:23.570518+00	{"EnqueueAt": "1784864700000", "ScheduledAt": "1784815163288"}	0
16055	5045	Processing	\N	2026-09-08 15:00:05.920931+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "54ad23b1-3311-415d-91c9-a1c465c0f4ed", "StartedAt": "1788879605132"}	0
16057	5045	Succeeded	\N	2026-09-08 15:00:08.120683+00	{"Latency": "3595", "SucceededAt": "1788879606988", "PerformanceDuration": "434"}	0
15503	4859	Succeeded	\N	2026-09-08 04:00:17.154483+00	{"Latency": "3128", "SucceededAt": "1788840016015", "PerformanceDuration": "427"}	0
15505	4860	Succeeded	\N	2026-09-08 04:00:19.632522+00	{"Latency": "3159", "SucceededAt": "1788840018498", "PerformanceDuration": "465"}	0
15589	4888	Succeeded	\N	2026-09-08 05:35:20.899101+00	{"Latency": "3157", "SucceededAt": "1788845719656", "PerformanceDuration": "448"}	0
15591	4889	Processing	\N	2026-09-08 05:40:08.022033+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "f953e7fe-c9a8-4ac8-834a-0fcf3d7e9142", "StartedAt": "1788846007313"}	0
5423	1601	Scheduled	Retry attempt 6 of 10: The operation has timed out.	2026-07-24 04:09:37.223084+00	{"EnqueueAt": "1784866871034", "ScheduledAt": "1784866177034"}	0
15592	4890	Enqueued	Triggered by recurring job scheduler	2026-09-08 05:40:08.356321+00	{"Queue": "default", "EnqueuedAt": "1788846008356"}	0
5431	1603	Processing	\N	2026-07-24 04:20:52.754815+00	{"ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "WorkerId": "21dd2d47-d166-4ac1-beb8-d2bbe9465a8e", "StartedAt": "1784866852286"}	0
5433	1602	Processing	\N	2026-07-24 04:21:25.560558+00	{"ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "WorkerId": "9ca1c88a-fb22-4a9d-995a-5a2190b2b1a6", "StartedAt": "1784866885093"}	0
16060	5047	Scheduled	\N	2026-09-08 15:01:12.054403+00	{"EnqueueAt": "1788879971619", "ScheduledAt": "1788879671619"}	0
5435	1601	Processing	\N	2026-07-24 04:21:27.254555+00	{"ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "WorkerId": "9ae7688f-8df2-4017-9893-0e4e5fd1b009", "StartedAt": "1784866886786"}	0
5438	1602	Failed	An exception occurred during performance of the job.	2026-07-24 04:23:26.97074+00	{"FailedAt": "1784867006313", "ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
5439	1602	Scheduled	Retry attempt 7 of 10: The operation has timed out.	2026-07-24 04:23:27.157996+00	{"EnqueueAt": "1784868485970", "ScheduledAt": "1784867006970"}	0
5440	1601	Failed	An exception occurred during performance of the job.	2026-07-24 04:23:28.665344+00	{"FailedAt": "1784867008009", "ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
5441	1601	Scheduled	Retry attempt 7 of 10: The operation has timed out.	2026-07-24 04:23:28.852754+00	{"EnqueueAt": "1784868466664", "ScheduledAt": "1784867008664"}	0
16070	5048	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 15:06:28.173865+00	{"Queue": "default", "EnqueuedAt": "1788879987119"}	0
16260	5110	Scheduled	\N	2026-09-08 16:01:04.103669+00	{"EnqueueAt": "1788883563595", "ScheduledAt": "1788883263595"}	0
5430	1603	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 04:20:51.724479+00	{"Queue": "default", "EnqueuedAt": "1784866851070"}	0
5432	1602	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 04:21:24.530736+00	{"Queue": "default", "EnqueuedAt": "1784866883876"}	0
5434	1601	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 04:21:25.652192+00	{"Queue": "default", "EnqueuedAt": "1784866884999"}	0
15506	4861	Enqueued	Triggered by recurring job scheduler	2026-09-08 04:05:04.400947+00	{"Queue": "default", "EnqueuedAt": "1788840304400"}	0
15507	4861	Processing	\N	2026-09-08 04:05:06.57846+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788840305869"}	0
15509	4862	Enqueued	Triggered by recurring job scheduler	2026-09-08 04:10:10.352722+00	{"Queue": "default", "EnqueuedAt": "1788840610352"}	0
15510	4862	Processing	\N	2026-09-08 04:10:12.633685+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788840611843"}	0
15515	4864	Enqueued	Triggered by recurring job scheduler	2026-09-08 04:15:02.163725+00	{"Queue": "default", "EnqueuedAt": "1788840902163"}	0
15593	4889	Succeeded	\N	2026-09-08 05:40:10.189294+00	{"Latency": "3248", "SucceededAt": "1788846009044", "PerformanceDuration": "454"}	0
15594	4890	Processing	\N	2026-09-08 05:40:10.645781+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788846009869"}	0
15595	4890	Succeeded	\N	2026-09-08 05:40:12.921226+00	{"Latency": "3362", "SucceededAt": "1788846011718", "PerformanceDuration": "450"}	0
15596	4891	Enqueued	Triggered by recurring job scheduler	2026-09-08 05:45:12.962751+00	{"Queue": "default", "EnqueuedAt": "1788846312962"}	0
5508	1603	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 06:45:12.366775+00	{"Queue": "default", "EnqueuedAt": "1784875511647"}	0
5510	1602	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 06:45:13.527869+00	{"Queue": "default", "EnqueuedAt": "1784875512866"}	0
15597	4891	Processing	\N	2026-09-08 05:45:15.096059+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "f953e7fe-c9a8-4ac8-834a-0fcf3d7e9142", "StartedAt": "1788846314384"}	0
5554	1602	Processing	\N	2026-07-24 08:37:10.902259+00	{"ServerId": "nailify background server:1:05495341-665e-41b1-a8ea-fb7b6cd0dbff", "WorkerId": "a8a9ac63-d67c-478e-8a71-142707164283", "StartedAt": "1784882230435"}	0
5558	1601	Processing	\N	2026-07-24 08:38:47.684066+00	{"ServerId": "nailify background server:1:05495341-665e-41b1-a8ea-fb7b6cd0dbff", "WorkerId": "ddd5ff48-0808-4869-8b45-757b16379bfd", "StartedAt": "1784882327218"}	0
5560	1603	Failed	An exception occurred during performance of the job.	2026-07-24 08:39:45.251028+00	{"FailedAt": "1784882384494", "ServerId": "nailify background server:1:05495341-665e-41b1-a8ea-fb7b6cd0dbff", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
15598	4891	Succeeded	\N	2026-09-08 05:45:17.22312+00	{"Latency": "3133", "SucceededAt": "1788846316091", "PerformanceDuration": "427"}	0
15776	4951	Processing	\N	2026-09-08 09:50:15.578343+00	{"ServerId": "nailify background server:1:7cf87f97-3d37-4c17-aebb-7fe4739b0337", "WorkerId": "dc17abe1-5707-46f0-87b8-8c4f5b930412", "StartedAt": "1788861014383"}	0
15783	4953	Processing	\N	2026-09-08 10:00:18.771767+00	{"ServerId": "nailify background server:1:7cf87f97-3d37-4c17-aebb-7fe4739b0337", "WorkerId": "dc17abe1-5707-46f0-87b8-8c4f5b930412", "StartedAt": "1788861617861"}	0
15785	4953	Succeeded	\N	2026-09-08 10:00:21.681298+00	{"Latency": "5782", "SucceededAt": "1788861620224", "PerformanceDuration": "713"}	0
15786	4954	Processing	\N	2026-09-08 10:00:24.122911+00	{"ServerId": "nailify background server:1:7cf87f97-3d37-4c17-aebb-7fe4739b0337", "WorkerId": "dc17abe1-5707-46f0-87b8-8c4f5b930412", "StartedAt": "1788861623215"}	0
15787	4954	Succeeded	\N	2026-09-08 10:00:26.924984+00	{"Latency": "5262", "SucceededAt": "1788861625470", "PerformanceDuration": "620"}	0
15830	4970	Processing	\N	2026-09-08 10:50:06.35987+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "a7f4d136-bee6-48c7-a7b1-7dcb7bf0ce5c", "StartedAt": "1788864605641"}	0
15832	4970	Succeeded	\N	2026-09-08 10:50:08.51823+00	{"Latency": "4265", "SucceededAt": "1788864607379", "PerformanceDuration": "440"}	0
15905	4994	Succeeded	\N	2026-09-08 12:10:14.719084+00	{"Latency": "5832", "SucceededAt": "1788869412064", "PerformanceDuration": "822"}	0
15946	5009	Enqueued	Triggered by recurring job scheduler	2026-09-08 13:00:11.938917+00	{"Queue": "default", "EnqueuedAt": "1788872411938"}	0
15948	5010	Enqueued	Triggered by recurring job scheduler	2026-09-08 13:00:17.650439+00	{"Queue": "default", "EnqueuedAt": "1788872417650"}	0
15960	5013	Succeeded	\N	2026-09-08 13:10:16.025329+00	{"Latency": "4199", "SucceededAt": "1788873014686", "PerformanceDuration": "606"}	0
15650	4909	Enqueued	Triggered by recurring job scheduler	2026-09-08 06:45:14.408979+00	{"Queue": "default", "EnqueuedAt": "1788849914408"}	0
15962	5014	Processing	\N	2026-09-08 13:15:07.323515+00	{"ServerId": "nailify background server:1:cc435256-039f-4352-ad49-91c860e852fd", "WorkerId": "911ea8c4-edf6-46a7-b556-d294ca960732", "StartedAt": "1788873306486"}	0
16071	5048	Processing	\N	2026-09-08 15:06:29.795531+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "5bb4a812-7c29-4b27-a144-6d5dc2bb42f5", "StartedAt": "1788879989087"}	0
16073	5048	Succeeded	\N	2026-09-08 15:06:31.807812+00	{"Latency": "316050", "SucceededAt": "1788879990670", "PerformanceDuration": "305"}	0
5448	1603	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 04:47:08.847492+00	{"Queue": "default", "EnqueuedAt": "1784868428184"}	0
5451	1601	Processing	\N	2026-07-24 04:47:58.28788+00	{"ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "WorkerId": "21dd2d47-d166-4ac1-beb8-d2bbe9465a8e", "StartedAt": "1784868477815"}	0
5452	1602	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 04:48:14.511721+00	{"Queue": "default", "EnqueuedAt": "1784868493850"}	0
5454	1603	Failed	An exception occurred during performance of the job.	2026-07-24 04:49:11.318091+00	{"FailedAt": "1784868550663", "ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
15511	4863	Enqueued	Triggered by recurring job scheduler	2026-09-08 04:10:12.930434+00	{"Queue": "default", "EnqueuedAt": "1788840612930"}	0
15513	4863	Processing	\N	2026-09-08 04:10:15.169765+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "f953e7fe-c9a8-4ac8-834a-0fcf3d7e9142", "StartedAt": "1788840614458"}	0
15514	4863	Succeeded	\N	2026-09-08 04:10:17.435711+00	{"Latency": "3272", "SucceededAt": "1788840616173", "PerformanceDuration": "436"}	0
15516	4864	Processing	\N	2026-09-08 04:15:04.298991+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788840903588"}	0
15517	4864	Succeeded	\N	2026-09-08 04:15:06.558205+00	{"Latency": "3138", "SucceededAt": "1788840905349", "PerformanceDuration": "481"}	0
5455	1603	Scheduled	Retry attempt 8 of 10: The operation has timed out.	2026-07-24 04:49:11.504718+00	{"EnqueueAt": "1784870991317", "ScheduledAt": "1784868551317"}	0
15518	4865	Enqueued	Triggered by recurring job scheduler	2026-09-08 04:20:07.259692+00	{"Queue": "default", "EnqueuedAt": "1788841207259"}	0
15520	4866	Enqueued	Triggered by recurring job scheduler	2026-09-08 04:20:09.684831+00	{"Queue": "default", "EnqueuedAt": "1788841209684"}	0
15521	4865	Succeeded	\N	2026-09-08 04:20:11.575437+00	{"Latency": "3161", "SucceededAt": "1788841210445", "PerformanceDuration": "455"}	0
15599	4892	Enqueued	Triggered by recurring job scheduler	2026-09-08 05:50:01.575506+00	{"Queue": "default", "EnqueuedAt": "1788846601575"}	0
5509	1603	Processing	\N	2026-07-24 06:45:13.429108+00	{"ServerId": "nailify background server:1:05495341-665e-41b1-a8ea-fb7b6cd0dbff", "WorkerId": "cb2a3217-6c4c-48e0-ab66-c505d384af8a", "StartedAt": "1784875512959"}	0
5512	1601	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 06:46:17.492815+00	{"Queue": "default", "EnqueuedAt": "1784875576835"}	0
15600	4892	Processing	\N	2026-09-08 05:50:03.703996+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "f953e7fe-c9a8-4ac8-834a-0fcf3d7e9142", "StartedAt": "1788846602995"}	0
15603	4893	Processing	\N	2026-09-08 05:50:06.115541+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788846605408"}	0
5555	1603	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 08:37:42.700176+00	{"Queue": "default", "EnqueuedAt": "1784882262039"}	0
15604	4893	Succeeded	\N	2026-09-08 05:50:08.244568+00	{"Latency": "3121", "SucceededAt": "1788846607110", "PerformanceDuration": "427"}	0
15777	4950	Succeeded	\N	2026-09-08 09:50:16.967772+00	{"Latency": "-25194012", "SucceededAt": "1788861014254", "PerformanceDuration": "1099"}	0
15836	4972	Processing	\N	2026-09-08 10:55:20.118652+00	{"ServerId": "nailify background server:3512:b809f0f4-ed72-4bdd-a858-9934eac117e1", "WorkerId": "149c9abf-1717-4605-81da-698b943e1f5a", "StartedAt": "1788864918149"}	0
16261	5095	Succeeded	\N	2026-09-08 16:01:04.308208+00	{"Latency": "308521", "SucceededAt": "1788883262767", "PerformanceDuration": "353"}	0
15605	4894	Enqueued	Triggered by recurring job scheduler	2026-09-08 05:55:09.439363+00	{"Queue": "default", "EnqueuedAt": "1788846909439"}	0
15842	4973	Succeeded	\N	2026-09-08 11:00:18.365054+00	{"Latency": "-25189633", "SucceededAt": "1788865214880", "PerformanceDuration": "1048"}	0
15843	4974	Succeeded	\N	2026-09-08 11:00:25.042092+00	{"Latency": "-25191266", "SucceededAt": "1788865221422", "PerformanceDuration": "1646"}	0
15907	4996	Enqueued	Triggered by recurring job scheduler	2026-09-08 12:15:02.78883+00	{"Queue": "default", "EnqueuedAt": "1788869702788"}	0
15914	4998	Processing	\N	2026-09-08 12:20:21.545463+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788870020749"}	0
15915	4998	Succeeded	\N	2026-09-08 12:20:23.790043+00	{"Latency": "5354", "SucceededAt": "1788870022659", "PerformanceDuration": "472"}	0
15947	5009	Processing	\N	2026-09-08 13:00:16.50109+00	{"ServerId": "nailify background server:1:25ec5e43-897a-4d2b-be96-cc49f0a7d34b", "WorkerId": "80094ebf-edd7-43b7-aa13-275740456f6c", "StartedAt": "1788872415351"}	0
15949	5009	Succeeded	\N	2026-09-08 13:00:20.013134+00	{"Latency": "6473", "SucceededAt": "1788872418151", "PerformanceDuration": "726"}	0
15984	5022	Enqueued	Triggered by recurring job scheduler	2026-09-08 13:40:04.522614+00	{"Queue": "default", "EnqueuedAt": "1788874804522"}	0
5449	1603	Processing	\N	2026-07-24 04:47:09.893058+00	{"ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "WorkerId": "0cfcd5db-6ca3-4857-93f2-7a4d33aa325b", "StartedAt": "1784868429419"}	0
5450	1601	Enqueued	Triggered by DelayedJobScheduler	2026-07-24 04:47:57.249983+00	{"Queue": "default", "EnqueuedAt": "1784868476594"}	0
5453	1602	Processing	\N	2026-07-24 04:48:15.550781+00	{"ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "WorkerId": "9ca1c88a-fb22-4a9d-995a-5a2190b2b1a6", "StartedAt": "1784868495081"}	0
5456	1601	Failed	An exception occurred during performance of the job.	2026-07-24 04:49:59.703618+00	{"FailedAt": "1784868599049", "ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
16072	5047	Succeeded	\N	2026-09-08 15:06:30.05839+00	{"Latency": "317019", "SucceededAt": "1788879988929", "PerformanceDuration": "284"}	0
16074	5049	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 15:07:02.669395+00	{"Queue": "default", "EnqueuedAt": "1788880021618"}	0
16076	5050	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 15:07:04.472879+00	{"Queue": "default", "EnqueuedAt": "1788880023422"}	0
16078	5049	Succeeded	\N	2026-09-08 15:07:06.244047+00	{"Latency": "315041", "SucceededAt": "1788880025113", "PerformanceDuration": "296"}	0
16121	5058	Processing	\N	2026-09-08 15:21:36.919006+00	{"ServerId": "nailify background server:1:72e9e6cb-dd7c-4ec2-a0bc-32fa62e228f3", "WorkerId": "4fe47fac-a840-4aed-934d-357db4c2910a", "StartedAt": "1788880895698"}	0
5457	1601	Scheduled	Retry attempt 8 of 10: The operation has timed out.	2026-07-24 04:49:59.890544+00	{"EnqueueAt": "1784871247703", "ScheduledAt": "1784868599703"}	0
5461	1602	Failed	An exception occurred during performance of the job.	2026-07-24 04:50:16.965874+00	{"FailedAt": "1784868616310", "ServerId": "nailify background server:1:21fcdf9d-d4fa-4557-9a67-d889e185571f", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
5462	1602	Scheduled	Retry attempt 8 of 10: The operation has timed out.	2026-07-24 04:50:17.152624+00	{"EnqueueAt": "1784871120965", "ScheduledAt": "1784868616965"}	0
15601	4893	Enqueued	Triggered by recurring job scheduler	2026-09-08 05:50:03.987922+00	{"Queue": "default", "EnqueuedAt": "1788846603987"}	0
5511	1602	Processing	\N	2026-07-24 06:45:14.579987+00	{"ServerId": "nailify background server:1:05495341-665e-41b1-a8ea-fb7b6cd0dbff", "WorkerId": "ddd5ff48-0808-4869-8b45-757b16379bfd", "StartedAt": "1784875514093"}	0
5514	1603	Failed	An exception occurred during performance of the job.	2026-07-24 06:47:15.562825+00	{"FailedAt": "1784875634889", "ServerId": "nailify background server:1:05495341-665e-41b1-a8ea-fb7b6cd0dbff", "ExceptionType": "System.TimeoutException", "ExceptionDetails": "System.TimeoutException: The operation has timed out.\\n   at MailKit.Net.SocketUtils.ConnectAsync(String host, Int32 port, IPEndPoint localEndPoint, Int32 timeout, CancellationToken cancellationToken)\\n   at MailKit.MailService.ConnectNetworkAsync(String host, Int32 port, CancellationToken cancellationToken)\\n   at MailKit.Net.Smtp.SmtpClient.ConnectAsync(String host, Int32 port, SecureSocketOptions options, CancellationToken cancellationToken)\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 35\\n   at Nailify.Capstone.Infrastructure.Service.SmtpEmailService.SendEmailAsync(MailRequest request) in /src/Nailify.Capstone.Infrastructure/Service/SmtpEmailService.cs:line 42\\n   at Nailify.Capstone.Application.Services.BackgroundJobs.BookingJobExecutor.SendBookingReminderEmailAsync(Guid bookingId) in /src/Nailify.Capstone.Application/Services/BackgroundJobs/BookingJobExecutor.cs:line 77\\n   at InvokeStub_TaskAwaiter.GetResult(Object, Object, IntPtr*)\\n   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)\\n", "ExceptionMessage": "The operation has timed out."}	0
5515	1603	Scheduled	Retry attempt 10 of 10: The operation has timed out.	2026-07-24 06:47:15.769869+00	{"EnqueueAt": "1784882261550", "ScheduledAt": "1784875635550"}	0
5556	1603	Processing	\N	2026-07-24 08:37:43.739712+00	{"ServerId": "nailify background server:1:05495341-665e-41b1-a8ea-fb7b6cd0dbff", "WorkerId": "80e6e0ca-a7ef-4233-86e7-5b60240c4826", "StartedAt": "1784882263268"}	0
16262	5111	Scheduled	\N	2026-09-08 16:01:04.880796+00	{"EnqueueAt": "1788883564453", "ScheduledAt": "1788883264453"}	0
15779	4952	Enqueued	Triggered by recurring job scheduler	2026-09-08 09:55:10.535747+00	{"Queue": "default", "EnqueuedAt": "1788861310535"}	0
15519	4865	Processing	\N	2026-09-08 04:20:09.419295+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788841208710"}	0
15602	4892	Succeeded	\N	2026-09-08 05:50:05.835908+00	{"Latency": "3127", "SucceededAt": "1788846604700", "PerformanceDuration": "428"}	0
15606	4894	Processing	\N	2026-09-08 05:55:11.766904+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "f953e7fe-c9a8-4ac8-834a-0fcf3d7e9142", "StartedAt": "1788846911016"}	0
15607	4894	Succeeded	\N	2026-09-08 05:55:13.964186+00	{"Latency": "3411", "SucceededAt": "1788846912823", "PerformanceDuration": "454"}	0
15609	4895	Processing	\N	2026-09-08 06:00:18.062353+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "f953e7fe-c9a8-4ac8-834a-0fcf3d7e9142", "StartedAt": "1788847217266"}	0
15651	4909	Processing	\N	2026-09-08 06:45:16.655084+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788849915865"}	0
15653	4910	Enqueued	Triggered by recurring job scheduler	2026-09-08 06:50:06.225417+00	{"Queue": "default", "EnqueuedAt": "1788850206225"}	0
15655	4911	Enqueued	Triggered by recurring job scheduler	2026-09-08 06:50:08.643907+00	{"Queue": "default", "EnqueuedAt": "1788850208643"}	0
15839	4973	Processing	\N	2026-09-08 11:00:12.359333+00	{"ServerId": "nailify background server:3512:b809f0f4-ed72-4bdd-a858-9934eac117e1", "WorkerId": "149c9abf-1717-4605-81da-698b943e1f5a", "StartedAt": "1788865209227"}	0
15841	4974	Processing	\N	2026-09-08 11:00:18.365098+00	{"ServerId": "nailify background server:3512:b809f0f4-ed72-4bdd-a858-9934eac117e1", "WorkerId": "a202a7d6-6289-435e-ab34-43b4c6541743", "StartedAt": "1788865216307"}	0
15844	4975	Enqueued	Triggered by recurring job scheduler	2026-09-08 11:05:03.87896+00	{"Queue": "default", "EnqueuedAt": "1788865503878"}	0
15845	4975	Processing	\N	2026-09-08 11:05:09.754555+00	{"ServerId": "nailify background server:3512:b809f0f4-ed72-4bdd-a858-9934eac117e1", "WorkerId": "149c9abf-1717-4605-81da-698b943e1f5a", "StartedAt": "1788865507758"}	0
15908	4996	Processing	\N	2026-09-08 12:15:04.925368+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "a7f4d136-bee6-48c7-a7b1-7dcb7bf0ce5c", "StartedAt": "1788869704216"}	0
15909	4996	Succeeded	\N	2026-09-08 12:15:07.086996+00	{"Latency": "3159", "SucceededAt": "1788869705957", "PerformanceDuration": "453"}	0
15924	5001	Succeeded	\N	2026-09-08 12:30:10.788514+00	{"Latency": "3177", "SucceededAt": "1788870609426", "PerformanceDuration": "452"}	0
15925	5002	Enqueued	Triggered by recurring job scheduler	2026-09-08 12:35:16.289604+00	{"Queue": "default", "EnqueuedAt": "1788870916289"}	0
15932	5004	Processing	\N	2026-09-08 12:40:11.046529+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788871210295"}	0
15950	5010	Processing	\N	2026-09-08 13:00:23.07156+00	{"ServerId": "nailify background server:1:25ec5e43-897a-4d2b-be96-cc49f0a7d34b", "WorkerId": "80094ebf-edd7-43b7-aa13-275740456f6c", "StartedAt": "1788872421922"}	0
15956	5012	Processing	\N	2026-09-08 13:10:09.917615+00	{"ServerId": "nailify background server:1:cc435256-039f-4352-ad49-91c860e852fd", "WorkerId": "911ea8c4-edf6-46a7-b556-d294ca960732", "StartedAt": "1788873008947"}	0
15958	5013	Processing	\N	2026-09-08 13:10:13.148465+00	{"ServerId": "nailify background server:1:cc435256-039f-4352-ad49-91c860e852fd", "WorkerId": "2619ce79-edfe-4df2-bada-97d6aa443c17", "StartedAt": "1788873012237"}	0
15985	5022	Processing	\N	2026-09-08 13:40:06.687511+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "a7f4d136-bee6-48c7-a7b1-7dcb7bf0ce5c", "StartedAt": "1788874805978"}	0
16075	5049	Processing	\N	2026-09-08 15:07:04.25074+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "5bb4a812-7c29-4b27-a144-6d5dc2bb42f5", "StartedAt": "1788880023545"}	0
16079	5051	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 15:07:06.27542+00	{"Queue": "default", "EnqueuedAt": "1788880025225"}	0
16123	5059	Processing	\N	2026-09-08 15:21:39.916125+00	{"ServerId": "nailify background server:1:72e9e6cb-dd7c-4ec2-a0bc-32fa62e228f3", "WorkerId": "d8bb937d-15f2-45b4-8a25-4026051ce0e1", "StartedAt": "1788880898692"}	0
16127	5059	Succeeded	\N	2026-09-08 15:21:43.337843+00	{"Latency": "314444", "SucceededAt": "1788880901384", "PerformanceDuration": "488"}	0
16175	5078	Processing	\N	2026-09-08 15:40:17.866328+00	{"ServerId": "nailify background server:20928:d61ae4d6-fe62-40ab-9968-45c3569ce248", "WorkerId": "5e77c6ed-fec6-40c7-ba7c-8da9ccc4b67a", "StartedAt": "1788882016241"}	0
16181	5080	Processing	\N	2026-09-08 15:45:21.161202+00	{"ServerId": "nailify background server:20928:d61ae4d6-fe62-40ab-9968-45c3569ce248", "WorkerId": "5e77c6ed-fec6-40c7-ba7c-8da9ccc4b67a", "StartedAt": "1788882319529"}	0
16182	5080	Succeeded	\N	2026-09-08 15:45:26.114164+00	{"Latency": "-25192796", "SucceededAt": "1788882323470", "PerformanceDuration": "997"}	0
16223	5092	Processing	\N	2026-09-08 15:55:06.020156+00	{"ServerId": "nailify background server:1:72e9e6cb-dd7c-4ec2-a0bc-32fa62e228f3", "WorkerId": "d8bb937d-15f2-45b4-8a25-4026051ce0e1", "StartedAt": "1788882905098"}	0
16224	5092	Succeeded	\N	2026-09-08 15:55:08.757921+00	{"Latency": "3901", "SucceededAt": "1788882907311", "PerformanceDuration": "541"}	0
15656	4911	Processing	\N	2026-09-08 06:50:10.855685+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788850210146"}	0
15658	4911	Succeeded	\N	2026-09-08 06:50:13.053093+00	{"Latency": "3207", "SucceededAt": "1788850211917", "PerformanceDuration": "492"}	0
15694	4924	Enqueued	Triggered by recurring job scheduler	2026-09-08 08:20:12.471585+00	{"Queue": "default", "EnqueuedAt": "1788855612471"}	0
15697	4924	Succeeded	\N	2026-09-08 08:20:16.9774+00	{"Latency": "3303", "SucceededAt": "1788855615778", "PerformanceDuration": "452"}	0
15698	4925	Enqueued	Triggered by recurring job scheduler	2026-09-08 08:25:03.979938+00	{"Queue": "default", "EnqueuedAt": "1788855903979"}	0
15699	4925	Processing	\N	2026-09-08 08:25:06.14993+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "a7f4d136-bee6-48c7-a7b1-7dcb7bf0ce5c", "StartedAt": "1788855905393"}	0
16077	5050	Processing	\N	2026-09-08 15:07:06.137399+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "54ad23b1-3311-415d-91c9-a1c465c0f4ed", "StartedAt": "1788880025357"}	0
16080	5051	Processing	\N	2026-09-08 15:07:08.081291+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "5bb4a812-7c29-4b27-a144-6d5dc2bb42f5", "StartedAt": "1788880027376"}	0
16082	5051	Succeeded	\N	2026-09-08 15:07:10.138444+00	{"Latency": "316459", "SucceededAt": "1788880028937", "PerformanceDuration": "291"}	0
16128	5060	Processing	\N	2026-09-08 15:21:43.770912+00	{"ServerId": "nailify background server:1:72e9e6cb-dd7c-4ec2-a0bc-32fa62e228f3", "WorkerId": "4fe47fac-a840-4aed-934d-357db4c2910a", "StartedAt": "1788880902507"}	0
16177	5078	Succeeded	\N	2026-09-08 15:40:23.071671+00	{"Latency": "-25192894", "SucceededAt": "1788882020512", "PerformanceDuration": "1353"}	0
16225	5093	Scheduled	\N	2026-09-08 15:55:27.929819+00	{"EnqueueAt": "1788883227503", "ScheduledAt": "1788882927503"}	0
16245	5093	Processing	\N	2026-09-08 16:00:30.303609+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "54ad23b1-3311-415d-91c9-a1c465c0f4ed", "StartedAt": "1788883229597"}	0
16246	5094	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 16:00:31.612693+00	{"Queue": "default", "EnqueuedAt": "1788883230426"}	0
16263	5112	Scheduled	\N	2026-09-08 16:01:05.733364+00	{"EnqueueAt": "1788883565307", "ScheduledAt": "1788883265307"}	0
16267	5098	Processing	\N	2026-09-08 16:01:08.135832+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "5bb4a812-7c29-4b27-a144-6d5dc2bb42f5", "StartedAt": "1788883267425"}	0
16280	5101	Processing	\N	2026-09-08 16:05:21.509185+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "54ad23b1-3311-415d-91c9-a1c465c0f4ed", "StartedAt": "1788883520800"}	0
16281	5104	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 16:05:22.585281+00	{"Queue": "default", "EnqueuedAt": "1788883521533"}	0
16288	5105	Succeeded	\N	2026-09-08 16:06:04.828164+00	{"Latency": "305176", "SucceededAt": "1788883563393", "PerformanceDuration": "359"}	0
16291	5106	Succeeded	\N	2026-09-08 16:06:10.10276+00	{"Latency": "308493", "SucceededAt": "1788883568732", "PerformanceDuration": "408"}	0
16297	5108	Succeeded	\N	2026-09-08 16:06:16.23091+00	{"Latency": "312648", "SucceededAt": "1788883574791", "PerformanceDuration": "437"}	0
16301	5110	Processing	\N	2026-09-08 16:06:20.524902+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "3483caa3-e204-437c-8e1e-f5559ef0e1a0", "StartedAt": "1788883579627"}	0
16303	5111	Processing	\N	2026-09-08 16:06:23.663522+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "d8caa9dc-5ae2-432a-978a-357a1be22e7e", "StartedAt": "1788883582766"}	0
16309	5114	Enqueued	Triggered by recurring job scheduler	2026-09-08 16:10:02.276703+00	{"Queue": "default", "EnqueuedAt": "1788883802276"}	0
16313	5115	Processing	\N	2026-09-08 16:10:07.873655+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "5bb4a812-7c29-4b27-a144-6d5dc2bb42f5", "StartedAt": "1788883807165"}	0
16319	5118	Enqueued	Triggered by recurring job scheduler	2026-09-08 16:20:11.466324+00	{"Queue": "default", "EnqueuedAt": "1788884411466"}	0
16322	5118	Processing	\N	2026-09-08 16:20:15.67569+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "d8caa9dc-5ae2-432a-978a-357a1be22e7e", "StartedAt": "1788884414841"}	0
16323	5118	Succeeded	\N	2026-09-08 16:20:18.397389+00	{"Latency": "5380", "SucceededAt": "1788884417058", "PerformanceDuration": "713"}	0
16327	5120	Enqueued	Triggered by recurring job scheduler	2026-09-08 16:30:05.716535+00	{"Queue": "default", "EnqueuedAt": "1788885005716"}	0
16329	5121	Enqueued	Triggered by recurring job scheduler	2026-09-08 16:30:09.819356+00	{"Queue": "default", "EnqueuedAt": "1788885009819"}	0
16332	5121	Succeeded	\N	2026-09-08 16:30:17.86009+00	{"Latency": "6161", "SucceededAt": "1788885015935", "PerformanceDuration": "677"}	0
16333	5122	Enqueued	Triggered by recurring job scheduler	2026-09-08 16:35:09.156019+00	{"Queue": "default", "EnqueuedAt": "1788885309155"}	0
16334	5122	Processing	\N	2026-09-08 16:35:11.984642+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "d8caa9dc-5ae2-432a-978a-357a1be22e7e", "StartedAt": "1788885311008"}	0
16335	5122	Succeeded	\N	2026-09-08 16:35:14.913192+00	{"Latency": "4165", "SucceededAt": "1788885313353", "PerformanceDuration": "584"}	0
16336	5123	Enqueued	Triggered by recurring job scheduler	2026-09-08 16:40:03.755022+00	{"Queue": "default", "EnqueuedAt": "1788885603754"}	0
16081	5050	Succeeded	\N	2026-09-08 15:07:08.29538+00	{"Latency": "315650", "SucceededAt": "1788880027056", "PerformanceDuration": "295"}	0
16089	5055	Enqueued	Triggered by recurring job scheduler	2026-09-08 15:15:02.947244+00	{"Queue": "default", "EnqueuedAt": "1788880502947"}	0
16090	5055	Processing	\N	2026-09-08 15:15:07.206925+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "5bb4a812-7c29-4b27-a144-6d5dc2bb42f5", "StartedAt": "1788880506416"}	0
16091	5055	Succeeded	\N	2026-09-08 15:15:09.459023+00	{"Latency": "5348", "SucceededAt": "1788880508327", "PerformanceDuration": "486"}	0
15422	4833	Enqueued	Triggered by recurring job scheduler	2026-09-08 02:14:02.095698+00	{"Queue": "default", "EnqueuedAt": "1788833642084"}	0
15426	4835	Enqueued	Triggered by recurring job scheduler	2026-09-08 02:14:07.263578+00	{"Queue": "default", "EnqueuedAt": "1788833647263"}	0
15427	4833	Succeeded	\N	2026-09-08 02:14:09.221403+00	{"Latency": "3506", "SucceededAt": "1788833647624", "PerformanceDuration": "2586"}	0
15430	4835	Succeeded	\N	2026-09-08 02:14:14.379157+00	{"Latency": "5331", "SucceededAt": "1788833653024", "PerformanceDuration": "1003"}	0
15522	4866	Processing	\N	2026-09-08 04:20:11.940179+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "f953e7fe-c9a8-4ac8-834a-0fcf3d7e9142", "StartedAt": "1788841211164"}	0
15523	4866	Succeeded	\N	2026-09-08 04:20:14.162558+00	{"Latency": "3307", "SucceededAt": "1788841213022", "PerformanceDuration": "457"}	0
15608	4895	Enqueued	Triggered by recurring job scheduler	2026-09-08 06:00:15.808177+00	{"Queue": "default", "EnqueuedAt": "1788847215808"}	0
15610	4896	Enqueued	Triggered by recurring job scheduler	2026-09-08 06:00:18.223112+00	{"Queue": "default", "EnqueuedAt": "1788847218223"}	0
15611	4896	Processing	\N	2026-09-08 06:00:20.346665+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788847219640"}	0
15614	4897	Enqueued	Triggered by recurring job scheduler	2026-09-08 06:05:07.530337+00	{"Queue": "default", "EnqueuedAt": "1788847507530"}	0
15615	4897	Processing	\N	2026-09-08 06:05:09.783396+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "f953e7fe-c9a8-4ac8-834a-0fcf3d7e9142", "StartedAt": "1788847508986"}	0
15616	4897	Succeeded	\N	2026-09-08 06:05:12.144771+00	{"Latency": "3323", "SucceededAt": "1788847510868", "PerformanceDuration": "446"}	0
15620	4898	Succeeded	\N	2026-09-08 06:10:17.54833+00	{"Latency": "3456", "SucceededAt": "1788847816414", "PerformanceDuration": "452"}	0
15621	4899	Processing	\N	2026-09-08 06:10:18.103745+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788847817197"}	0
15652	4909	Succeeded	\N	2026-09-08 06:45:19.012216+00	{"Latency": "3312", "SucceededAt": "1788849917748", "PerformanceDuration": "459"}	0
15654	4910	Processing	\N	2026-09-08 06:50:08.516409+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "f953e7fe-c9a8-4ac8-834a-0fcf3d7e9142", "StartedAt": "1788850207729"}	0
15701	4926	Enqueued	Triggered by recurring job scheduler	2026-09-08 08:30:09.419079+00	{"Queue": "default", "EnqueuedAt": "1788856209418"}	0
15703	4927	Enqueued	Triggered by recurring job scheduler	2026-09-08 08:30:11.825729+00	{"Queue": "default", "EnqueuedAt": "1788856211825"}	0
15704	4926	Succeeded	\N	2026-09-08 08:30:13.942946+00	{"Latency": "3280", "SucceededAt": "1788856212813", "PerformanceDuration": "540"}	0
15706	4927	Succeeded	\N	2026-09-08 08:30:16.287261+00	{"Latency": "3188", "SucceededAt": "1788856215160", "PerformanceDuration": "569"}	0
15710	4929	Enqueued	Triggered by recurring job scheduler	2026-09-08 08:40:14.353736+00	{"Queue": "default", "EnqueuedAt": "1788856814353"}	0
15711	4929	Processing	\N	2026-09-08 08:40:16.481209+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788856815771"}	0
15713	4929	Succeeded	\N	2026-09-08 08:40:18.743678+00	{"Latency": "3122", "SucceededAt": "1788856817613", "PerformanceDuration": "563"}	0
15782	4953	Enqueued	Triggered by recurring job scheduler	2026-09-08 10:00:14.751437+00	{"Queue": "default", "EnqueuedAt": "1788861614751"}	0
15784	4954	Enqueued	Triggered by recurring job scheduler	2026-09-08 10:00:20.606718+00	{"Queue": "default", "EnqueuedAt": "1788861620606"}	0
15848	4976	Processing	\N	2026-09-08 11:10:04.251075+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788865803544"}	0
15849	4977	Enqueued	Triggered by recurring job scheduler	2026-09-08 11:10:04.543139+00	{"Queue": "default", "EnqueuedAt": "1788865804543"}	0
15850	4976	Succeeded	\N	2026-09-08 11:10:06.394792+00	{"Latency": "3142", "SucceededAt": "1788865805247", "PerformanceDuration": "426"}	0
15910	4997	Enqueued	Triggered by recurring job scheduler	2026-09-08 12:20:13.312582+00	{"Queue": "default", "EnqueuedAt": "1788870013312"}	0
15912	4998	Enqueued	Triggered by recurring job scheduler	2026-09-08 12:20:17.585701+00	{"Queue": "default", "EnqueuedAt": "1788870017585"}	0
15951	5010	Succeeded	\N	2026-09-08 13:00:27.418507+00	{"Latency": "7340", "SucceededAt": "1788872424894", "PerformanceDuration": "901"}	0
15986	5021	Succeeded	\N	2026-09-08 13:40:06.749714+00	{"Latency": "3485", "SucceededAt": "1788874805602", "PerformanceDuration": "482"}	0
15987	5022	Succeeded	\N	2026-09-08 13:40:09.18151+00	{"Latency": "3161", "SucceededAt": "1788874807756", "PerformanceDuration": "499"}	0
16092	5056	Scheduled	\N	2026-09-08 15:16:23.993605+00	{"EnqueueAt": "1788880883563", "ScheduledAt": "1788880583563"}	0
16093	5057	Scheduled	\N	2026-09-08 15:16:24.99878+00	{"EnqueueAt": "1788880884521", "ScheduledAt": "1788880584521"}	0
16095	5059	Scheduled	\N	2026-09-08 15:16:26.876036+00	{"EnqueueAt": "1788880886451", "ScheduledAt": "1788880586451"}	0
16096	5060	Scheduled	\N	2026-09-08 15:16:27.726726+00	{"EnqueueAt": "1788880887302", "ScheduledAt": "1788880587302"}	0
16097	5061	Scheduled	\N	2026-09-08 15:16:28.661973+00	{"EnqueueAt": "1788880888152", "ScheduledAt": "1788880588152"}	0
16098	5062	Scheduled	\N	2026-09-08 15:16:29.546106+00	{"EnqueueAt": "1788880889118", "ScheduledAt": "1788880589118"}	0
16103	5067	Scheduled	\N	2026-09-08 15:16:58.83791+00	{"EnqueueAt": "1788880918413", "ScheduledAt": "1788880618413"}	0
16130	5061	Processing	\N	2026-09-08 15:21:46.21858+00	{"ServerId": "nailify background server:1:72e9e6cb-dd7c-4ec2-a0bc-32fa62e228f3", "WorkerId": "d8bb937d-15f2-45b4-8a25-4026051ce0e1", "StartedAt": "1788880905307"}	0
16133	5061	Succeeded	\N	2026-09-08 15:21:48.911717+00	{"Latency": "318797", "SucceededAt": "1788880907451", "PerformanceDuration": "501"}	0
16083	5053	Enqueued	Triggered by recurring job scheduler	2026-09-08 15:10:07.759951+00	{"Queue": "default", "EnqueuedAt": "1788880207751"}	0
16085	5054	Enqueued	Triggered by recurring job scheduler	2026-09-08 15:10:10.888013+00	{"Queue": "default", "EnqueuedAt": "1788880210887"}	0
16132	5063	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 15:21:48.751818+00	{"Queue": "default", "EnqueuedAt": "1788880907057"}	0
16137	5064	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 15:21:51.659047+00	{"Queue": "default", "EnqueuedAt": "1788880909964"}	0
16178	5079	Processing	\N	2026-09-08 15:40:23.16728+00	{"ServerId": "nailify background server:20928:d61ae4d6-fe62-40ab-9968-45c3569ce248", "WorkerId": "f97244b8-6fbd-4cc8-af78-c3772b41c632", "StartedAt": "1788882021572"}	0
16179	5079	Succeeded	\N	2026-09-08 15:40:27.983561+00	{"Latency": "-25193021", "SucceededAt": "1788882025423", "PerformanceDuration": "973"}	0
16180	5080	Enqueued	Triggered by recurring job scheduler	2026-09-08 15:45:16.266394+00	{"Queue": "default", "EnqueuedAt": "1788882316265"}	0
16228	5096	Scheduled	\N	2026-09-08 15:55:56.094756+00	{"EnqueueAt": "1788883255670", "ScheduledAt": "1788882955670"}	0
16229	5097	Scheduled	\N	2026-09-08 15:55:57.700786+00	{"EnqueueAt": "1788883257274", "ScheduledAt": "1788882957274"}	0
16231	5091	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 15:56:28.02075+00	{"Queue": "default", "EnqueuedAt": "1788882986921"}	0
16232	5091	Processing	\N	2026-09-08 15:56:29.735238+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "5bb4a812-7c29-4b27-a144-6d5dc2bb42f5", "StartedAt": "1788882988949"}	0
16234	5099	Scheduled	\N	2026-09-08 15:59:21.659861+00	{"EnqueueAt": "1788883461207", "ScheduledAt": "1788883161207"}	0
16235	5100	Scheduled	\N	2026-09-08 15:59:53.840156+00	{"EnqueueAt": "1788883493413", "ScheduledAt": "1788883193413"}	0
16236	5101	Scheduled	\N	2026-09-08 16:00:10.523577+00	{"EnqueueAt": "1788883510066", "ScheduledAt": "1788883210066"}	0
16247	5093	Succeeded	\N	2026-09-08 16:00:32.366466+00	{"Latency": "303366", "SucceededAt": "1788883231163", "PerformanceDuration": "293"}	0
16249	5094	Succeeded	\N	2026-09-08 16:00:35.513177+00	{"Latency": "304775", "SucceededAt": "1788883234159", "PerformanceDuration": "310"}	0
16252	5106	Scheduled	\N	2026-09-08 16:01:00.313252+00	{"EnqueueAt": "1788883559830", "ScheduledAt": "1788883259830"}	0
16265	5096	Succeeded	\N	2026-09-08 16:01:06.311514+00	{"Latency": "308881", "SucceededAt": "1788883264977", "PerformanceDuration": "424"}	0
16289	5106	Processing	\N	2026-09-08 16:06:07.356089+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "3483caa3-e204-437c-8e1e-f5559ef0e1a0", "StartedAt": "1788883566150"}	0
16290	5107	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 16:06:07.718083+00	{"Queue": "default", "EnqueuedAt": "1788883565924"}	0
16292	5107	Processing	\N	2026-09-08 16:06:10.431484+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "d8caa9dc-5ae2-432a-978a-357a1be22e7e", "StartedAt": "1788883569227"}	0
16293	5108	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 16:06:10.795002+00	{"Queue": "default", "EnqueuedAt": "1788883569001"}	0
16294	5108	Processing	\N	2026-09-08 16:06:13.38768+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "3483caa3-e204-437c-8e1e-f5559ef0e1a0", "StartedAt": "1788883572181"}	0
16296	5109	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 16:06:13.872067+00	{"Queue": "default", "EnqueuedAt": "1788883572078"}	0
16298	5109	Processing	\N	2026-09-08 16:06:17.042634+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "d8caa9dc-5ae2-432a-978a-357a1be22e7e", "StartedAt": "1788883575760"}	0
16299	5110	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 16:06:18.303411+00	{"Queue": "default", "EnqueuedAt": "1788883576618"}	0
16302	5111	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 16:06:21.192828+00	{"Queue": "default", "EnqueuedAt": "1788883579509"}	0
16305	5112	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 16:06:24.083038+00	{"Queue": "default", "EnqueuedAt": "1788883582398"}	0
16306	5112	Processing	\N	2026-09-08 16:06:26.710538+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "3483caa3-e204-437c-8e1e-f5559ef0e1a0", "StartedAt": "1788883585428"}	0
16308	5112	Succeeded	\N	2026-09-08 16:06:30.268897+00	{"Latency": "322430", "SucceededAt": "1788883588219", "PerformanceDuration": "481"}	0
16315	5116	Enqueued	Triggered by recurring job scheduler	2026-09-08 16:15:08.648594+00	{"Queue": "default", "EnqueuedAt": "1788884108648"}	0
16318	5117	Enqueued	Triggered by recurring job scheduler	2026-09-08 16:20:08.624986+00	{"Queue": "default", "EnqueuedAt": "1788884408624"}	0
16084	5053	Processing	\N	2026-09-08 15:10:10.434596+00	{"ServerId": "nailify background server:1:72e9e6cb-dd7c-4ec2-a0bc-32fa62e228f3", "WorkerId": "d8bb937d-15f2-45b4-8a25-4026051ce0e1", "StartedAt": "1788880209587"}	0
16135	5063	Processing	\N	2026-09-08 15:21:51.232201+00	{"ServerId": "nailify background server:1:72e9e6cb-dd7c-4ec2-a0bc-32fa62e228f3", "WorkerId": "4fe47fac-a840-4aed-934d-357db4c2910a", "StartedAt": "1788880910320"}	0
16183	5081	Scheduled	\N	2026-09-08 15:49:32.325036+00	{"EnqueueAt": "1788882871895", "ScheduledAt": "1788882571895"}	0
16186	5084	Scheduled	\N	2026-09-08 15:49:35.350177+00	{"EnqueueAt": "1788882874918", "ScheduledAt": "1788882574918"}	0
16187	5085	Scheduled	\N	2026-09-08 15:49:36.322702+00	{"EnqueueAt": "1788882875897", "ScheduledAt": "1788882575897"}	0
16188	5086	Scheduled	\N	2026-09-08 15:49:37.178589+00	{"EnqueueAt": "1788882876754", "ScheduledAt": "1788882576754"}	0
16192	5089	Processing	\N	2026-09-08 15:50:08.676587+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "54ad23b1-3311-415d-91c9-a1c465c0f4ed", "StartedAt": "1788882607968"}	0
16194	5089	Succeeded	\N	2026-09-08 15:50:10.827689+00	{"Latency": "3157", "SucceededAt": "1788882609696", "PerformanceDuration": "452"}	0
16196	5090	Succeeded	\N	2026-09-08 15:50:13.620949+00	{"Latency": "3381", "SucceededAt": "1788882612307", "PerformanceDuration": "425"}	0
16197	5091	Scheduled	\N	2026-09-08 15:51:24.693245+00	{"EnqueueAt": "1788882984201", "ScheduledAt": "1788882684201"}	0
16198	5081	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 15:54:36.431181+00	{"Queue": "default", "EnqueuedAt": "1788882875440"}	0
16199	5082	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 15:54:38.125657+00	{"Queue": "default", "EnqueuedAt": "1788882877139"}	0
15423	4833	Processing	\N	2026-09-08 02:14:04.409218+00	{"ServerId": "nailify background server:1:a0292a93-5fe1-4130-9485-2ea5702666b1", "WorkerId": "d167b873-3f4b-4b1d-bfc9-a25631755035", "StartedAt": "1788833643610"}	0
15524	4867	Enqueued	Triggered by recurring job scheduler	2026-09-08 04:25:17.148076+00	{"Queue": "default", "EnqueuedAt": "1788841517147"}	0
15525	4867	Processing	\N	2026-09-08 04:25:19.366944+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788841518593"}	0
15526	4867	Succeeded	\N	2026-09-08 04:25:21.604548+00	{"Latency": "3300", "SucceededAt": "1788841520469", "PerformanceDuration": "481"}	0
15528	4868	Processing	\N	2026-09-08 04:30:08.938524+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "f953e7fe-c9a8-4ac8-834a-0fcf3d7e9142", "StartedAt": "1788841808229"}	0
15531	4869	Processing	\N	2026-09-08 04:30:11.411525+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788841810623"}	0
15532	4869	Succeeded	\N	2026-09-08 04:30:13.625313+00	{"Latency": "3262", "SucceededAt": "1788841812497", "PerformanceDuration": "454"}	0
15612	4895	Succeeded	\N	2026-09-08 06:00:20.412517+00	{"Latency": "3319", "SucceededAt": "1788847219159", "PerformanceDuration": "459"}	0
15613	4896	Succeeded	\N	2026-09-08 06:00:22.510587+00	{"Latency": "3115", "SucceededAt": "1788847221376", "PerformanceDuration": "462"}	0
15657	4910	Succeeded	\N	2026-09-08 06:50:10.851916+00	{"Latency": "3350", "SucceededAt": "1788850209638", "PerformanceDuration": "490"}	0
15702	4926	Processing	\N	2026-09-08 08:30:11.64202+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "a7f4d136-bee6-48c7-a7b1-7dcb7bf0ce5c", "StartedAt": "1788856210854"}	0
15708	4928	Processing	\N	2026-09-08 08:35:07.85834+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788856507152"}	0
15709	4928	Succeeded	\N	2026-09-08 08:35:10.447188+00	{"Latency": "3317", "SucceededAt": "1788856509010", "PerformanceDuration": "584"}	0
15791	4956	Enqueued	Triggered by recurring job scheduler	2026-09-08 10:10:07.969572+00	{"Queue": "default", "EnqueuedAt": "1788862207952"}	0
15793	4957	Enqueued	Triggered by recurring job scheduler	2026-09-08 10:10:14.228459+00	{"Queue": "default", "EnqueuedAt": "1788862214226"}	0
15853	4978	Enqueued	Triggered by recurring job scheduler	2026-09-08 11:15:09.973048+00	{"Queue": "default", "EnqueuedAt": "1788866109972"}	0
15854	4978	Processing	\N	2026-09-08 11:15:12.283981+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788866111571"}	0
15855	4978	Succeeded	\N	2026-09-08 11:15:14.419345+00	{"Latency": "3361", "SucceededAt": "1788866113280", "PerformanceDuration": "428"}	0
15919	5000	Enqueued	Triggered by recurring job scheduler	2026-09-08 12:30:03.801934+00	{"Queue": "default", "EnqueuedAt": "1788870603801"}	0
15921	5001	Enqueued	Triggered by recurring job scheduler	2026-09-08 12:30:06.223254+00	{"Queue": "default", "EnqueuedAt": "1788870606223"}	0
15922	5001	Processing	\N	2026-09-08 12:30:08.407351+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "a7f4d136-bee6-48c7-a7b1-7dcb7bf0ce5c", "StartedAt": "1788870607696"}	0
15926	5002	Processing	\N	2026-09-08 12:35:18.551894+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788870917845"}	0
15927	5002	Succeeded	\N	2026-09-08 12:35:20.701291+00	{"Latency": "3310", "SucceededAt": "1788870919570", "PerformanceDuration": "451"}	0
15928	5003	Enqueued	Triggered by recurring job scheduler	2026-09-08 12:40:06.05+00	{"Queue": "default", "EnqueuedAt": "1788871206049"}	0
15931	5003	Succeeded	\N	2026-09-08 12:40:10.898985+00	{"Latency": "3388", "SucceededAt": "1788871209447", "PerformanceDuration": "483"}	0
15933	5004	Succeeded	\N	2026-09-08 12:40:13.557794+00	{"Latency": "3392", "SucceededAt": "1788871212107", "PerformanceDuration": "458"}	0
15952	5011	Enqueued	Triggered by recurring job scheduler	2026-09-08 13:05:08.183806+00	{"Queue": "default", "EnqueuedAt": "1788872708183"}	0
15953	5011	Processing	\N	2026-09-08 13:05:10.536757+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "a7f4d136-bee6-48c7-a7b1-7dcb7bf0ce5c", "StartedAt": "1788872709742"}	0
15954	5011	Succeeded	\N	2026-09-08 13:05:12.767183+00	{"Latency": "3476", "SucceededAt": "1788872711633", "PerformanceDuration": "458"}	0
15966	5016	Enqueued	Triggered by recurring job scheduler	2026-09-08 13:20:04.694746+00	{"Queue": "default", "EnqueuedAt": "1788873604639"}	0
16200	5081	Processing	\N	2026-09-08 15:54:39.06758+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "54ad23b1-3311-415d-91c9-a1c465c0f4ed", "StartedAt": "1788882878260"}	0
15792	4956	Processing	\N	2026-09-08 10:10:13.639938+00	{"ServerId": "nailify background server:3512:b809f0f4-ed72-4bdd-a858-9934eac117e1", "WorkerId": "a202a7d6-6289-435e-ab34-43b4c6541743", "StartedAt": "1788862211739"}	0
15794	4956	Succeeded	\N	2026-09-08 10:10:19.382357+00	{"Latency": "-25191750", "SucceededAt": "1788862216420", "PerformanceDuration": "1233"}	0
15796	4957	Succeeded	\N	2026-09-08 10:10:24.812878+00	{"Latency": "-25192266", "SucceededAt": "1788862222014", "PerformanceDuration": "1064"}	0
15857	4979	Processing	\N	2026-09-08 11:20:15.707077+00	{"ServerId": "nailify background server:1:7cf87f97-3d37-4c17-aebb-7fe4739b0337", "WorkerId": "dc17abe1-5707-46f0-87b8-8c4f5b930412", "StartedAt": "1788866414240"}	0
15859	4979	Succeeded	\N	2026-09-08 11:20:20.44858+00	{"Latency": "6786", "SucceededAt": "1788866417675", "PerformanceDuration": "715"}	0
15865	4982	Enqueued	Triggered by recurring job scheduler	2026-09-08 11:30:03.834126+00	{"Queue": "default", "EnqueuedAt": "1788867003833"}	0
15867	4983	Enqueued	Triggered by recurring job scheduler	2026-09-08 11:30:07.934566+00	{"Queue": "default", "EnqueuedAt": "1788867007934"}	0
16086	5053	Succeeded	\N	2026-09-08 15:10:13.57177+00	{"Latency": "4014", "SucceededAt": "1788880212231", "PerformanceDuration": "1123"}	0
16088	5054	Succeeded	\N	2026-09-08 15:10:16.46462+00	{"Latency": "4112", "SucceededAt": "1788880215128", "PerformanceDuration": "675"}	0
16141	5065	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 15:22:12.617416+00	{"Queue": "default", "EnqueuedAt": "1788880930845"}	0
16143	5066	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 15:22:15.648964+00	{"Queue": "default", "EnqueuedAt": "1788880933881"}	0
16146	5067	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 15:22:18.684502+00	{"Queue": "default", "EnqueuedAt": "1788880936915"}	0
16149	5068	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 15:22:21.719762+00	{"Queue": "default", "EnqueuedAt": "1788880939948"}	0
16152	5069	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 15:22:24.751389+00	{"Queue": "default", "EnqueuedAt": "1788880942984"}	0
16155	5070	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 15:22:27.7827+00	{"Queue": "default", "EnqueuedAt": "1788880946015"}	0
16157	5071	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 15:22:30.814338+00	{"Queue": "default", "EnqueuedAt": "1788880949047"}	0
16169	5076	Processing	\N	2026-09-08 15:30:15.248011+00	{"ServerId": "nailify background server:1:72e9e6cb-dd7c-4ec2-a0bc-32fa62e228f3", "WorkerId": "d8bb937d-15f2-45b4-8a25-4026051ce0e1", "StartedAt": "1788881414308"}	0
16184	5082	Scheduled	\N	2026-09-08 15:49:33.69531+00	{"EnqueueAt": "1788882873213", "ScheduledAt": "1788882573213"}	0
16189	5087	Scheduled	\N	2026-09-08 15:49:38.188493+00	{"EnqueueAt": "1788882877763", "ScheduledAt": "1788882577763"}	0
16191	5089	Enqueued	Triggered by recurring job scheduler	2026-09-08 15:50:06.51283+00	{"Queue": "default", "EnqueuedAt": "1788882606512"}	0
16193	5090	Enqueued	Triggered by recurring job scheduler	2026-09-08 15:50:08.92605+00	{"Queue": "default", "EnqueuedAt": "1788882608925"}	0
16201	5083	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 15:54:39.832552+00	{"Queue": "default", "EnqueuedAt": "1788882878833"}	0
16204	5083	Processing	\N	2026-09-08 15:54:41.423807+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "5bb4a812-7c29-4b27-a144-6d5dc2bb42f5", "StartedAt": "1788882880719"}	0
16208	5084	Processing	\N	2026-09-08 15:54:43.237573+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "54ad23b1-3311-415d-91c9-a1c465c0f4ed", "StartedAt": "1788882882488"}	0
16210	5084	Succeeded	\N	2026-09-08 15:54:45.325002+00	{"Latency": "308920", "SucceededAt": "1788882884120", "PerformanceDuration": "282"}	0
16215	5087	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 15:54:47.844631+00	{"Queue": "default", "EnqueuedAt": "1788882886745"}	0
16217	5086	Succeeded	\N	2026-09-08 15:54:49.570415+00	{"Latency": "311335", "SucceededAt": "1788882888372", "PerformanceDuration": "282"}	0
16219	5088	Processing	\N	2026-09-08 15:54:51.548652+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "54ad23b1-3311-415d-91c9-a1c465c0f4ed", "StartedAt": "1788882890762"}	0
16227	5095	Scheduled	\N	2026-09-08 15:55:54.320156+00	{"EnqueueAt": "1788883253892", "ScheduledAt": "1788882953892"}	0
16237	5102	Enqueued	Triggered by recurring job scheduler	2026-09-08 16:00:11.783417+00	{"Queue": "default", "EnqueuedAt": "1788883211767"}	0
16239	5103	Enqueued	Triggered by recurring job scheduler	2026-09-08 16:00:14.899574+00	{"Queue": "default", "EnqueuedAt": "1788883214899"}	0
16248	5094	Processing	\N	2026-09-08 16:00:33.280413+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "5bb4a812-7c29-4b27-a144-6d5dc2bb42f5", "StartedAt": "1788883232573"}	0
16250	5105	Scheduled	\N	2026-09-08 16:00:58.281683+00	{"EnqueueAt": "1788883557857", "ScheduledAt": "1788883257857"}	0
16253	5107	Scheduled	\N	2026-09-08 16:01:01.108961+00	{"EnqueueAt": "1788883560683", "ScheduledAt": "1788883260683"}	0
16295	5107	Succeeded	\N	2026-09-08 16:06:13.819915+00	{"Latency": "310713", "SucceededAt": "1788883571881", "PerformanceDuration": "484"}	0
15920	5000	Processing	\N	2026-09-08 12:30:06.031917+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788870605229"}	0
15923	5000	Succeeded	\N	2026-09-08 12:30:08.41173+00	{"Latency": "3300", "SucceededAt": "1788870607129", "PerformanceDuration": "455"}	0
15955	5012	Enqueued	Triggered by recurring job scheduler	2026-09-08 13:10:06.762201+00	{"Queue": "default", "EnqueuedAt": "1788873006754"}	0
15957	5013	Enqueued	Triggered by recurring job scheduler	2026-09-08 13:10:10.428622+00	{"Queue": "default", "EnqueuedAt": "1788873010428"}	0
15961	5014	Enqueued	Triggered by recurring job scheduler	2026-09-08 13:15:04.695584+00	{"Queue": "default", "EnqueuedAt": "1788873304695"}	0
15970	5017	Enqueued	Triggered by recurring job scheduler	2026-09-08 13:25:08.881484+00	{"Queue": "default", "EnqueuedAt": "1788873908880"}	0
15972	5017	Succeeded	\N	2026-09-08 13:25:15.532455+00	{"Latency": "4774", "SucceededAt": "1788873913631", "PerformanceDuration": "695"}	0
15973	5018	Enqueued	Triggered by recurring job scheduler	2026-09-08 13:30:06.114478+00	{"Queue": "default", "EnqueuedAt": "1788874206114"}	0
15975	5019	Enqueued	Triggered by recurring job scheduler	2026-09-08 13:30:10.028081+00	{"Queue": "default", "EnqueuedAt": "1788874210026"}	0
15978	5019	Succeeded	\N	2026-09-08 13:30:16.275175+00	{"Latency": "4519", "SucceededAt": "1788874214437", "PerformanceDuration": "582"}	0
15990	5023	Succeeded	\N	2026-09-08 13:45:09.865404+00	{"Latency": "5041", "SucceededAt": "1788875108401", "PerformanceDuration": "663"}	0
15795	4957	Processing	\N	2026-09-08 10:10:19.558036+00	{"ServerId": "nailify background server:3512:b809f0f4-ed72-4bdd-a858-9934eac117e1", "WorkerId": "149c9abf-1717-4605-81da-698b943e1f5a", "StartedAt": "1788862217736"}	0
15871	4984	Enqueued	Triggered by recurring job scheduler	2026-09-08 11:35:13.891034+00	{"Queue": "default", "EnqueuedAt": "1788867313890"}	0
15874	4985	Enqueued	Triggered by recurring job scheduler	2026-09-08 11:40:03.278701+00	{"Queue": "default", "EnqueuedAt": "1788867603278"}	0
15875	4985	Processing	\N	2026-09-08 11:40:05.405433+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "a7f4d136-bee6-48c7-a7b1-7dcb7bf0ce5c", "StartedAt": "1788867604696"}	0
15877	4985	Succeeded	\N	2026-09-08 11:40:07.535939+00	{"Latency": "3122", "SucceededAt": "1788867606400", "PerformanceDuration": "425"}	0
15882	4987	Succeeded	\N	2026-09-08 11:45:14.374039+00	{"Latency": "3117", "SucceededAt": "1788867913244", "PerformanceDuration": "425"}	0
15892	4991	Enqueued	Triggered by recurring job scheduler	2026-09-08 12:00:02.282976+00	{"Queue": "default", "EnqueuedAt": "1788868802282"}	0
15896	4992	Processing	\N	2026-09-08 12:00:07.829696+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788868807122"}	0
15897	4992	Succeeded	\N	2026-09-08 12:00:10.092958+00	{"Latency": "4101", "SucceededAt": "1788868808837", "PerformanceDuration": "441"}	0
15929	5003	Processing	\N	2026-09-08 12:40:08.361486+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "a7f4d136-bee6-48c7-a7b1-7dcb7bf0ce5c", "StartedAt": "1788871207612"}	0
15930	5004	Enqueued	Triggered by recurring job scheduler	2026-09-08 12:40:08.731286+00	{"Queue": "default", "EnqueuedAt": "1788871208731"}	0
15959	5012	Succeeded	\N	2026-09-08 13:10:13.195046+00	{"Latency": "4478", "SucceededAt": "1788873011273", "PerformanceDuration": "624"}	0
16087	5054	Processing	\N	2026-09-08 15:10:13.663535+00	{"ServerId": "nailify background server:1:72e9e6cb-dd7c-4ec2-a0bc-32fa62e228f3", "WorkerId": "4fe47fac-a840-4aed-934d-357db4c2910a", "StartedAt": "1788880212679"}	0
16142	5065	Processing	\N	2026-09-08 15:22:14.904859+00	{"ServerId": "nailify background server:1:72e9e6cb-dd7c-4ec2-a0bc-32fa62e228f3", "WorkerId": "d8bb937d-15f2-45b4-8a25-4026051ce0e1", "StartedAt": "1788880933992"}	0
16144	5066	Processing	\N	2026-09-08 15:22:17.940311+00	{"ServerId": "nailify background server:1:72e9e6cb-dd7c-4ec2-a0bc-32fa62e228f3", "WorkerId": "4fe47fac-a840-4aed-934d-357db4c2910a", "StartedAt": "1788880937029"}	0
16147	5066	Succeeded	\N	2026-09-08 15:22:20.494949+00	{"Latency": "321107", "SucceededAt": "1788880939034", "PerformanceDuration": "364"}	0
16150	5068	Processing	\N	2026-09-08 15:22:24.006605+00	{"ServerId": "nailify background server:1:72e9e6cb-dd7c-4ec2-a0bc-32fa62e228f3", "WorkerId": "4fe47fac-a840-4aed-934d-357db4c2910a", "StartedAt": "1788880943096"}	0
16153	5068	Succeeded	\N	2026-09-08 15:22:26.556999+00	{"Latency": "325470", "SucceededAt": "1788880945100", "PerformanceDuration": "364"}	0
16156	5070	Processing	\N	2026-09-08 15:22:30.06884+00	{"ServerId": "nailify background server:1:72e9e6cb-dd7c-4ec2-a0bc-32fa62e228f3", "WorkerId": "4fe47fac-a840-4aed-934d-357db4c2910a", "StartedAt": "1788880949158"}	0
16159	5070	Succeeded	\N	2026-09-08 15:22:32.791022+00	{"Latency": "329801", "SucceededAt": "1788880951280", "PerformanceDuration": "399"}	0
16161	5071	Succeeded	\N	2026-09-08 15:22:37.06655+00	{"Latency": "333239", "SucceededAt": "1788880955607", "PerformanceDuration": "434"}	0
16185	5083	Scheduled	\N	2026-09-08 15:49:34.490984+00	{"EnqueueAt": "1788882874064", "ScheduledAt": "1788882574064"}	0
16238	5102	Processing	\N	2026-09-08 16:00:14.52251+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "d8caa9dc-5ae2-432a-978a-357a1be22e7e", "StartedAt": "1788883213601"}	0
16240	5103	Processing	\N	2026-09-08 16:00:17.778284+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "3483caa3-e204-437c-8e1e-f5559ef0e1a0", "StartedAt": "1788883216792"}	0
16242	5103	Succeeded	\N	2026-09-08 16:00:21.935508+00	{"Latency": "4206", "SucceededAt": "1788883220483", "PerformanceDuration": "1923"}	0
16254	5095	Processing	\N	2026-09-08 16:01:01.68616+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "d8caa9dc-5ae2-432a-978a-357a1be22e7e", "StartedAt": "1788883260777"}	0
16258	5096	Processing	\N	2026-09-08 16:01:03.825274+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "3483caa3-e204-437c-8e1e-f5559ef0e1a0", "StartedAt": "1788883262917"}	0
16266	5097	Processing	\N	2026-09-08 16:01:06.751+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "d8caa9dc-5ae2-432a-978a-357a1be22e7e", "StartedAt": "1788883265787"}	0
16268	5097	Succeeded	\N	2026-09-08 16:01:09.444039+00	{"Latency": "310248", "SucceededAt": "1788883267878", "PerformanceDuration": "355"}	0
16270	5099	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 16:04:23.093304+00	{"Queue": "default", "EnqueuedAt": "1788883461284"}	0
16271	5099	Processing	\N	2026-09-08 16:04:25.298612+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "3483caa3-e204-437c-8e1e-f5559ef0e1a0", "StartedAt": "1788883464464"}	0
16272	5099	Succeeded	\N	2026-09-08 16:04:28.356268+00	{"Latency": "304763", "SucceededAt": "1788883466305", "PerformanceDuration": "334"}	0
16273	5100	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 16:05:00.794437+00	{"Queue": "default", "EnqueuedAt": "1788883499000"}	0
16274	5100	Processing	\N	2026-09-08 16:05:02.990502+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "3483caa3-e204-437c-8e1e-f5559ef0e1a0", "StartedAt": "1788883502155"}	0
16275	5100	Succeeded	\N	2026-09-08 16:05:05.327163+00	{"Latency": "310244", "SucceededAt": "1788883503993", "PerformanceDuration": "335"}	0
16276	5113	Enqueued	Triggered by recurring job scheduler	2026-09-08 16:05:09.761449+00	{"Queue": "default", "EnqueuedAt": "1788883509761"}	0
16277	5113	Processing	\N	2026-09-08 16:05:13.070877+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "d8caa9dc-5ae2-432a-978a-357a1be22e7e", "StartedAt": "1788883512173"}	0
16278	5113	Succeeded	\N	2026-09-08 16:05:16.460212+00	{"Latency": "4799", "SucceededAt": "1788883514410", "PerformanceDuration": "621"}	0
16285	5105	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 16:06:00.029801+00	{"Queue": "default", "EnqueuedAt": "1788883558234"}	0
16286	5105	Processing	\N	2026-09-08 16:06:02.313957+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "d8caa9dc-5ae2-432a-978a-357a1be22e7e", "StartedAt": "1788883561416"}	0
16287	5106	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 16:06:04.643067+00	{"Queue": "default", "EnqueuedAt": "1788883562850"}	0
15797	4958	Enqueued	Triggered by recurring job scheduler	2026-09-08 10:15:08.124539+00	{"Queue": "default", "EnqueuedAt": "1788862508124"}	0
15818	4966	Processing	\N	2026-09-08 10:35:22.01263+00	{"ServerId": "nailify background server:3512:b809f0f4-ed72-4bdd-a858-9934eac117e1", "WorkerId": "149c9abf-1717-4605-81da-698b943e1f5a", "StartedAt": "1788863720209"}	0
16094	5058	Scheduled	\N	2026-09-08 15:16:26.057435+00	{"EnqueueAt": "1788880885545", "ScheduledAt": "1788880585545"}	0
16107	5071	Scheduled	\N	2026-09-08 15:17:02.356749+00	{"EnqueueAt": "1788880921932", "ScheduledAt": "1788880621932"}	0
16136	5062	Succeeded	\N	2026-09-08 15:21:51.611612+00	{"Latency": "321069", "SucceededAt": "1788880910479", "PerformanceDuration": "288"}	0
16138	5064	Processing	\N	2026-09-08 15:21:53.459267+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "5bb4a812-7c29-4b27-a144-6d5dc2bb42f5", "StartedAt": "1788880912753"}	0
16140	5064	Succeeded	\N	2026-09-08 15:21:55.436992+00	{"Latency": "323184", "SucceededAt": "1788880914308", "PerformanceDuration": "282"}	0
16145	5065	Succeeded	\N	2026-09-08 15:22:17.998287+00	{"Latency": "319095", "SucceededAt": "1788880936062", "PerformanceDuration": "426"}	0
16148	5067	Processing	\N	2026-09-08 15:22:21.14984+00	{"ServerId": "nailify background server:1:72e9e6cb-dd7c-4ec2-a0bc-32fa62e228f3", "WorkerId": "d8bb937d-15f2-45b4-8a25-4026051ce0e1", "StartedAt": "1788880939938"}	0
16151	5067	Succeeded	\N	2026-09-08 15:22:24.480922+00	{"Latency": "323706", "SucceededAt": "1788880942545", "PerformanceDuration": "424"}	0
16154	5069	Processing	\N	2026-09-08 15:22:27.629328+00	{"ServerId": "nailify background server:1:72e9e6cb-dd7c-4ec2-a0bc-32fa62e228f3", "WorkerId": "d8bb937d-15f2-45b4-8a25-4026051ce0e1", "StartedAt": "1788880946420"}	0
16158	5069	Succeeded	\N	2026-09-08 15:22:30.960584+00	{"Latency": "328480", "SucceededAt": "1788880949024", "PerformanceDuration": "426"}	0
16160	5071	Processing	\N	2026-09-08 15:22:34.203524+00	{"ServerId": "nailify background server:1:72e9e6cb-dd7c-4ec2-a0bc-32fa62e228f3", "WorkerId": "d8bb937d-15f2-45b4-8a25-4026051ce0e1", "StartedAt": "1788880952993"}	0
16171	5077	Enqueued	Triggered by recurring job scheduler	2026-09-08 15:35:10.369088+00	{"Queue": "default", "EnqueuedAt": "1788881710368"}	0
16190	5088	Scheduled	\N	2026-09-08 15:49:39.103131+00	{"EnqueueAt": "1788882878623", "ScheduledAt": "1788882578623"}	0
16195	5090	Processing	\N	2026-09-08 15:50:11.164012+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "5bb4a812-7c29-4b27-a144-6d5dc2bb42f5", "StartedAt": "1788882610340"}	0
16205	5084	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 15:54:41.533034+00	{"Queue": "default", "EnqueuedAt": "1788882880545"}	0
16209	5083	Succeeded	\N	2026-09-08 15:54:43.538474+00	{"Latency": "307924", "SucceededAt": "1788882882280", "PerformanceDuration": "291"}	0
16212	5086	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 15:54:45.958963+00	{"Queue": "default", "EnqueuedAt": "1788882884859"}	0
16213	5086	Processing	\N	2026-09-08 15:54:47.525751+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "54ad23b1-3311-415d-91c9-a1c465c0f4ed", "StartedAt": "1788882886819"}	0
16218	5088	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 15:54:49.732557+00	{"Queue": "default", "EnqueuedAt": "1788882888631"}	0
16220	5087	Succeeded	\N	2026-09-08 15:54:51.604626+00	{"Latency": "312423", "SucceededAt": "1788882890477", "PerformanceDuration": "290"}	0
16099	5063	Scheduled	\N	2026-09-08 15:16:30.406424+00	{"EnqueueAt": "1788880889980", "ScheduledAt": "1788880589980"}	0
16100	5064	Scheduled	\N	2026-09-08 15:16:31.352675+00	{"EnqueueAt": "1788880890842", "ScheduledAt": "1788880590842"}	0
16105	5069	Scheduled	\N	2026-09-08 15:17:00.544369+00	{"EnqueueAt": "1788880920118", "ScheduledAt": "1788880620118"}	0
16106	5070	Scheduled	\N	2026-09-08 15:17:01.588623+00	{"EnqueueAt": "1788880921078", "ScheduledAt": "1788880621078"}	0
16134	5062	Processing	\N	2026-09-08 15:21:49.612969+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "5bb4a812-7c29-4b27-a144-6d5dc2bb42f5", "StartedAt": "1788880908901"}	0
16162	5074	Enqueued	Triggered by recurring job scheduler	2026-09-08 15:25:01.33556+00	{"Queue": "default", "EnqueuedAt": "1788881101335"}	0
16202	5082	Processing	\N	2026-09-08 15:54:39.984927+00	{"ServerId": "nailify background server:1:72e9e6cb-dd7c-4ec2-a0bc-32fa62e228f3", "WorkerId": "4fe47fac-a840-4aed-934d-357db4c2910a", "StartedAt": "1788882879132"}	0
16206	5082	Succeeded	\N	2026-09-08 15:54:42.361076+00	{"Latency": "307467", "SucceededAt": "1788882881024", "PerformanceDuration": "343"}	0
16222	5092	Enqueued	Triggered by recurring job scheduler	2026-09-08 15:55:03.372688+00	{"Queue": "default", "EnqueuedAt": "1788882903372"}	0
16241	5102	Succeeded	\N	2026-09-08 16:00:21.827363+00	{"Latency": "4171", "SucceededAt": "1788883220483", "PerformanceDuration": "5228"}	0
16251	5095	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 16:00:59.662449+00	{"Queue": "default", "EnqueuedAt": "1788883258381"}	0
16255	5096	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 16:01:01.848137+00	{"Queue": "default", "EnqueuedAt": "1788883260573"}	0
16259	5097	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 16:01:04.073691+00	{"Queue": "default", "EnqueuedAt": "1788883262772"}	0
16264	5098	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 16:01:06.259126+00	{"Queue": "default", "EnqueuedAt": "1788883264984"}	0
16269	5098	Succeeded	\N	2026-09-08 16:01:10.252115+00	{"Latency": "310526", "SucceededAt": "1788883268989", "PerformanceDuration": "284"}	0
16279	5101	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 16:05:19.931008+00	{"Queue": "default", "EnqueuedAt": "1788883518938"}	0
16282	5101	Succeeded	\N	2026-09-08 16:05:23.594487+00	{"Latency": "312008", "SucceededAt": "1788883522387", "PerformanceDuration": "311"}	0
16283	5104	Processing	\N	2026-09-08 16:05:24.178466+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "5bb4a812-7c29-4b27-a144-6d5dc2bb42f5", "StartedAt": "1788883523469"}	0
16284	5104	Succeeded	\N	2026-09-08 16:05:26.170841+00	{"Latency": "302880", "SucceededAt": "1788883525037", "PerformanceDuration": "292"}	0
16300	5109	Succeeded	\N	2026-09-08 16:06:20.430225+00	{"Latency": "315491", "SucceededAt": "1788883578505", "PerformanceDuration": "435"}	0
16304	5110	Succeeded	\N	2026-09-08 16:06:23.606829+00	{"Latency": "317650", "SucceededAt": "1788883581682", "PerformanceDuration": "436"}	0
16307	5111	Succeeded	\N	2026-09-08 16:06:26.74625+00	{"Latency": "319929", "SucceededAt": "1788883584819", "PerformanceDuration": "435"}	0
16310	5114	Processing	\N	2026-09-08 16:10:04.465735+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "54ad23b1-3311-415d-91c9-a1c465c0f4ed", "StartedAt": "1788883803760"}	0
16311	5115	Enqueued	Triggered by recurring job scheduler	2026-09-08 16:10:04.82748+00	{"Queue": "default", "EnqueuedAt": "1788883804827"}	0
16312	5114	Succeeded	\N	2026-09-08 16:10:06.589181+00	{"Latency": "3210", "SucceededAt": "1788883805460", "PerformanceDuration": "425"}	0
16314	5115	Succeeded	\N	2026-09-08 16:10:10.076557+00	{"Latency": "4062", "SucceededAt": "1788883808875", "PerformanceDuration": "435"}	0
16316	5116	Processing	\N	2026-09-08 16:15:12.332128+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "3483caa3-e204-437c-8e1e-f5559ef0e1a0", "StartedAt": "1788884111175"}	0
16317	5116	Succeeded	\N	2026-09-08 16:15:15.802475+00	{"Latency": "5376", "SucceededAt": "1788884113966", "PerformanceDuration": "714"}	0
16320	5117	Processing	\N	2026-09-08 16:20:11.563375+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "3483caa3-e204-437c-8e1e-f5559ef0e1a0", "StartedAt": "1788884410420"}	0
16321	5117	Succeeded	\N	2026-09-08 16:20:15.001062+00	{"Latency": "4365", "SucceededAt": "1788884413171", "PerformanceDuration": "692"}	0
16324	5119	Enqueued	Triggered by recurring job scheduler	2026-09-08 16:25:09.219633+00	{"Queue": "default", "EnqueuedAt": "1788884709219"}	0
16325	5119	Processing	\N	2026-09-08 16:25:12.07054+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "d8caa9dc-5ae2-432a-978a-357a1be22e7e", "StartedAt": "1788884711157"}	0
16326	5119	Succeeded	\N	2026-09-08 16:25:14.871465+00	{"Latency": "4196", "SucceededAt": "1788884713408", "PerformanceDuration": "600"}	0
16328	5120	Processing	\N	2026-09-08 16:30:09.014491+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "d8caa9dc-5ae2-432a-978a-357a1be22e7e", "StartedAt": "1788885008041"}	0
16330	5120	Succeeded	\N	2026-09-08 16:30:11.942457+00	{"Latency": "4820", "SucceededAt": "1788885010385", "PerformanceDuration": "588"}	0
16331	5121	Processing	\N	2026-09-08 16:30:14.478456+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "d8caa9dc-5ae2-432a-978a-357a1be22e7e", "StartedAt": "1788885013504"}	0
16337	5123	Processing	\N	2026-09-08 16:40:06.595132+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "d8caa9dc-5ae2-432a-978a-357a1be22e7e", "StartedAt": "1788885605684"}	0
16338	5124	Enqueued	Triggered by recurring job scheduler	2026-09-08 16:40:07.078492+00	{"Queue": "default", "EnqueuedAt": "1788885607078"}	0
16339	5123	Succeeded	\N	2026-09-08 16:40:09.358774+00	{"Latency": "4164", "SucceededAt": "1788885607873", "PerformanceDuration": "548"}	0
16340	5124	Processing	\N	2026-09-08 16:40:11.879482+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "d8caa9dc-5ae2-432a-978a-357a1be22e7e", "StartedAt": "1788885610903"}	0
16341	5124	Succeeded	\N	2026-09-08 16:40:14.798498+00	{"Latency": "6171", "SucceededAt": "1788885613235", "PerformanceDuration": "570"}	0
16342	5125	Enqueued	Triggered by recurring job scheduler	2026-09-08 16:45:01.797167+00	{"Queue": "default", "EnqueuedAt": "1788885901796"}	0
16343	5125	Processing	\N	2026-09-08 16:45:04.627861+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "d8caa9dc-5ae2-432a-978a-357a1be22e7e", "StartedAt": "1788885903651"}	0
16344	5125	Succeeded	\N	2026-09-08 16:45:07.503129+00	{"Latency": "4172", "SucceededAt": "1788885905943", "PerformanceDuration": "531"}	0
15424	4834	Enqueued	Triggered by recurring job scheduler	2026-09-08 02:14:04.573324+00	{"Queue": "default", "EnqueuedAt": "1788833644573"}	0
15425	4834	Processing	\N	2026-09-08 02:14:07.022067+00	{"ServerId": "nailify background server:1:a0292a93-5fe1-4130-9485-2ea5702666b1", "WorkerId": "ad02d99d-dc93-48c5-9b47-789ecbfb2ff5", "StartedAt": "1788833646121"}	0
15428	4835	Processing	\N	2026-09-08 02:14:11.426216+00	{"ServerId": "nailify background server:1:a0292a93-5fe1-4130-9485-2ea5702666b1", "WorkerId": "d167b873-3f4b-4b1d-bfc9-a25631755035", "StartedAt": "1788833650672"}	0
15429	4834	Succeeded	\N	2026-09-08 02:14:14.229219+00	{"Latency": "3503", "SucceededAt": "1788833652982", "PerformanceDuration": "5336"}	0
15527	4868	Enqueued	Triggered by recurring job scheduler	2026-09-08 04:30:06.794744+00	{"Queue": "default", "EnqueuedAt": "1788841806794"}	0
15529	4869	Enqueued	Triggered by recurring job scheduler	2026-09-08 04:30:09.206885+00	{"Queue": "default", "EnqueuedAt": "1788841809206"}	0
15536	4871	Enqueued	Triggered by recurring job scheduler	2026-09-08 04:40:02.854504+00	{"Queue": "default", "EnqueuedAt": "1788842402854"}	0
15538	4872	Enqueued	Triggered by recurring job scheduler	2026-09-08 04:40:05.259396+00	{"Queue": "default", "EnqueuedAt": "1788842405259"}	0
15539	4871	Succeeded	\N	2026-09-08 04:40:07.139073+00	{"Latency": "3155", "SucceededAt": "1788842406005", "PerformanceDuration": "426"}	0
15617	4898	Enqueued	Triggered by recurring job scheduler	2026-09-08 06:10:12.980879+00	{"Queue": "default", "EnqueuedAt": "1788847812980"}	0
15659	4912	Enqueued	Triggered by recurring job scheduler	2026-09-08 07:46:03.596723+00	{"Queue": "default", "EnqueuedAt": "1788853563495"}	0
15660	4912	Processing	\N	2026-09-08 07:46:06.795218+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "a7f4d136-bee6-48c7-a7b1-7dcb7bf0ce5c", "StartedAt": "1788853565595"}	0
15671	4916	Enqueued	Triggered by recurring job scheduler	2026-09-08 07:55:09.381323+00	{"Queue": "default", "EnqueuedAt": "1788854109381"}	0
15672	4916	Processing	\N	2026-09-08 07:55:11.524533+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "a7f4d136-bee6-48c7-a7b1-7dcb7bf0ce5c", "StartedAt": "1788854110815"}	0
15673	4916	Succeeded	\N	2026-09-08 07:55:14.547122+00	{"Latency": "3154", "SucceededAt": "1788854112520", "PerformanceDuration": "428"}	0
15717	4931	Processing	\N	2026-09-08 08:45:11.645027+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788857110858"}	0
15718	4931	Succeeded	\N	2026-09-08 08:45:14.260511+00	{"Latency": "3612", "SucceededAt": "1788857112815", "PerformanceDuration": "538"}	0
15798	4958	Processing	\N	2026-09-08 10:15:12.517951+00	{"ServerId": "nailify background server:1:7cf87f97-3d37-4c17-aebb-7fe4739b0337", "WorkerId": "6d7d773f-565b-44eb-9fd5-1dee50ab5f52", "StartedAt": "1788862511289"}	0
15872	4984	Processing	\N	2026-09-08 11:35:16.167663+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "a7f4d136-bee6-48c7-a7b1-7dcb7bf0ce5c", "StartedAt": "1788867315403"}	0
15873	4984	Succeeded	\N	2026-09-08 11:35:18.462234+00	{"Latency": "3362", "SucceededAt": "1788867317258", "PerformanceDuration": "463"}	0
15876	4986	Enqueued	Triggered by recurring job scheduler	2026-09-08 11:40:05.689645+00	{"Queue": "default", "EnqueuedAt": "1788867605689"}	0
15934	5005	Enqueued	Triggered by recurring job scheduler	2026-09-08 12:45:11.473227+00	{"Queue": "default", "EnqueuedAt": "1788871511465"}	0
15963	5014	Succeeded	\N	2026-09-08 13:15:09.936746+00	{"Latency": "3855", "SucceededAt": "1788873308533", "PerformanceDuration": "533"}	0
15971	5017	Processing	\N	2026-09-08 13:25:12.094422+00	{"ServerId": "nailify background server:1:cc435256-039f-4352-ad49-91c860e852fd", "WorkerId": "911ea8c4-edf6-46a7-b556-d294ca960732", "StartedAt": "1788873911105"}	0
16101	5065	Scheduled	\N	2026-09-08 15:16:56.96622+00	{"EnqueueAt": "1788880916539", "ScheduledAt": "1788880616539"}	0
16102	5066	Scheduled	\N	2026-09-08 15:16:57.988126+00	{"EnqueueAt": "1788880917563", "ScheduledAt": "1788880617563"}	0
16163	5074	Processing	\N	2026-09-08 15:25:03.473406+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "5bb4a812-7c29-4b27-a144-6d5dc2bb42f5", "StartedAt": "1788881102763"}	0
16164	5074	Succeeded	\N	2026-09-08 15:25:05.631928+00	{"Latency": "3143", "SucceededAt": "1788881104499", "PerformanceDuration": "450"}	0
16203	5081	Succeeded	\N	2026-09-08 15:54:41.242283+00	{"Latency": "307801", "SucceededAt": "1788882879980", "PerformanceDuration": "282"}	0
16207	5085	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 15:54:43.227212+00	{"Queue": "default", "EnqueuedAt": "1788882882241"}	0
16211	5085	Processing	\N	2026-09-08 15:54:45.454861+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "5bb4a812-7c29-4b27-a144-6d5dc2bb42f5", "StartedAt": "1788882884749"}	0
16214	5085	Succeeded	\N	2026-09-08 15:54:47.56516+00	{"Latency": "310121", "SucceededAt": "1788882886310", "PerformanceDuration": "291"}	0
16216	5087	Processing	\N	2026-09-08 15:54:49.558651+00	{"ServerId": "nailify background server:1:946a7661-99f0-451a-b96b-3a6249661bba", "WorkerId": "5bb4a812-7c29-4b27-a144-6d5dc2bb42f5", "StartedAt": "1788882888773"}	0
16226	5094	Scheduled	\N	2026-09-08 15:55:29.55048+00	{"EnqueueAt": "1788883229072", "ScheduledAt": "1788882929072"}	0
16230	5098	Scheduled	\N	2026-09-08 15:55:58.602151+00	{"EnqueueAt": "1788883258179", "ScheduledAt": "1788882958179"}	0
16233	5091	Succeeded	\N	2026-09-08 15:56:31.858939+00	{"Latency": "306164", "SucceededAt": "1788882990648", "PerformanceDuration": "282"}	0
16243	5104	Scheduled	\N	2026-09-08 16:00:22.289375+00	{"EnqueueAt": "1788883521864", "ScheduledAt": "1788883221864"}	0
16244	5093	Enqueued	Triggered by DelayedJobScheduler	2026-09-08 16:00:28.730985+00	{"Queue": "default", "EnqueuedAt": "1788883227742"}	0
15431	4836	Enqueued	Triggered by recurring job scheduler	2026-09-08 02:15:14.887066+00	{"Queue": "default", "EnqueuedAt": "1788833714886"}	0
15530	4868	Succeeded	\N	2026-09-08 04:30:11.091587+00	{"Latency": "3144", "SucceededAt": "1788841809964", "PerformanceDuration": "456"}	0
15533	4870	Enqueued	Triggered by recurring job scheduler	2026-09-08 04:35:14.131357+00	{"Queue": "default", "EnqueuedAt": "1788842114131"}	0
15534	4870	Processing	\N	2026-09-08 04:35:16.267853+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "f953e7fe-c9a8-4ac8-834a-0fcf3d7e9142", "StartedAt": "1788842115557"}	0
15535	4870	Succeeded	\N	2026-09-08 04:35:18.401804+00	{"Latency": "3136", "SucceededAt": "1788842117268", "PerformanceDuration": "432"}	0
15618	4898	Processing	\N	2026-09-08 06:10:15.320666+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "f953e7fe-c9a8-4ac8-834a-0fcf3d7e9142", "StartedAt": "1788847814523"}	0
15619	4899	Enqueued	Triggered by recurring job scheduler	2026-09-08 06:10:15.657138+00	{"Queue": "default", "EnqueuedAt": "1788847815657"}	0
15622	4899	Succeeded	\N	2026-09-08 06:10:20.538847+00	{"Latency": "3644", "SucceededAt": "1788847819282", "PerformanceDuration": "452"}	0
15623	4900	Enqueued	Triggered by recurring job scheduler	2026-09-08 06:15:05.695643+00	{"Queue": "default", "EnqueuedAt": "1788848105695"}	0
15624	4900	Processing	\N	2026-09-08 06:15:07.817111+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "f953e7fe-c9a8-4ac8-834a-0fcf3d7e9142", "StartedAt": "1788848107111"}	0
15625	4900	Succeeded	\N	2026-09-08 06:15:09.960711+00	{"Latency": "3117", "SucceededAt": "1788848108828", "PerformanceDuration": "444"}	0
15626	4901	Enqueued	Triggered by recurring job scheduler	2026-09-08 06:20:11.147881+00	{"Queue": "default", "EnqueuedAt": "1788848411147"}	0
15628	4902	Enqueued	Triggered by recurring job scheduler	2026-09-08 06:20:13.560714+00	{"Queue": "default", "EnqueuedAt": "1788848413560"}	0
15630	4902	Processing	\N	2026-09-08 06:20:15.729931+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788848414978"}	0
15631	4902	Succeeded	\N	2026-09-08 06:20:17.905758+00	{"Latency": "3196", "SucceededAt": "1788848416777", "PerformanceDuration": "446"}	0
15636	4904	Processing	\N	2026-09-08 06:30:11.131065+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788849010418"}	0
15637	4905	Enqueued	Triggered by recurring job scheduler	2026-09-08 06:30:11.554307+00	{"Queue": "default", "EnqueuedAt": "1788849011554"}	0
15661	4913	Enqueued	Triggered by recurring job scheduler	2026-09-08 07:46:06.895853+00	{"Queue": "default", "EnqueuedAt": "1788853566895"}	0
15662	4913	Processing	\N	2026-09-08 07:46:09.697023+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788853568797"}	0
15664	4912	Succeeded	\N	2026-09-08 07:46:14.596923+00	{"Latency": "4962", "SucceededAt": "1788853572898", "PerformanceDuration": "5341"}	0
15799	4958	Succeeded	\N	2026-09-08 10:15:16.458692+00	{"Latency": "6437", "SucceededAt": "1788862514497", "PerformanceDuration": "979"}	0
15809	4963	Processing	\N	2026-09-08 10:25:04.732956+00	{"ServerId": "nailify background server:3512:b809f0f4-ed72-4bdd-a858-9934eac117e1", "WorkerId": "a202a7d6-6289-435e-ab34-43b4c6541743", "StartedAt": "1788863102837"}	0
15812	4964	Processing	\N	2026-09-08 10:30:16.116412+00	{"ServerId": "nailify background server:3512:b809f0f4-ed72-4bdd-a858-9934eac117e1", "WorkerId": "149c9abf-1717-4605-81da-698b943e1f5a", "StartedAt": "1788863414278"}	0
15814	4964	Succeeded	\N	2026-09-08 10:30:21.324853+00	{"Latency": "-25192110", "SucceededAt": "1788863418567", "PerformanceDuration": "1081"}	0
15817	4966	Enqueued	Triggered by recurring job scheduler	2026-09-08 10:35:16.579039+00	{"Queue": "default", "EnqueuedAt": "1788863716578"}	0
15826	4969	Enqueued	Triggered by recurring job scheduler	2026-09-08 10:45:14.383395+00	{"Queue": "default", "EnqueuedAt": "1788864314382"}	0
15827	4969	Processing	\N	2026-09-08 10:45:22.281726+00	{"ServerId": "nailify background server:3512:b809f0f4-ed72-4bdd-a858-9934eac117e1", "WorkerId": "a202a7d6-6289-435e-ab34-43b4c6541743", "StartedAt": "1788864320483"}	0
15838	4973	Enqueued	Triggered by recurring job scheduler	2026-09-08 11:00:04.515762+00	{"Queue": "default", "EnqueuedAt": "1788865204515"}	0
15840	4974	Enqueued	Triggered by recurring job scheduler	2026-09-08 11:00:12.370793+00	{"Queue": "default", "EnqueuedAt": "1788865212370"}	0
15846	4975	Succeeded	\N	2026-09-08 11:05:15.322269+00	{"Latency": "-25191621", "SucceededAt": "1788865512277", "PerformanceDuration": "1154"}	0
15878	4986	Processing	\N	2026-09-08 11:40:07.812761+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788867607107"}	0
15879	4986	Succeeded	\N	2026-09-08 11:40:09.980578+00	{"Latency": "3120", "SucceededAt": "1788867608849", "PerformanceDuration": "467"}	0
15880	4987	Enqueued	Triggered by recurring job scheduler	2026-09-08 11:45:10.132338+00	{"Queue": "default", "EnqueuedAt": "1788867910132"}	0
15881	4987	Processing	\N	2026-09-08 11:45:12.252475+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "a7f4d136-bee6-48c7-a7b1-7dcb7bf0ce5c", "StartedAt": "1788867911546"}	0
15893	4991	Processing	\N	2026-09-08 12:00:04.435549+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "a7f4d136-bee6-48c7-a7b1-7dcb7bf0ce5c", "StartedAt": "1788868803729"}	0
15894	4992	Enqueued	Triggered by recurring job scheduler	2026-09-08 12:00:04.727115+00	{"Queue": "default", "EnqueuedAt": "1788868804727"}	0
15895	4991	Succeeded	\N	2026-09-08 12:00:06.566039+00	{"Latency": "3165", "SucceededAt": "1788868805436", "PerformanceDuration": "429"}	0
15432	4836	Processing	\N	2026-09-08 02:15:17.15908+00	{"ServerId": "nailify background server:1:a0292a93-5fe1-4130-9485-2ea5702666b1", "WorkerId": "d167b873-3f4b-4b1d-bfc9-a25631755035", "StartedAt": "1788833716380"}	0
15433	4836	Succeeded	\N	2026-09-08 02:15:19.428005+00	{"Latency": "3359", "SucceededAt": "1788833718225", "PerformanceDuration": "442"}	0
15434	4837	Enqueued	Triggered by recurring job scheduler	2026-09-08 02:20:04.92457+00	{"Queue": "default", "EnqueuedAt": "1788834004924"}	0
15436	4838	Enqueued	Triggered by recurring job scheduler	2026-09-08 02:20:07.645533+00	{"Queue": "default", "EnqueuedAt": "1788834007645"}	0
15439	4838	Succeeded	\N	2026-09-08 02:20:12.28934+00	{"Latency": "3438", "SucceededAt": "1788834011048", "PerformanceDuration": "444"}	0
15440	4839	Enqueued	Triggered by recurring job scheduler	2026-09-08 02:25:12.908229+00	{"Queue": "default", "EnqueuedAt": "1788834312907"}	0
15441	4839	Processing	\N	2026-09-08 02:25:15.033133+00	{"ServerId": "nailify background server:1:a0292a93-5fe1-4130-9485-2ea5702666b1", "WorkerId": "ad02d99d-dc93-48c5-9b47-789ecbfb2ff5", "StartedAt": "1788834314326"}	0
15442	4839	Succeeded	\N	2026-09-08 02:25:17.157607+00	{"Latency": "3121", "SucceededAt": "1788834316025", "PerformanceDuration": "426"}	0
15537	4871	Processing	\N	2026-09-08 04:40:05.011814+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "f953e7fe-c9a8-4ac8-834a-0fcf3d7e9142", "StartedAt": "1788842404305"}	0
15540	4872	Processing	\N	2026-09-08 04:40:07.383758+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "bb64c652-45af-4bc7-aa0e-d0d639f91d25", "StartedAt": "1788842406676"}	0
15541	4872	Succeeded	\N	2026-09-08 04:40:09.507492+00	{"Latency": "3116", "SucceededAt": "1788842408378", "PerformanceDuration": "427"}	0
15627	4901	Processing	\N	2026-09-08 06:20:13.364299+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "f953e7fe-c9a8-4ac8-834a-0fcf3d7e9142", "StartedAt": "1788848412567"}	0
15629	4901	Succeeded	\N	2026-09-08 06:20:15.59242+00	{"Latency": "3284", "SucceededAt": "1788848414459", "PerformanceDuration": "455"}	0
15632	4903	Enqueued	Triggered by recurring job scheduler	2026-09-08 06:25:03.869711+00	{"Queue": "default", "EnqueuedAt": "1788848703869"}	0
15633	4903	Processing	\N	2026-09-08 06:25:06.011545+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "f953e7fe-c9a8-4ac8-834a-0fcf3d7e9142", "StartedAt": "1788848705304"}	0
15634	4903	Succeeded	\N	2026-09-08 06:25:08.211822+00	{"Latency": "3142", "SucceededAt": "1788848707012", "PerformanceDuration": "434"}	0
15638	4904	Succeeded	\N	2026-09-08 06:30:13.292045+00	{"Latency": "3305", "SucceededAt": "1788849012163", "PerformanceDuration": "463"}	0
15639	4905	Processing	\N	2026-09-08 06:30:13.810163+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "f953e7fe-c9a8-4ac8-834a-0fcf3d7e9142", "StartedAt": "1788849013099"}	0
15640	4905	Succeeded	\N	2026-09-08 06:30:16.096259+00	{"Latency": "3299", "SucceededAt": "1788849014833", "PerformanceDuration": "453"}	0
15663	4913	Succeeded	\N	2026-09-08 07:46:14.395136+00	{"Latency": "4297", "SucceededAt": "1788853572799", "PerformanceDuration": "2304"}	0
15666	4914	Processing	\N	2026-09-08 07:50:14.970365+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788853814254"}	0
15668	4914	Succeeded	\N	2026-09-08 07:50:17.137705+00	{"Latency": "4461", "SucceededAt": "1788853816006", "PerformanceDuration": "462"}	0
15676	4918	Enqueued	Triggered by recurring job scheduler	2026-09-08 08:00:16.796785+00	{"Queue": "default", "EnqueuedAt": "1788854416796"}	0
15679	4918	Succeeded	\N	2026-09-08 08:00:21.15584+00	{"Latency": "3214", "SucceededAt": "1788854420014", "PerformanceDuration": "440"}	0
15680	4919	Enqueued	Triggered by recurring job scheduler	2026-09-08 08:05:06.053957+00	{"Queue": "default", "EnqueuedAt": "1788854706053"}	0
15681	4919	Processing	\N	2026-09-08 08:05:08.290187+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788854707585"}	0
15684	4920	Processing	\N	2026-09-08 08:10:14.575049+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788855013869"}	0
15686	4920	Succeeded	\N	2026-09-08 08:10:16.703622+00	{"Latency": "3310", "SucceededAt": "1788855015572", "PerformanceDuration": "431"}	0
15687	4921	Processing	\N	2026-09-08 08:10:17.287393+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "a7f4d136-bee6-48c7-a7b1-7dcb7bf0ce5c", "StartedAt": "1788855016579"}	0
15695	4923	Succeeded	\N	2026-09-08 08:20:14.284052+00	{"Latency": "3223", "SucceededAt": "1788855613149", "PerformanceDuration": "459"}	0
15696	4924	Processing	\N	2026-09-08 08:20:14.724086+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788855613973"}	0
15700	4925	Succeeded	\N	2026-09-08 08:25:08.457014+00	{"Latency": "3205", "SucceededAt": "1788855907325", "PerformanceDuration": "571"}	0
15705	4927	Processing	\N	2026-09-08 08:30:13.98818+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788856213236"}	0
15707	4928	Enqueued	Triggered by recurring job scheduler	2026-09-08 08:35:05.584697+00	{"Queue": "default", "EnqueuedAt": "1788856505584"}	0
15712	4930	Enqueued	Triggered by recurring job scheduler	2026-09-08 08:40:16.763543+00	{"Queue": "default", "EnqueuedAt": "1788856816763"}	0
15714	4930	Processing	\N	2026-09-08 08:40:19.733022+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "a7f4d136-bee6-48c7-a7b1-7dcb7bf0ce5c", "StartedAt": "1788856818444"}	0
15715	4930	Succeeded	\N	2026-09-08 08:40:22.372624+00	{"Latency": "4477", "SucceededAt": "1788856821241", "PerformanceDuration": "426"}	0
15716	4931	Enqueued	Triggered by recurring job scheduler	2026-09-08 08:45:09.184012+00	{"Queue": "default", "EnqueuedAt": "1788857109183"}	0
15435	4837	Processing	\N	2026-09-08 02:20:07.194201+00	{"ServerId": "nailify background server:1:a0292a93-5fe1-4130-9485-2ea5702666b1", "WorkerId": "ad02d99d-dc93-48c5-9b47-789ecbfb2ff5", "StartedAt": "1788834006487"}	0
15437	4837	Succeeded	\N	2026-09-08 02:20:09.346884+00	{"Latency": "3319", "SucceededAt": "1788834008186", "PerformanceDuration": "425"}	0
15438	4838	Processing	\N	2026-09-08 02:20:09.982247+00	{"ServerId": "nailify background server:1:a0292a93-5fe1-4130-9485-2ea5702666b1", "WorkerId": "d167b873-3f4b-4b1d-bfc9-a25631755035", "StartedAt": "1788834009207"}	0
15542	4873	Enqueued	Triggered by recurring job scheduler	2026-09-08 04:45:12.254694+00	{"Queue": "default", "EnqueuedAt": "1788842712254"}	0
15543	4873	Processing	\N	2026-09-08 04:45:14.453223+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "f953e7fe-c9a8-4ac8-834a-0fcf3d7e9142", "StartedAt": "1788842713701"}	0
15544	4873	Succeeded	\N	2026-09-08 04:45:16.725907+00	{"Latency": "3252", "SucceededAt": "1788842715522", "PerformanceDuration": "447"}	0
15545	4874	Enqueued	Triggered by recurring job scheduler	2026-09-08 04:50:02.649381+00	{"Queue": "default", "EnqueuedAt": "1788843002649"}	0
15546	4874	Processing	\N	2026-09-08 04:50:04.868584+00	{"ServerId": "nailify background server:1:a5d46ce2-9056-437d-ae88-e9cc9b4fdf71", "WorkerId": "f953e7fe-c9a8-4ac8-834a-0fcf3d7e9142", "StartedAt": "1788843004070"}	0
15635	4904	Enqueued	Triggered by recurring job scheduler	2026-09-08 06:30:08.870786+00	{"Queue": "default", "EnqueuedAt": "1788849008870"}	0
15665	4914	Enqueued	Triggered by recurring job scheduler	2026-09-08 07:50:11.868774+00	{"Queue": "default", "EnqueuedAt": "1788853811868"}	0
15667	4915	Enqueued	Triggered by recurring job scheduler	2026-09-08 07:50:16.310629+00	{"Queue": "default", "EnqueuedAt": "1788853816310"}	0
15669	4915	Processing	\N	2026-09-08 07:50:19.334003+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "a7f4d136-bee6-48c7-a7b1-7dcb7bf0ce5c", "StartedAt": "1788853818624"}	0
15670	4915	Succeeded	\N	2026-09-08 07:50:21.476288+00	{"Latency": "4362", "SucceededAt": "1788853820346", "PerformanceDuration": "443"}	0
15674	4917	Enqueued	Triggered by recurring job scheduler	2026-09-08 08:00:14.36504+00	{"Queue": "default", "EnqueuedAt": "1788854414364"}	0
15675	4917	Processing	\N	2026-09-08 08:00:16.541932+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "a7f4d136-bee6-48c7-a7b1-7dcb7bf0ce5c", "StartedAt": "1788854415792"}	0
15677	4918	Processing	\N	2026-09-08 08:00:18.972924+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788854418222"}	0
15678	4917	Succeeded	\N	2026-09-08 08:00:20.177207+00	{"Latency": "3212", "SucceededAt": "1788854417599", "PerformanceDuration": "455"}	0
15682	4919	Succeeded	\N	2026-09-08 08:05:10.527829+00	{"Latency": "3272", "SucceededAt": "1788854709282", "PerformanceDuration": "427"}	0
15683	4920	Enqueued	Triggered by recurring job scheduler	2026-09-08 08:10:12.310194+00	{"Queue": "default", "EnqueuedAt": "1788855012310"}	0
15685	4921	Enqueued	Triggered by recurring job scheduler	2026-09-08 08:10:15.020369+00	{"Queue": "default", "EnqueuedAt": "1788855015019"}	0
15688	4921	Succeeded	\N	2026-09-08 08:10:19.576097+00	{"Latency": "3313", "SucceededAt": "1788855018298", "PerformanceDuration": "443"}	0
15689	4922	Enqueued	Triggered by recurring job scheduler	2026-09-08 08:15:04.296539+00	{"Queue": "default", "EnqueuedAt": "1788855304296"}	0
15690	4922	Processing	\N	2026-09-08 08:15:06.560018+00	{"ServerId": "nailify background server:1:7e640b77-6b6f-46ed-bc67-6dd6ccf3a3cb", "WorkerId": "8f62579f-86f5-41f9-94f1-728e80b5a90d", "StartedAt": "1788855305855"}	0
15691	4922	Succeeded	\N	2026-09-08 08:15:08.792593+00	{"Latency": "3310", "SucceededAt": "1788855307551", "PerformanceDuration": "426"}	0
15692	4923	Enqueued	Triggered by recurring job scheduler	2026-09-08 08:20:09.920551+00	{"Queue": "default", "EnqueuedAt": "1788855609920"}	0
15800	4959	Scheduled	\N	2026-09-08 10:19:49.812464+00	{"EnqueueAt": "1788916500001", "ScheduledAt": "1788862788477"}	0
15806	4961	Succeeded	\N	2026-09-08 10:20:14.977705+00	{"Latency": "-25194682", "SucceededAt": "1788862811760", "PerformanceDuration": "1641"}	0
15883	4988	Enqueued	Triggered by recurring job scheduler	2026-09-08 11:50:14.686975+00	{"Queue": "default", "EnqueuedAt": "1788868214485"}	0
15884	4989	Enqueued	Triggered by recurring job scheduler	2026-09-08 11:50:17.901572+00	{"Queue": "default", "EnqueuedAt": "1788868217900"}	0
16345	5126	Enqueued	Triggered by recurring job scheduler	2026-09-08 16:50:13.308335+00	{"Queue": "default", "EnqueuedAt": "1788886213308"}	0
16347	5127	Enqueued	Triggered by recurring job scheduler	2026-09-08 16:50:17.208349+00	{"Queue": "default", "EnqueuedAt": "1788886217208"}	0
16349	5127	Processing	\N	2026-09-08 16:50:22.121557+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "3483caa3-e204-437c-8e1e-f5559ef0e1a0", "StartedAt": "1788886220976"}	0
16358	5130	Processing	\N	2026-09-08 17:00:24.172381+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "d8caa9dc-5ae2-432a-978a-357a1be22e7e", "StartedAt": "1788886822908"}	0
16346	5126	Processing	\N	2026-09-08 16:50:16.748452+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "d8caa9dc-5ae2-432a-978a-357a1be22e7e", "StartedAt": "1788886215604"}	0
16348	5126	Succeeded	\N	2026-09-08 16:50:20.192941+00	{"Latency": "5048", "SucceededAt": "1788886218364", "PerformanceDuration": "698"}	0
16350	5127	Succeeded	\N	2026-09-08 16:50:25.668242+00	{"Latency": "6518", "SucceededAt": "1788886223744", "PerformanceDuration": "704"}	0
16351	5128	Enqueued	Triggered by recurring job scheduler	2026-09-08 16:55:17.016678+00	{"Queue": "default", "EnqueuedAt": "1788886517016"}	0
16352	5128	Processing	\N	2026-09-08 16:55:20.352491+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "3483caa3-e204-437c-8e1e-f5559ef0e1a0", "StartedAt": "1788886519072"}	0
16353	5128	Succeeded	\N	2026-09-08 16:55:24.06651+00	{"Latency": "4948", "SucceededAt": "1788886522020", "PerformanceDuration": "641"}	0
16354	5129	Enqueued	Triggered by recurring job scheduler	2026-09-08 17:00:17.570154+00	{"Queue": "default", "EnqueuedAt": "1788886817570"}	0
16355	5129	Processing	\N	2026-09-08 17:00:20.470773+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "3483caa3-e204-437c-8e1e-f5559ef0e1a0", "StartedAt": "1788886819633"}	0
16356	5130	Enqueued	Triggered by recurring job scheduler	2026-09-08 17:00:20.859375+00	{"Queue": "default", "EnqueuedAt": "1788886820859"}	0
16357	5129	Succeeded	\N	2026-09-08 17:00:23.154209+00	{"Latency": "4151", "SucceededAt": "1788886821815", "PerformanceDuration": "673"}	0
16359	5130	Succeeded	\N	2026-09-08 17:00:27.34106+00	{"Latency": "4905", "SucceededAt": "1788886825800", "PerformanceDuration": "614"}	0
16360	5131	Enqueued	Triggered by recurring job scheduler	2026-09-08 17:05:03.265789+00	{"Queue": "default", "EnqueuedAt": "1788887103265"}	0
16361	5131	Processing	\N	2026-09-08 17:05:07.109787+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "d8caa9dc-5ae2-432a-978a-357a1be22e7e", "StartedAt": "1788887105844"}	0
16362	5131	Succeeded	\N	2026-09-08 17:05:11.040736+00	{"Latency": "5629", "SucceededAt": "1788887108892", "PerformanceDuration": "767"}	0
16363	5132	Enqueued	Triggered by recurring job scheduler	2026-09-08 17:10:05.069662+00	{"Queue": "default", "EnqueuedAt": "1788887405069"}	0
16364	5132	Processing	\N	2026-09-08 17:10:07.662202+00	{"ServerId": "nailify background server:1:297ea4f9-65cc-43b6-95e2-2d650b912255", "WorkerId": "3dba18f2-83be-4a8d-9508-6656748b13bb", "StartedAt": "1788887406789"}	0
16365	5133	Enqueued	Triggered by recurring job scheduler	2026-09-08 17:10:07.913448+00	{"Queue": "default", "EnqueuedAt": "1788887407913"}	0
16366	5133	Processing	\N	2026-09-08 17:10:11.04452+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "3483caa3-e204-437c-8e1e-f5559ef0e1a0", "StartedAt": "1788887409764"}	0
16367	5132	Succeeded	\N	2026-09-08 17:10:11.695696+00	{"Latency": "3675", "SucceededAt": "1788887410556", "PerformanceDuration": "2317"}	0
16368	5133	Succeeded	\N	2026-09-08 17:10:14.802979+00	{"Latency": "4661", "SucceededAt": "1788887412752", "PerformanceDuration": "681"}	0
16369	5134	Enqueued	Triggered by recurring job scheduler	2026-09-08 17:15:05.646284+00	{"Queue": "default", "EnqueuedAt": "1788887705646"}	0
16370	5134	Processing	\N	2026-09-08 17:15:09.170279+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "3483caa3-e204-437c-8e1e-f5559ef0e1a0", "StartedAt": "1788887707969"}	0
16371	5134	Succeeded	\N	2026-09-08 17:15:12.757834+00	{"Latency": "5183", "SucceededAt": "1788887710835", "PerformanceDuration": "704"}	0
16372	5135	Enqueued	Triggered by recurring job scheduler	2026-09-08 17:20:04.727054+00	{"Queue": "default", "EnqueuedAt": "1788888004726"}	0
16373	5135	Processing	\N	2026-09-08 17:20:07.441781+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "3483caa3-e204-437c-8e1e-f5559ef0e1a0", "StartedAt": "1788888006543"}	0
16374	5136	Enqueued	Triggered by recurring job scheduler	2026-09-08 17:20:07.825617+00	{"Queue": "default", "EnqueuedAt": "1788888007825"}	0
16375	5135	Succeeded	\N	2026-09-08 17:20:10.140867+00	{"Latency": "3985", "SucceededAt": "1788888008703", "PerformanceDuration": "541"}	0
16376	5136	Processing	\N	2026-09-08 17:20:10.814914+00	{"ServerId": "nailify background server:1:297ea4f9-65cc-43b6-95e2-2d650b912255", "WorkerId": "3dba18f2-83be-4a8d-9508-6656748b13bb", "StartedAt": "1788888010107"}	0
16377	5136	Succeeded	\N	2026-09-08 17:20:15.250924+00	{"Latency": "4104", "SucceededAt": "1788888014120", "PerformanceDuration": "2738"}	0
16378	5137	Enqueued	Triggered by recurring job scheduler	2026-09-08 17:25:06.939354+00	{"Queue": "default", "EnqueuedAt": "1788888306939"}	0
16379	5137	Processing	\N	2026-09-08 17:25:09.666783+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "d8caa9dc-5ae2-432a-978a-357a1be22e7e", "StartedAt": "1788888308762"}	0
16380	5137	Succeeded	\N	2026-09-08 17:25:12.963279+00	{"Latency": "4021", "SucceededAt": "1788888310998", "PerformanceDuration": "608"}	0
16381	5138	Enqueued	Triggered by recurring job scheduler	2026-09-08 17:30:03.528051+00	{"Queue": "default", "EnqueuedAt": "1788888603527"}	0
16382	5138	Processing	\N	2026-09-08 17:30:07.2529+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "d8caa9dc-5ae2-432a-978a-357a1be22e7e", "StartedAt": "1788888605981"}	0
16383	5139	Enqueued	Triggered by recurring job scheduler	2026-09-08 17:30:07.663409+00	{"Queue": "default", "EnqueuedAt": "1788888607663"}	0
16384	5138	Succeeded	\N	2026-09-08 17:30:11.072932+00	{"Latency": "5474", "SucceededAt": "1788888609036", "PerformanceDuration": "765"}	0
16385	5139	Processing	\N	2026-09-08 17:30:11.947617+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "3483caa3-e204-437c-8e1e-f5559ef0e1a0", "StartedAt": "1788888611051"}	0
16386	5139	Succeeded	\N	2026-09-08 17:30:14.767131+00	{"Latency": "5732", "SucceededAt": "1788888613331", "PerformanceDuration": "666"}	0
16387	5140	Enqueued	Triggered by recurring job scheduler	2026-09-08 17:35:06.091694+00	{"Queue": "default", "EnqueuedAt": "1788888906091"}	0
16388	5140	Processing	\N	2026-09-08 17:35:08.731609+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "d8caa9dc-5ae2-432a-978a-357a1be22e7e", "StartedAt": "1788888907899"}	0
16389	5140	Succeeded	\N	2026-09-08 17:35:11.857148+00	{"Latency": "3812", "SucceededAt": "1788888909961", "PerformanceDuration": "563"}	0
16390	5141	Enqueued	Triggered by recurring job scheduler	2026-09-08 17:40:02.76127+00	{"Queue": "default", "EnqueuedAt": "1788889202761"}	0
16392	5142	Enqueued	Triggered by recurring job scheduler	2026-09-08 17:40:06.661498+00	{"Queue": "default", "EnqueuedAt": "1788889206661"}	0
16391	5141	Processing	\N	2026-09-08 17:40:05.772222+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "d8caa9dc-5ae2-432a-978a-357a1be22e7e", "StartedAt": "1788889204933"}	0
16393	5141	Succeeded	\N	2026-09-08 17:40:08.324802+00	{"Latency": "4374", "SucceededAt": "1788889206981", "PerformanceDuration": "535"}	0
16395	5142	Succeeded	\N	2026-09-08 17:40:13.121479+00	{"Latency": "5169", "SucceededAt": "1788889211781", "PerformanceDuration": "639"}	0
16394	5142	Processing	\N	2026-09-08 17:40:10.181189+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "3483caa3-e204-437c-8e1e-f5559ef0e1a0", "StartedAt": "1788889208980"}	0
16396	5143	Enqueued	Triggered by recurring job scheduler	2026-09-08 17:45:06.015197+00	{"Queue": "default", "EnqueuedAt": "1788889506015"}	0
16402	5145	Processing	\N	2026-09-08 17:50:16.638474+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "d8caa9dc-5ae2-432a-978a-357a1be22e7e", "StartedAt": "1788889815675"}	0
16404	5145	Succeeded	\N	2026-09-08 17:50:19.639403+00	{"Latency": "4817", "SucceededAt": "1788889818097", "PerformanceDuration": "686"}	0
16409	5147	Processing	\N	2026-09-08 18:00:14.576451+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "d8caa9dc-5ae2-432a-978a-357a1be22e7e", "StartedAt": "1788890413741"}	0
16411	5147	Succeeded	\N	2026-09-08 18:00:17.119224+00	{"Latency": "3887", "SucceededAt": "1788890415783", "PerformanceDuration": "533"}	0
16414	5149	Enqueued	Triggered by recurring job scheduler	2026-09-08 18:05:10.805866+00	{"Queue": "default", "EnqueuedAt": "1788890710805"}	0
16422	5151	Succeeded	\N	2026-09-08 18:11:07.147874+00	{"Latency": "5453", "SucceededAt": "1788891065310", "PerformanceDuration": "3010"}	0
16397	5143	Processing	\N	2026-09-08 17:45:09.828189+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "d8caa9dc-5ae2-432a-978a-357a1be22e7e", "StartedAt": "1788889508553"}	0
16398	5143	Succeeded	\N	2026-09-08 17:45:13.655674+00	{"Latency": "5598", "SucceededAt": "1788889511616", "PerformanceDuration": "766"}	0
16399	5144	Enqueued	Triggered by recurring job scheduler	2026-09-08 17:50:09.157512+00	{"Queue": "default", "EnqueuedAt": "1788889809157"}	0
16400	5144	Processing	\N	2026-09-08 17:50:12.784328+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "3483caa3-e204-437c-8e1e-f5559ef0e1a0", "StartedAt": "1788889811509"}	0
16401	5145	Enqueued	Triggered by recurring job scheduler	2026-09-08 17:50:13.328535+00	{"Queue": "default", "EnqueuedAt": "1788889813328"}	0
16403	5144	Succeeded	\N	2026-09-08 17:50:16.609624+00	{"Latency": "5399", "SucceededAt": "1788889814571", "PerformanceDuration": "767"}	0
16405	5146	Enqueued	Triggered by recurring job scheduler	2026-09-08 17:55:13.661879+00	{"Queue": "default", "EnqueuedAt": "1788890113661"}	0
16406	5146	Processing	\N	2026-09-08 17:55:17.034753+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "d8caa9dc-5ae2-432a-978a-357a1be22e7e", "StartedAt": "1788890116196"}	0
16407	5146	Succeeded	\N	2026-09-08 17:55:20.409353+00	{"Latency": "4819", "SucceededAt": "1788890118370", "PerformanceDuration": "665"}	0
16408	5147	Enqueued	Triggered by recurring job scheduler	2026-09-08 18:00:11.912336+00	{"Queue": "default", "EnqueuedAt": "1788890411912"}	0
16410	5148	Enqueued	Triggered by recurring job scheduler	2026-09-08 18:00:15.024791+00	{"Queue": "default", "EnqueuedAt": "1788890415024"}	0
16412	5148	Processing	\N	2026-09-08 18:00:17.769477+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "3483caa3-e204-437c-8e1e-f5559ef0e1a0", "StartedAt": "1788890416854"}	0
16413	5148	Succeeded	\N	2026-09-08 18:00:20.479264+00	{"Latency": "4026", "SucceededAt": "1788890419017", "PerformanceDuration": "517"}	0
16415	5149	Processing	\N	2026-09-08 18:05:13.950429+00	{"ServerId": "nailify background server:1:83ff1386-8eaa-4d3f-9f36-ecc928b5b48a", "WorkerId": "3483caa3-e204-437c-8e1e-f5559ef0e1a0", "StartedAt": "1788890712732"}	0
16416	5149	Succeeded	\N	2026-09-08 18:05:17.469035+00	{"Latency": "4668", "SucceededAt": "1788890715527", "PerformanceDuration": "603"}	0
16417	5150	Enqueued	Triggered by recurring job scheduler	2026-09-08 18:10:53.258773+00	{"Queue": "default", "EnqueuedAt": "1788891053251"}	0
16418	5150	Processing	\N	2026-09-08 18:10:56.991746+00	{"ServerId": "nailify background server:1:62c1a316-b29a-4c75-b45d-d714376eaadd", "WorkerId": "3500b943-e810-426c-92d5-46fb8433fddd", "StartedAt": "1788891055830"}	0
16419	5151	Enqueued	Triggered by recurring job scheduler	2026-09-08 18:10:57.614054+00	{"Queue": "default", "EnqueuedAt": "1788891057613"}	0
16420	5151	Processing	\N	2026-09-08 18:11:01.37879+00	{"ServerId": "nailify background server:1:62c1a316-b29a-4c75-b45d-d714376eaadd", "WorkerId": "910e6214-89e9-4f34-b33c-8afe7b9e064f", "StartedAt": "1788891060158"}	0
16421	5150	Succeeded	\N	2026-09-08 18:11:02.079987+00	{"Latency": "5456", "SucceededAt": "1788891060632", "PerformanceDuration": "2716"}	0
16423	5152	Enqueued	Triggered by recurring job scheduler	2026-09-08 18:15:10.907128+00	{"Queue": "default", "EnqueuedAt": "1788891310906"}	0
16424	5152	Processing	\N	2026-09-08 18:15:14.607409+00	{"ServerId": "nailify background server:1:62c1a316-b29a-4c75-b45d-d714376eaadd", "WorkerId": "910e6214-89e9-4f34-b33c-8afe7b9e064f", "StartedAt": "1788891313458"}	0
16425	5152	Succeeded	\N	2026-09-08 18:15:18.244438+00	{"Latency": "5394", "SucceededAt": "1788891316198", "PerformanceDuration": "669"}	0
16426	5153	Enqueued	Triggered by recurring job scheduler	2026-09-08 18:20:09.947469+00	{"Queue": "default", "EnqueuedAt": "1788891609947"}	0
16427	5154	Enqueued	Triggered by recurring job scheduler	2026-09-08 18:20:12.797547+00	{"Queue": "default", "EnqueuedAt": "1788891612797"}	0
16428	5153	Processing	\N	2026-09-08 18:20:13.090358+00	{"ServerId": "nailify background server:1:62c1a316-b29a-4c75-b45d-d714376eaadd", "WorkerId": "910e6214-89e9-4f34-b33c-8afe7b9e064f", "StartedAt": "1788891611806"}	0
16429	5154	Processing	\N	2026-09-08 18:20:16.516554+00	{"ServerId": "nailify background server:1:62c1a316-b29a-4c75-b45d-d714376eaadd", "WorkerId": "3500b943-e810-426c-92d5-46fb8433fddd", "StartedAt": "1788891615681"}	0
16430	5153	Succeeded	\N	2026-09-08 18:20:16.941309+00	{"Latency": "4674", "SucceededAt": "1788891614890", "PerformanceDuration": "772"}	0
16431	5154	Succeeded	\N	2026-09-08 18:20:19.089369+00	{"Latency": "4890", "SucceededAt": "1788891617750", "PerformanceDuration": "564"}	0
16432	5155	Enqueued	Triggered by recurring job scheduler	2026-09-08 18:25:15.869654+00	{"Queue": "default", "EnqueuedAt": "1788891915869"}	0
16433	5155	Processing	\N	2026-09-08 18:25:19.538428+00	{"ServerId": "nailify background server:1:62c1a316-b29a-4c75-b45d-d714376eaadd", "WorkerId": "3500b943-e810-426c-92d5-46fb8433fddd", "StartedAt": "1788891918323"}	0
16434	5155	Succeeded	\N	2026-09-08 18:25:23.325815+00	{"Latency": "5377", "SucceededAt": "1788891921268", "PerformanceDuration": "756"}	0
16435	5156	Enqueued	Triggered by recurring job scheduler	2026-09-08 18:30:10.254706+00	{"Queue": "default", "EnqueuedAt": "1788892210254"}	0
16436	5156	Processing	\N	2026-09-08 18:30:12.883899+00	{"ServerId": "nailify background server:1:62c1a316-b29a-4c75-b45d-d714376eaadd", "WorkerId": "3500b943-e810-426c-92d5-46fb8433fddd", "StartedAt": "1788892212050"}	0
16437	5157	Enqueued	Triggered by recurring job scheduler	2026-09-08 18:30:13.361687+00	{"Queue": "default", "EnqueuedAt": "1788892213361"}	0
16438	5156	Succeeded	\N	2026-09-08 18:30:15.392707+00	{"Latency": "3847", "SucceededAt": "1788892214058", "PerformanceDuration": "506"}	0
16439	5157	Processing	\N	2026-09-08 18:30:17.675649+00	{"ServerId": "nailify background server:1:62c1a316-b29a-4c75-b45d-d714376eaadd", "WorkerId": "3500b943-e810-426c-92d5-46fb8433fddd", "StartedAt": "1788892216841"}	0
16440	5157	Succeeded	\N	2026-09-08 18:30:20.335434+00	{"Latency": "5530", "SucceededAt": "1788892218874", "PerformanceDuration": "531"}	0
16441	5158	Enqueued	Triggered by recurring job scheduler	2026-09-08 18:35:07.29468+00	{"Queue": "default", "EnqueuedAt": "1788892507294"}	0
16442	5158	Processing	\N	2026-09-08 18:35:10.037806+00	{"ServerId": "nailify background server:1:62c1a316-b29a-4c75-b45d-d714376eaadd", "WorkerId": "910e6214-89e9-4f34-b33c-8afe7b9e064f", "StartedAt": "1788892509124"}	0
16443	5158	Succeeded	\N	2026-09-08 18:35:12.784303+00	{"Latency": "4032", "SucceededAt": "1788892511319", "PerformanceDuration": "550"}	0
16444	5159	Enqueued	Triggered by recurring job scheduler	2026-09-08 18:40:03.820867+00	{"Queue": "default", "EnqueuedAt": "1788892803820"}	0
16445	5159	Processing	\N	2026-09-08 18:40:06.938486+00	{"ServerId": "nailify background server:1:62c1a316-b29a-4c75-b45d-d714376eaadd", "WorkerId": "3500b943-e810-426c-92d5-46fb8433fddd", "StartedAt": "1788892806026"}	0
16450	5161	Enqueued	Triggered by recurring job scheduler	2026-09-08 18:45:06.869095+00	{"Queue": "default", "EnqueuedAt": "1788893106868"}	0
16451	5161	Processing	\N	2026-09-08 18:45:09.526794+00	{"ServerId": "nailify background server:1:62c1a316-b29a-4c75-b45d-d714376eaadd", "WorkerId": "3500b943-e810-426c-92d5-46fb8433fddd", "StartedAt": "1788893108694"}	0
16446	5160	Enqueued	Triggered by recurring job scheduler	2026-09-08 18:40:07.730823+00	{"Queue": "default", "EnqueuedAt": "1788892807730"}	0
16447	5159	Succeeded	\N	2026-09-08 18:40:09.68672+00	{"Latency": "4543", "SucceededAt": "1788892808221", "PerformanceDuration": "550"}	0
16448	5160	Processing	\N	2026-09-08 18:40:10.7803+00	{"ServerId": "nailify background server:1:62c1a316-b29a-4c75-b45d-d714376eaadd", "WorkerId": "910e6214-89e9-4f34-b33c-8afe7b9e064f", "StartedAt": "1788892809942"}	0
16449	5160	Succeeded	\N	2026-09-08 18:40:13.383804+00	{"Latency": "4409", "SucceededAt": "1788892812048", "PerformanceDuration": "598"}	0
16452	5161	Succeeded	\N	2026-09-08 18:45:12.797027+00	{"Latency": "3834", "SucceededAt": "1788893110841", "PerformanceDuration": "647"}	0
16453	5162	Enqueued	Triggered by recurring job scheduler	2026-09-08 19:10:20.648017+00	{"Queue": "default", "EnqueuedAt": "1788894620638"}	0
16454	5162	Processing	\N	2026-09-08 19:10:23.593157+00	{"ServerId": "nailify background server:1:109514e9-a06e-41a7-b461-a3d8d0f6fd5c", "WorkerId": "44fe2745-4386-4fdb-88d0-d2056580aec3", "StartedAt": "1788894622664"}	0
16455	5163	Enqueued	Triggered by recurring job scheduler	2026-09-08 19:10:24.049563+00	{"Queue": "default", "EnqueuedAt": "1788894624049"}	0
16456	5163	Processing	\N	2026-09-08 19:10:27.304896+00	{"ServerId": "nailify background server:1:109514e9-a06e-41a7-b461-a3d8d0f6fd5c", "WorkerId": "6b325446-2d20-45d6-9842-1e7030ebc3c7", "StartedAt": "1788894626019"}	0
16457	5162	Succeeded	\N	2026-09-08 19:10:28.669524+00	{"Latency": "4398", "SucceededAt": "1788894627221", "PerformanceDuration": "2894"}	0
16458	5163	Succeeded	\N	2026-09-08 19:10:32.593437+00	{"Latency": "4812", "SucceededAt": "1788894631126", "PerformanceDuration": "2849"}	0
16459	5164	Enqueued	Triggered by recurring job scheduler	2026-09-08 19:15:06.935372+00	{"Queue": "default", "EnqueuedAt": "1788894906935"}	0
16460	5164	Processing	\N	2026-09-08 19:15:09.805811+00	{"ServerId": "nailify background server:1:109514e9-a06e-41a7-b461-a3d8d0f6fd5c", "WorkerId": "44fe2745-4386-4fdb-88d0-d2056580aec3", "StartedAt": "1788894908892"}	0
16461	5164	Succeeded	\N	2026-09-08 19:15:12.496221+00	{"Latency": "4117", "SucceededAt": "1788894911152", "PerformanceDuration": "613"}	0
16462	5165	Enqueued	Triggered by recurring job scheduler	2026-09-08 19:20:03.908844+00	{"Queue": "default", "EnqueuedAt": "1788895203908"}	0
16463	5166	Enqueued	Triggered by recurring job scheduler	2026-09-08 19:20:08.097602+00	{"Queue": "default", "EnqueuedAt": "1788895208097"}	0
16464	5165	Processing	\N	2026-09-08 19:20:09.606389+00	{"ServerId": "nailify background server:1:109514e9-a06e-41a7-b461-a3d8d0f6fd5c", "WorkerId": "44fe2745-4386-4fdb-88d0-d2056580aec3", "StartedAt": "1788895208765"}	0
16465	5165	Succeeded	\N	2026-09-08 19:20:12.122441+00	{"Latency": "7088", "SucceededAt": "1788895210783", "PerformanceDuration": "505"}	0
16466	5166	Processing	\N	2026-09-08 19:20:12.516396+00	{"ServerId": "nailify background server:1:109514e9-a06e-41a7-b461-a3d8d0f6fd5c", "WorkerId": "6b325446-2d20-45d6-9842-1e7030ebc3c7", "StartedAt": "1788895211553"}	0
16467	5166	Succeeded	\N	2026-09-08 19:20:15.330995+00	{"Latency": "5981", "SucceededAt": "1788895213984", "PerformanceDuration": "683"}	0
16468	5167	Enqueued	Triggered by recurring job scheduler	2026-09-08 19:25:05.76244+00	{"Queue": "default", "EnqueuedAt": "1788895505762"}	0
16469	5167	Processing	\N	2026-09-08 19:25:09.804009+00	{"ServerId": "nailify background server:1:109514e9-a06e-41a7-b461-a3d8d0f6fd5c", "WorkerId": "6b325446-2d20-45d6-9842-1e7030ebc3c7", "StartedAt": "1788895508887"}	0
16470	5167	Succeeded	\N	2026-09-08 19:25:12.753115+00	{"Latency": "5377", "SucceededAt": "1788895511156", "PerformanceDuration": "601"}	0
16471	5168	Enqueued	Triggered by recurring job scheduler	2026-09-08 19:30:16.081376+00	{"Queue": "default", "EnqueuedAt": "1788895816081"}	0
16472	5168	Processing	\N	2026-09-08 19:30:18.732149+00	{"ServerId": "nailify background server:1:109514e9-a06e-41a7-b461-a3d8d0f6fd5c", "WorkerId": "6b325446-2d20-45d6-9842-1e7030ebc3c7", "StartedAt": "1788895817897"}	0
16473	5169	Enqueued	Triggered by recurring job scheduler	2026-09-08 19:30:19.173258+00	{"Queue": "default", "EnqueuedAt": "1788895819172"}	0
16474	5168	Succeeded	\N	2026-09-08 19:30:21.267267+00	{"Latency": "3877", "SucceededAt": "1788895819932", "PerformanceDuration": "529"}	0
16475	5169	Processing	\N	2026-09-08 19:30:21.895722+00	{"ServerId": "nailify background server:1:109514e9-a06e-41a7-b461-a3d8d0f6fd5c", "WorkerId": "44fe2745-4386-4fdb-88d0-d2056580aec3", "StartedAt": "1788895820998"}	0
16476	5169	Succeeded	\N	2026-09-08 19:30:24.596879+00	{"Latency": "3987", "SucceededAt": "1788895823144", "PerformanceDuration": "530"}	0
16477	5170	Enqueued	Triggered by recurring job scheduler	2026-09-08 19:35:14.53509+00	{"Queue": "default", "EnqueuedAt": "1788896114534"}	0
16478	5170	Processing	\N	2026-09-08 19:35:17.263064+00	{"ServerId": "nailify background server:1:109514e9-a06e-41a7-b461-a3d8d0f6fd5c", "WorkerId": "44fe2745-4386-4fdb-88d0-d2056580aec3", "StartedAt": "1788896116350"}	0
16479	5170	Succeeded	\N	2026-09-08 19:35:20.002365+00	{"Latency": "4010", "SucceededAt": "1788896118542", "PerformanceDuration": "547"}	0
\.


--
-- Data for Name: BookingDiscounts; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."BookingDiscounts" ("BookingDiscountId", "BookingId", "Name", "DiscountAmount", "IsAutoApplied", "AppliedDate", "PromotionId", "LoyaltyTierId", "LoyaltyTransactionId") FROM stdin;
274	88642819-451f-4089-ac79-9bc8ef7ed4c4	Perfect Match	15000.00	t	2026-09-07 10:47:48.586698	2	\N	\N
222	37c9450a-fa86-4d51-b1b8-17cd94c18426	Platinum Tier	500.00	t	2026-08-18 21:36:52.559608	\N	5	\N
275	88642819-451f-4089-ac79-9bc8ef7ed4c4	Platinum Tier	1000.00	t	2026-09-07 17:47:48.586712	\N	5	\N
276	0f7b2049-386c-4142-8f2d-8f768632b078	Perfect Match	22500.00	t	2026-09-08 12:48:50.478217	2	\N	\N
239	32060188-f6aa-42d5-b12c-60c30b579d58	Platinum Tier	2100.00	t	2026-08-19 15:54:14.550831	\N	5	\N
256	738529a7-3c13-4311-b5fc-2005f1aba573	Winter Wonderland	20000.00	t	2026-08-23 14:51:13.671764	1	\N	\N
257	738529a7-3c13-4311-b5fc-2005f1aba573	Platinum Tier	1000.00	t	2026-08-23 21:51:13.671798	\N	5	\N
258	3a81ba1b-9a1c-48db-9094-4d318ba065e4	Winter Wonderland	20000.00	t	2026-08-23 14:54:27.23028	1	\N	\N
259	3a81ba1b-9a1c-48db-9094-4d318ba065e4	Platinum Tier	1000.00	t	2026-08-23 21:54:27.230307	\N	5	\N
260	f4730e5d-340c-4121-bddf-923e4d400c6c	Platinum Tier	1600.00	t	2026-08-24 08:29:52.183141	\N	5	\N
261	70eb03a9-e158-4428-a196-e4d81eb232d3	Platinum Tier	1600.00	t	2026-08-24 08:52:33.606815	\N	5	\N
262	5864c2a3-6e31-4f7a-a143-c4afbc025327	Platinum Tier	1600.00	t	2026-08-24 12:08:49.419565	\N	5	\N
265	36100c80-a36d-41e5-a65f-822e3cc830a6	Winter Wonderland	30000.00	t	2026-09-04 15:18:14.472674	1	\N	\N
266	36100c80-a36d-41e5-a65f-822e3cc830a6	Platinum Tier	1500.00	t	2026-09-04 22:18:14.473628	\N	5	\N
267	046f33ec-d9c1-479f-b29f-a08ae9869902	Winter Wonderland	30000.00	t	2026-09-04 15:44:01.24177	1	\N	\N
268	046f33ec-d9c1-479f-b29f-a08ae9869902	Giảm giá 2/9	30000.00	f	2026-09-04 15:44:01.241774	8	\N	\N
269	046f33ec-d9c1-479f-b29f-a08ae9869902	Platinum Tier	1500.00	t	2026-09-04 22:44:01.241793	\N	5	\N
270	8a2e0d2e-8ecf-4f89-9fa3-354275e60f02	Platinum Tier	1500.00	t	2026-09-04 22:58:07.189444	\N	5	\N
\.


--
-- Data for Name: BookingHistories; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."BookingHistories" ("BookingHistoryId", "BookingId", "EventType", "Payload", "ActorId", "CreatedAt") FROM stdin;
cd3eb9a0-8093-409e-9460-d5dcf6ce2594	37c9450a-fa86-4d51-b1b8-17cd94c18426	BookingCreated	Đơn đặt lịch được tạo thành công bởi khách hàng. Mã QR (Base64) đã được khởi tạo.	0ddb8972-36cd-4b67-8887-829aadbdf942	2026-08-18 14:36:53.030645
b53196c6-183a-4fee-b175-1abbc244c72f	37c9450a-fa86-4d51-b1b8-17cd94c18426	BookingConfirmed	Quản lý Salon xác nhận duyệt đơn đặt lịch.	3c63f226-f626-4757-9683-8e0376606b1a	2026-08-18 14:46:11.068061
ce8547c9-e9a6-4965-8970-854a4d35e010	32060188-f6aa-42d5-b12c-60c30b579d58	BookingCreated	Đơn đặt lịch được tạo thành công bởi khách hàng. Mã QR (Base64) đã được khởi tạo.	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	2026-08-19 08:54:15.127965
87f5079a-69ae-4419-9cea-0022812298d0	32060188-f6aa-42d5-b12c-60c30b579d58	CheckedIn	Xác thực mã QR thành công. Trạng thái đơn hàng chuyển sang CheckedIn.	3d089065-f088-4508-b5b3-f89b47722125	2026-08-19 08:58:10.871059
5f6b7640-2ebd-4324-bd27-b0abe17de613	37c9450a-fa86-4d51-b1b8-17cd94c18426	Cancelled	Hủy đơn từ trạng thái 'Approved' sang 'Cancelled'. Lý do: Hệ thống tự động hủy do khách trễ quá 15 phút mà không check-in.	\N	2026-08-18 15:30:07.924319
264e5ebf-8afc-4281-a4dd-a35c392df8f3	32060188-f6aa-42d5-b12c-60c30b579d58	ServiceCompleted	Thợ nail đã hoàn thành các dịch vụ. Ảnh trạng thái tay sau khi làm: https://res.cloudinary.com/devu5qabc/image/upload/v1787131698/4522faa6-10df-431d-9fd0-7b50116e4f85.png?cors=anonymous	ca83eb05-91ea-4865-b4fe-82948bf5dbbf	2026-08-19 09:28:20.890751
47f7f820-07e5-4c2a-9998-fdbe2bd6e069	738529a7-3c13-4311-b5fc-2005f1aba573	BookingCreated	Đơn đặt lịch được tạo thành công bởi khách hàng. Mã QR (Base64) đã được khởi tạo.	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	2026-08-23 14:51:14.162303
2c035106-1605-407f-9e90-d64bfd013332	f4730e5d-340c-4121-bddf-923e4d400c6c	BookingCreated	Đơn đặt lịch được tạo thành công bởi khách hàng. Mã QR (Base64) đã được khởi tạo.	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	2026-08-24 01:29:54.252372
43d63227-db3f-4399-85fe-8d8e4a41bec8	70eb03a9-e158-4428-a196-e4d81eb232d3	BookingCreated	Đơn đặt lịch được tạo thành công bởi khách hàng. Mã QR (Base64) đã được khởi tạo.	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	2026-08-24 01:52:34.227304
6b4b839e-0a03-451e-83f0-7116aa06f4fb	5864c2a3-6e31-4f7a-a143-c4afbc025327	BookingRejected	Quản lý Salon từ chối đơn đặt lịch. Lý do: no_response	f9e14612-1973-4180-98a4-7e1b239d242c	2026-08-25 11:40:48.461274
0f53d718-cf92-4b2e-a523-508a356ebd4f	70eb03a9-e158-4428-a196-e4d81eb232d3	Completed	Khách hàng đã thanh toán hóa đơn và hoàn thành thủ tục check-out.	3d089065-f088-4508-b5b3-f89b47722125	2026-08-28 06:36:11.819762
53cbf5a1-c7f3-4d5e-a8cc-0c6992881aca	70eb03a9-e158-4428-a196-e4d81eb232d3	Completed	Khách hàng đã thanh toán hóa đơn và hoàn thành thủ tục check-out.	3d089065-f088-4508-b5b3-f89b47722125	2026-08-28 06:56:49.22075
006ea1be-59aa-4d2c-a671-b6b9b646e86c	36100c80-a36d-41e5-a65f-822e3cc830a6	BookingCreated	Đơn đặt lịch được tạo thành công bởi khách hàng. Mã QR (Base64) đã được khởi tạo.	0ddb8972-36cd-4b67-8887-829aadbdf942	2026-09-04 15:18:17.17063
f9b28b56-85cc-46fb-82ff-67020cbd3c7e	046f33ec-d9c1-479f-b29f-a08ae9869902	BookingCreated	Đơn đặt lịch được tạo thành công bởi khách hàng. Mã QR (Base64) đã được khởi tạo.	0ddb8972-36cd-4b67-8887-829aadbdf942	2026-09-04 15:44:11.712765
c63a23f5-4816-4708-a338-528338b94be1	8a2e0d2e-8ecf-4f89-9fa3-354275e60f02	BookingConfirmed	Quản lý Salon xác nhận duyệt đơn đặt lịch.	0ddb8972-36cd-4b67-8887-829aadbdf942	2026-09-04 16:00:44.308469
6ad39cfe-bd55-4239-9f78-855e27248881	8a2e0d2e-8ecf-4f89-9fa3-354275e60f02	CheckIn	Khách hàng đã check-in.	0ddb8972-36cd-4b67-8887-829aadbdf942	2026-09-04 16:01:04.342198
266275c4-e72c-4f9b-adf7-705603b9278b	8a2e0d2e-8ecf-4f89-9fa3-354275e60f02	ServiceStarted	Thợ làm móng bắt đầu thực hiện các dịch vụ trong đơn.	0ddb8972-36cd-4b67-8887-829aadbdf942	2026-09-04 16:03:23.040457
14b2a695-1e6c-4884-9f39-4ac5f7e93b4f	8a2e0d2e-8ecf-4f89-9fa3-354275e60f02	ServiceCompleted	Thợ nail đã hoàn thành các dịch vụ. Ảnh trạng thái tay sau khi làm: https://res.cloudinary.com/devu5qabc/image/upload/v1788537918/fc052d46-3d97-4fc4-82eb-adf8d60a483f.png?cors=anonymous	0ddb8972-36cd-4b67-8887-829aadbdf942	2026-09-04 16:05:19.933925
21d0936f-8bfb-4317-a46d-91d439c0b95d	046f33ec-d9c1-479f-b29f-a08ae9869902	BookingRejected	Quản lý Salon từ chối đơn đặt lịch. Lý do: other	3c63f226-f626-4757-9683-8e0376606b1a	2026-09-07 09:28:31.637182
269041b6-53cc-4c99-bb66-efda0c667cb3	88642819-451f-4089-ac79-9bc8ef7ed4c4	BookingCreated	Đơn đặt lịch được tạo thành công bởi khách hàng. Mã QR (Base64) đã được khởi tạo.	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	2026-09-07 10:47:48.914271
e3262e8f-d4d7-4ac1-a24b-3b7dd85d2858	05d91038-03d5-4743-80d9-34020b38e94a	RescheduleSuggested	Quản lý Salon đề xuất đổi lịch sang ngày 09/09/2026 lúc 08:30:00. Lý do: Salon manager proposed alternative time slot.	3c63f226-f626-4757-9683-8e0376606b1a	2026-09-08 10:17:48.205644
3059fb8c-2d00-487a-838b-5fc26b6fdbbc	05d91038-03d5-4743-80d9-34020b38e94a	BookingUpdated	Đơn đặt lịch được cập nhật. Tổng tiền mới: 100000.00. Tổng thời gian: 175 phút.	3c63f226-f626-4757-9683-8e0376606b1a	2026-09-08 10:24:10.096211
639a337d-dc1b-4aad-bd9b-559029342dc2	05d91038-03d5-4743-80d9-34020b38e94a	CheckIn	Khách hàng đã check-in.	3d089065-f088-4508-b5b3-f89b47722125	2026-09-08 12:21:51.22046
53756f56-f3a3-4da8-aa64-db533292c84b	0f7b2049-386c-4142-8f2d-8f768632b078	BookingUpdated	Đơn đặt lịch được cập nhật. Tổng tiền mới: 300000.00. Tổng thời gian: 190 phút.	ca83eb05-91ea-4865-b4fe-82948bf5dbbf	2026-09-08 12:48:51.094521
9039378d-03d0-4650-9924-5f69f9f08eaa	0f7b2049-386c-4142-8f2d-8f768632b078	ServiceStarted	Thợ làm móng bắt đầu thực hiện các dịch vụ trong đơn.	ca83eb05-91ea-4865-b4fe-82948bf5dbbf	2026-09-08 12:49:12.683885
66a412d0-c78e-4320-a218-919e10dee6c2	05d91038-03d5-4743-80d9-34020b38e94a	ServiceStarted	Thợ làm móng bắt đầu thực hiện các dịch vụ trong đơn.	ca83eb05-91ea-4865-b4fe-82948bf5dbbf	2026-09-08 13:29:49.387874
efe85339-8973-4cec-be96-5068761da197	32060188-f6aa-42d5-b12c-60c30b579d58	BookingConfirmed	Quản lý Salon xác nhận duyệt đơn đặt lịch.	3c63f226-f626-4757-9683-8e0376606b1a	2026-08-19 08:56:51.312919
0497ac89-18b2-415a-8448-59cf9c1ce2f1	32060188-f6aa-42d5-b12c-60c30b579d58	ServiceStarted	Thợ làm móng bắt đầu thực hiện các dịch vụ trong đơn.	ca83eb05-91ea-4865-b4fe-82948bf5dbbf	2026-08-19 09:18:06.829027
b79085c3-0f87-428b-8629-372cb42abada	32060188-f6aa-42d5-b12c-60c30b579d58	Completed	Khách hàng đã thanh toán hóa đơn và hoàn thành thủ tục check-out.	\N	2026-08-19 09:29:22.997206
d9a049a3-1e41-4259-b3f8-3f96c489b215	738529a7-3c13-4311-b5fc-2005f1aba573	Completed	Khách hàng đã thanh toán hóa đơn và hoàn thành thủ tục check-out.	\N	2026-08-23 14:53:38.114697
c36536ca-e889-45ca-abe4-ff5bc254248f	3a81ba1b-9a1c-48db-9094-4d318ba065e4	BookingCreated	Đơn đặt lịch được tạo thành công bởi khách hàng. Mã QR (Base64) đã được khởi tạo.	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	2026-08-23 14:54:27.702547
2f376630-84a6-4a0d-98d2-cfdf387e3542	3a81ba1b-9a1c-48db-9094-4d318ba065e4	Completed	Khách hàng đã thanh toán hóa đơn và hoàn thành thủ tục check-out.	\N	2026-08-23 14:55:34.470177
f9d28ab7-56a2-4744-bd7f-46aa53855faf	f4730e5d-340c-4121-bddf-923e4d400c6c	ServiceStarted	Thợ làm móng bắt đầu thực hiện các dịch vụ trong đơn.	ca83eb05-91ea-4865-b4fe-82948bf5dbbf	2026-08-24 01:46:18.268123
82b363a7-4226-4ecb-bcf5-0db0e6da7638	5864c2a3-6e31-4f7a-a143-c4afbc025327	BookingCreated	Đơn đặt lịch được tạo thành công bởi khách hàng. Mã QR (Base64) đã được khởi tạo.	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	2026-08-24 05:08:51.185883
40b72f2d-1ffa-445c-987c-315a1cbd6e8f	f4730e5d-340c-4121-bddf-923e4d400c6c	Completed	Khách hàng đã thanh toán hóa đơn và hoàn thành thủ tục check-out.	3d089065-f088-4508-b5b3-f89b47722125	2026-08-28 05:07:19.958534
67045bd4-b7d7-4308-a3ac-c1fb3eab08b4	70eb03a9-e158-4428-a196-e4d81eb232d3	Completed	Khách hàng đã thanh toán hóa đơn và hoàn thành thủ tục check-out.	3d089065-f088-4508-b5b3-f89b47722125	2026-08-28 06:46:12.525384
26e6c0d2-12b0-4c4a-979a-860bafe4fc2b	70eb03a9-e158-4428-a196-e4d81eb232d3	Completed	Khách hàng đã thanh toán hóa đơn và hoàn thành thủ tục check-out.	3d089065-f088-4508-b5b3-f89b47722125	2026-08-28 06:49:26.010506
6811d1d6-d755-4552-b4fd-5bc455f2d86f	36100c80-a36d-41e5-a65f-822e3cc830a6	BookingConfirmed	Quản lý Salon xác nhận duyệt đơn đặt lịch.	0ddb8972-36cd-4b67-8887-829aadbdf942	2026-09-04 15:31:58.621611
7156ab43-6303-4dba-98e0-8c822b863f9a	8a2e0d2e-8ecf-4f89-9fa3-354275e60f02	BookingCreated	Đơn đặt lịch được tạo thành công bởi khách hàng. Mã QR (Base64) đã được khởi tạo.	0ddb8972-36cd-4b67-8887-829aadbdf942	2026-09-04 15:58:08.790785
b6aa2cfc-5dad-483f-9fef-2473e2e102e4	8a2e0d2e-8ecf-4f89-9fa3-354275e60f02	Completed	Khách hàng đã thanh toán hóa đơn và hoàn thành thủ tục check-out.	0ddb8972-36cd-4b67-8887-829aadbdf942	2026-09-04 16:05:31.584607
d7503d9d-7937-4171-906f-fa9ae0c3afe5	36100c80-a36d-41e5-a65f-822e3cc830a6	CheckIn	Khách hàng đã check-in.	0ddb8972-36cd-4b67-8887-829aadbdf942	2026-09-04 16:12:43.733216
f9d9f9ab-1113-4606-9517-371f7bf754e7	05d91038-03d5-4743-80d9-34020b38e94a	BookingCreated	Đơn đặt lịch được tạo thành công bởi khách hàng. Mã QR (Base64) đã được khởi tạo.	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	2026-09-07 10:43:59.449609
62fa9815-1fee-405e-b96d-477c3790d3f0	88642819-451f-4089-ac79-9bc8ef7ed4c4	BookingRejected	Quản lý Salon từ chối đơn đặt lịch. Lý do: other	3c63f226-f626-4757-9683-8e0376606b1a	2026-09-07 13:40:55.125501
bda2c438-ad4f-4f6e-997a-69e7405fb919	05d91038-03d5-4743-80d9-34020b38e94a	RescheduleAccepted	Xác nhận thay đổi lịch hẹn từ ngày 08/09/2026 lúc 10:00:00 sang ngày 09/09/2026 lúc 08:30:00.	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	2026-09-08 10:19:48.42261
142a43c1-e99e-4ce3-a9a7-36478d7a87ea	36100c80-a36d-41e5-a65f-822e3cc830a6	ChairAssigned	Đã phân bổ ghế 1A cho khách hàng.	3d089065-f088-4508-b5b3-f89b47722125	2026-09-08 11:03:22.511914
f52f7b90-1d34-47ab-baef-c13a787593e8	05d91038-03d5-4743-80d9-34020b38e94a	ChairAssigned	Đã phân bổ ghế 1A cho khách hàng.	3d089065-f088-4508-b5b3-f89b47722125	2026-09-08 12:27:53.780478
2e34591a-256c-4df4-baee-84365d461fd3	0f7b2049-386c-4142-8f2d-8f768632b078	ServiceCompleted	Thợ nail đã hoàn thành các dịch vụ. Ảnh trạng thái tay sau khi làm: https://res.cloudinary.com/devu5qabc/image/upload/v1788873729/4cc1d41b-9725-4def-b3d9-7efabcc818c3.png?cors=anonymous	ca83eb05-91ea-4865-b4fe-82948bf5dbbf	2026-09-08 13:22:10.84182
\.


--
-- Data for Name: BookingItems; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."BookingItems" ("BookingItemId", "BookingId", "ServiceId", "NailVariantId", "Quantity", "Price", "Duration", "CustomerNailRequestId", "ShapeMethodConfigId") FROM stdin;
760124cf-1f65-4bf3-b48a-99296954c4c9	32060188-f6aa-42d5-b12c-60c30b579d58	\N	37	1	160000.00	187	\N	7
795b92f9-5a9e-4406-87bf-0cbdae37a462	32060188-f6aa-42d5-b12c-60c30b579d58	3135047c-f048-4f8b-926c-d5675e3387c3	\N	1	50000.00	20	\N	\N
4baf28be-871f-415d-b802-ddf0e8c27be4	738529a7-3c13-4311-b5fc-2005f1aba573	\N	26	1	100000.00	175	\N	7
863d6747-1d38-47ee-82b6-f9e91f7889db	5864c2a3-6e31-4f7a-a143-c4afbc025327	\N	37	1	160000.00	187	\N	7
ff20d639-ba46-4260-8f26-4be02b7617a3	046f33ec-d9c1-479f-b29f-a08ae9869902	\N	17	1	150000.00	130	\N	\N
9101f539-4b16-4b66-9856-73352e3f491f	05d91038-03d5-4743-80d9-34020b38e94a	\N	26	1	100000.00	175	\N	1
0cd4d30e-05dd-4ace-affd-1cf534657023	bd7f9cab-5cc5-4002-a0f2-9e40d8212d34	3135047c-f048-4f8b-926c-d5675e3387c3	17	1	82000.00	34	\N	\N
99c297a3-cd70-47d5-927e-d430c3712753	bd7f9cab-5cc5-4002-a0f2-9e40d8212d34	51870a0e-cae5-4f0f-9991-f330bb35462f	\N	1	12000.00	12	\N	\N
a6c6d5f1-a325-44e4-bcb8-9b41ed0772f5	bd7f9cab-5cc5-4002-a0f2-9e40d8212d34	f29a17c3-f99e-4b5d-a0ff-3e01b2e6a264	\N	1	12000.00	10	\N	\N
fe89ab73-8f7c-4f42-9736-41231930d70b	bd7f9cab-5cc5-4002-a0f2-9e40d8212d34	3135047c-f048-4f8b-926c-d5675e3387c3	\N	3	12000.00	12	\N	\N
b5bbdb78-63d2-41a6-9189-b1e29e6dd0fd	084a0474-6a0a-4cda-9043-7ab6511e15af	\N	17	1	70000.00	22	\N	\N
9f5bea01-7740-4dcb-99bf-021df7cab16f	e0778881-34f5-42ad-ad7c-f4dfe63b6131	\N	17	1	70000.00	22	\N	\N
fcaf0ea5-8550-4f76-be1f-2b4289f43e36	a3a6cfdd-9aaa-4904-8513-342fab243790	\N	32	1	671200.00	262	\N	14
72ace6e1-d555-4c46-9f4c-a660e71ac2e4	55923b8b-2f46-4698-ae94-10793ee8674e	3135047c-f048-4f8b-926c-d5675e3387c3	\N	1	12000.00	12	\N	\N
a28b0643-ced7-45e3-8f46-ddc045547e8c	55923b8b-2f46-4698-ae94-10793ee8674e	\N	17	1	70000.00	22	\N	\N
b02b070c-f577-4474-bef0-b9d9efe5a83e	55923b8b-2f46-4698-ae94-10793ee8674e	51870a0e-cae5-4f0f-9991-f330bb35462f	\N	1	12000.00	12	\N	\N
a7bcbd77-a3f3-4717-b13b-ea9b6c1c157b	37c9450a-fa86-4d51-b1b8-17cd94c18426	3135047c-f048-4f8b-926c-d5675e3387c3	\N	1	50000.00	20	\N	\N
5f2a4b59-b769-4f79-9add-c0d730579ddc	8833af4e-bb29-486b-9a45-7bd33f4241e1	3135047c-f048-4f8b-926c-d5675e3387c3	\N	1	12000.00	12	\N	\N
1761c3d3-9671-42ee-8a56-4a5d04516acf	6f816baa-48a9-477d-9700-db60482ab272	\N	17	1	70000.00	22	\N	\N
6e728820-df91-4aa2-a1e7-ce54ee0733dc	6f816baa-48a9-477d-9700-db60482ab272	51870a0e-cae5-4f0f-9991-f330bb35462f	\N	1	12000.00	12	\N	\N
70eefc0a-8b0d-4ff5-a243-702200db9958	6f816baa-48a9-477d-9700-db60482ab272	3135047c-f048-4f8b-926c-d5675e3387c3	\N	1	12000.00	12	\N	\N
b296ff00-e176-40a7-9c93-c1723356dbe7	f4730e5d-340c-4121-bddf-923e4d400c6c	\N	37	1	160000.00	187	\N	7
1bc017f2-53e3-4447-9d65-073a5904dd0e	98a90c97-1cfd-4eb6-8c85-33d6ffbac9bf	\N	17	1	70000.00	22	\N	\N
31ced356-0b85-4a69-b0d4-86fb45612b97	98a90c97-1cfd-4eb6-8c85-33d6ffbac9bf	51870a0e-cae5-4f0f-9991-f330bb35462f	\N	1	12000.00	12	\N	\N
8ac3de52-3a62-4655-aaea-ff7d22d11d92	98a90c97-1cfd-4eb6-8c85-33d6ffbac9bf	3135047c-f048-4f8b-926c-d5675e3387c3	\N	1	12000.00	12	\N	\N
431bdbba-7743-487d-bd4d-836f5b4e6ef2	5ed810a0-f0ee-4046-85ba-62183a3a49da	51870a0e-cae5-4f0f-9991-f330bb35462f	\N	1	12000.00	12	\N	\N
60a8f4ae-851e-4e53-a6f8-e1a4cc8ecd0e	5ed810a0-f0ee-4046-85ba-62183a3a49da	3135047c-f048-4f8b-926c-d5675e3387c3	\N	1	12000.00	12	\N	\N
d1c947e8-0e1c-4baa-b2d5-95d55102a779	8a2e0d2e-8ecf-4f89-9fa3-354275e60f02	\N	17	1	150000.00	130	\N	\N
37ac551e-271e-4b75-8f5f-98535ab169ee	0d0db69e-14d2-45fe-876d-31767e9f553c	\N	17	1	150000.00	130	\N	\N
03cebc6a-5b70-4498-835e-5d17f6e0331d	4e3f981e-f1f3-42d5-b67b-299f07b178c2	3135047c-f048-4f8b-926c-d5675e3387c3	\N	1	12000.00	12	\N	\N
0ee211ed-3d85-4577-b9bb-ce2a3e0d452c	4e3f981e-f1f3-42d5-b67b-299f07b178c2	\N	17	1	70000.00	22	\N	\N
3e64b31e-9fad-4a30-89fe-3a6d24064ee7	4e3f981e-f1f3-42d5-b67b-299f07b178c2	51870a0e-cae5-4f0f-9991-f330bb35462f	\N	1	12000.00	12	\N	\N
a09b0da1-f4fa-4b28-82ec-15eee5de99dd	dabb9a56-e346-4f3b-9a8d-7e4c175ff1fc	3135047c-f048-4f8b-926c-d5675e3387c3	\N	1	12000.00	12	\N	\N
abd05d08-6a00-48da-8fd8-78e39b98fa0e	dabb9a56-e346-4f3b-9a8d-7e4c175ff1fc	\N	17	1	70000.00	22	\N	\N
bd3d27b3-71f0-4e51-b30c-fe88b8560ad7	dabb9a56-e346-4f3b-9a8d-7e4c175ff1fc	51870a0e-cae5-4f0f-9991-f330bb35462f	\N	1	12000.00	12	\N	\N
0a7714a2-33c2-4471-b924-32efed6f195c	c2a91a31-1018-4a5e-a8e1-36e7249fecee	3135047c-f048-4f8b-926c-d5675e3387c3	\N	1	12000.00	12	\N	\N
3f8f4c50-e19b-4ad5-ac4d-a7be48cf6c76	0d0db69e-14d2-45fe-876d-31767e9f553c	39850d34-91e5-4973-8a0f-644476dfe48a	\N	2	50000.00	20	\N	\N
c77a3985-d37e-4d36-8152-ca57eb7ecadd	0d0db69e-14d2-45fe-876d-31767e9f553c	f512b732-231c-4584-b3f0-647603b1f167	\N	1	50000.00	20	\N	\N
7eac41a2-f0f7-48d1-a942-3d3ffda2fa63	931c5c48-eace-4ef1-a173-b791a22ca726	\N	36	1	160000.00	57	\N	7
bf310edd-4efc-4158-a4c3-e0b473b38d9c	f4b92bcb-3365-4354-9902-f9c084922767	\N	36	1	160000.00	57	\N	7
d01fe84d-bfd1-48c5-84df-5db973c195cc	c13ad8a8-e179-48c0-9907-af31451e0565	\N	36	1	160000.00	57	\N	7
689f5e44-93eb-4209-be61-46cf03f07869	c1106f3d-8095-443e-b035-2df761fcac6d	\N	36	1	160000.00	57	\N	7
e72722e2-a3ca-4196-9ba3-db94a6e03fb5	84853280-d3b7-4150-901a-4dd60e63d745	\N	36	1	160000.00	57	\N	7
b06e4066-5f6e-4e1c-b88c-94b5480a7bab	ddc31886-f48e-4fcc-943c-d3cc9e14c8f5	\N	36	1	160000.00	57	\N	7
d2f26f62-8de8-40b7-b4d7-e778bd4d2f35	571bda98-a035-4b14-9d2a-6e6a6bb2acf2	\N	36	1	160000.00	57	\N	7
eeadb801-5d36-4387-9238-04638694825b	58fc44bd-2ed3-40e1-9fbc-c5966335e604	\N	17	1	70000.00	22	\N	\N
b6d25fff-c462-4459-b3bd-1699524ef323	0c24750d-074f-4ae1-9e3f-98e25f955b25	3135047c-f048-4f8b-926c-d5675e3387c3	\N	1	12000.00	12	\N	\N
1f5a84af-5e0d-4cb8-8559-b8cdd2a36cb4	0ecf1a61-9159-4bbd-9b2c-45a83bd42e4c	f29a17c3-f99e-4b5d-a0ff-3e01b2e6a264	\N	1	12000.00	10	\N	\N
1b7493f6-c1d5-401c-b733-ab6ee756cec5	9c996c7a-7996-4e01-b4f6-626910de046e	3135047c-f048-4f8b-926c-d5675e3387c3	\N	1	12000.00	12	\N	\N
5a9232ff-2139-4799-bd04-49855f0fde4f	9c996c7a-7996-4e01-b4f6-626910de046e	51870a0e-cae5-4f0f-9991-f330bb35462f	\N	1	12000.00	12	\N	\N
6be32485-f5f6-4d80-8354-5f034e285d18	9c996c7a-7996-4e01-b4f6-626910de046e	3135047c-f048-4f8b-926c-d5675e3387c3	\N	1	12000.00	12	\N	\N
701293a1-2e71-45d2-8104-69055d5e372f	5ed810a0-f0ee-4046-85ba-62183a3a49da	\N	\N	1	60000.00	120	\N	\N
80ac8485-82d1-4f20-a61f-d857284912d3	bd3b6417-fa2b-4ef4-8ab7-3e33c61efff0	\N	17	1	70000.00	22	\N	\N
eee7b591-80ba-42f0-9fcf-f1ae94839e8a	bd3b6417-fa2b-4ef4-8ab7-3e33c61efff0	51870a0e-cae5-4f0f-9991-f330bb35462f	\N	1	12000.00	12	\N	\N
f35c1fd7-9bbb-422a-8f73-7b5d2a4ad80c	bd3b6417-fa2b-4ef4-8ab7-3e33c61efff0	3135047c-f048-4f8b-926c-d5675e3387c3	\N	1	12000.00	12	\N	\N
06247c34-d09b-408b-9926-d250dfbc04be	0e2ccd36-f0bd-4fa2-a4f4-f9c81218658c	51870a0e-cae5-4f0f-9991-f330bb35462f	\N	1	12000.00	12	\N	\N
691a146e-62fa-4b45-ade9-04049fbb77b3	0e2ccd36-f0bd-4fa2-a4f4-f9c81218658c	3135047c-f048-4f8b-926c-d5675e3387c3	\N	1	12000.00	12	\N	\N
aa1e3d43-c073-4faa-93d6-6c617723edf7	0e2ccd36-f0bd-4fa2-a4f4-f9c81218658c	\N	17	1	70000.00	22	\N	\N
733400cc-fd58-481b-a759-4af7589d6cdf	3a81ba1b-9a1c-48db-9094-4d318ba065e4	\N	26	1	100000.00	175	\N	7
469b365a-620a-4bad-bcc6-1b122fd20840	70eb03a9-e158-4428-a196-e4d81eb232d3	\N	37	1	160000.00	187	\N	7
09ea6636-dbeb-4d51-aea8-28fa899e7c12	36100c80-a36d-41e5-a65f-822e3cc830a6	\N	17	1	150000.00	130	\N	\N
455cddcb-643b-470d-92d6-9534e93af5af	36100c80-a36d-41e5-a65f-822e3cc830a6	3135047c-f048-4f8b-926c-d5675e3387c3	\N	1	50000.00	20	\N	\N
db6c20af-30f7-45ad-8e67-575dbb34052d	88642819-451f-4089-ac79-9bc8ef7ed4c4	\N	26	1	100000.00	175	\N	1
76804696-618e-4508-be0a-f705969d6561	26d9fef8-cc52-4112-ad1a-50beb7a87a3a	51870a0e-cae5-4f0f-9991-f330bb35462f	\N	1	12000.00	12	\N	\N
7a390e0e-0026-4389-be4a-fc32bbed8b1d	0f7b2049-386c-4142-8f2d-8f768632b078	387fdc42-a732-46cb-abe0-3bbe4fa11906	\N	2	50000.00	20	\N	\N
7dfa4355-c6ed-40f6-8923-dd4851aac7a5	0f7b2049-386c-4142-8f2d-8f768632b078	\N	17	1	150000.00	130	\N	\N
f13dd3c1-e06f-4ecc-8bd1-e819e321466d	0f7b2049-386c-4142-8f2d-8f768632b078	39850d34-91e5-4973-8a0f-644476dfe48a	\N	1	50000.00	20	\N	\N
e985e08d-27a6-44d5-8b51-b91458b899fc	0f7b2049-386c-4142-8f2d-8f768632b078	387fdc42-a732-46cb-abe0-3bbe4fa11906	\N	1	50000.00	20	\N	\N
56dae948-2603-43b1-b019-6afc9e80358a	0f7b2049-386c-4142-8f2d-8f768632b078	f29a17c3-f99e-4b5d-a0ff-3e01b2e6a264	\N	1	50000.00	20	\N	\N
b44f6a74-d00b-4fc0-a289-e2f7d5455d58	0f7b2049-386c-4142-8f2d-8f768632b078	387fdc42-a732-46cb-abe0-3bbe4fa11906	\N	1	50000.00	20	\N	\N
292fac63-ffbe-47fc-ba51-8fc0b63e017b	2730e2f8-78c1-4296-a970-1174dc2c017d	387fdc42-a732-46cb-abe0-3bbe4fa11906	\N	1	50000.00	20	\N	\N
d46d4acf-db4d-4796-b5b7-a926449613c9	2730e2f8-78c1-4296-a970-1174dc2c017d	\N	37	1	210000.00	202	\N	8
e13d7d32-de16-4db6-b99f-33729e60c00e	2730e2f8-78c1-4296-a970-1174dc2c017d	3135047c-f048-4f8b-926c-d5675e3387c3	\N	1	50000.00	20	\N	\N
5e848861-5661-4f83-bbf3-c7cc13609989	5418ce86-ced6-4f45-a492-dba029ec3329	3135047c-f048-4f8b-926c-d5675e3387c3	\N	1	50000.00	20	\N	\N
62f27620-b3d3-4cb6-904e-b1eb65966e64	5418ce86-ced6-4f45-a492-dba029ec3329	387fdc42-a732-46cb-abe0-3bbe4fa11906	\N	1	50000.00	20	\N	\N
87e4d9ba-c7fe-4cea-8fe8-1255a6a89ccf	5418ce86-ced6-4f45-a492-dba029ec3329	\N	37	1	210000.00	202	\N	8
1eaf7dfe-6e6e-49b9-94b5-7f6c5dfefff7	bf5f02bf-6294-4ce7-9097-07974db57906	\N	17	1	150000.00	22	\N	\N
\.


--
-- Data for Name: BookingProcedures; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."BookingProcedures" ("BookingProcedureId", "BookingItemId", "ProcedureId", "ProcedureName", "Description", "StepOrder", "Status", "CompletedAt", "CompletedById", "IsRequired", "ActiveDuration", "ActualEndTime", "ActualStartTime", "AssignedArtistId", "CanOverlap", "Duration", "EstimatedEndTime", "EstimatedStartTime", "PassiveDuration", "IsMainStep", "TransitionBuffer") FROM stdin;
03794896-ee58-4a00-af1d-835942f8c822	c77a3985-d37e-4d36-8152-ca57eb7ecadd	\N	Chà gót chân	\N	1	Pending	\N	\N	t	20	\N	\N	\N	f	20	\N	\N	0	t	1
1c69e0be-79c6-42c2-92d6-570d3accfb31	37ac551e-271e-4b75-8f5f-98535ab169ee	ee1da04a-45f5-4b7f-8da8-66f8ef3c4240	Đắp bột	\N	1	Pending	\N	\N	t	1	\N	\N	\N	f	38	\N	\N	37	f	1
1cad7ff6-edfa-4e7b-a663-172e45dc5a5d	37ac551e-271e-4b75-8f5f-98535ab169ee	0d109eb3-57de-4f52-a603-865e868b203f	Bề mặt mắt mèo	\N	2	Pending	\N	\N	f	1	\N	\N	\N	f	92	\N	\N	91	t	1
bce541c9-41cc-4037-bb23-ec3402fab54b	3f8f4c50-e19b-4ad5-ac4d-a7be48cf6c76	\N	Cắt da – chân\t	\N	1	Pending	\N	\N	t	20	\N	\N	\N	f	20	\N	\N	0	t	1
794c6230-805f-4fdd-a995-3861da6f094e	795b92f9-5a9e-4406-87bf-0cbdae37a462	\N	Dưỡng dầu / massage tay\t	\N	1	Completed	2026-08-19 16:18:55.574206	53fca09d-2b87-41ae-95ea-640953f66815	t	20	2026-08-19 16:18:55.574207	2026-08-19 16:18:53.472847	53fca09d-2b87-41ae-95ea-640953f66815	f	20	13:12:00	12:52:00	0	t	1
54f501bd-9d1b-43e3-8e7f-8935dac1bcc8	09ea6636-dbeb-4d51-aea8-28fa899e7c12	0d109eb3-57de-4f52-a603-865e868b203f	Bề mặt mắt mèo	\N	2	Pending	\N	\N	t	1	\N	\N	\N	f	65	10:10:00	09:05:00	64	f	1
cbc3f1bb-2c32-4f6a-ba06-825407c4eac7	09ea6636-dbeb-4d51-aea8-28fa899e7c12	ee1da04a-45f5-4b7f-8da8-66f8ef3c4240	Đắp bột	\N	1	Completed	2026-09-04 23:14:11.683615	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	t	1	2026-09-04 23:14:11.683616	2026-09-04 23:14:11.683618	\N	f	65	09:05:00	08:00:00	64	f	1
6649aaac-95f1-47ee-bf7c-48fc45cb4ed9	760124cf-1f65-4bf3-b48a-99296954c4c9	96d1b102-8b31-4f23-ac99-c0766ee7a059	Bể mặt matte	\N	1	Completed	2026-08-19 16:18:27.021934	53fca09d-2b87-41ae-95ea-640953f66815	f	47	2026-08-19 16:18:27.021935	2026-08-19 16:18:22.81147	53fca09d-2b87-41ae-95ea-640953f66815	f	47	09:47:00	09:00:00	0	f	1
c6a8b077-224f-4d2c-9dfb-e691cc975052	760124cf-1f65-4bf3-b48a-99296954c4c9	c77ce15e-e1c1-40f6-9a44-35a65fab2334	Sơn màu gradient	\N	2	Completed	2026-08-19 16:18:33.549809	53fca09d-2b87-41ae-95ea-640953f66815	t	1	2026-08-19 16:18:33.549809	2026-08-19 16:18:30.260197	53fca09d-2b87-41ae-95ea-640953f66815	f	47	10:34:00	09:47:00	46	f	1
26205968-f790-44c6-a18c-e74e8cfc8335	760124cf-1f65-4bf3-b48a-99296954c4c9	3ff78933-3817-4152-86f5-8ada4d33bf9f	Thêm sticker/icon	\N	3	Completed	2026-08-19 16:18:39.003098	53fca09d-2b87-41ae-95ea-640953f66815	t	1	2026-08-19 16:18:39.003098	2026-08-19 16:18:36.234197	53fca09d-2b87-41ae-95ea-640953f66815	f	47	11:21:00	10:34:00	46	f	1
9b36c128-3490-4b69-98f5-f9a0b20c49c1	760124cf-1f65-4bf3-b48a-99296954c4c9	82744afe-3799-4bc1-863b-72568f988644	Đính đá nhỏ	\N	4	Completed	2026-08-19 16:18:44.917786	53fca09d-2b87-41ae-95ea-640953f66815	t	1	2026-08-19 16:18:44.917786	2026-08-19 16:18:42.332952	53fca09d-2b87-41ae-95ea-640953f66815	f	46	12:07:00	11:21:00	45	f	1
3c969b4e-55a2-44b6-a0bb-5591aeee788f	fcaf0ea5-8550-4f76-be1f-2b4289f43e36	e45527a6-cea6-465a-a1a9-d86166a0fe07	Bề mặt chrome	\N	1	Pending	\N	\N	t	0	\N	\N	\N	f	5	11:35:00	11:30:00	0	f	0
75d57291-31a8-4919-a524-45d6771ca556	b5bbdb78-63d2-41a6-9189-b1e29e6dd0fd	ee1da04a-45f5-4b7f-8da8-66f8ef3c4240	Đắp bột	\N	2	Pending	\N	\N	t	0	\N	\N	\N	f	5	10:10:00	10:05:00	0	f	0
c8f086af-af4e-40d7-b8d6-729b6a60fb42	760124cf-1f65-4bf3-b48a-99296954c4c9	\N	Tạo dáng & làm móng: Sơn trực tiếp	\N	5	Completed	2026-08-19 16:18:50.643263	53fca09d-2b87-41ae-95ea-640953f66815	t	45	2026-08-19 16:18:50.643264	2026-08-19 16:18:48.1767	53fca09d-2b87-41ae-95ea-640953f66815	f	45	12:52:00	12:07:00	0	t	1
289dabf8-4b43-410d-bd8e-21e9d1d0d0e3	9f5bea01-7740-4dcb-99bf-021df7cab16f	c77ce15e-e1c1-40f6-9a44-35a65fab2334	Sơn màu gradient	\N	1	Pending	\N	\N	t	0	\N	\N	\N	f	5	10:05:00	10:00:00	0	f	0
33ff3832-c160-4b0b-be1d-31d96649bf36	9f5bea01-7740-4dcb-99bf-021df7cab16f	0d109eb3-57de-4f52-a603-865e868b203f	Bề mặt mắt mèo	\N	3	Pending	\N	\N	t	0	\N	\N	\N	f	5	10:15:00	10:10:00	0	f	0
5d1aac62-5c72-42e3-b71f-ec77f0837d06	9f5bea01-7740-4dcb-99bf-021df7cab16f	ee1da04a-45f5-4b7f-8da8-66f8ef3c4240	Đắp bột	\N	2	Pending	\N	\N	t	0	\N	\N	\N	f	5	10:10:00	10:05:00	0	f	0
89fae328-26b7-4c45-bd23-26bb6ea735ad	ff20d639-ba46-4260-8f26-4be02b7617a3	0d109eb3-57de-4f52-a603-865e868b203f	Bề mặt mắt mèo	\N	2	Pending	\N	\N	t	1	\N	\N	\N	f	65	10:10:00	09:05:00	64	f	1
ec55e8b4-3795-4c50-bd46-f182e6f59e63	ff20d639-ba46-4260-8f26-4be02b7617a3	ee1da04a-45f5-4b7f-8da8-66f8ef3c4240	Đắp bột	\N	1	Pending	\N	\N	t	1	\N	\N	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	f	65	09:05:00	08:00:00	64	f	1
de96a4c0-418f-4d1c-919d-ba8b6a3e16a7	7a390e0e-0026-4389-be4a-fc32bbed8b1d	\N	Ngâm chân thảo mộc	\N	1	Completed	2026-09-08 19:49:50.124895	53fca09d-2b87-41ae-95ea-640953f66815	t	20	2026-09-08 19:49:50.124897	2026-09-08 19:49:40.328618	53fca09d-2b87-41ae-95ea-640953f66815	f	20	20:02:28.426575	19:42:28.426575	0	t	1
fc41c640-e363-41c6-86bc-c282dc5ee4d1	7dfa4355-c6ed-40f6-8923-dd4851aac7a5	ee1da04a-45f5-4b7f-8da8-66f8ef3c4240	Đắp bột	\N	1	Completed	2026-09-08 19:50:26.857551	53fca09d-2b87-41ae-95ea-640953f66815	t	1	2026-09-08 19:50:26.857552	2026-09-08 19:49:58.715601	53fca09d-2b87-41ae-95ea-640953f66815	f	38	20:40:28.426575	20:02:28.426575	37	f	1
1ca91993-bd82-44dd-8491-74dfbce93178	7dfa4355-c6ed-40f6-8923-dd4851aac7a5	0d109eb3-57de-4f52-a603-865e868b203f	Bề mặt mắt mèo	\N	2	Completed	2026-09-08 19:55:36.057611	53fca09d-2b87-41ae-95ea-640953f66815	f	1	2026-09-08 19:55:36.057613	2026-09-08 19:50:38.552436	53fca09d-2b87-41ae-95ea-640953f66815	f	92	22:12:28.426575	20:40:28.426575	91	t	1
8deb6417-835f-4917-b258-acc007fb1bf3	f13dd3c1-e06f-4ecc-8bd1-e819e321466d	\N	Cắt da – chân\t	\N	1	Completed	2026-09-08 19:55:49.669224	53fca09d-2b87-41ae-95ea-640953f66815	t	20	2026-09-08 19:55:49.669227	2026-09-08 19:55:43.346777	53fca09d-2b87-41ae-95ea-640953f66815	f	20	22:32:28.426575	22:12:28.426575	0	t	1
c1ea4465-8057-486e-8f9b-1844456c9d43	e985e08d-27a6-44d5-8b51-b91458b899fc	\N	Ngâm chân thảo mộc	\N	3	Completed	2026-09-08 19:56:04.618517	53fca09d-2b87-41ae-95ea-640953f66815	t	20	2026-09-08 19:56:04.618518	2026-09-08 19:55:57.827181	53fca09d-2b87-41ae-95ea-640953f66815	f	20	\N	\N	0	t	1
8c34dcee-72aa-481e-98c8-174b22389b8d	b44f6a74-d00b-4fc0-a289-e2f7d5455d58	\N	Ngâm chân thảo mộc	\N	4	Completed	2026-09-08 20:15:37.161424	53fca09d-2b87-41ae-95ea-640953f66815	t	20	2026-09-08 20:15:37.161425	2026-09-08 20:14:26.679147	53fca09d-2b87-41ae-95ea-640953f66815	f	20	\N	\N	0	t	1
556b6af5-be0d-4b81-b6a7-ae4634bd6156	56dae948-2603-43b1-b019-6afc9e80358a	\N	Cắt da – tay\t	\N	5	Completed	2026-09-08 20:15:45.39612	53fca09d-2b87-41ae-95ea-640953f66815	t	20	2026-09-08 20:15:45.39612	2026-09-08 20:15:41.251398	53fca09d-2b87-41ae-95ea-640953f66815	f	20	\N	\N	0	t	1
2c8e81ea-78e5-44c9-ab52-81ae70bd6657	72ace6e1-d555-4c46-9f4c-a660e71ac2e4	\N	Dưỡng dầu / massage tay\t	\N	1	Pending	\N	\N	t	12	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	12	12:12:00	12:00:00	0	t	0
7573eee0-a7b4-44c8-9d16-3530fb50d8fb	b02b070c-f577-4474-bef0-b9d9efe5a83e	\N	Vệ sinh móng	\N	1	Pending	\N	\N	t	12	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	12	12:39:00	12:27:00	0	t	0
da1e62f7-be57-445d-9d73-0904ab1bd957	a28b0643-ced7-45e3-8f46-ddc045547e8c	0d109eb3-57de-4f52-a603-865e868b203f	Bề mặt mắt mèo	\N	3	Pending	\N	\N	t	0	\N	\N	\N	f	5	12:27:00	12:22:00	0	f	0
dad2009f-16d4-488f-8645-efab429df0b5	a28b0643-ced7-45e3-8f46-ddc045547e8c	c77ce15e-e1c1-40f6-9a44-35a65fab2334	Sơn màu gradient	\N	1	Pending	\N	\N	t	0	\N	\N	\N	f	5	12:17:00	12:12:00	0	f	0
e95c34b1-8b37-43a2-851a-7cf292f1153f	a28b0643-ced7-45e3-8f46-ddc045547e8c	ee1da04a-45f5-4b7f-8da8-66f8ef3c4240	Đắp bột	\N	2	Pending	\N	\N	t	0	\N	\N	\N	f	5	12:22:00	12:17:00	0	f	0
6cb96a73-7cf1-44cf-81df-4863e4715407	fcaf0ea5-8550-4f76-be1f-2b4289f43e36	da168e67-9448-4975-b3b2-542a4ff13a7a	Sơn màu đơn	\N	2	Pending	\N	\N	t	0	\N	\N	\N	f	5	11:40:00	11:35:00	0	f	0
8000f2b6-4082-48d2-8bc0-53bd95e5a0d4	b5bbdb78-63d2-41a6-9189-b1e29e6dd0fd	0d109eb3-57de-4f52-a603-865e868b203f	Bề mặt mắt mèo	\N	3	Pending	\N	\N	t	0	\N	\N	\N	f	5	10:15:00	10:10:00	0	f	0
cc08bf2a-6885-44c8-9dbb-0b03f809e0d4	b5bbdb78-63d2-41a6-9189-b1e29e6dd0fd	c77ce15e-e1c1-40f6-9a44-35a65fab2334	Sơn màu gradient	\N	1	Pending	\N	\N	t	0	\N	\N	\N	f	5	10:05:00	10:00:00	0	f	0
3464ac62-dc71-4bcd-8ed8-c38ad8124d68	4baf28be-871f-415d-b802-ddf0e8c27be4	c77ce15e-e1c1-40f6-9a44-35a65fab2334	Sơn màu gradient	\N	3	Pending	\N	\N	t	1	\N	\N	\N	f	35	\N	\N	34	f	1
3c492cb4-db9e-4950-9d69-c8f864a52b96	4baf28be-871f-415d-b802-ddf0e8c27be4	e45527a6-cea6-465a-a1a9-d86166a0fe07	Bề mặt chrome	\N	5	Pending	\N	\N	t	1	\N	\N	\N	f	35	\N	\N	34	f	1
6b3ff47f-2d71-406c-b301-8f8c731a910f	4baf28be-871f-415d-b802-ddf0e8c27be4	82744afe-3799-4bc1-863b-72568f988644	Đính đá nhỏ	\N	1	Pending	\N	\N	t	1	\N	\N	\N	f	35	\N	\N	34	f	1
8cc462e0-89a6-489e-91cd-81f4b1e1e136	4baf28be-871f-415d-b802-ddf0e8c27be4	\N	Tạo dáng & làm móng: Sơn trực tiếp	\N	6	Pending	\N	\N	t	45	\N	\N	\N	f	45	\N	\N	0	t	1
b95f84fd-82e9-4dba-a94b-809ed715b28d	4baf28be-871f-415d-b802-ddf0e8c27be4	8b09327d-7887-4a36-9df9-2967c9ebdd59	Vẽ nghệ thuật	\N	2	Pending	\N	\N	t	1	\N	\N	\N	f	35	\N	\N	34	f	1
ba609d1b-e0b4-491d-a894-d71ea08489c2	4baf28be-871f-415d-b802-ddf0e8c27be4	da168e67-9448-4975-b3b2-542a4ff13a7a	Sơn màu đơn	\N	4	Pending	\N	\N	t	1	\N	\N	\N	f	35	\N	\N	34	f	1
bd33b44e-2c45-4b06-9603-aeca73d5c430	d1c947e8-0e1c-4baa-b2d5-95d55102a779	ee1da04a-45f5-4b7f-8da8-66f8ef3c4240	Đắp bột	\N	1	Completed	2026-09-04 23:02:32.399641	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	t	1	2026-09-04 23:02:32.399643	2026-09-04 23:02:22.914241	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	f	65	14:05:00	13:00:00	64	f	1
0c8f3e3f-2f8f-4230-b6ad-371395a05aa0	d1c947e8-0e1c-4baa-b2d5-95d55102a779	0d109eb3-57de-4f52-a603-865e868b203f	Bề mặt mắt mèo	\N	2	Completed	2026-09-04 23:03:00.026126	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	t	1	2026-09-04 23:03:00.026127	2026-09-04 23:02:54.421716	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	f	65	15:10:00	14:05:00	64	f	1
7f0cbb1f-8a94-459e-ab71-d7e7bef8dcd0	80ac8485-82d1-4f20-a61f-d857284912d3	c77ce15e-e1c1-40f6-9a44-35a65fab2334	Sơn màu gradient	\N	1	Completed	2026-07-27 23:35:30.302037	53fca09d-2b87-41ae-95ea-640953f66815	t	0	2026-07-27 23:35:30.302038	2026-07-27 23:35:26.517647	53fca09d-2b87-41ae-95ea-640953f66815	f	5	12:05:00	12:00:00	0	f	1
ee962c61-6999-41ba-a830-2ad2b9657ecb	eee7b591-80ba-42f0-9fcf-f1ae94839e8a	\N	Vệ sinh móng	\N	1	Completed	2026-07-27 23:36:03.62834	53fca09d-2b87-41ae-95ea-640953f66815	t	12	2026-07-27 23:36:03.628341	2026-07-27 23:36:03.628342	53fca09d-2b87-41ae-95ea-640953f66815	f	12	12:27:00	12:15:00	0	t	1
8ce60b66-dd0e-4b47-9509-8eec88c914db	f35c1fd7-9bbb-422a-8f73-7b5d2a4ad80c	\N	Dưỡng dầu / massage tay\t	\N	1	Completed	2026-07-27 23:36:09.569448	53fca09d-2b87-41ae-95ea-640953f66815	t	12	2026-07-27 23:36:09.569449	2026-07-27 23:36:09.56945	53fca09d-2b87-41ae-95ea-640953f66815	f	12	12:39:00	12:27:00	0	t	1
460e2bcd-5c54-4833-9a27-0a7e0c56b0f6	80ac8485-82d1-4f20-a61f-d857284912d3	ee1da04a-45f5-4b7f-8da8-66f8ef3c4240	Đắp bột	\N	2	Completed	2026-07-27 23:36:39.076155	53fca09d-2b87-41ae-95ea-640953f66815	t	0	2026-07-27 23:36:39.076157	2026-07-27 23:36:36.691323	53fca09d-2b87-41ae-95ea-640953f66815	f	5	12:10:00	12:05:00	0	f	1
03627eae-39b2-4210-bd6c-ba91fb4f6bf6	80ac8485-82d1-4f20-a61f-d857284912d3	0d109eb3-57de-4f52-a603-865e868b203f	Bề mặt mắt mèo	\N	3	Completed	2026-07-27 23:37:02.960895	53fca09d-2b87-41ae-95ea-640953f66815	t	0	2026-07-27 23:37:02.960896	2026-07-27 23:37:00.286192	53fca09d-2b87-41ae-95ea-640953f66815	f	5	12:15:00	12:10:00	0	f	1
ea542f1f-2014-446c-b5ab-3659137eedcc	0ee211ed-3d85-4577-b9bb-ce2a3e0d452c	c77ce15e-e1c1-40f6-9a44-35a65fab2334	Sơn màu gradient	\N	1	Completed	2026-07-17 16:02:16.480968	53fca09d-2b87-41ae-95ea-640953f66815	t	0	2026-07-17 16:02:16.480968	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	5	08:17:00	08:12:00	0	f	0
14fc44dc-716a-4c9e-b3b7-7874db42b73a	0ee211ed-3d85-4577-b9bb-ce2a3e0d452c	ee1da04a-45f5-4b7f-8da8-66f8ef3c4240	Đắp bột	\N	2	Completed	2026-07-17 16:02:45.877547	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	t	0	2026-07-17 16:02:45.877548	\N	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	f	5	08:22:00	08:17:00	0	f	0
635f2210-0392-4609-bb5a-d599c892481e	0ee211ed-3d85-4577-b9bb-ce2a3e0d452c	0d109eb3-57de-4f52-a603-865e868b203f	Bề mặt mắt mèo	\N	3	Completed	2026-07-17 16:03:38.022156	53fca09d-2b87-41ae-95ea-640953f66815	t	0	2026-07-17 16:03:38.022157	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	5	08:27:00	08:22:00	0	f	0
289e6be1-0a2a-4086-96e1-608c2e0b8ec6	3e64b31e-9fad-4a30-89fe-3a6d24064ee7	\N	Vệ sinh móng	\N	1	Completed	2026-07-17 16:03:42.332334	53fca09d-2b87-41ae-95ea-640953f66815	t	12	2026-07-17 16:03:42.332335	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	12	08:39:00	08:27:00	0	t	0
779fc0d2-0f5b-4446-b7fe-6b4d0ed4662b	03cebc6a-5b70-4498-835e-5d17f6e0331d	\N	Dưỡng dầu / massage tay\t	\N	1	Completed	2026-07-17 16:03:46.691326	53fca09d-2b87-41ae-95ea-640953f66815	t	12	2026-07-17 16:03:46.691327	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	12	08:12:00	08:00:00	0	t	0
251632e6-2e1c-4da6-a9e3-eb4dff8b19e6	5f2a4b59-b769-4f79-9add-c0d730579ddc	\N	Dưỡng dầu / massage tay\t	\N	1	Skipped	\N	\N	t	12	\N	\N	b53808e3-7219-4c65-899c-197f204e5581	f	12	09:42:00	09:30:00	0	t	0
2fcf2acc-2fdc-452b-b852-6b66f21313c8	1761c3d3-9671-42ee-8a56-4a5d04516acf	ee1da04a-45f5-4b7f-8da8-66f8ef3c4240	Đắp bột	\N	2	Completed	2026-07-18 09:04:29.839618	53fca09d-2b87-41ae-95ea-640953f66815	t	0	2026-07-18 09:04:29.83962	2026-07-18 09:04:23.009011	53fca09d-2b87-41ae-95ea-640953f66815	f	5	15:40:00	15:35:00	0	f	0
77c2d616-2129-46b2-9e8b-ba3e24b7d94c	a09b0da1-f4fa-4b28-82ec-15eee5de99dd	\N	Dưỡng dầu / massage tay\t	\N	1	Completed	2026-07-18 11:05:49.299096	53fca09d-2b87-41ae-95ea-640953f66815	t	12	2026-07-18 11:05:49.299257	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	12	14:12:00	14:00:00	0	t	0
5e8e101c-059e-44da-9c3c-9c0e473a4fc6	733400cc-fd58-481b-a759-4af7589d6cdf	e45527a6-cea6-465a-a1a9-d86166a0fe07	Bề mặt chrome	\N	5	Pending	\N	\N	t	1	\N	\N	\N	f	35	\N	\N	34	f	1
4a20a76c-fec0-4cb3-8ad0-27d9d386a410	abd05d08-6a00-48da-8fd8-78e39b98fa0e	c77ce15e-e1c1-40f6-9a44-35a65fab2334	Sơn màu gradient	\N	1	Completed	2026-07-18 11:06:29.046479	53fca09d-2b87-41ae-95ea-640953f66815	t	0	2026-07-18 11:06:29.046479	2026-07-18 11:06:24.229849	53fca09d-2b87-41ae-95ea-640953f66815	f	5	14:17:00	14:12:00	0	f	0
6c65013d-1067-4fdf-89ed-803be9c6434f	733400cc-fd58-481b-a759-4af7589d6cdf	82744afe-3799-4bc1-863b-72568f988644	Đính đá nhỏ	\N	1	Pending	\N	\N	t	1	\N	\N	\N	f	35	\N	\N	34	f	1
07198527-2eda-48e9-98d4-3017b443366e	abd05d08-6a00-48da-8fd8-78e39b98fa0e	ee1da04a-45f5-4b7f-8da8-66f8ef3c4240	Đắp bột	\N	2	Completed	2026-07-18 11:09:43.664178	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	t	0	2026-07-18 11:09:43.664179	2026-07-18 11:09:33.702267	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	f	5	14:22:00	14:17:00	0	f	0
6f68febb-9756-417a-8b54-b4b2fc3dc223	733400cc-fd58-481b-a759-4af7589d6cdf	c77ce15e-e1c1-40f6-9a44-35a65fab2334	Sơn màu gradient	\N	3	Pending	\N	\N	t	1	\N	\N	\N	f	35	\N	\N	34	f	1
8248baac-4f24-482e-b69c-154de43e3cd0	abd05d08-6a00-48da-8fd8-78e39b98fa0e	0d109eb3-57de-4f52-a603-865e868b203f	Bề mặt mắt mèo	\N	3	Completed	2026-07-18 11:11:07.094414	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	t	0	2026-07-18 11:11:07.094415	2026-07-18 11:11:01.210516	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	f	5	14:27:00	14:22:00	0	f	0
6632e9da-3520-4193-bb83-0ce48dc16200	bd3d27b3-71f0-4e51-b30c-fe88b8560ad7	\N	Vệ sinh móng	\N	1	Completed	2026-07-18 11:12:09.031997	53fca09d-2b87-41ae-95ea-640953f66815	t	12	2026-07-18 11:12:09.031998	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	12	14:39:00	14:27:00	0	t	0
b6d0772a-dc7c-417b-a87c-83800b3d8895	733400cc-fd58-481b-a759-4af7589d6cdf	8b09327d-7887-4a36-9df9-2967c9ebdd59	Vẽ nghệ thuật	\N	2	Pending	\N	\N	t	1	\N	\N	\N	f	35	\N	\N	34	f	1
be4c0847-a2de-43ee-91f3-29f23c2a0810	1761c3d3-9671-42ee-8a56-4a5d04516acf	c77ce15e-e1c1-40f6-9a44-35a65fab2334	Sơn màu gradient	\N	1	Completed	2026-07-18 09:04:15.936649	53fca09d-2b87-41ae-95ea-640953f66815	t	0	2026-07-18 09:04:15.936651	2026-07-18 09:04:10.627922	53fca09d-2b87-41ae-95ea-640953f66815	f	5	15:35:00	15:30:00	0	f	0
6bd30bf3-f82b-4e49-a5e6-da916939fc50	eeadb801-5d36-4387-9238-04638694825b	c77ce15e-e1c1-40f6-9a44-35a65fab2334	Sơn màu gradient	\N	1	Pending	\N	\N	t	0	\N	\N	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	f	5	11:05:00	11:00:00	0	f	1
3419a296-35a7-44e9-92cc-59d1ec982d4c	1761c3d3-9671-42ee-8a56-4a5d04516acf	0d109eb3-57de-4f52-a603-865e868b203f	Bề mặt mắt mèo	\N	3	Completed	2026-07-18 09:04:45.176659	53fca09d-2b87-41ae-95ea-640953f66815	t	0	2026-07-18 09:04:45.17666	2026-07-18 09:04:43.261925	53fca09d-2b87-41ae-95ea-640953f66815	f	5	15:45:00	15:40:00	0	f	0
ff04c743-5d7e-4613-8bcc-f44795e9821c	6e728820-df91-4aa2-a1e7-ce54ee0733dc	\N	Vệ sinh móng	\N	1	Completed	2026-07-18 10:00:35.66606	53fca09d-2b87-41ae-95ea-640953f66815	t	12	2026-07-18 10:00:35.666092	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	12	15:57:00	15:45:00	0	t	0
c1efdf05-33d0-484c-a39b-21b47de26e26	70eefc0a-8b0d-4ff5-a243-702200db9958	\N	Dưỡng dầu / massage tay\t	\N	1	Completed	2026-07-18 10:00:39.587124	53fca09d-2b87-41ae-95ea-640953f66815	t	12	2026-07-18 10:00:39.587125	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	12	16:09:00	15:57:00	0	t	0
e024a051-5f7f-4e5f-8293-0951024dfb3a	733400cc-fd58-481b-a759-4af7589d6cdf	da168e67-9448-4975-b3b2-542a4ff13a7a	Sơn màu đơn	\N	4	Pending	\N	\N	t	1	\N	\N	\N	f	35	\N	\N	34	f	1
e8391ac1-cbf9-4877-b320-67b64f348e64	733400cc-fd58-481b-a759-4af7589d6cdf	\N	Tạo dáng & làm móng: Sơn trực tiếp	\N	6	Pending	\N	\N	t	45	\N	\N	\N	f	45	\N	\N	0	t	1
8250c718-d5da-4e68-99bd-4bda6ad23c5a	455cddcb-643b-470d-92d6-9534e93af5af	\N	Dưỡng dầu / massage tay\t	\N	3	Pending	\N	\N	t	20	\N	\N	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	f	20	\N	\N	0	t	1
0243c583-b0e7-49a6-924b-747753755904	8ac3de52-3a62-4655-aaea-ff7d22d11d92	\N	Dưỡng dầu / massage tay\t	\N	1	Pending	\N	\N	t	12	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	12	17:54:00	17:42:00	0	t	0
1f66a7f9-fef5-44b2-a702-489a503946c5	31ced356-0b85-4a69-b0d4-86fb45612b97	\N	Vệ sinh móng	\N	1	Pending	\N	\N	t	12	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	12	17:42:00	17:30:00	0	t	0
474b8286-4f2f-4ed0-95ee-60faa11117df	1bc017f2-53e3-4447-9d65-073a5904dd0e	ee1da04a-45f5-4b7f-8da8-66f8ef3c4240	Đắp bột	\N	2	Pending	\N	\N	t	0	\N	\N	\N	f	5	17:25:00	17:20:00	0	f	0
953385dd-0f3f-4814-9c79-e3976abbcf58	1bc017f2-53e3-4447-9d65-073a5904dd0e	c77ce15e-e1c1-40f6-9a44-35a65fab2334	Sơn màu gradient	\N	1	Pending	\N	\N	t	0	\N	\N	\N	f	5	17:20:00	17:15:00	0	f	0
9d13871c-a860-44c9-91b1-fd93277ee61b	1bc017f2-53e3-4447-9d65-073a5904dd0e	0d109eb3-57de-4f52-a603-865e868b203f	Bề mặt mắt mèo	\N	3	Pending	\N	\N	t	0	\N	\N	\N	f	5	17:30:00	17:25:00	0	f	0
799adb00-7deb-4c7e-b0ef-24f47a616ac5	431bdbba-7743-487d-bd4d-836f5b4e6ef2	\N	Vệ sinh móng	\N	1	Pending	\N	\N	t	12	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	12	12:12:00	12:00:00	0	t	0
a374ed01-4324-4c1e-84b6-0eacef0ebf19	60a8f4ae-851e-4e53-a6f8-e1a4cc8ecd0e	\N	Dưỡng dầu / massage tay\t	\N	1	Pending	\N	\N	t	12	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	12	12:24:00	12:12:00	0	t	0
cbeb68d6-e0be-463c-b114-b03b6296f488	701293a1-2e71-45d2-8104-69055d5e372f	\N	Pháo hoa 3 màu • Almond • Chrome	\N	1	Pending	\N	\N	t	120	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	120	14:24:00	12:24:00	0	t	0
d19eb230-28a0-48f3-9d95-a9a51dd67c78	0a7714a2-33c2-4471-b924-32efed6f195c	\N	Dưỡng dầu / massage tay\t	\N	1	Pending	\N	\N	t	12	\N	\N	\N	f	12	\N	\N	0	t	0
802a0969-bd78-4c03-9cf3-30d34e976a34	7eac41a2-f0f7-48d1-a942-3d3ffda2fa63	96d1b102-8b31-4f23-ac99-c0766ee7a059	Bể mặt matte	\N	1	Pending	\N	\N	f	5	\N	\N	\N	f	0	\N	\N	0	f	0
f13e8c05-efcf-4db3-9216-96645e63dd40	7eac41a2-f0f7-48d1-a942-3d3ffda2fa63	da168e67-9448-4975-b3b2-542a4ff13a7a	Sơn màu đơn	\N	2	Pending	\N	\N	t	0	\N	\N	\N	f	5	\N	\N	0	f	0
1c3b86da-ba33-44d8-a2e0-e28d23b9109a	bf310edd-4efc-4158-a4c3-e0b473b38d9c	da168e67-9448-4975-b3b2-542a4ff13a7a	Sơn màu đơn	\N	2	Pending	\N	\N	t	0	\N	\N	\N	f	5	\N	\N	0	f	0
a7141204-0c60-4949-8a1a-c3705ad46609	bf310edd-4efc-4158-a4c3-e0b473b38d9c	96d1b102-8b31-4f23-ac99-c0766ee7a059	Bể mặt matte	\N	1	Pending	\N	\N	f	5	\N	\N	\N	f	0	\N	\N	0	f	0
2f496876-607c-482a-ba79-2c10583387e9	d01fe84d-bfd1-48c5-84df-5db973c195cc	da168e67-9448-4975-b3b2-542a4ff13a7a	Sơn màu đơn	\N	2	Pending	\N	\N	t	0	\N	\N	\N	f	5	\N	\N	0	f	0
4f308419-7e0b-4ce6-971f-8a1ed0e593d1	689f5e44-93eb-4209-be61-46cf03f07869	da168e67-9448-4975-b3b2-542a4ff13a7a	Sơn màu đơn	\N	2	Pending	\N	\N	t	0	\N	\N	\N	f	5	\N	\N	0	f	0
786de2f1-46aa-47a8-ba53-a236a7e1fb18	689f5e44-93eb-4209-be61-46cf03f07869	96d1b102-8b31-4f23-ac99-c0766ee7a059	Bể mặt matte	\N	1	Pending	\N	\N	f	5	\N	\N	\N	f	0	\N	\N	0	f	0
1ee2bb44-f443-4089-9da3-c888d720f402	e72722e2-a3ca-4196-9ba3-db94a6e03fb5	96d1b102-8b31-4f23-ac99-c0766ee7a059	Bể mặt matte	\N	1	Pending	\N	\N	f	5	\N	\N	\N	f	0	\N	\N	0	f	0
760b4e89-d8f6-48c8-be94-afade503cb40	e72722e2-a3ca-4196-9ba3-db94a6e03fb5	da168e67-9448-4975-b3b2-542a4ff13a7a	Sơn màu đơn	\N	2	Pending	\N	\N	t	0	\N	\N	\N	f	5	\N	\N	0	f	0
1988d529-cf08-4b7e-bb95-45ebece3816d	b06e4066-5f6e-4e1c-b88c-94b5480a7bab	96d1b102-8b31-4f23-ac99-c0766ee7a059	Bể mặt matte	\N	1	Pending	\N	\N	f	5	\N	\N	\N	f	0	\N	\N	0	f	0
ce69b1c9-1ccc-4e23-bfbb-93db64892840	b06e4066-5f6e-4e1c-b88c-94b5480a7bab	da168e67-9448-4975-b3b2-542a4ff13a7a	Sơn màu đơn	\N	2	Pending	\N	\N	t	0	\N	\N	\N	f	5	\N	\N	0	f	0
4d83b518-3018-4621-bae3-4fbfa41fb298	d2f26f62-8de8-40b7-b4d7-e778bd4d2f35	da168e67-9448-4975-b3b2-542a4ff13a7a	Sơn màu đơn	\N	2	Pending	\N	\N	t	0	\N	\N	\N	f	5	\N	\N	0	f	0
5b7f5ad3-fff0-4246-96f0-cbbeff5c65d4	d2f26f62-8de8-40b7-b4d7-e778bd4d2f35	96d1b102-8b31-4f23-ac99-c0766ee7a059	Bể mặt matte	\N	1	Pending	\N	\N	f	5	\N	\N	\N	f	0	\N	\N	0	f	0
e6f2d46b-c55e-432f-b632-661570db12b2	a7bcbd77-a3f3-4717-b13b-ea9b6c1c157b	\N	Dưỡng dầu / massage tay\t	\N	1	Pending	\N	\N	t	20	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	20	22:35:00	22:15:00	0	t	1
db0d66ec-92e9-4f7c-ae87-66fd28cbed60	eeadb801-5d36-4387-9238-04638694825b	ee1da04a-45f5-4b7f-8da8-66f8ef3c4240	Đắp bột	\N	2	Pending	\N	\N	t	0	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	5	11:10:00	11:05:00	0	f	1
fe4ac733-83d9-4b15-a87a-37b153c26b1f	eeadb801-5d36-4387-9238-04638694825b	0d109eb3-57de-4f52-a603-865e868b203f	Bề mặt mắt mèo	\N	3	Pending	\N	\N	t	0	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	5	11:15:00	11:10:00	0	f	1
15fe9f36-6f33-4aa0-a8e9-43a6d34af33e	b296ff00-e176-40a7-9c93-c1723356dbe7	3ff78933-3817-4152-86f5-8ada4d33bf9f	Thêm sticker/icon	\N	3	Pending	\N	\N	t	1	\N	\N	\N	f	47	11:21:00	10:34:00	46	f	1
6e7893fc-1008-4f36-bc49-bc398967a463	b296ff00-e176-40a7-9c93-c1723356dbe7	96d1b102-8b31-4f23-ac99-c0766ee7a059	Bể mặt matte	\N	1	Pending	\N	\N	f	47	\N	\N	\N	f	47	09:47:00	09:00:00	0	f	1
7e2cbeeb-5fc4-4740-beaa-9b7ece388db3	b296ff00-e176-40a7-9c93-c1723356dbe7	c77ce15e-e1c1-40f6-9a44-35a65fab2334	Sơn màu gradient	\N	2	Pending	\N	\N	t	1	\N	\N	\N	f	47	10:34:00	09:47:00	46	f	1
c011f6c9-cf4c-445c-8843-348199360eac	b296ff00-e176-40a7-9c93-c1723356dbe7	\N	Tạo dáng & làm móng: Sơn trực tiếp	\N	5	Pending	\N	\N	t	45	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	45	12:52:00	12:07:00	0	t	1
df490c4b-0d49-4187-8a94-1475efda60c6	b296ff00-e176-40a7-9c93-c1723356dbe7	82744afe-3799-4bc1-863b-72568f988644	Đính đá nhỏ	\N	4	Pending	\N	\N	t	1	\N	\N	\N	f	46	12:07:00	11:21:00	45	f	1
3c3f584e-87cf-4db2-93ee-17a31396a821	b6d25fff-c462-4459-b3bd-1699524ef323	\N	Dưỡng dầu / massage tay\t	\N	1	Pending	\N	\N	t	12	\N	\N	dba37f5a-970c-41da-be69-438732e98402	f	12	08:12:00	08:00:00	0	t	1
bf575264-746e-4d49-b4a9-489353759f51	1f5a84af-5e0d-4cb8-8559-b8cdd2a36cb4	\N	Cắt da – tay\t	\N	1	Completed	2026-07-27 23:20:58.875042	53fca09d-2b87-41ae-95ea-640953f66815	t	10	2026-07-27 23:20:58.875144	2026-07-27 23:20:58.875224	53fca09d-2b87-41ae-95ea-640953f66815	f	10	18:40:00	18:30:00	0	t	1
1612ec05-9c06-4f20-8de0-b3439e556a63	469b365a-620a-4bad-bcc6-1b122fd20840	82744afe-3799-4bc1-863b-72568f988644	Đính đá nhỏ	\N	4	Pending	\N	\N	t	1	\N	\N	\N	f	46	16:07:00	15:21:00	45	f	1
1889177e-134d-4034-96ef-6284a20e743f	469b365a-620a-4bad-bcc6-1b122fd20840	3ff78933-3817-4152-86f5-8ada4d33bf9f	Thêm sticker/icon	\N	3	Pending	\N	\N	t	1	\N	\N	\N	f	47	15:21:00	14:34:00	46	f	1
253188f1-9f8f-4f23-ba6f-6be61c33a37e	469b365a-620a-4bad-bcc6-1b122fd20840	c77ce15e-e1c1-40f6-9a44-35a65fab2334	Sơn màu gradient	\N	2	Pending	\N	\N	t	1	\N	\N	\N	f	47	14:34:00	13:47:00	46	f	1
be97adb1-ef57-4ee4-b000-79e800a7ee41	469b365a-620a-4bad-bcc6-1b122fd20840	\N	Tạo dáng & làm móng: Sơn trực tiếp	\N	5	Pending	\N	\N	t	45	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	45	16:52:00	16:07:00	0	t	1
cbb70ff5-4b44-4870-9d38-2c3187d4aa2c	469b365a-620a-4bad-bcc6-1b122fd20840	96d1b102-8b31-4f23-ac99-c0766ee7a059	Bể mặt matte	\N	1	Pending	\N	\N	f	47	\N	\N	\N	f	47	13:47:00	13:00:00	0	f	1
04852499-1b10-491b-85b8-cef76a903f08	aa1e3d43-c073-4faa-93d6-6c617723edf7	c77ce15e-e1c1-40f6-9a44-35a65fab2334	Sơn màu gradient	\N	1	Pending	\N	\N	t	0	\N	\N	\N	f	5	12:29:00	12:24:00	0	f	1
5181719c-5ddd-41b3-a7fa-f60a8fd980a5	06247c34-d09b-408b-9926-d250dfbc04be	\N	Vệ sinh móng	\N	1	Pending	\N	\N	t	12	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	12	12:12:00	12:00:00	0	t	1
8521afee-b93b-4c89-b4f1-69df3ee78c8c	aa1e3d43-c073-4faa-93d6-6c617723edf7	ee1da04a-45f5-4b7f-8da8-66f8ef3c4240	Đắp bột	\N	2	Pending	\N	\N	t	0	\N	\N	\N	f	5	12:34:00	12:29:00	0	f	1
9a8a941c-325b-439c-b90b-f2d330a3de01	691a146e-62fa-4b45-ade9-04049fbb77b3	\N	Dưỡng dầu / massage tay\t	\N	1	Pending	\N	\N	t	12	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	12	12:24:00	12:12:00	0	t	1
caa185c7-c0f2-4910-82de-b4c97f6907a2	aa1e3d43-c073-4faa-93d6-6c617723edf7	0d109eb3-57de-4f52-a603-865e868b203f	Bề mặt mắt mèo	\N	3	Pending	\N	\N	t	0	\N	\N	\N	f	5	12:39:00	12:34:00	0	f	1
a8d06755-9038-482c-9e84-c2682ff451f6	76804696-618e-4508-be0a-f705969d6561	\N	Vệ sinh móng	\N	1	Pending	\N	\N	t	12	\N	\N	b53808e3-7219-4c65-899c-197f204e5581	f	12	13:57:00	13:45:00	0	t	1
8fb874cc-51c7-4c9c-9d10-0388fc714fcc	5a9232ff-2139-4799-bd04-49855f0fde4f	\N	Vệ sinh móng	\N	1	Pending	\N	\N	t	12	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	12	10:24:00	10:12:00	0	t	1
c55d3355-d04e-4a39-b0f0-f0d5b7e31c17	1b7493f6-c1d5-401c-b733-ab6ee756cec5	\N	Dưỡng dầu / massage tay\t	\N	1	Pending	\N	\N	t	12	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	12	10:12:00	10:00:00	0	t	1
d1188965-d8dc-4b96-9b6f-b0ec973dd82c	6be32485-f5f6-4d80-8354-5f034e285d18	\N	Dưỡng dầu / massage tay\t	\N	1	Pending	\N	\N	t	12	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	12	10:36:00	10:24:00	0	t	1
139b86d4-8443-44ad-b8a5-713d71c0296a	863d6747-1d38-47ee-82b6-f9e91f7889db	3ff78933-3817-4152-86f5-8ada4d33bf9f	Thêm sticker/icon	\N	3	Pending	\N	\N	t	1	\N	\N	\N	f	47	11:21:00	10:34:00	46	f	1
49e121ad-371a-472d-93ac-acfd9d7a3765	863d6747-1d38-47ee-82b6-f9e91f7889db	\N	Tạo dáng & làm móng: Sơn trực tiếp	\N	5	Pending	\N	\N	t	45	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	45	12:52:00	12:07:00	0	t	1
6fb1c478-4676-45c6-b3b7-8d024db69755	863d6747-1d38-47ee-82b6-f9e91f7889db	c77ce15e-e1c1-40f6-9a44-35a65fab2334	Sơn màu gradient	\N	2	Pending	\N	\N	t	1	\N	\N	\N	f	47	10:34:00	09:47:00	46	f	1
e568b531-5af4-424d-b521-ee3a250ed633	863d6747-1d38-47ee-82b6-f9e91f7889db	96d1b102-8b31-4f23-ac99-c0766ee7a059	Bể mặt matte	\N	1	Pending	\N	\N	f	47	\N	\N	\N	f	47	09:47:00	09:00:00	0	f	1
fad3f2f5-c24c-4201-b2c3-7aaf2db0184d	863d6747-1d38-47ee-82b6-f9e91f7889db	82744afe-3799-4bc1-863b-72568f988644	Đính đá nhỏ	\N	4	Pending	\N	\N	t	1	\N	\N	\N	f	46	12:07:00	11:21:00	45	f	1
182bfd62-be13-45e5-ad94-e903a5aa5a39	db6c20af-30f7-45ad-8e67-575dbb34052d	da168e67-9448-4975-b3b2-542a4ff13a7a	Sơn màu đơn	\N	3	Pending	\N	\N	t	1	\N	\N	\N	f	44	\N	\N	43	f	1
28723fd0-3532-487c-9b7c-582e03125312	db6c20af-30f7-45ad-8e67-575dbb34052d	8b09327d-7887-4a36-9df9-2967c9ebdd59	Vẽ nghệ thuật	\N	2	Pending	\N	\N	t	1	\N	\N	\N	f	44	\N	\N	43	f	1
5b43ec5f-f039-4e9d-8c8b-abb7f89ed21d	db6c20af-30f7-45ad-8e67-575dbb34052d	82744afe-3799-4bc1-863b-72568f988644	Đính đá nhỏ	\N	1	Pending	\N	\N	t	1	\N	\N	\N	f	44	\N	\N	43	f	1
44b625c6-4bcf-43f0-8783-fbed9fb4fde2	d46d4acf-db4d-4796-b5b7-a926449613c9	82744afe-3799-4bc1-863b-72568f988644	Đính đá nhỏ	\N	4	Pending	\N	\N	t	0	\N	\N	\N	f	5	08:50:00	08:45:00	0	f	1
66ed78e8-7136-4d2c-bd44-85f30e9f11fe	d46d4acf-db4d-4796-b5b7-a926449613c9	3ff78933-3817-4152-86f5-8ada4d33bf9f	HieuNT	\N	3	Pending	\N	\N	t	0	\N	\N	\N	f	5	08:45:00	08:40:00	0	f	1
69584a43-c128-44a1-827f-fa4060075a78	db6c20af-30f7-45ad-8e67-575dbb34052d	e45527a6-cea6-465a-a1a9-d86166a0fe07	Bề mặt chrome	\N	4	Pending	\N	\N	t	1	\N	\N	\N	f	43	\N	\N	42	f	1
bf82ad87-d9e3-4f7a-8925-f81c19ef919e	db6c20af-30f7-45ad-8e67-575dbb34052d	\N	Tạo dáng & làm móng: Sơn thường	\N	5	Pending	\N	\N	t	45	\N	\N	\N	f	45	\N	\N	0	t	1
a22e8228-bc7f-4876-a9ba-03727b6895fb	d46d4acf-db4d-4796-b5b7-a926449613c9	96d1b102-8b31-4f23-ac99-c0766ee7a059	Bể mặt matte	\N	1	Pending	\N	\N	f	5	\N	\N	\N	f	0	08:35:00	08:35:00	0	f	1
a857aa4d-f789-4b26-973c-8d16dbaa5c72	292fac63-ffbe-47fc-ba51-8fc0b63e017b	\N	Ngâm chân thảo mộc	\N	1	Pending	\N	\N	t	20	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	20	08:35:00	08:15:00	0	t	1
7e0c09f3-6639-479c-8d16-8e41b91c11c9	87e4d9ba-c7fe-4cea-8fe8-1255a6a89ccf	96d1b102-8b31-4f23-ac99-c0766ee7a059	Bể mặt matte	\N	1	Pending	\N	\N	f	5	\N	\N	\N	f	0	09:55:00	09:55:00	0	f	1
80d8119a-885b-4ead-821f-b1e44c9b0319	62f27620-b3d3-4cb6-904e-b1eb65966e64	\N	Ngâm chân thảo mộc	\N	1	Pending	\N	\N	t	20	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	20	09:55:00	09:35:00	0	t	1
a4ea0f7a-eab3-48b3-8a98-6474ed6c598c	87e4d9ba-c7fe-4cea-8fe8-1255a6a89ccf	3ff78933-3817-4152-86f5-8ada4d33bf9f	HieuNT	\N	3	Pending	\N	\N	t	0	\N	\N	\N	f	5	10:05:00	10:00:00	0	f	1
afe3e3b9-8e85-4000-a3c6-287297cfa7c4	87e4d9ba-c7fe-4cea-8fe8-1255a6a89ccf	c77ce15e-e1c1-40f6-9a44-35a65fab2334	Sơn màu gradient	\N	2	Pending	\N	\N	t	0	\N	\N	\N	f	5	10:00:00	09:55:00	0	f	1
220cdf52-da6d-497e-9044-fe55587ae3ce	87e4d9ba-c7fe-4cea-8fe8-1255a6a89ccf	82744afe-3799-4bc1-863b-72568f988644	Đính đá nhỏ	\N	4	Pending	\N	\N	t	0	\N	\N	\N	f	5	10:10:00	10:05:00	0	f	1
ff185e36-503d-445f-b89f-98260aaaba14	5e848861-5661-4f83-bbf3-c7cc13609989	\N	Dưỡng dầu / massage tay\t	\N	1	Pending	\N	\N	t	20	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	20	09:35:00	09:15:00	0	t	1
1de502a9-98bc-4027-a3f9-9e00c52c6611	d46d4acf-db4d-4796-b5b7-a926449613c9	c77ce15e-e1c1-40f6-9a44-35a65fab2334	Sơn màu gradient	\N	2	Pending	\N	\N	t	0	\N	\N	\N	f	5	08:40:00	08:35:00	0	f	1
cbbca8ff-f034-4226-9285-e0f521814c98	e13d7d32-de16-4db6-b99f-33729e60c00e	\N	Dưỡng dầu / massage tay\t	\N	1	Pending	\N	\N	t	20	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	20	09:10:00	08:50:00	0	t	1
aaea6e9d-f09f-4837-ac3c-2fa73d24e27d	9101f539-4b16-4b66-9856-73352e3f491f	82744afe-3799-4bc1-863b-72568f988644	Đính đá nhỏ	\N	1	Pending	\N	\N	t	1	\N	\N	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	f	58	09:28:00	08:30:00	57	f	1
f424d5e3-a3c4-41e9-a828-f2660bc3e0c7	9101f539-4b16-4b66-9856-73352e3f491f	da168e67-9448-4975-b3b2-542a4ff13a7a	Sơn màu đơn	\N	2	Pending	\N	\N	t	1	\N	\N	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	f	58	10:26:00	09:28:00	57	f	1
333807d6-5ba4-4672-9cbf-4564b9fb14ce	9101f539-4b16-4b66-9856-73352e3f491f	e45527a6-cea6-465a-a1a9-d86166a0fe07	Bề mặt chrome	\N	3	Pending	\N	\N	t	1	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	59	11:25:00	10:26:00	58	f	1
0bf41189-92e7-48e7-9c30-b36117a20bda	1eaf7dfe-6e6e-49b9-94b5-7f6c5dfefff7	ee1da04a-45f5-4b7f-8da8-66f8ef3c4240	Đắp bột	\N	2	Pending	\N	\N	t	0	\N	\N	\N	f	5	16:10:00	16:05:00	0	f	1
e1e4db7a-2bc8-403a-8ab2-993c0d920e00	1eaf7dfe-6e6e-49b9-94b5-7f6c5dfefff7	c77ce15e-e1c1-40f6-9a44-35a65fab2334	Sơn màu gradient	\N	1	Pending	\N	\N	t	0	\N	\N	\N	f	5	16:05:00	16:00:00	0	f	1
f27fdbba-899c-454e-867c-9d17912c2857	1eaf7dfe-6e6e-49b9-94b5-7f6c5dfefff7	0d109eb3-57de-4f52-a603-865e868b203f	ThanhDT	\N	3	Pending	\N	\N	t	0	\N	\N	\N	f	5	16:15:00	16:10:00	0	f	1
df4e9c6e-8481-45d8-b63f-0cb70c782a2c	9101f539-4b16-4b66-9856-73352e3f491f	\N	Tạo dáng & làm móng: Sơn thường	\N	4	Pending	\N	\N	t	45	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	f	45	12:10:00	11:25:00	0	t	1
\.


--
-- Data for Name: BookingRatings; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."BookingRatings" ("BookingRatingId", "BookingId", "CustomerId", "OverallScore", "Comment", "ImageUrl", "ServiceQuality", "Punctuality", "Cleanliness", "IsUpdated", "Status", "CreatedAt", "UpdatedAt", "DeletedAt") FROM stdin;
37289458-efe0-422b-b40a-ebe9c8a27d8f	571bda98-a035-4b14-9d2a-6e6a6bb2acf2	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	3	Trải nghiệm bình thường.	\N	4	2	2	f	Active	2026-09-02 00:00:00	\N	\N
be76b217-c17f-459d-8de3-75e41f845d1f	bd3b6417-fa2b-4ef4-8ab7-3e33c61efff0	0ddb8972-36cd-4b67-8887-829aadbdf942	2	Cần cải thiện.	\N	2	3	2	f	Active	2026-09-02 00:00:00	\N	\N
04471951-1271-4104-b89a-01e057f4e4b7	8833af4e-bb29-486b-9a45-7bd33f4241e1	0ddb8972-36cd-4b67-8887-829aadbdf942	3	Trải nghiệm bình thường.	\N	5	2	2	f	Active	2026-09-02 00:00:00	\N	\N
d48bdd15-96a2-4957-8817-f48be5407d69	dabb9a56-e346-4f3b-9a8d-7e4c175ff1fc	0ddb8972-36cd-4b67-8887-829aadbdf942	2	Cần cải thiện.	\N	2	3	1	f	Active	2026-09-02 00:00:00	\N	\N
642583d5-399d-4848-9008-2c0088fa2e18	4b713e5d-d0da-4c9d-81ee-216f510cde31	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	3	Trải nghiệm bình thường.	\N	1	4	3	f	Active	2026-09-02 00:00:00	\N	\N
134b33c4-c9ee-4141-819b-fae40566e418	84853280-d3b7-4150-901a-4dd60e63d745	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	3	Trải nghiệm bình thường.	\N	4	2	3	f	Active	2026-09-02 00:00:00	\N	\N
c76abf4a-e292-4047-b606-7dfdf186d49a	4e3f981e-f1f3-42d5-b67b-299f07b178c2	0ddb8972-36cd-4b67-8887-829aadbdf942	3	Trải nghiệm bình thường.	\N	2	4	3	f	Active	2026-09-02 00:00:00	\N	\N
7dd46c3d-8e9c-4f31-b497-f10e46f46938	ddc31886-f48e-4fcc-943c-d3cc9e14c8f5	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	3	Trải nghiệm bình thường.	\N	3	3	2	f	Active	2026-09-02 00:00:00	\N	\N
f54e9aa4-0d9b-4df4-b0e5-70319c217d90	931c5c48-eace-4ef1-a173-b791a22ca726	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	2	Cần cải thiện.	\N	1	2	4	f	Active	2026-09-02 00:00:00	\N	\N
2e196ff7-8e08-4199-95ee-06b80ebfd9d6	c1106f3d-8095-443e-b035-2df761fcac6d	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	3	Trải nghiệm bình thường.	\N	3	3	4	f	Active	2026-09-02 00:00:00	\N	\N
02296656-c669-4c11-90a7-cb7ff31f9c33	98a90c97-1cfd-4eb6-8c85-33d6ffbac9bf	0ddb8972-36cd-4b67-8887-829aadbdf942	3	Trải nghiệm bình thường.	\N	1	4	4	f	Active	2026-09-02 00:00:00	\N	\N
700de256-1a97-4dbf-9ac1-e7dcc3b1b63b	c2a91a31-1018-4a5e-a8e1-36e7249fecee	0ddb8972-36cd-4b67-8887-829aadbdf942	3	Trải nghiệm bình thường.	\N	4	3	3	f	Active	2026-09-02 00:00:00	\N	\N
cc751b4b-7c65-4bbe-bd05-31771a036d03	32060188-f6aa-42d5-b12c-60c30b579d58	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	3	Trải nghiệm bình thường.	\N	3	3	3	f	Active	2026-09-02 00:00:00	\N	\N
31f1b66c-192a-4f1d-88bf-3e7db557d766	6f816baa-48a9-477d-9700-db60482ab272	0ddb8972-36cd-4b67-8887-829aadbdf942	4	Chất lượng tốt, sẽ ủng hộ tiếp.	\N	5	2	4	f	Active	2026-09-02 00:00:00	\N	\N
e8fa220a-9fb9-4419-8156-48dcadb6e990	f4b92bcb-3365-4354-9902-f9c084922767	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	2	Cần cải thiện.	\N	2	1	3	f	Active	2026-09-02 00:00:00	\N	\N
9d9d8959-8924-4146-b5f7-f8579f1f9a26	c13ad8a8-e179-48c0-9907-af31451e0565	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	2	Cần cải thiện.	\N	1	3	1	f	Active	2026-09-02 00:00:00	\N	\N
e0b91a8f-7fc3-41cb-80e0-f80884ce1a52	0ecf1a61-9159-4bbd-9b2c-45a83bd42e4c	0ddb8972-36cd-4b67-8887-829aadbdf942	4	Chất lượng tốt, sẽ ủng hộ tiếp.	\N	2	4	5	f	Active	2026-09-02 00:00:00	\N	\N
25acc4dd-a71c-4ecf-b51b-3d71f8f744f9	2993e6d6-b957-436d-a0f2-b5d9782eeddd	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	2	Cần cải thiện.	\N	2	1	2	f	Active	2026-09-02 00:00:00	\N	\N
f64a078d-b1b1-4184-b8f7-f8531bcdd3a0	738529a7-3c13-4311-b5fc-2005f1aba573	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	3	Trải nghiệm bình thường.	\N	3	4	2	f	Active	2026-09-02 00:00:00	\N	\N
9fe677a3-c3ed-42d7-944e-63cd28ec1cd7	bd7f9cab-5cc5-4002-a0f2-9e40d8212d34	0ddb8972-36cd-4b67-8887-829aadbdf942	3	Trải nghiệm bình thường.	\N	3	3	4	f	Active	2026-09-02 00:00:00	\N	\N
819ab55d-823e-4a43-8e30-29556069a9e8	3a81ba1b-9a1c-48db-9094-4d318ba065e4	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	3	Trải nghiệm bình thường.	\N	2	3	3	f	Active	2026-09-02 00:00:00	\N	\N
58aa3151-96b0-4d0a-91d7-30506942a5d6	f4730e5d-340c-4121-bddf-923e4d400c6c	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	2	Cần cải thiện.	\N	3	1	1	f	Active	2026-09-02 00:00:00	\N	\N
f25ee110-af8e-4f5f-80f7-6ae6fccae381	70eb03a9-e158-4428-a196-e4d81eb232d3	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	2	Cần cải thiện.	\N	2	2	3	f	Active	2026-09-02 00:00:00	\N	\N
cb9b2f99-3595-4be2-badb-7f0271a5ae0f	5ed810a0-f0ee-4046-85ba-62183a3a49da	0ddb8972-36cd-4b67-8887-829aadbdf942	3	Trải nghiệm bình thường.	\N	2	4	2	f	Active	2026-09-02 00:00:00	\N	\N
\.


--
-- Data for Name: BookingWaitlists; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."BookingWaitlists" ("WailistId", "CustomerId", "SalonId", "PreferredNailArtistId", "RequesetedDate", "RequestedStartTime", "EstimatedDuration", "Position", "Status", "CreatedAt", "NotifiedAt", "ExpiresAt", "ConvertedBookingId") FROM stdin;
e99f358b-4327-46f9-959c-24e7798f1b1f	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-19 00:00:00	10:00:00	0	1	Cancelled	2026-08-19 00:13:58.300784	\N	\N	\N
\.


--
-- Data for Name: Bookings; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Bookings" ("BookingId", "CustomerId", "SalonId", "NailArtistId", "BookingDate", "StartTime", "TotalPrice", "Status", "Price", "TotalDuration", "UpdatedAt", "CheckInImageUrl", "CheckOutImagesUrl", "QRCode", "Discount", "IsRated", "ActualCheckInTime", "ActualStartTime", "IsLateArrival", "IsRefunded", "ChairId", "AmountDue", "AmountPaid", "ProposedBookingDate", "ProposedBy", "ProposedStartTime", "RescheduleReason", "WarrantyForBookingId") FROM stdin;
55923b8b-2f46-4698-ae94-10793ee8674e	0ddb8972-36cd-4b67-8887-829aadbdf942	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-07-14 02:34:45.682	12:00:00	94000.00	Cancelled	94000.00	46	2026-07-14 13:16:13.556483	\N	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADlklEQVR4nO3YQY7bMAwFUN3A97+lb+CihW1SlNyZLmql6OMiSCzxP2ZHuB0v196IRCKRSCQSiUQikUgkEolEIpFIJBKJRCKRSCQSicT/UWy1tp/Ptnya72352R2XDkoUkUgkEolE4kJxy93nz713fhF7DXkedEgmEolEIpFIXCQOXZdYpigL1NHXPIpIJBKJRCLx08R4NrtXli8ikUgkEonEf0MsWL7c7ndJ3bdZL5FIJBKJROIHiMMAhX3YovJbpW0eRSQSiUQikbhQLLX17B99jFFEIpFIJBKJy8R57f3roW6UcnrOvX2RRyQSiUQikfi+WILbgJUXRfFC6cSueeLeMAWRSCQSiUTi++KZ2d3I3NZnxozdz+GDSCQSiUQicbmYD699qqxSc6xcvuaOn0QikUgkEomrxbIJdQ2l9fFKwSKUSCQSiUQicbW4D8TQuvfBsXe1PMDwD4hEIpFIJBJXilElM592RGGHf3DUZ0QikUgkEolvi0PI9W04nWU+bGXRSyQSiUQikbhMjK5ZXK5xsrxZzfapbBCJRCKRSCQuEo97i4rFqMsM7Kuhtvvgt5sVkUgkEolE4l8W4+4QN04RV2bpeaijthGJRCKRSCS+LUZDDJC7Wt6ihilK73b/g9aKQSQSiUQikfi2mPefcYo8QCxVLdt5ntmgRCKRSCQSiYvEUgNxPctr07hKxYzz/0IkEolEIpH4vnh2Ret5o+WkOChDzdiWD4hEIpFIJBI/Qtzua/vdv+XWM3CrcdWeXCESiUQikUh8WyzpZZT5jvWNeVqfRyQSiUQikbhIzOmd89TfYbFZdfeIRCKRSCQS14rzG481LlVlsplNJBKJRCKRuEx8XKByelSkX0tVOOUZkUgkEolE4loxN4zfysoVwXnaLiUCiEQikUgkEleL+0106XdDIvK34xZLbX0UkUgkEolE4vvirKIxZxZi/CgjE4lEIpFIJK4VW60IucTYu/J7o3ZuW8O7pCslF5FIJBKJROIKsTsse1JknsERt7euyj4VMxKJRCKRSCSuFKNr2I72SWZ3OT8rKRuRSCQSiUTix4nXs2GzuqaYB0THQSQSiUQikfixYiHOuO8ROYVIJBKJRCJxpfg4QGaP/t6F5WljilJEIpFIJBKJK8RS4wADu08ux1ul6/LzZkUkEolEIpH4hvhOEYlEIpFIJBKJRCKRSCQSiUQikUgkEolEIpFIJBKJ/5H4A6oRNwMwjl5yAAAAAElFTkSuQmCC	\N	f	\N	\N	f	f	\N	94000.00	\N	\N	\N	\N	\N	\N
084a0474-6a0a-4cda-9043-7ab6511e15af	0ddb8972-36cd-4b67-8887-829aadbdf942	c2325bfa-edca-4803-92c1-9c2507f5b4a8	2adce07e-6ef8-4f7d-8920-accc288417e9	2026-07-12 16:16:41.37	10:00:00	70000.00	Cancelled	70000.00	22	2026-07-14 09:50:08.551869	\N	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADg0lEQVR4nO3aQY6kMAwF0Nwg979l3aBmpAbi2KGnVxVG/bJAQJz/XDsLVXt/eL0akUgkEolEIpFIJBKJRCKRSCQSiUQikUgkEolEIpH4G8WWV782+ldti3d/15z0tXtcbqKIRCKRSCQS94g9RkTxTD/uXuUxYv2bKCKRSCQSicRtYvtaPT5etW19eRdsFUUkEolEIpH4KDFlHnfteleLiUQikUgkEv8PsTz2WLxaRCKRSCQSiQ8V10enkDFAjbnriqvEm0gkEolEInG/mNZK/OmlRhGJRCKRSCRuE29XPHCcaqt3P4giEolEIpFI3CNO//6JI1K/2Ff5v3M52+JHpkVnRCKRSCQSiZ8W7ypyUpqsRivpbOyRSCQSiUQicacYg+tdEmPm6CxtTO+IRCKRSCQS94rzZiuXM240EGes1eq5MyKRSCQSicTPi6+rop6KI9LkrM9ODpFIJBKJROIDxHRX7KmBEVycNHIRiUQikUgkPkiMZaeddkcPx7vzbjgphUgkEolEInGbOJJWwYlN6dFpdyyRSCQSiUTiJnEVMlY8vxq++nxpiygikUgkEonETWLMTK2kuhaLyyjVYj9EIpFIJBKJe8Xj1Dlexbge2RJ3rpS+ekckEolEIpG4TbwlrgM3Pb6v9qYGiEQikUgkEh8gppBYVtnV3PWDaYtIJBKJRCJxjzg+Hk21CYuXfzZKJBKJRCKR+AhxZE5YmphK3ShJ3bbSGZFIJBKJROIOMX0PWk1RcTc1MPV4O2MRiUQikUgk7hC/SX/FkNjZdDZOYKmzHuqIRCKRSCQSPy22r9XXB8q7NFS1UlzyiEQikUgkEjeJ8Xw9dcxOrQxaZSBblYxFJBKJRCKRuElcTEL5fNpdd9Yv+51PEIlEIpFIJH5aPCrOAWqUjY3xGIt7fBdXX/4CIpFIJBKJxE+Lq1WOTk2NLlZzVykhEolEIpFI3CS2vPqRtI4bj9MsVtpLi0gkEolEInGH2ONWjOulZHW32kiNEolEIpFIJG4Ty4g0Hlu0x5ehsXs81vmMSCQSiUQi8XniCC4zVm0gOSmZSCQSiUQi8WHiGbfKvBVTo1EnEolEIpFI3CSuGhhDVRJXZ8dAln4BkUgkEolE4kYxrSnzOpBLRlOlrl99E4lEIpFIJO4UP7OIRCKRSCQSiUQikUgkEolEIpFIJBKJRCKRSCQSicRfJP4Bl07K4P9NGrMAAAAASUVORK5CYII=	\N	f	\N	\N	f	f	\N	70000.00	\N	\N	\N	\N	\N	\N
571bda98-a035-4b14-9d2a-6e6a6bb2acf2	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-07-26 12:34:21.593	12:00:00	160000.00	Completed	160000.00	57	2026-09-02 11:13:06.482316	\N	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADpElEQVR4nO3ZQW7jMAwFUN3A97+lb+BZVA4pSkmBAcYKME+LIJbJ/9gdkbbr4XM2IpFIJBKJRCKRSCQSiUQikUgkEolEIpFIJBKJRCLxfxRbPcdPRfpW6uLuFfdTvI4iEolEIpFI3CYeubu3HiMRdWcXp3mOdRSRSCQSiUTiXrFHz9hkv1m0pt6DSCQSiUQi8RvFHnLfxeOrfxigrFdEIpFIJBKJXy4W5zVC6p2WKiKRSCQSicRvE6fHYbPqTtzdL2KUD1FEIpFIJBKJ28Q2Jf31xxxFJBKJRCKRuE38cO7/gJUflPJkq/R1FJFIJBKJROLzYvxD7Myt/e7Ku1OUZHHGopdIJBKJRCJxm1iCMzuk58fizHtXGZ5IJBKJRCJxm5jPkPmbeE51UXKlQyQSiUQikfi8GItR/7gyEY89JO5aXqDefhCJRCKRSCTuFeNl+znH+La9gts4xTHaLQ9PJBKJRCKRuFu8+4OYFqjh23RiqJj2bOWvIhKJRCKRSNwkDqfsU9PydeQZP6xhHzcrIpFIJBKJxCfEaCiZ/WOYIn8rvbFP/bZZEYlEIpFIJP5zMTL7OaYX/bFNmdHxoY5IJBKJRCJxm1i6ImkYIKYYk+oA7+qIRCKRSCQSN4mldrU2lbuoO6aoyCMSiUQikUjcKB6v2qE1r01XLgksF89Reb0iEolEIpFIfF7srav+mCJ+HjrH9NWMw7ZFJBKJRCKRuFccl6AWccUJexLLZFd+SyQSiUQikbhDzBUtN0x3Z/6Ygs+pbfyriEQikUgkEp8Xo+HMIdO21UannNixWo4iEolEIpFI3Caud6K4O3tcnmK4y4MW+8NvVkQikUgkEonPiFPrke3+bQoZ0ofiYhOJRCKRSCRuE0tt3K1D5s0qp5fJiEQikUgkEjeJuau9s++6qeR4vRh2rF5HJBKJRCKRuFP8wK6SZna1fBGJRCKRSCR+i7geYOUMd2XHym+JRCKRSCQSt4utnjtpbKgv8uOwhpUBiEQikUgkEjeKw8vcWvapI483BtZdbEomEolEIpFI3CT2mONVVjam++0i6U4fzlUPkUgkEolE4neIq4/Vz0gTduR5xsmIRCKRSCQSv0NsfbPKzpX6W55iLu53F5FIJBKJROJucRpgRQzBcRfTloCPmxWRSCQSiUTiA2L7rTWnl8f2WqDOcZW6iEQikUgkEneLzxwikUgkEolEIpFIJBKJRCKRSCQSiUQikUgkEolEIvE/Ev8Aynb+uW8BCIQAAAAASUVORK5CYII=	\N	t	\N	\N	f	f	df8e6759-dc4c-4689-8bae-1adc75fd50ae	160000.00	\N	\N	\N	\N	\N	\N
8833af4e-bb29-486b-9a45-7bd33f4241e1	0ddb8972-36cd-4b67-8887-829aadbdf942	84cc584d-335e-40a7-90c3-abebfe573b01	b53808e3-7219-4c65-899c-197f204e5581	2026-07-18 00:00:00	09:30:00	12000.00	Completed	12000.00	12	2026-09-02 11:13:06.482316	\N	https://res.cloudinary.com/devu5qabc/image/upload/v1784329795/c48e65cf-11b9-4aa2-89d4-781e36e31823.jpg?cors=anonymous	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADmklEQVR4nO3ZQW7cMAwFUN3A97+lb+CihW1SlJykG2uAPC4GHkv8j9kRk3a8XHsjEolEIpFIJBKJRCKRSCQSiUQikUgkEolEIpFIJBJ/o9hqbX/fbdOuFh3l4Gwbo4hEIpFIJBIXil16f63lp+vjMf3LKCKRSCQSicQl4tC1fxkSAxzD6XwAIpFIJBKJxE8R42Drr1yVF612/7REJBKJRCKR+OFieZe3qPFnpLKGEYlEIpFIJH6UOB/gissL1H5PcQ1QBp0nE4lEIpFIJK4QS3X9//kxRhGJRCKRSCQuE+d1/WT0GBJvzq/bQxKRSCQSiUTiOnEfYvPBdsd1U0R7HqX7fel5syISiUQikUh8TTxqlX+NtZxUgs+AGK/lr0QikUgkEonLxFlw/rhOz8vdQekt0xKJRCKRSCR+ipgbyhYVo4zODBvWNSKRSCQSicT3xdJaGk577xeob4fKvUQikUgkEokrxRIS/WHHQSGGGvOIRCKRSCQSV4ixDkWFc55uT1N06fFBJBKJRCKR+BFiuxva3d/yPlUy82ZVptj7thxAJBKJRCKR+LZYqgTHuzLj4zzfb1ZEIpFIJBKJb4nXTjQEbzn9fOouP6WPgxKJRCKRSCQuEuMwO3Hw76llJw5Kyn2zGEQikUgkEonvi0ffP6aXVSri8pXjDjjyFSKRSCQSicRlYk4fa7ZeFWy4V/4MIpFIJBKJxEViTtqHuLIslc2qpOdBiUQikUgkEpeL+W5ZlsahIqSPa49RRCKRSCQSiQvFLin6z6du74qn82sXnLEhlEgkEolEInGxmK9dp3HQ7sqXY7N6jCISiUQikUhcJo53S1xZtKKjPA0BRCKRSCQSicvE7vDs2ocByrt8+Rh6h8tEIpFIJBKJi8S4NhBR3VBlsvlpFJFIJBKJROIiMTIj6b47HeXoq9XqpiASiUQikUhcIc6q7El5srJAbZOO8YlIJBKJRCJxhdhqbbez35n7/TWuxEE3T4kiEolEIpFIXCjG+yN3Pf5QdIe0TGw5YOglEolEIpFIXCRG1xdibFatx9rkYOv/FiKRSCQSicRPEaOh+xkpno6+ihjJRCKRSCQSiZ8nxiqVM7e+o+UNLL7mFCKRSCQSicSV4nyANoj5Xhxs+V25RyQSiUQikbhQLHUtVX1DPc3p3Qb2k82KSCQSiUQi8Q3xnSISiUQikUgkEolEIpFIJBKJRCKRSCQSiUQikUgkEn+R+AdfSodriJunxwAAAABJRU5ErkJggg==	\N	t	2026-07-17 23:08:47.939633	2026-07-17 23:09:30.617764	f	f	\N	12000.00	\N	\N	\N	\N	\N	\N
a3a6cfdd-9aaa-4904-8513-342fab243790	08db5518-d779-4024-a345-3a7acace4095	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-07-13 00:00:00	11:30:00	671200.00	Cancelled	671200.00	262	2026-07-14 09:50:08.54835	\N	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADoklEQVR4nO3aSw7iOhAFUO8g+99ldpCn10pwfQzqEUbq4wFKbNc9xayEGNeX1zmIRCKRSCQSiUQikUgkEolEIpFIJBKJRCKRSCQSicR/URx1Hf/vHfF0ffm5Fz96FJFIJBKJROJGce7311dBbyU1VU5bFJFIJBKJROImcVU18oohZ36dKW+iiEQikUgkEn9H/JP5zFPlXqktDRCJRCKRSCT+shj3CjFy+iO2MiKRSCQSicTfEFsDrWDM17dPqygikUgkEonEjWJZR2P//qNHEYlEIpFIJG4T/2KV+vb6iB8WkUgkEolE4g7xHHXF4HH/N2j+8ac58+DNL01EIpFIJBKJO8S7dIzURRc/2ON2ZgNt2iISiUQikUj8vjjZ1WBUSj+MUn2PSCQSiUQicbfYb4zXKkNV7Gzk8erIeemUSCQSiUQicZsYnSMfnC89DV+L2Sk5Z40iEolEIpFI/L545dLUReynD1/lqbBEIpFIJBKJe8U4Mf1Zz2sU54xVhqozY2XQIhKJRCKRSNwpxsOn/s4cL+zKo1TpcYyUt1pEIpFIJBKJO8REzHEoDkblynOvXZn2x8mKSCQSiUQi8TtiGZHOOE/FpDNfWZ0ed15rmUgkEolEIvH74pyYZn1s4IohM3h9LwXkb0UkEolEIpG4WZx7uWAUtoxXce66Wh6RSCQSiUTiNjFWHfEp/ig0T5/MO7jY62QikUgkEonELWJJv5OeLuLlEZuKnaVFJBKJRCKR+APiJJp95aR5mvqJDaSKbBCJRCKRSCR+X7ziULUalubBzIxPZ2ug9E0kEolEIpG4TWzOkQ8mcbTK2NnqaxCJRCKRSCRuEmPcKjhNW2W8anvHuikikUgkEonEPeIRn8Zrfdh7sHuv/240LxOJRCKRSCTuFcuNmXTHPUQ5bbVvZywikUgkEonETeJMKmuyjTgXTc2yi0gkEolEInG3uFol+C59xqZop4/2DYhEIpFIJBJ3iqOuo4nx43odXK2pWTajiEQikUgkEjeKc/96VfWJadVeqVjlEYlEIpFIJO4V74Qjv6a9GJd+S7onq1LxPBGJRCKRSCT+mPjslZ+H7ooUHNnUD5FIJBKJROIviinzDr5ecXOyulptDCUSiUQikUjcLr5toBFnbKC0cr/OGWsuIpFIJBKJxB1iWUc8mNhsJSeNslfGKyKRSCQSicRt4ncWkUgkEolEIpFIJBKJRCKRSCQSiUQikUgkEolEIpH4D4n/ARf3v2fP6XHZAAAAAElFTkSuQmCC	\N	f	\N	\N	f	f	\N	671200.00	\N	\N	\N	\N	\N	\N
bd3b6417-fa2b-4ef4-8ab7-3e33c61efff0	0ddb8972-36cd-4b67-8887-829aadbdf942	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-01 02:34:45.682	12:00:00	94000.00	Completed	94000.00	46	2026-09-02 11:13:06.482316	https://res.cloudinary.com/devu5qabc/image/upload/v1785170183/c262c5d7-3cb7-4ebf-a066-643602e5af15.jpg?cors=anonymous	https://res.cloudinary.com/devu5qabc/image/upload/v1785170248/c4656432-fa02-4d79-89a5-b44bcea22e59.jpg?cors=anonymous	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADpUlEQVR4nO3aQXLbMAwFUN5A97+lb6BOY8kAQcqZLCq6k4eFJhIJvL/EuG37zfVoRCKRSCQSiUQikUgkEolEIpFIJBKJRCKRSCQSiUTibxRbre3vt60/3Z5dz/r6qx93to2jiEQikUgkEheKW+4u0w/iPMh5ugAl6DCZSCQSiUQicZF4NHddcRAzL6O8H0UkEolEIpH4OeLl4Itfi4hEIpFIJBL/DzHPiq49H3zZ8Rh6iUQikUgkEj9DHAJsz2utvMa3HGDr22aTiUQikUgkEleIpbr+Hz7GUUQikUgkEonLxHl1/yAWyYaDx/Dfgq7mEYlEIpFIJN4vfg3pqhzMfjeK9uPSloMOKYhEIpFIJBIXiXl63I21qczsxOHembG/QiQSiUQikXi3eDm9LEtlUptUBMgZiUQikUgkEheJeQmKcZHi3aOkzXvX97sckUgkEolE4j8WjzvnKnU0RH/sWJeXT6dg17sckUgkEolE4j1iNy6wzIYTM2OziirLVz4gEolEIpFIvFvMg2fLUuQp4pkxguY1rOQhEolEIpFIvF+MhrIx5f64HOOibR+ifLPLEYlEIpFIJN4tzl5PIm9bY0feu0ovkUgkEolE4jJxm95oZVwJkJev7kruJRKJRCKRSFwrnpV3p204jel7qu6HohxguEckEolEIpF4txjO0R+/FgVx+Xo+cu8+TUYkEolEIpF4t5gb9n5IFyqcIpbeYTKRSCQSiUTiMrG9GmZrU7dyRQ0ZW75CJBKJRCKR+AFitF4GGDarcGZbWcuRiUQikUgkEheK++tam0+fh+qcPKVjiUQikUgkEleLW34c4kWA42DPl4tYohCJRCKRSCSuEKP/dZhq0joNcKSddRCJRCKRSCQuE2Opeu90HTleWbke0wFEIpFIJBKJi8T2mvno/zpnzpJlbFy0chGJRCKRSCSuFWN6V7MUswFDbUQikUgkEokLxVkddzsnNqvyLXd0vUQikUgkEolrxVZrm4jnvfhWBudHm8YjEolEIpFIvF+M73vfuvXfzkmvIa2wJUoeSyQSiUQikbhIjHH5tRD7cNCmNUYmEolEIpFI/Bxxe157VvmhaFibxigxmUgkEolEIvHDxM6JmUPHWZko8YhEIpFIJBJXirMA+bWbVALEBha5hyISiUQikUhcIbZJwyOPi0e2I1TYEeDtZkUkEolEIpF4h3hPEYlEIpFIJBKJRCKRSCQSiUQikUgkEolEIpFIJBKJv0j8A+FLNSQhT/aTAAAAAElFTkSuQmCC	\N	t	2026-07-27 23:36:24.598982	2026-07-27 23:36:25.641711	f	f	\N	94000.00	\N	\N	\N	\N	\N	\N
dabb9a56-e346-4f3b-9a8d-7e4c175ff1fc	0ddb8972-36cd-4b67-8887-829aadbdf942	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-07-21 02:34:45.682	14:00:00	94000.00	Completed	94000.00	46	2026-09-02 11:13:06.482316	https://res.cloudinary.com/devu5qabc/image/upload/v1784372712/801dffcd-357a-4e1d-ab69-1c1bfc7a42cc.png?cors=anonymous	https://res.cloudinary.com/devu5qabc/image/upload/v1784373162/7e2d602f-a907-4a89-94c4-11f8bc0c53b1.png?cors=anonymous	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADpklEQVR4nO3aQY7bMAwFUN1A97+lb+CirWNSlBLMplaAPi6C2BL/45LITDsfrqMRiUQikUgkEolEIpFIJBKJRCKRSCQSiUQikUgkEon/o9hq9evd72uvx37fi8d+B/QPUUQikUgkEokbxXh/5vRzrHIv1zDZuoNIJBKJRCJxk7hem47FnnRcK9f1MYslikgkEolEIvH7xHzQ8g9KgcWVt1FEIpFIJBKJXyaGczW0kn71lkcikUgkEonEbxN/8FgOpt+N+scoIpFIJBKJxB1iqTnk5x9zFJFIJBKJROI2cVV/rt93X63l72N97OjnpyISiUQikUjcIc7/uHylD//gnKd4Q0zr1bvNikgkEolEIvEJcSKGKaY6Wq3ond7lNiKRSCQSicQtYt6s4teiSJrfBRGjXHn9nuzDZkUkEolEIpH4jJi7ClGuvOmItuuUSCQSiUQicbuYK7piOyrVszhm1itEIpFIJBKJe8XcPySVg1LhrBeyH+1yRCKRSCQSif9WHDahvFT1KfP6mIeKd2XHIhKJRCKRSNwrlsoN0R9DDd/eRhGJRCKRSCR+hfham66QWKBaXqpi+cptZ56s3CMSiUQikUjcKA5EpEdrTNbGWg96jHlEIpFIJBKJ28R+s+edOSxQ6+XrNV7uaOPcRCKRSCQSiXvFNoYMo8SVvGi9Xb5i7p4DiEQikUgkEjeK0RX71FDlcsnMB8MVIpFIJBKJxI1i//uqlf54V6bIB4NTes9URCKRSCQSiTvFaYA2Becfj85xlH7Wyu+IRCKRSCQSN4nrhnanF6flb6uRpzwikUgkEonE58X7VSv71PUY/WXbWn20O6WwRCKRSCQSiZvE0rA6yKfHOFScRtvwSCQSiUQikbhDXLUWO/cfi4NYpdr9rZ9DEYlEIpFIJD4v5oZ+Z67qGNlhlGnewhKJRCKRSCQ+L37erHJmH4P74t3rICcTiUQikUgkbhJXlTNDfH2bBh22qOxEEYlEIpFIJD4vtlrhHOPH4Ey9sWgNoxCJRCKRSCRuFIdNaNqY2sSup503KyKRSCQSicQvEKMrb0fDANcUrQ29LU8WRSQSiUQikfilYsSVHSvexQDRO814EolEIpFIJH6ZWD7yAOcdd+QBSkq2iUQikUgkEneKbwfI6fE43Mt25B3ZJhKJRCKRSNwjlioNQ+vlxMFKfKUQiUQikUgk7hWfKSKRSCQSiUQikUgkEolEIpFIJBKJRCKRSCQSiUQi8T8SfwF+tXHapnTEBQAAAABJRU5ErkJggg==	\N	t	2026-07-18 11:05:14.397805	2026-07-18 11:05:15.040869	f	f	\N	94000.00	\N	\N	\N	\N	\N	\N
84853280-d3b7-4150-901a-4dd60e63d745	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-07-26 12:34:21.593	10:00:00	160000.00	Completed	160000.00	57	2026-09-02 11:13:06.482316	\N	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADqUlEQVR4nO3aS47bMAwAUN3A97+lb+ACGdukKDlFN1GKeVwEsfl57I5Ipx0fjr0RiUQikUgkEolEIpFIJBKJRCKRSCQSiUQikUgkEom/UWw1tjuxpYZWil/vzpLr2+MoIpFIJBKJxDVivL8e86QgIns9xhaPo4hEIpFIJBLXiufUBywnuqVmJbMOIpFIJBKJxG8RXw9HH7n/6BcgEolEIpFI/I/E7aes/m4UdfP/KSMSiUQikUj8NnH2OAxp598BndnOfj+KSCQSiUQicY1YYuuxf/oYRxGJRCKRSCQuE2fR17bh2/X4ivxb0mMQiUQikUgkrhD3u6udrbNJ57eu+H0vkUgkEolE4loxamOB+FEosPJDUd7i6OtaXoBIJBKJRCJxmZjZKNvzZXUSez+kW6W8+8stRyQSiUQikfgZcZ9gkYjzKtgrSqLMIxKJRCKRSFwr5klxRV2JPL2bFOvlpba7rZxcRCKRSCQSiSvE8Z4apl9izm79gKt3mEckEolEIpG4SIyydkeIsxi27fYejioikUgkEonEZeLVULqGbLsT446xSq4jEolEIpFIXCk+1p6JskCcXFu/yljctxGJRCKRSCSuE18NQ2K/P0Lchn3iY7Y8kUgkEolE4goxZpau+bsjT299DP+CbBCJRCKRSCQuErs4u/Y8ZDigumur7D0MJRKJRCKRSPy82O44G7rpw5A9180XPfrNiEQikUgkEleKjzdRYVuKrSfafAsikUgkEonE1eI1vdxE/XXUSke+tvL0VqYQiUQikUgkrhHjYoorqpuZs0cuztu2fJ+VDiKRSCQSicQVYomMlaWufeLdcFntt7MTiUQikUgkrhbzidT1t58I7MjE7ICKbUuCSCQSiUQicY3YhiHnY0RMv2aWK2ruRBCJRCKRSCSuEON22nssFjiyOCS6ASVBJBKJRCKRuFYsrWfXdTHluv1OlONrFkQikUgkEokrxVlEz5DYhpIyPR6JRCKRSCQS14rz1j13lWzep9Q9XGVEIpFIJBKJa8QtpwbnWiD/ZLTdbXv/eNxiCSKRSCQSicRFYplUnMmd9LBK2ZFIJBKJRCLxe8VXHDWuceWeKsWRJRKJRCKRSPwWMQbPZp6DH46qvC2RSCQSiUTicnFYoGPzKRWD2z1zz1i8y2OJRCKRSCQSV4glYkg8HsMWryjf8j319rIiEolEIpFI/IT4mSASiUQikUgkEolEIpFIJBKJRCKRSCQSiUQikUgkEn+R+AfurodrSzTuDQAAAABJRU5ErkJggg==	\N	t	\N	\N	f	f	df8e6759-dc4c-4689-8bae-1adc75fd50ae	160000.00	\N	\N	\N	\N	\N	\N
4b713e5d-d0da-4c9d-81ee-216f510cde31	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	84cc584d-335e-40a7-90c3-abebfe573b01	618e9f63-8360-4cf4-b178-9457dd66761b	2026-08-11 18:00:27.129	10:00:00	350000.00	Completed	350000.00	232	2026-09-02 11:13:06.482316	\N	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADlklEQVR4nO3YQW7jMAwFUN1A97+lb+ABMnZIUUrbWUwUoE8L17HI/7gk2s43n6MRiUQikUgkEolEIpFIJBKJRCKRSCQSiUQikUgkEom/UWz19Kk8vk23x9VxPV5GEYlEIpFIJO4Qh5DVz0d3pD/eJqx/GUUkEolEIpG4SWx/T2l9BF8NbXWxEksUkUgkEolE4keJObM9t6j7FGzasYhEIpFIJBI/Vwwnr1fnOErPn4lEIpFIJBI/VFy1hhgD5FHOXDcRJ5FIJBKJROJ+sZye2X98zFFEIpFIJBKJ28T1Oabua7w7Pab9LopIJBKJRCJxm3hMcVdSXMTP+79FU2/Lt7mESCQSiUQicZsYrcfo9MU8rdVpoy5j02REIpFIJBKJW8SWzrBeBVE2pjzPixnHyYhEIpFIJBK3iOuG4ymWBarlzDJtlHy5WRGJRCKRSCS+SzxycG4d3iI7twW76iUSiUQikUjcKa6S8rd5WcoDvHCIRCKRSCQS94rRUEKy3XN/W5yXxUQikUgkEol7xUdtdMUqtVqqcnGkDCWLDiKRSCQSicT3i+2Zfg+Qt6Mj38buVEZu45nqiEQikUgkEt8vli1quoi48nb/nNhYr4hEIpFIJBJ3irl2OOU2D9DHQc+xZOglEolEIpFI3CauglePsk9NQ/WRIBKJRCKRSNwurlvbRTwbWrmY6o7WVqMQiUQikUgkbhMjc/p5PFvbuGhFXDi9JhKJRCKRSCRuFwcs0q+3r6e4L4LN4+VeIpFIJBKJxE1iYfv3dtSV4nnHIhKJRCKRSNwh5q77sW4dnOut5br2PKWOSCQSiUQica+Yv8W5BygbWL6IlJZ3MSKRSCQSicS94vPTsrZsW5mI9AGLb2M8kUgkEolE4j7x6hpOGWBi+xjQp1GIRCKRSCQSt4mR2RZnmqw456KtLwclEolEIpFIfLf4MjMe+VuZJ/apqGs/2+WIRCKRSCQS/7PY6ukTG/9fyiXD/5zyjPe3fIhEIpFIJBJ3iD1fjetQm4IHYr1Z9XFaIpFIJBKJxJ3ildoXP+Osgld1czGRSCQSiUTip4jTUnXkzWpMGi5KMZFIJBKJROJHirnhzN+KGAF5iqO1sZdIJBKJRCJxizgNUB5tMcDQWwadOohEIpFIJBJ3iOWsgtuU9Pg7jTwsZEQikUgkEol7xfccIpFIJBKJRCKRSCQSiUQikUgkEolEIpFIJBKJRCLxF4l/AIDy0TqItUXmAAAAAElFTkSuQmCC	\N	t	\N	\N	f	f	\N	350000.00	\N	\N	\N	\N	\N	\N
4e3f981e-f1f3-42d5-b67b-299f07b178c2	0ddb8972-36cd-4b67-8887-829aadbdf942	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-07-18 02:34:45.682	08:00:00	94000.00	Completed	94000.00	46	2026-09-02 11:13:06.482316	https://res.cloudinary.com/devu5qabc/image/upload/v1784304088/aba0a89b-c98f-44db-a7e6-e3ad8d4f1674.png?cors=anonymous	https://res.cloudinary.com/devu5qabc/image/upload/v1784304300/386c716d-3cb3-4ce1-8d11-805f121a5b6d.png?cors=anonymous	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADqElEQVR4nO3ZQY6kMAwF0NyA+9+SGzBSD2DHCSXNYkhJ/bIoERL7uXdfdDteXnsjEolEIpFIJBKJRCKRSCQSiUQikUgkEolEIpFIJBJ/o9jq2vLB+bT9rWrxc57G06dWRCKRSCQSiWvEeH9tM3FkrNhxOZfNOhOJRCKRSCQuEiMJFay8ixT1gS2tiEQikUgkEr9IjLuFiG27V5mMSCQSiUQi8bvFazvc27J9ivu8lkgkEolEIvELxGG75wAVX4tmn5FiO29FJBKJRCKRuEwsKyLSv/+MrYhEIpFIJBKXibP1c/18ylUtDs5tONukz5FqiUQikUgkEt8Xuy9D85y0ZTtvY4pugPgERSQSiUQikbhMLI1nA5Tx5gN0lydlRCKRSCQSiW+LwWan5bAUUxRxmHb2FxCJRCKRSCQuEh/j0JCOjts+brGz8zzt0xckIpFIJBKJxBfFCFXFiVB1nJ2i5+Bs/ZWNSCQSiUQicakYVa3vPtafV0rZ1o93PRGJRCKRSCR+gThjczqK3BWrO32MV0QikUgkEonLxCjIwejo2x2TTg8pasCIRCKRSCQSV4o/Kz+VUWaT7blilqc+JisikUgkEonEt8QSlrpR+tI2lB1D7aSMSCQSiUQi8W0xsEhHcRA/pXHcG4gyN5FIJBKJROIiMWPb5F10CqecXu9yFmutEYlEIpFIJK4WL+LsueXG4czmGbalS75CJBKJRCKR+L5Y6jsi21dsiiv5qZwSiUQikUgkLhdzJoqq6ynYaBeDDuLVKmqJRCKRSCQSF4qtLx1HyRmrW2WeMhmRSCQSiUTianGbdJ9he9+py2Ln2u6m3ZZIJBKJRCJxmfj4FPXl6e6UpojxJveIRCKRSCQS3xd/VoeVn3yvZWJWkdPWU7IiEolEIpFIfEd8KBg+KF3s3eQ6bX2Kuk77LZFIJBKJROIScagv/wG7Vu7U9Zxf/5CsiEQikUgkEv+7OFuFLQOUyDULX+UdkUgkEolE4gqx1RVB6+o+jHI2qflsiGZEIpFIJBKJa8V435WW09KupK2SrMo8RCKRSCQSicvEqCrbzO73lXa2a9M1TkskEolEIpH4VeJ9Nz3lKw/zlHtEIpFIJBKJXyUOB+1u91BWctdQQSQSiUQikbhIHDqNbMlYueI6LSu3JRKJRCKRSFwhlrX1nWKofdJu78v2+3TPrYhEIpFIJBJXiO8sIpFIJBKJRCKRSCQSiUQikUgkEolEIpFIJBKJRCLxF4l/AO8qbhxNPbNJAAAAAElFTkSuQmCC	\N	t	2026-07-17 16:01:29.968006	2026-07-17 16:01:30.63517	f	f	\N	94000.00	\N	\N	\N	\N	\N	\N
6fe12f7a-81de-42e3-8696-341fdbbb60f9	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	84cc584d-335e-40a7-90c3-abebfe573b01	618e9f63-8360-4cf4-b178-9457dd66761b	2026-08-10 18:00:27.129	10:00:00	350000.00	RescheduleSuggested	350000.00	232	2026-08-10 08:21:25.040745	\N	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADlUlEQVR4nO3YQW7jMAwFUN3A97+lb+BBgTgkRaXFLCbKoI8LI7bF/5gd4XG9uc5BJBKJRCKRSCQSiUQikUgkEolEIpFIJBKJRCKRSCT+RnHMdXw9O/LxuI1TNe5u61FEIpFIJBKJG8VV+pkb8rPAzpb+clAikUgkEonEbWLrOr+Cy8aUO0a1v48iEolEIpFI/FAxgmOVOvLt4zCRSCQSiUTi/yA29sjnMjHWR4hEIpFIJBI/RVwNMCW1zep6PpsGXSUTiUQikUgk7hCnKv1/eelRRCKRSCQSidvEn+p8fjI661B3PeY+fkghEolEIpFIfL8YST2kbkcjX+6KKeKDUpuCSCQSiUQi8f3iy+Bmj/Zi3XHWt0QikUgkEonbxOj/ZjFKmTHAeopyjkgkEolEInGjeFS2dOUjRz18j7fqbesVkUgkEolE4nYx4lprnyI/i63srEeIRCKRSCQSN4nR1bB4Oy1Q54IN7KwDEIlEIpFIJG4SsxMh94vpdhIbMXUQiUQikUgk7hWPml5CphVpzHU8AyJ9VUQikUgkEok7xGhdJY28NkXHM6ksVeXFt7sckUgkEolE4j8Xc8O0HZWkVVzsYtPtYzIikUgkEonEnWJeoK4aUp69TI/g9XhEIpFIJBKJm8Qp/bEYlW0r0lej5Lb4G0QikUgkEonbxam/pR85ePr16D0XA7R5iEQikUgkEreIweYq61Ue78zOlDL9AyKRSCQSicRt4rRA5UtMURat1WRBxIxEIpFIJBKJHyC24JHTa+t8JJ+LZ6OORyQSiUQikbhDvGrX8Qhu69WLjmmzenWESCQSiUQi8d3iSyzi2qI18gelvE/F5ZvNikgkEolEIvGN4iPkag359nauVGW89REikUgkEonEnWIsUKuNKULyKKNihY1/QCQSiUQikbhXbA3hnLl/WsPaeFEllEgkEolEInGHuKqcedW4o80YbAwVuxiRSCQSiUTiNnHMdeSuvEqdC7ZPMUURiUQikUgkbhTj+ZVb8xekPsW0O0XK6i2RSCQSiUTiNnG1IuX0Uuv1quxTOYVIJBKJRCLxc8U2wLRU9YpkIpFIJBKJxM8TH639M1LuuNsmIg9KJBKJRCKRuFNsAxSx9V/Pt1HnGKspiEQikUgkEjeKY3E2nP5sSr/mur8vvd6siEQikUgkEt8hvqeIRCKRSCQSiUQikUgkEolEIpFIJBKJRCKRSCQSicRfJP4B+Z2BUDY3QTkAAAAASUVORK5CYII=	\N	f	\N	\N	f	f	\N	350000.00	\N	2026-08-10 00:00:00	Manager	13:00:00	Salon manager proposed alternative time slot.	\N
ddc31886-f48e-4fcc-943c-d3cc9e14c8f5	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-07-26 12:34:21.593	08:00:00	160000.00	Completed	160000.00	57	2026-09-02 11:13:06.482316	\N	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADlklEQVR4nO3aQY7bMAwAQP1A//+lf+CiqB1SlLLYk2Wgw0NgRSSHeyO8aefDcTQikUgkEolEIpFIJBKJRCKRSCQSiUQikUgkEolEIvF/FFuN/ve7ntOv47FOyU9zKyKRSCQSicSN4pfuQcTx+q5PU/zQikgkEolEInGneNX1fMwpwa7imGrzUEQikUgkEomvEqPg3p1yXswzb1ZEIpFIJBKJbxbjojxNW1QfuxCJRCKRSCS+TZwGKD3LP84Gu5StOxOJRCKRSCTuEEvM4u8/5lZEIpFIJBKJ28RVXLn3T4DKMcfXXwlNeUQikUgkEonPi/froXhHlIkjr0rx3ug6DvNMKd82KyKRSCQSicRnxDtyaZ+OZbwyRe5SRiESiUQikUjcJObiYR1aia3GcFGmJRKJRCKRSNwt9tx9wtpnd5qP+bvzWxcikUgkEonETWIu7XlFKvO0T8Rt6ZLtdvUjEolEIpFI3Ca2FLFPhT0MVcYrWFnIiEQikUgkEl8gXk+rLWqeLJcdn+/CuTEikUgkEonEF4hT954/1qvUfbEaProQiUQikUgkbhOjPpaqUp+f+rmMPNkqmUgkEolEInGTmHNjWSoLVEzW8kU+DuONBpFIJBKJROLTYl6qynpV2h2feY6FeEf+M4hEIpFIJBJ3iqXTdDFPVm6nbWuoIBKJRCKRSHyFuKq/d6fSJG9R57qCSCQSiUQica8YaZkd2v2wch1X4/zRlksakUgkEolE4tNiIUp9HiqIlidbbVulAZFIJBKJROIO8bq8c0tpEdsizkVMF0QikUgkEonPi6uXR1fBkZ9WjfPIMffwVolIJBKJRCLxBeKVEQVt2rbiYnpvdEyTEYlEIpFIJO4Vwymr1GpZihm/RhmPSCQSiUQicZs4bVbRsyxawypVlq8JHiqIRCKRSCQSd4glI45xm5N7/igL2dSKSCQSiUQicae4isspK9dwjJTSPTsRRCKRSCQSic+LrcawY2WxzHOuWSKRSCQSicT3iKVdzz1X7abuw1Brm0gkEolEInGT2P7F0CSLbfG2KC7OT4M2JROJRCKRSCS+Siyl0fM6Hp+eEfOgZwoikUgkEonEd4j5IjqtsP4ZZagorYhEIpFIJBK3idMAs/0pbXnGG8vzrDoTiUQikUgk7hBL9Nx49fSLFCKRSCQSicQXiM8EkUgkEolEIpFIJBKJRCKRSCQSiUQikUgkEolEIpH4H4l/AJnXZ8KNMj4TAAAAAElFTkSuQmCC	\N	t	\N	\N	f	f	\N	160000.00	\N	\N	\N	\N	\N	\N
32060188-f6aa-42d5-b12c-60c30b579d58	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-22 00:00:00	09:00:00	207900.00	Completed	210000.00	207	2026-09-02 11:13:06.482316	https://res.cloudinary.com/devu5qabc/image/upload/v1787131085/d8b918e5-5de7-4759-80cb-23f1b3986ab5.jpg?cors=anonymous	https://res.cloudinary.com/devu5qabc/image/upload/v1787131698/4522faa6-10df-431d-9fd0-7b50116e4f85.png?cors=anonymous	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADkElEQVR4nO3ZTa6kOgwGUHaQ/e+SHfD0WhXs/IDUkwSpjwclqMTf8Z1ZdY9rcZ0HkUgkEolEIpFIJBKJRCKRSCQSiUQikUgkEolEIpH4L4pHXyV/l+/Vg//7m3vlJYpIJBKJRCJxoxjfx+t5Y/VpSI97McAsikgkEolEInGnGHGRNK8m+M83eahZFJFIJBKJROL3xC74akeJuPcoIpFIJBKJxA+JwXa/EQ2jzHqJRCKRSCQSPyQOA0Rr9Dc1t8coIpFIJBKJxI1iV6Vl/+pjjCISiUQikUjcJs6r/kMs/4xUx8tXGvE1j0gkEolEInG9WNOjOiKc2ZXuKeznzYpIJBKJRCJxhRj9ObjcB/HaLFDD70alJw4ikUgkEonED4iz9OH0zB85rhLXcxGJRCKRSCTuEEvbUOO6Ud7HGwYgEolEIpFI/ID4WCU1HLPM9w2sDSASiUQikUhcLQ6ZsVnV9SqHHPdSVZ/yVna1HTmeSCQSiUQicbWYQyoRXXmK436tl/MopZ1i+IOIRCKRSCQSV4s5Lpxo7eLOJzZeiUQikUgkEr8hdv0RPIScw1B573p8IhKJRCKRSNwp5v6H7/IUR7t3xRQlnxKJRCKRSCTuFbu7Q3+5W89jbKuXr7byUEQikUgkEombxHcsXwm2+YiD4QqRSCQSiUTidvHXcEy6utZrnhnjzb8jEolEIpFI3CHWyiFn/m62aM16c3UDEIlEIpFIJK4XI3Poaqb4vY5Pw71yH8w3KyKRSCQSicSFYvcUXe/zdFheyGoKkUgkEolE4jYxfh763S7Z7q7MKtvdAZFIJBKJROJesXNilfrdbeLq6Wy8+fBEIpFIJBKJ28QRy3evfJrTj3uUZopuKyMSiUQikUjcKw6rVGmf4qBx8jzjZK+bFZFIJBKJROIa8ffUxZ3Da9zLvc0owymRSCQSiUTiJnFWeYpyf/ypSuQZy9DxulkRiUQikUgkrhGPvkp2hoptq/a+TJbbiEQikUgkEteLzWEndkl5xrpAdVvU06BEIpFIJBKJW8QIGYi3/5ndcU014xGJRCKRSCR+TDxu4sr2bKh82rGFSCQSiUQi8XPi+HG39pvVjCASiUQikUj8ijgbIIjIzL8lXZN79crVF5FIJBKJROIOsavH4OY3onAyFiO/blZEIpFIJBKJK8Q1RSQSiUQikUgkEolEIpFIJBKJRCKRSCQSiUQikUgk/kPif2p53xDrugsCAAAAAElFTkSuQmCC	-2100.00	t	2026-08-19 15:58:10.870731	2026-08-19 16:18:06.828735	f	f	\N	0	207900.00	\N	\N	\N	\N	\N
f4b92bcb-3365-4354-9902-f9c084922767	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-07-25 12:34:21.593	09:00:00	160000.00	Completed	160000.00	57	2026-09-02 11:13:06.482316	\N	\N	iVBORw0KGgoAAAANSUhEUgAAAzQAAAM0AQAAAABFCVraAAAC70lEQVR4nO3ZS46cMBAAUN+A+9+SGxApoamPYSabRDLzvOhuXJ9XyxI9jv9y9sHhcDgcDofD4XA4HA6Hw+FwOBwOh8PhcDgcDucnOKOf7U/GuHL/3E3RFmgNOBwOh8PhLO3E/ecxakJs2Bl9aMDhcDgcDmd9J9eXjy/OXitKAw6Hw+FwOK9zIhDvJKaNZPuuAYfD4XA4nHc5bckoIofD4XA4nB/hTI/R7lPfopm4a8DhcDgcDmdpp50t1//Vx9yAw+FwOBzO0s7XJ6rPx61Gt9sqDofD4XA4qzv71enzduLu74r4HyPnzUNxOBwOh8N5l9POQ8/8dmK77saNzeFwOBwOZ00nV40q7vntRHTKmY+1HA6Hw+FwVnfOX414fDERgc9j1NY8DofD4XA4KzoRvF83tqvxUe+OKTnbHA6Hw+FwVnceN43caZuKYsanLhwOh8PhcNZ0Wrtx5RasDZB3jthS2igcDofD4XDWdEY9j42n6F4nG7WMw+FwOBzOws6jGMtI1N8tKK37N+9DOBwOh8PhLODkdvOv/Hhcd49loydzOBwOh8NZ0xm5NKfN0XwX4iflZigOh8PhcDhrOmXniNKTHbl0GmUffWQOh8PhcDjrO+fyMDf+/R0DxLkbr60vHA6Hw+FwlnbaknFXmtmjprRRGsbhcDgcDmdNJ3cqH63TvbNN43E4HA6Hw3mD82kSPaMgi9EuAjHjmAIcDofD4XBWdep9P7nxuMHibLUVh8PhcDichZ27E1UNi+55NykVHA6Hw+Fw1ndGPyW3xaa7rQ7VMA6Hw+FwOKs6rXHk7nUFmf/gyHvIPCOHw+FwOJylnbZVnC8hYvvYc3Q8Hw6Hw+FwOK918n7xieY9pLCRl5cWDofD4XA4r3Nm9hG7epZHDofD4XA4qzv3bDQ+qridjXPZ4HA4HA6H8yKnne0KtBXkqAMcNVDyOBwOh8PhLO38y8PhcDgcDofD4XA4HA6Hw+FwOBwOh8PhcDgcDufFzi+EwdKO5YfywAAAAABJRU5ErkJggg==	\N	t	\N	\N	f	f	\N	160000.00	\N	\N	\N	\N	\N	\N
738529a7-3c13-4311-b5fc-2005f1aba573	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-24 12:35:15.72	17:00:00	79000.00	Completed	100000.00	175	2026-09-02 11:13:06.482316	\N	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAAE3ElEQVR4nO3dUW7jMAwE0Nyg979lb7ALLOpSIimj/WiYRZ8+gsSNRZcgZ8Zj2Xn8efJ4f4goqypHd8AcuIo78CPVQVlRjxSy8w7nVs4fnSNzHrgrHCQuGZeMS8Yl45JxybhkXDIuGZeMS8Yl45Jxybhkv9Ele+Tx9vmHf+/eH4/4+PFyjXWCt5upRJRVlaM7YA5cxR34cUZ1xPY0cUyy7ppnT7v1M4soqypHd8AcuIo78OOQ6ljV0aWnkkSK2T/UVno5TiWirKoc3QFz4CruwI8vpDq62Yu/FB9DbYkoqypHd8AcuIo78OP/oDpurpTFH+J4RJRVlaM7YA5cxR348SVVR/l4xYnpQk+t7y7JdTOViLKqcnQHzIGruAM/jqmONNKKoG+91KlElFWVoztgDlzFHfhxTHV0o0ikS0UlebUe8rZHGSLKqsrRHTAHruIO/DihOuqan26lTxxKKKfVQdpu/VotKBFlVeXoDpgDV3EHfhxSHd2lseP+nT3UeUlFn4koqypHd8AcuIo78OOQ6ui1U1r53Omp+ENaDZ2GiLKqcnQHzIGruAM/Pl91lOmuj0ljpeMpouraVo5CRFlVOboD5sBV3IEfx1RHukhWdVI30oEeBZmIsqpydAfMgau4Az+OqY7+ylZSR916oTpBJ8NElFWVoztgDlzFHfhxWnVsDyBcd+2ewJx+VqteJAvxJaKsqhzdAXPgKu7Aj6+iOm4eSpgOZRNfaZn0eigiyqrK0R0wB67iDvw4pjq2P5ZH76R71a93+5eWfftn94goqypHd8AcuIo78OOE6jje0rUJqHL71nab19FkElFWVY7ugDlwFXfgx0HVcY3ynJ5OWW2rf/qwX/RzRJRVlaM7YA5cxR348cdVx2EJUBJV6YJYeRbzUYGJKKsqR3fAHLiKO/DjkOqIsGmSsvL5+EDm6jSVa2YiyqrK0R0wB67iDvw4qzpih04iJfF1nGDbJqKsqhzdAXPgKu7Aj2Oqo9NT67vtpTyoMB1ovfVLRFlVOboD5sBV3IEfx1THcWlP+Uoa3dWzNESUVZWjO2AOXMUd+HFWdWyjWxZ0vK+rj7OpMhFlVeXoDpgDV3EHfhxTHR/7b5fBYlv6XvflONDv3MkuoqyqHN0Bc+Aq7sCPz1Edx5m6lc9JWZWI17Z1iCirKkd3wBy4ijvw44TqSDdohfbp1gHd3fZePJrb37cSUVZVju6AOXAVd+DHZ6iOfs3y4Ucm1uNJ77Zxq6xElFWVoztgDlzFHfjxx1VHN9bZO2fo8NzCFbTiuEWUVZWjO2AOXMUd+HFIdRRBFLuW1cvbV+pPUKTH9ogoqypHd8AcuIo78OO06ojtW5wye5pzuwG+X3MjoqyqHN0Bc+Aq7sCP46oj9iovxQra9FQ3tv9ARFlVOboD5sBV3IEfX0x11Em6dzcPZP62shJRVlWO7oA5cBV34MdnqI40U4iqstvBbvrKHV4iyqrK0R0wB67iDvz4DNXRi5At2MkZ6lRUfqqhiLKqcnQHzIGruAM/zqiOesVo3au7mys5SPURPetXRJRVlaM7YA5cxR34cUh1PGeIKKsqR3fAHLiKO/Aj1UFZUY8UsvMO51bOH50jcx64KxwkLhmXjEvGJeOSccm4ZFwyLhmXjEvGJeOSccm4ZL/IJfsLTEiXXz9yFfQAAAAASUVORK5CYII=	-21000.00	t	\N	\N	f	f	\N	0	79000.00	\N	\N	\N	\N	\N
0c24750d-074f-4ae1-9e3f-98e25f955b25	08db5518-d779-4024-a345-3a7acace4095	118341cc-0df0-44cd-b40a-0a3f5d167c58	dba37f5a-970c-41da-be69-438732e98402	2026-07-31 00:00:00	08:00:00	12000.00	Cancelled	12000.00	12	2026-07-31 04:59:02.555362	\N	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADjklEQVR4nO3ZQW7jMAwFUN9A979lbpABOnJJimowqyiDPi0MWxL/45JIrueb1+MiEolEIpFIJBKJRCKRSCQSiUQikUgkEolEIpFIJBJ/o3ita+xrvs6+66/5GW+vo4hEIpFIJBJPiIX4+px7j5k5g8cGG5uyXTKRSCQSiUTiIXHGdGxZSxfzUcQWRSQSiUQikfhB4kyfd38W4x6RSCQSiUTi/yKWx47NZb1bIpFIJBKJxE8Rd587dh709TqKSCQSiUQi8Yy4rNGwf3/0KCKRSCQSicRj4m59Xc+/G42/VWXvqoPWeJlIJBKJRCKReEJ8tPTlLe7lNfLefBu5WyKRSCQSicSz4vLbT26g/zIUFbuAcNqgRSQSiUQikfh+cd64Nk6rutfIZfu9JYBIJBKJRCLxhBg/9pTBaD4ibieWe7nl6+dfkIhEIpFIJBLfI+7iYi+3Eo/ndyv3anuRQiQSiUQikXhIzFgpzWK53Gp3PT6JRCKRSCQST4tRkOGoj1Gq7EVcbnSJIhKJRCKRSDwpxmSViZiYrsy2stJoNJAxIpFIJBKJxGNiuba8NfvZumgVEfrTZEUkEolEIpH4HvGerOqNMmjd9fletDJyytIFkUgkEolE4kFxZ1/NaQ0sV4q9SSYSiUQikUh8txhxy2Q118iFOycTvWUikUgkEonEg2KUxueN5UckFeLFQDaIRCKRSCQSj4p3ehuRxiYzQmKVK0QikUgkEomfIu4zu90msEi/2rTV2iMSiUQikUh8vxhEhOTPqC/BuYu+cjKRSCQSiUTiWXFxllbKPJXtR5WWe0QikUgkEonHxXjkvZLUTu8ru704IBKJRCKRSDwo3pkxRc2CRz6I4N1bburReiQSiUQikUg8I47KPuvd0lk+Xdpb1qPuEYlEIpFIJB4S2+wU/5nFf2HlIK9ouR8QiUQikUgkHhN3a+liYWcr9714zD0ikUgkEonEzxCvde2w4sTB0mO+9yASiUQikUg8L44U06vutzxeXfWzT1ZLP0QikUgkEonHxKU0PjO2zE7P73vLGjmZSCQSiUQi8RPFOzjSc0WO2w5aRCKRSCQSiZ8nLvPUEtfKrnr5mfOIRCKRSCQSz4q7BsL5rupvJSD3feUrRCKRSCQSiWfEZe0mq1y67WLpMUcRiUQikUgkHhLfs4hEIpFIJBKJRCKRSCQSiUQikUgkEolEIpFIJBKJxF8k/gGsib6EZBiAAwAAAABJRU5ErkJggg==	\N	f	\N	\N	f	f	\N	12000.00	\N	\N	\N	\N	\N	\N
bd7f9cab-5cc5-4002-a0f2-9e40d8212d34	0ddb8972-36cd-4b67-8887-829aadbdf942	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-07-11 02:34:45.682	15:00:00	142000.00	Completed	142000.00	92	2026-09-02 11:13:06.482316	https://res.cloudinary.com/devu5qabc/image/upload/v1783743419/9fc8316f-9485-40f2-a97b-2e6d32dd9bf8.png?cors=anonymous	https://res.cloudinary.com/devu5qabc/image/upload/v1783748364/6e2adf43-d643-4d01-8acd-6b13576ee38f.png?cors=anonymous	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADl0lEQVR4nO3YQY7bMAwFUN1A979lbuCihR1SlDyDLmqlmMdFYFnSf8yOcDserlcjEolEIpFIJBKJRCKRSCQSiUQikUgkEolEIpFIJBJ/othq9fdGzz+/b7U/P1Gx/DqKSCQSiUQicY8Y78O5rWL3RWerZCKRSCQSicRNYsTlzGNMei3YYxKnKCKRSCQSicQPEienPB1f3CASiUQikUj8D8S58nhVlkQikUgkEokfKU7L6/PQ+fTKRHl3btxGEYlEIpFIJG4TS5Xx6q9+5igikUgkEonEbeKqVg2co1QOaflwX0cRiUQikUgkbhTL96BrRIrdnHmzjH5iSSQSiUQikbhXnI5dZ2MjxqbcyhyQWy6DFpFIJBKJROLzYnZe7/QYjGJ2OsaNIzdamiqNEolEIpFIJO4Qx/e1ldXcldPLyBU3vpvliEQikUgkEv+5mK9eU1Q4sYx+8m55Wl0jEolEIpFI3CZezpTZcyvnxuCsRqnsZINIJBKJRCLxaTGnt8mOzBxXnBjIWt7INpFIJBKJROLzYgTnp9tR6vZGtNdyj0QikUgkEonbxDIJTcPSMCLF7tTUVSWZSCQSiUQicZtY2MByXB+D41w/lpXPEYlEIpFIJG4Sc2b8zOlTakxWw427w0QikUgkEonPi6vM6xvRGRLnylMQ8wclIpFIJBKJxI8SW6rYGPopY1h0VtqLzohEIpFIJBJ3iHmAKukxXvWzlVhOG8f4RCQSiUQikbhdjPd5YmptaGpuIDrL3Q7/gEgkEolEIvEDxMWJOmi9Y27uDlX+C5FIJBKJROI2MS/DvqaoPErFeLWqvkghEolEIpFI3CZeNXUxiDFFvUPStXKk/A0ikUgkEonEbWK5dQ5Qpan+Xsa5gkWjhSUSiUQikUjcIfYpJDst27mVmLvKkVURiUQikUgkbhKLHWKek65l+XhU2jur/AMikUgkEonE58VVxcRU+snpfbpRdolEIpFIJBL3iq1WnzZinipfkFZE3CASiUQikUjcLc4j0nR1YKeNISXe5SISiUQikUjcJJ6X+1os81R2BjtSvp+siEQikUgkEneKw0awsbtm+3SESCQSiUQi8VPEnB7iUPlaiEMXRCKRSCQSiR8gTklX3Gojf1A6MlsqxxKJRCKRSCTuEEtdmbEsU1SOe+UjeRlRRCKRSCQSiZvEZ4pIJBKJRCKRSCQSiUQikUgkEolEIpFIJBKJRCKRSPxB4i/zIAihlCXd8gAAAABJRU5ErkJggg==	\N	t	2026-07-11 04:17:00.960913	2026-07-11 04:17:01.622211	f	f	\N	142000.00	\N	\N	\N	\N	\N	\N
e0778881-34f5-42ad-ad7c-f4dfe63b6131	0ddb8972-36cd-4b67-8887-829aadbdf942	c2325bfa-edca-4803-92c1-9c2507f5b4a8	2adce07e-6ef8-4f7d-8920-accc288417e9	2026-07-13 16:16:41.37	10:00:00	70000.00	Cancelled	70000.00	22	2026-07-13 10:20:16.24541	\N	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADqElEQVR4nO3aQXLbMAwFUN5A97+lbqA2qWSAIJVMFxXdycPCE0rEf1hi7LTj4dobkUgkEolEIpFIJBKJRCKRSCQSiUQikUgkEolEIpH4E8VWa/t4tvVvx+P58bs+4locSxSRSCQSiUTiQjGex3GfTBEhex/3dRSRSCQSiUTiSjEH52ttlh73PpvOe99FEYlEIpFIJL6HGM61YxV7YIlEIpFIJBL/BzE/u7Dzcjw7Ulwb2ohEIpFIJBLfQxwGuFrDyVeitqFtnkwkEolEIpG4Qiy19exffYxRRCKRSCQSicvEecXvYzHZ9nqxvY5HFr/MIxKJRCKRSHxevNKjzp/Gun8LGo7dFDmqG4VIJBKJRCJxmXguRuEcr2N5e72IVSre5vVqz1FEIpFIJBKJy8QSfHYdE6e9QvbX8WbbihQikUgkEonEZWK0lq7S31KVuNLRhhQikUgkEonEFeKstaRnZx/Y0pH3rigikUgkEonEReLQcJs+JN1c2YlEIpFIJBJXixnrGs6NaRvuDdtWN8AxLSKRSCQSicTnxfNafO0TXXuffkPkKbq3+UgkEolEIpG4TDyGHausV61WEF9NSyQSiUQikbhQHOuWKDOWraysV0QikUgkEonvI8Yq1fV/Hkp62btOorvXJxOJRCKRSCQ+LZ7XLrF8lB1rFpxrm4QSiUQikUgkLhO3jOVlqduO4tlADFtUyx1EIpFIJBKJi8SCnXdb3rbO494/K9jeT0EkEolEIpH4BuL1sqXaasPNjG2YJ7flDiKRSCQSicSnxbwila5Yqo7XttXZw7P9fmQikUgkEonE58WrK+pItd9ldqPM5yESiUQikUhcK86/8WllvTpr/KZpvpB9v8sRiUQikUgkPiHO068aRhnF+Si5jUgkEolEInGdGOllYyrYbduwkH23WRGJRCKRSCT+czFvR9vrWefEs+HezbbVT0skEolEIpH4tDirM65NsPaRVNamI89IJBKJRCKR+C5iq7Xl4BL36u/eRtvNoEQikUgkEolrxHFFiu0o70nd8Yvla/yLSCQSiUQicZkYXcOxZSyOR1/FznMTiUQikUgkvpW45aScOc5zXt7PQw4lEolEIpFIfD/xao0vj3Jm2azGL5QimUgkEolEInGtOAxQ1quYoixQ3V+lox+KSCQSiUQi8Xmx1PbnxpV+HaPyiw6bDUokEolEIpG4THymiEQikUgkEolEIpFIJBKJRCKRSCQSiUQikUgkEonEHyT+AvEXBGWnXIkLAAAAAElFTkSuQmCC	\N	f	\N	\N	f	f	\N	70000.00	\N	\N	\N	\N	\N	\N
931c5c48-eace-4ef1-a173-b791a22ca726	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-07-23 12:34:21.593	10:00:00	160000.00	Completed	160000.00	57	2026-09-02 11:13:06.482316	\N	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADj0lEQVR4nO3ZQW7cMAwFUN/A97+lbzAFGtukSE2Cbqwp8rQYZCLxP3b3kW6vh8+xEYlEIpFIJBKJRCKRSCQSiUQikUgkEolEIpFIJBKJv1Hc6tm/Xmzx8fecj+M2x32NzaOIRCKRSCQSl4l7nm6j+/i766eG7fMoIpFIJBKJxLViS9/v2+sit63Avt+MSCQSiUQi8dPE/HejYxRf7TYuiEQikUgkEv8DcbjIWFz0MSKRSCQSicTPE+dfe216W6V+jiISiUQikUh8Xiyn9KR/+uhRRCKRSCQSicvE+YkCFf8rFhfDu5Y+zyMSiUQikUh8XozMI4+OxajauUp1LO9IJBKJRCKRuEhsRJSla4u2SnGihgXWGhiRSCQSiUTi02IhzsYU9n6HXEvF1/ykfGxEIpFIJBKJHyEGmyvSDBv2mV3E2PmOSCQSiUQicZEYlzMi9gminVLNSgCRSCQSiUTiMjH/vpelmJ8lzdihdxGJRCKRSCR+ihjBrU8dZ1x85PWGzcoTIpFIJBKJxBViPGvp12heIDJjs7L38JhIJBKJRCJxmdh6UjhDlYqiVW7Lym15IpFIJBKJxEVisbMzG33l9doC212+9nGGSCQSiUQi8XlxVptCLGfetoao/JhIJBKJRCLxM8SoQ+fIsEqkNywu4sRSRCKRSCQSiYvEwOYDXcwTseMVUJYnEolEIpFIXCbOgnPStg2Ph8zyLk5OJhKJRCKRSFwkFjaSzuGhct3zqWiV2dmiRCKRSCQSiSvE8/K4M990rIxdP+WztzwikUgkEonEtWJclhZ1jsbFULnmAcXe0xMikUgkEonEdWKc8210p44V9vzaJ4hEIpFIJBJXi8f97BqI2/I1P94nT/o+RCKRSCQSiSvEeNucLafH7/KJyhUp2z1LJBKJRCKRuFLMWK9XebTcvj1ljEgkEolEInGl2Ba4BnJtuv54NG9bpVntYyKRSCQSiUTi8+JWT0kKuwR3sfWunUgkEolEInGpOJSgMhVY9Km4yBNDs2rJRCKRSCQSiYvEMz9Gj/YXpHlS37HsQyQSiUQikfhR4jl1jPPH5KdIKSuXzYhEIpFIJBI/SuxPQjzj+hazACKRSCQSicRlYkv/nj3G3tV/Kh2LSCQSiUQicY1YztCY3t5m+5WC3/xbiEQikUgkEp8XnzlEIpFIJBKJRCKRSCQSiUQikUgkEolEIpFIJBKJROIvEv8Auzucfmyel04AAAAASUVORK5CYII=	\N	t	\N	\N	f	f	\N	160000.00	\N	\N	\N	\N	\N	\N
0e2ccd36-f0bd-4fa2-a4f4-f9c81218658c	0ddb8972-36cd-4b67-8887-829aadbdf942	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-07-30 08:03:02.828	12:00:00	94000.00	Cancelled	94000.00	46	2026-07-30 08:50:10.007864	\N	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADi0lEQVR4nO3ZQa6jMAwA0NyA+9+SGzDSDGDHCZ2/IpX+8wI1xPZzd1bbjpdjb0QikUgkEolEIpFIJBKJRCKRSCQSiUQikUgkEolE4m8UW40tpdXjWbHdj6siH+etiEQikUgkEt8X4313jMI4nlN07bL40IpIJBKJRCJxmRhNcul+EqVxsD/YtohEIpFIJBK/SszY1TOSz09RUaYlEolEIpFI/F6xPM5VaptUzMqIRCKRSCQSv0gcBhjF/GtRuwfYB3bemUgkEolEInGFWGIUf/4YWxGJRCKRSCQuEz/H38LYrErjM2X7TxMikUgkEonEReKeS6NnFo980W9M17EMVaYgEolEIpFIXCEed8anqpzycGx35BQikUgkEonEReKZsQ9ppckZ8W4mdrU5iEQikUgkEt8XS/c7ozqxd81qYyGbDUAkEolEIpG4QizLUmEHorwbJ5uvV0QikUgkEonvi7lqXIxigDzZGHmeWRCJRCKRSCS+L57OtTENTY6bLZPt83dlvSISiUQikUhcJgab0/a5WJLzLhbsvB+RSCQSiUTi2+JQup3iedGGT49l7Y7cikgkEolEInGlGERpF/VDk6Of7KELkUgkEolE4kKx3Wmzgu4PsZJSflXKeUQikUgkEonLxaHnrH7PQ8UxV7R+FCKRSCQSicQvEB/ZC8vHaDcjWt+ASCQSiUQica04W5sitlz6eCzzTL4QkUgkEolE4vtixw6l5aIsWjHt8dEmEolEIpFIfF9s/2LcmEr3uI2Lx10spxCJRCKRSCQuEgsx2G2+SpUtKt/GuzwjkUgkEolE4tti6+POGKsiyj41G2+rFUQikUgkEolvi5ntlqXYovI+VW5j0JhiPiORSCQSiUTiEjFWqeLkR0zW+sbFLl2IRCKRSCQSF4lRP9udzpS46DpFchllMhSRSCQSiUTi2+Iscm53nO1TQ8o1WW5BJBKJRCKR+L7Yamx3u+vTQOxD0bCaEYlEIpFIJH6BONuiZmtTZ/ftWnTJdl7DiEQikUgkEpeIZ/+tL2j3ihSPo3ZquUEEkUgkEolE4leK+XbG7n1yvGvPkxGJRCKRSCR+h1gaj2xUDNieWSKRSCQSicS14myAUpUna5nI9vgNiEQikUgkEheKJbZ8cReMy9LDRexnRCKRSCQSiWvFd4JIJBKJRCKRSCQSiUQikUgkEolEIpFIJBKJRCKRSPxF4h8XSclA4xJQ0wAAAABJRU5ErkJggg==	\N	f	\N	\N	f	f	\N	94000.00	\N	\N	\N	\N	\N	\N
c1106f3d-8095-443e-b035-2df761fcac6d	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-07-26 12:34:21.593	10:00:00	160000.00	Completed	160000.00	57	2026-09-02 11:13:06.482316	\N	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADkUlEQVR4nO3YQa7bMAwFQN1A97+lb+ACrR1SlJKiGyvFHy6C2BLfMDsi7Xy4jkYkEolEIpFIJBKJRCKRSCQSiUQikUgkEolEIpFIJP5EsdXqf260/HF1/TmNq9e7dl1ZRxGJRCKRSCRuE+P9/biyx/6WBxg+1slEIpFIJBKJm8Qg4trvN0GUAT6wcxSRSCQSiUTit4gXdrT5XmHPESMSiUQikUj8H8Rgc+v5Es88GZFIJBKJROKXih8e20uMRSv+PIrJPkcRiUQikUgk7hBLxYr07x9zFJFIJBKJROI2cVXZbuPaVNar4fKHIhKJRCKRSNwhlv+DhhUpsJaq5wGue0fuzZMRiUQikUgkbhJLUh5gfiwfq3lyaCcSiUQikUjcKE5d4ZxZzFMM05aDSCESiUQikUjcK+au1oaD1SpV0vsipWBEIpFIJBKJ28ThMAfPNYbc6W38FtXTZSKRSCQSicTnxUi6/zcq6aX/7T41nRKJRCKRSCTuFHNmVM/s9Vh2pxzXckqfZiQSiUQikUjcIebgdu1JOaTng5x0jN9WV04ikUgkEonErWJZm+aQnDQPlS/feeUbkUgkEolE4g7xahg2oZgip6+cue361sZTIpFIJBKJxOfFEhxVTuNdiB9mLEUkEolEIpH4vDj13xvTOmTYp8p4q5GJRCKRSCQS94ptrNI/Xel/683TEolEIpFIJG4Tj3ytNFwhx5jaF1fmKCKRSCQSicSvEiPzOlitV/fpNFQcxD0ikUgkEonEneJ4WAfIIbcd1RYVAUQikUgkEokbxWgdiLDLtxx8jFJ/RQ2PRCKRSCQSiTvEy1ltVkNmbGAf1rD+wtq4gRGJRCKRSCTuENvUFZvVVWXlGqZd9S4mIxKJRCKRSHxe7GPDPUU4gZUrr7hhi1q9IxKJRCKRSNwmluqLzGO079N1wJ1CJBKJRCKRuE1cVd62biLs1WSrrSwXkUgkEolE4vNiq9VzcMGmxzLeMOP7XY5IJBKJRCLxGXFYgkrr2/WqDFU2q3yZSCQSiUQicaeY0/s4RYmLAWLa246U6bcQiUQikUgkfpv49rEMtZ6MSCQSiUQi8dvE6T+i87qyaitiySMSiUQikUjcK64HaFPIq7/lntU/SNMVIpFIJBKJxOfFtm6YiHnbyjMObTmKSCQSiUQicZP4TBGJRCKRSCQSiUQikUgkEolEIpFIJBKJRCKRSCQSiT9I/AV36T88w+R0nAAAAABJRU5ErkJggg==	\N	t	\N	\N	f	f	\N	160000.00	\N	\N	\N	\N	\N	\N
046f33ec-d9c1-479f-b29f-a08ae9869902	0ddb8972-36cd-4b67-8887-829aadbdf942	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-05 15:15:41.721	08:00:00	88500.00	Rejected	150000.00	130	2026-09-07 09:28:31.227899	\N	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAAE20lEQVR4nO3dUW7jMAwE0N6g97/l3qCLTSNIImkn+1HTQJ8/gjiNNC5Bzoxpy/n4unj78wFRVGWO6sA5eJV20Eeug7PiHjlk5x3OrZw/OkfWedBd0UHSJdMl0yXTJdMl0yXTJdMl0yXTJdMl0yXTJdMl0yX7jV2yj7h97p+N3X+jvl8enz3HPv76eTIVRFGVOaoD5+BV2kEfG13H/HzbnQMnzvoy53wcytlUEEVV5qgOnINXaQd9bHMdq4sKc26I6zaPIozddiGKqsxRHTgHr9IO+ng/1zFbRtNordiPd7O1dDAVRFGVOaoD5+BV2kEfb+U65iTrxGP3LUMGUVRljurAOXiVdtDHe7iOdADrd7fm0YHRSsPCzBBFVeaoDpyDV2kHfexwHWELPum/XvJUEEVV5qgOnINXaQd9bHMd1VZbqa2/NLtK6VahaoMoqjJHdeAcvEo76GOH65g3/oyZQnuo/kroFuVhx84KoqjKHNWBc/Aq7aCPV7iO+bX1PqA5yegM1RfJti0dGURRlTmqA+fgVdpBHztdx3OSfKErOKYAG1ayvzJfEEVV5qgOnINXaQd9vN51VEu6gos6WeKeF3zVDyqEKKoyR3XgHLxKO+jj9a4j+aQ5dCAmUzQhNpx00xBEUZU5qgPn4FXaQR97XUdwQi/Xqh8arQAGUVRljurAOXiVdtDHXtdR94i2A6j7RnmZV7isBlFUZY7qwDl4lXbQx/u4jnmhq/oJrc2GVWPXdtOr9VYQRVXmqA6cg1dpB338cddRDw3L1POclQ2rLpxBFFWZozpwDl6lHfSxzXWsdigfQAV7vqY9mC+IoipzVAfOwau0gz42uo7tacvrqGr51va94MXCVyCKqsxRHTgHr9IO+tjrOsKtPfV02/ZqnXv9v0AUVZmjOnAOXqUd9PFq17GCBReV7dV6UGE118EhQxRVmaM6cA5epR30sdd1pHXpE+LgByXSmvaBGMwXRFGVOaoD5+BV2kEf21zHtEOrszq8GWj7crBStQ2DKKoyR3XgHLxKO+hjk+sInaGTdxkimarwtB+IoipzVAfOwau0gz52uo6Thw1u7wLE3A1HljpSEEVV5qgOnINXaQd9bHMdwwkliOSOxmfVr2cd9pcgiqrMUR04B6/SDvrY5DqqpwuGO5rTL0181TgnsBBFVeaoDpyDV2kHfbzedVQrsp5Dqxt/wuGNLSFuDgqiqMoc1YFz8CrtoI8drmOOqptCwUodPIqwmgWiqMoc1YFz8CrtoI+9rqPa0gqv6bu2O4fSgc6xr1eyQxRVmaM6cA5epR308Yddxz54c0yVlcoTp2f3bH+AKKoyR3XgHLxKO+hjo+uons6TXdS7T1tONxJBFFWZozpwDl6lHfSx03XU3ikYrfFZeFf5s/S/QBRVmaM6cA5epR308R6uI4ClrlKYOLioYbn2XYiiKnNUB87Bq7SDPt7BdSTvlC+IrRO/8YPpEEVV5qgOnINXaQd9bHIdle2pHshc39Z84KcgiqrMUR04B6/SDvrY7TrCttmm6vGEJxfTThwYRFGVOaoD5+BV2kEfr3Yd12wQRVXmqA6cg1dpB33kOjgr7pFDdt7h3Mr5o3NknQfdFR0kXTJdMl0yXTJdMl0yXTJdMl0yXTJdMl0yXTJdMl2yX9Ql+wviRNV2SHBL3gAAAABJRU5ErkJggg==	-61500.00	f	\N	\N	f	f	\N	88500.0000	\N	\N	\N	\N	\N	\N
98a90c97-1cfd-4eb6-8c85-33d6ffbac9bf	0ddb8972-36cd-4b67-8887-829aadbdf942	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-07-22 02:34:45.682	17:15:00	94000.00	Completed	94000.00	46	2026-09-02 11:13:06.482316	\N	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADi0lEQVR4nO3YQY7bMAwAQP1A//9lfpACC8ukKO0ueolcdHgQbJnkMDci7f3heDUikUgkEolEIpFIJBKJRCKRSCQSiUQikUgkEolEIvF/FFuNntJSfL3GEXfX02+tiEQikUgkEj8vxv14jSZXz37nFazPZWsrIpFIJBKJxLPi1Waqyk8xwJgiH5O4a0UkEolEIpH4FHHOHT3b3L0RiUQikUgk/qPichfEOHIQiUQikUgkPlLcveanqfGFxVPL69W+M5FIJBKJROIJsUS/V6S/P9ZWRCKRSCQSicfEXXyl79am8iE7/ceORCKRSCQSiSfE16bJOHLKeMpEDPXKA+QKIpFIJBKJxENiJlq2Y5Qr75t59ncxFJFIJBKJROIh8b5qe7vfX6cmkbd8mCYjEolEIpFIPCaW0jzAGrmiz92nGTNGJBKJRCKReEj8Srs6le7TP0hlvcpEm511DSMSiUQikUg8JkbuZY/cvEWtG1Nm+74VkUgkEolE4kEx2IgQR884IiIlyq5j6kIkEolEIpF4QozSsJcPr9xuuZsalHmIRCKRSCQSj4mLE0tVvxu3ZbOKir097ohEIpFIJBIfIC6dRuwWqOyMyXJFIxKJRCKRSHyEGO3KU8Z+Jqa8zQ8iEolEIpFIPCcunca2VWIp+/WOSCQSiUQi8YR4fRxOvEZVrE2xWc1/D9V5NilEIpFIJBKJnxb3BfG6Llr5td8zvucnIpFIJBKJxAeIo75UReR9qgwVd+tkRCKRSCQSiQ8Qc8ZrFl9L47I2tU3EoEQikUgkEolnxbxZBbti5W6JMgWRSCQSiUTiSfFq956dUjDlLVPEXc8YkUgkEolE4gPEsizlrz0fQZQoKWVaIpFIJBKJxDNiT/ctF5R/i3quyOPtpih3RCKRSCQSicfEkZFfpyPvTu9L3A2QoxOJRCKRSCQeFHeRN6spAis9yyhlPCKRSCQSicQT4tK1NFmXqrtJTY4dKzY1IpFIJBKJxIPi1CmqdkR+ir+WyqBEIpFIJBKJDxKjqmxReb1qmdh9XexOJBKJRCKR+EQxE31OLkN9MwqRSCQSiUTi88Sv+uuY8nZlkVymJRKJRCKRSHyAuHTq92vL7e769p5jmbvNKUQikUgkEoknxBJT49yk5+Sc0pfJNq2IRCKRSCQSPy1+JohEIpFIJBKJRCKRSCQSiUQikUgkEolEIpFIJBKJxP9I/AM6ycAkRxf83QAAAABJRU5ErkJggg==	\N	t	2026-07-17 14:35:26.770079	\N	t	f	\N	94000.00	\N	\N	\N	\N	\N	\N
c2a91a31-1018-4a5e-a8e1-36e7249fecee	0ddb8972-36cd-4b67-8887-829aadbdf942	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-07-21 00:00:00	20:30:00	12000.00	Completed	12000.00	12	2026-09-02 11:13:06.482316	\N	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADmklEQVR4nO3aTW7bMBAGUN5A97+lbuCiqeT5IR0gGzFA3ywEy+R8b7IbuB2vh+scRCKRSCQSiUQikUgkEolEIpFIJBKJRCKRSCQSiUTi/yiOXkc9iNfjevztHzVuXKcfoohEIpFIJBL3iEfujtbpypdzXmETdqyjiEQikUgkEveKV8xRxbOy5UrYq95pACKRSCQSicTfIWZirtix4t73UUQikUgkEom/TCzL0ip99QsSkUgkEolE4q8Sp9f5H8SmR3S0KVbJRCKRSCQSiTvEVm1P+tFjjiISiUQikUjcJn6sSIpVKoe0aY9FRBSRSCQSiUTiDjF+LTrfrYXIa9NqlZqxK49IJBKJRCJxr3iz06d1a5nsY8d0hUgkEolEIvFpcb073c56i/p+AzuWgxKJRCKRSCQ+L45rE2rrVfTne6u2D2L0EolEIpFIJO4QW38m5k/XlYg787RtqSISiUQikUjcK2Zs5P6cPurpKwdPl8OJIhKJRCKRSHxevPqP+rhb43FlfrjS/oKJJRKJRCKRSNwmjsU+dbwPXpUd+co7s4ROr0QikUgkEon7xGmLGrkr0j8uWtPwRCKRSCQSiXvF833jzHfzAPe9mCx/d9SUV20jEolEIpFI3CS2w6/E3Hq/TgtUOG2eu4hEIpFIJBL3irET5fTW0DLbKnVOQy22MiKRSCQSicTnxVc+zA2FjdeIy9+Nya7TEolEIpFIJG4So6FNcdbMMlQeoFT7M4hEIpFIJBJ3iyvnmE7beNHR9rMMEIlEIpFIJD4vNiK3nu/g1QBf96IadhKJRCKRSCTuFpvdHvl05KFq3Igr2T7SFSKRSCQSicTnxXmBml4jLjLPmj7qtlWmJRKJRCKRSNwm5oZ5s7pOS/AU8HENIxKJRCKRSNwkZiKCQ2zpM7sa79vNikgkEolEIvEZ8f3VvVkFW1pjqPxpVWUAIpFIJBKJxN1iqbZo5d+I7smmQdtmVaYlEolEIpFI3CGOXm2Vmjerb17b8kUkEolEIpG4V4zvW+tHrDirzWpKJhKJRCKRSNwkjn91TANccXe19HgdtV69iEQikUgkEn+HmB8FC2cdUMSfbFZEIpFIJBKJT4lBtP8CdItX3DxFY4lEIpFIJBL3iqsBWlxOuqeI4EhpqxmRSCQSiUTiRrHVamNabVZzvWoRiUQikUgk7hWfKSKRSCQSiUQikUgkEolEIpFIJBKJRCKRSCQSiUQi8T8S/wBqQh02iUiMRwAAAABJRU5ErkJggg==	\N	t	\N	\N	f	f	\N	12000.00	\N	\N	\N	\N	\N	\N
8a2e0d2e-8ecf-4f89-9fa3-354275e60f02	0ddb8972-36cd-4b67-8887-829aadbdf942	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-05 15:15:41.721	13:00:00	148500.00	Completed	150000.00	130	2026-09-04 16:05:31.58421	https://res.cloudinary.com/devu5qabc/image/upload/v1788537687/fb52a6b8-2488-4ed2-bd7a-ec636d9ac889.png?cors=anonymous	https://res.cloudinary.com/devu5qabc/image/upload/v1788537918/fc052d46-3d97-4fc4-82eb-adf8d60a483f.png?cors=anonymous	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAAE1ElEQVR4nO3dW27rMAwE0Oyg+99ld5ALtA0kkZTS+xEzQI8+ijxq0SDImfHIkW/3i8fnTURZVTm6A+bAVdyBH6kOyop6pJBdd7i2cv3oGpnzwF3hIHHJuGRcMi4Zl4xLxiXjknHJuGRcMi4Zl4xLxiX7iy7ZLY6P7/+Ir37eLkf8fPv1ajuViLKqcnQHzIGruAM/tqmO8fnj7RxsmW5+9fh2HFFNJaKsqhzdAXPgKu7Aj72qIxwaxiyqPuaI4RwPU4koqypHd8AcuIo78ON7qI5gBM2GUj4ihRVRVlWO7oA5cBV34Md3Vh1ZQKVXw19aFtNElFWVoztgDlzFHfjxrVRH/XYzZ7pf6Otfnk0loqyqHN0Bc+Aq7sCP16uOMIK8+q8/eSoRZVXl6A6YA1dxB35sUx3V2B5a/eorzV4NEWVV5egOmANXcQd+7FAdy7pX9QuvMcL51CbT462IsqpydAfMgau4Az/2qo46zrJcNv8Zn40Jqh+GiSirKkd3wBy4ijvwY7vqCEcFnZSODwqsMpnqCUSUVZWjO2AOXMUd+PFq1VFNHNRWMI9mZbTEricVUVZVju6AOXAVd+DHJtVRL5LdC52U9dRhG+ajnyOirKoc3QFz4CruwI9XqI7ZAAqbKi/OUC2bzk6TiLKqcnQHzIGruAM/dqqOw4Oz0u6Ci7I6ra2lsxVRVlWO7oA5cBV34MfrVUewfYKyClIofJaEVhVMRFlVOboD5sBV3IEfm1THfPyy7hUmDvcLhV99Hbb3EVFWVY7ugDlwFXfgxybVUW2zE2yk+VX+rVc673ECIsqqytEdMAeu4g782Ks6xkLXoo4qyyjZTXkv5rRZj4iyqnJ0B8yBq7gDP3aqjnr25bP5zPLuPNV8IsqqytEdMAeu4g782K068haD4abneeK8F3M6qTHps3UrEWVV5egOmANXcQd+fKXqqCTS0B/VxGHOw63TIsqqytEdMAeu4g782Ks6sgEU4gRltf05+7wpYTplEWVV5egOmANXcQd+vFp11HsnPzWF0hfLeP5ELRFlVeXoDpgDV3EHfny56kj6YxwwNNYjRPisvmE63+AjoqyqHN0Bc+Aq7sCPHaojqaj8tIjtv9TyarhCIsqqytEdMAeu4g782K46wgjThZueU9iwXLachYiyqnJ0B8yBq7gDPzaqjmD2LCGSTtrs7BO0WBoiyqrK0R0wB67iDvzYpjo2Iz1pImisLKrmU/nl/jkiyqrK0R0wB67iDvz4EtVRjWq1q3rK1naDr1ltiSirKkd3wBy4ijvwY5PqqDTRwQpazmeeID9u4tf354goqypHd8AcuIo78OOLVMciZw5iKeyuU62j3XeP1RJRVlWO7oA5cBV34Mcm1ZHUUdhUOdwH9LCH5tjBngkKSkRZVTm6A+bAVdyBH99KdQQ9tRVa51MWUVZVju6AOXAVd+DHN1Id1RPS03Ow0pxLxEVjiSirKkd3wBy4ijvwY5vqOM+Unos+RrVIlk9PRFlVOboD5sBV3IEfe1RHGHl9LBlFQ4Ytr7ZrZiLKqsrRHTAHruIO/NihOq4ZIsqqytEdMAeu4g78SHVQVtQjhey6w7WV60fXyJwH7goHiUvGJeOSccm4ZFwyLhmXjEvGJeOSccm4ZFwyLtkfcsn+AUhUDn0HsFPKAAAAAElFTkSuQmCC	-1500.00	f	2026-09-04 23:01:04.341664	2026-09-04 23:03:23.039699	f	f	\N	148500.00	\N	\N	\N	\N	\N	\N
36100c80-a36d-41e5-a65f-822e3cc830a6	0ddb8972-36cd-4b67-8887-829aadbdf942	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-05 15:15:41.721	08:00:00	168500.00	CheckedIn	200000.00	150	2026-09-08 11:03:22.5047	https://res.cloudinary.com/devu5qabc/image/upload/v1788538405/58016475-029c-4e24-9b63-308dcb54e242.png?cors=anonymous	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAAE3klEQVR4nO3dYW6jQAwF4N6g97/l3iCrjYI8YxvarZQZpH78qEKAedSy33sYmHw8Fi9/PiCKqsxRHTgHr9IO+sh1cFbcI4fsusO1letH18g6D7orOki6ZLpkumS6ZLpkumS6ZLpkumS6ZLpkumS6ZLpkumS/sUv2kZfPccO/fYcN6bvx09VQEEVV5qgOnINXaQd93OM64vtjNY55jvka+LlfAvucD6tDQRRVmaM6cA5epR30ca/rGL3TNEhBfB0/nEpC7IaCKKoyR3XgHLxKO+jjXVzHdVep7BKHQRRVmaM6cA5epR308fauI7pFU9/o4nYZRFGVOaoD5+BV2kEfb+k6ympAPMc8TFXXQRp3+fH9R4iiKnNUB87Bq7SDPr7DddRu0Y//1KEgiqrMUR04B6/SDvq4zXV0S2oU9c87R98ojd4tEEVV5qgOnINXaQd93OE6pjGTT+oGHiG622VxohBFVeaoDpyDV2kHfdzrOsJPPfd9/E9naNq5W4UoqjJHdeAcvEo76OM219Ebozh08kll4HoWyZ9BFFWZozpwDl6lHfRxm+vo7oVdTNHT+a7uzI7zhiiqMkd14By8Sjvo4zbX0eNM75Uns1S+Cy+WTNpX/RyIoipzVAfOwau0gz6+03UUJ5RMVezS3R+b/FT36xMQRVXmqA6cg1dpB33c5jouRjpOpTwIfXwXOOOx0x+IoipzVAfOwau0gz5udB1hqrolTa884ZTXvOIc06AQRVXmqA6cg1dpB31c7zr6SXiO1e4BodQ36u6ZNY8UQRRVmaM6cA5epR30cbXr6CDGblF5JT2POR4bg5YmE0RRlTmqA+fgVdpBH9e7juqTyob053EJkSYlhCiqMkd14By8Sjvo4zbXkd5G71zUyXfjXbE0j/PFnTKIoipzVAfOwau0gz6ucR3jSOnpn1PLdfJs0OUuEEVV5qgOnINXaQd9XO86yo9fZdjxxlk4pulBojQURFGVOaoD5+BV2kEf7+M60hvq16tp9MmBne0MUVRljurAOXiVdtDH1a4jNnYNoHFrfZ29TMgcNuzYClFUZY7qwDl4lXbQxxu4jtEn1U/d/bH0CFA6eYiiKnNUB87Bq7SDPt7AdZRB0ow9sXQ/KTp5rG92kCCKqsxRHTgHr9IO+rjGdRRPNK0mxIAYR5nm7oEoqjJHdeAcvEo76OMtXEedWbncODs2dDMTfuONd4iiKnNUB87Bq7SDPm5zHaePAAVi9116/LksX/3uA0RRlTmqA+fgVdpBH9/pOrpl9FjHcvr0T7p7BlFUZY7qwDl4lXbQx7u4jtNBytw9J7PzXE9oCFFUZY7qwDl4lXbQx42uo3u058AeP3V31KqLgiiqMkd14By8Sjvo461cRxr4dVQyUJMXi0+9NUv/C0RRlTmqA+fgVdpBH+/mOrrhytw9x5mdejGIoipzVAfOwau0gz7exXWMYNPsPP3ky+mxoG+9yQ5RVGWO6sA5eJV20Mc1rqMzIQF7Oh9h/zOj/UQ/EEVV5qgOnINXaQd9XO86ao+om44n7ZXMVzqzL5wVRFGVOaoD5+BV2kEfV7iONQtEUZU5qgPn4FXaQR+5Ds6Ke+SQXXe4tnL96BpZ50F3RQdJl0yXTJdMl0yXTJdMl0yXTJdMl0yXTJdMl0yXTJfsF3XJ/gJGgMY/KXY7qAAAAABJRU5ErkJggg==	-31500.00	f	2026-09-04 23:12:43.730916	\N	f	f	df8e6759-dc4c-4689-8bae-1adc75fd50ae	168500.00	\N	\N	\N	\N	\N	\N
6f816baa-48a9-477d-9700-db60482ab272	0ddb8972-36cd-4b67-8887-829aadbdf942	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-07-18 02:34:45.682	15:30:00	94000.00	Completed	94000.00	46	2026-09-02 11:13:06.482316	https://res.cloudinary.com/devu5qabc/image/upload/v1784363284/e6cebca7-aa83-44d3-9272-9bfeba4c7da4.png?cors=anonymous	https://res.cloudinary.com/devu5qabc/image/upload/v1784369618/eb6fc508-37fa-4c6f-a56a-838cdb71ba40.png?cors=anonymous	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADjklEQVR4nO3aQW7cMAwFUN/A97+lb+CiA9mkKHmCbkZT5HFhxJb4H7Mjmm7nh+vYiEQikUgkEolEIpFIJBKJRCKRSCQSiUQikUgkEonE3yhutfa/3/Z8vXx73erjrrYxikgkEolEInGhOKa3kCs44iI9XwlxFkUkEolEIpG4UmzNey/GwY/YmygikUgkEonErxSHtem4t60tXyESiUQikUj8P8TXS5yWBaoR43pFJBKJRCKR+H3iMEC0hj1P6rAxikgkEolEInGhWGrv2X96jFFEIpFIJBKJy8SfqvylbBy0ve7TbiKRSCQSicSV4rHVipCCxQI12Hseb5iCSCQSiUQi8fNiIV5d7VEyY57H/yC0T/KIRCKRSCQSF4nR9bg25SkCO/oBoi2iiEQikUgkEr9DbI+z3S3Bb74VbMtRRCKRSCQSiQvFWebYHwczO67EAM+bFZFIJBKJROInxIxdd4cda7vFo08Ptht08o1IJBKJRCLx0+LMDrG1Ro2n+UpMceafiEQikUgkEleIce3VE/2zpLxojUQepRSRSCQSiUTiCnHb6msJyVceXmd2/0okEolEIpH4aXFeRw7Jcce9cj0uWtc8RCKRSCQSiWvFdqMkjTWborV1UbnjJBKJRCKRSFwq7ufZH6aGEjzbnR5tIpFIJBKJxLViOHE3B18DRHDMGFfKwfC7EIlEIpFIJC4Tg40VaXwdRgk75tn6rYxIJBKJRCJxkRghUbNRIrP0xrcctROJRCKRSCSuF7d+TzpuuCxLEdxNlterwCKASCQSiUQica14tZau4sRkeXcqo8QARCKRSCQSicvFN+nXPLOhijMMTyQSiUQikbhW7A5jWcpJs2XpnNt5biKRSCQSicTlYvzLUDTkzKvKtyHl4ZRIJBKJRCJxmVji8gDnPcW4MbXofZisLGREIpFIJBKJK8RZlR0r95e9q2xlD9+IRCKRSCQSV4jznaisV0f/euYrcRC9eUYikUgkEonEZWJ8P3PXfbdejikKMcsjEolEIpFIXCtGVxaPzOYr5adS4xREIpFIJBKJ3yNej1Lt22zbCuI6JRKJRCKRSPxGMSp3bUPHVe1g668QiUQikUgkLhfnA5TM609jJX3o3c+xiEQikUgkEleIpfZ6t65NzZ69Hvej/C5EIpFIJBKJnxc/U0QikUgkEolEIpFIJBKJRCKRSCQSiUQikUgkEolE4i8S/wB+ENdV76T7awAAAABJRU5ErkJggg==	\N	t	2026-07-18 08:28:05.916775	2026-07-18 08:28:06.537574	f	f	\N	94000.00	\N	\N	\N	\N	\N	\N
c13ad8a8-e179-48c0-9907-af31451e0565	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	c2325bfa-edca-4803-92c1-9c2507f5b4a8	2adce07e-6ef8-4f7d-8920-accc288417e9	2026-07-25 12:34:21.593	16:00:00	160000.00	Completed	160000.00	57	2026-09-02 11:13:06.482316	\N	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADm0lEQVR4nO3ZS67bMAwFUO/A+9+ld+ACrW1+JKTooFaAdzhII1u8h50Redv5ch0bkUgkEolEIpFIJBKJRCKRSCQSiUQikUgkEolEIpH4E8Wt1/7nRjr+/vd6tueOuPchikgkEolEInGZGM/vY3auhi0+Wmb7mCcTiUQikUgkLhJb//Uint0VA1zHY8KOUUQikUgkEonfIjbiaj0ycR1bCpFIJBKJROLXi/ezvDvFAPeLNgWRSCQSiUTi94nD8ahds8yYJ0aZRRGJRCKRSCQuE1tF8L9/jFFEIpFIJBKJy8RZRf/T0H88yuO19FkRiUQikUgkrhCPScg+vB0u79cxT7HnoYhEIpFIJBLXipm4rw0b0zEJGQMu7KxbGZFIJBKJROIysbzMXcHe1dh4FrtYDFr3MyKRSCQSicS3xWgYsLZyxa9FIRYnR+3nWQ0ikUgkEonEt8V8d7sa2ts2Tww6vzKLIhKJRCKRSHxfjP62ReWP2b0zT5Gfjd+IRCKRSCQSV4jNiQpi5lyrVPTOXuRBiUQikUgkEt8Wg62b0NaSgp1tYPlKG5lIJBKJRCJxpfg5rgVHesxz9d7fIo9IJBKJRCJxmZiJsKPupHzlnGSWyjaRSCQSiUTiInEg7ppnxga2PZvVuFQNIxCJRCKRSCS+L0Z63B3E2VD7pLfcIxKJRCKRSFwr5rt7/UFp5mz18ux4PtvWh1+QiEQikUgkEt8Wo7J91m8ts2xWRCKRSCQSid8ixjo0ODHKltk2SqxX+cpW/wdEIpFIJBKJK8RhCUo1sGWAq3esPDyRSCQSiUTiSrHtSRk7n7ez3anV/oSWI5FIJBKJROIacXiZqo2Sj/vzLI57ziMSiUQikUj8AnHAZnFbfRtxRbxSiEQikUgkEr9ALK3BXltUW6XG4LjSUuqRSCQSiUQicYkYRMNyerlyHUvAUB82KyKRSCQSicT/Ls5q1tomG7C99sYGRiQSiUQikbhInKVH3Hypul88cdOhiEQikUgkEleL8fysXeXtsFmVAdpmdfYiEolEIpFIXCS21i3VXkNi0Yp5jm1rHX/frIhEIpFIJBIXiSMbIfERU8xYIpFIJBKJxO8T40WOi/7S1q4MgxKJRCKRSCSuFIekG4tjDjnq38KOTIRXrxCJRCKRSCS+L7aKzBLcbgU23GvjEYlEIpFIJC4S3ykikUgkEolEIpFIJBKJRCKRSCQSiUQikUgkEolEIvEHib8ACUnKPI7jRbIAAAAASUVORK5CYII=	\N	t	\N	\N	f	f	\N	160000.00	\N	\N	\N	\N	\N	\N
bf5f02bf-6294-4ce7-9097-07974db57906	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	84cc584d-335e-40a7-90c3-abebfe573b01	b53808e3-7219-4c65-899c-197f204e5581	2026-08-11 15:11:45.636	16:00:00	150000.00	Cancelled	150000.00	22	2026-08-11 12:03:19.420367	\N	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADn0lEQVR4nO3YQZLkKAwFUG7A/W/pG+REdNhGEjh7ejGmJvqxyACM/lPtFNU+L6+jEYlEIpFIJBKJRCKRSCQSiUQikUgkEolEIpFIJBKJf6PY6urnXfz6666H+vQuHUsUkUgkEolE4kZx3I/jEcUzM6XHNcRVFJFIJBKJROJO8QzsX8SIlcnqexSRSCQSiUTijxLH15I5Gki1RCKRSCQSif8PcRUyicfdyjHVEolEIpFIJP4UcWqg5+O1pqGqr8umZCKRSCQSicQdYlk91v/hzxxFJBKJRCKRuE383ToWrbRzxiri1xQikUgkEonE98WRea1f12U3jgMbx9WH58mKSCQSiUQi8Q0xDkbHPTG1szTu+p05Wnl4ErsgEolEIpFI3CQWttQX+3ycdlO3PYYSiUQikUgkbhSPdWZM+uSQeXaKzrWyQSQSiUQikbhJbJlN/0uanqS5a+q7xyORSCQSiUTiNjHfp/orrjjrx6sP8a8iEolEIpFIfFuMY1M7S8duaiANUOXdVPH7WY5IJBKJRCLxPxbjT1pn1XpYanH3EEAkEolEIpG4TYyZR5ywyvG8u4gxWY1jyXuarIhEIpFIJBLfF8dduzP7/WRgbfG43cQVRSQSiUQikbhXjEmJiGzarSq+jGFEIpFIJBKJm8RSMOJi5sMqLcdQIpFIJBKJxO1inInmyWrdQM937dx98jrfEYlEIpFIJG4Tk32+vRqId6OLI/98a55IJBKJRCJxmxjvC5a6KHZ8fK349agpRCKRSCQSiW+LBfuy6zmuLcarP5zliEQikUgkEt8RxzjUwuo3lu5Wj6cUIpFIJBKJxJ3i+TZNR/H4uTOH3e8P/a74ZGL6W4hEIpFIJBLfF8ecNIKv3egidvZtAosVT5MVkUgkEolE4otisotTkqbMNIZNRyKRSCQSicSdYpmTRlycmFLSNJC12F7skUgkEolEInGTuFrRvo65vpXMcZwwIpFIJBKJxE1iq2uescrjx38yxdoyXhGJRCKRSCTuEOcRaVRNH/rklEHr30xWRCKRSCQSia+Jo346tnuUKj8joKxVy0QikUgkEok/SIwDVIvO1Nl4fA1a4x2RSCQSiUTiDxPH208OSfYYtGKjn0UKkUgkEolE4iZxaqDHuCKW4Kmf1SISiUQikUjcIZY1h+QRaa59qHierIhEIpFIJBLfEN9ZRCKRSCQSiUQikUgkEolEIpFIJBKJRCKRSCQSiUTiXyT+Ay2IKwx+HWrIAAAAAElFTkSuQmCC	\N	f	\N	\N	f	f	\N	150000.00	\N	\N	\N	\N	\N	\N
0ecf1a61-9159-4bbd-9b2c-45a83bd42e4c	0ddb8972-36cd-4b67-8887-829aadbdf942	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-01 00:00:00	18:30:00	12000.00	Completed	12000.00	10	2026-09-02 11:13:06.482316	https://res.cloudinary.com/devu5qabc/image/upload/v1785169821/008264d0-776f-4901-882a-c0c1cb9d1f37.jpg?cors=anonymous	https://res.cloudinary.com/devu5qabc/image/upload/v1785169853/0a4974f2-b586-4aeb-8fc9-facc73b072b0.jpg?cors=anonymous	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADo0lEQVR4nO3ZS47jOBAFQN5A97+lbuABDMn5Iat6NiN60JELwaTIF1m7hGq8Hq5zEIlEIpFIJBKJRCKRSCQSiUQikUgkEolEIpFIJBKJf6M4eh3p2Hg/Yu/9K45eb9+//hRFJBKJRCKR+LwY+y3z+LB3A3lv1cocRSQSiUQikbhXDCIy39XE9aOIUxSRSCQSiUTiF4kj1b1cBV+dHUQikUgkEon/H3EOjkdUXhKJRCKRSCR+pbheBnHUt4W97D9FEYlEIpFIJD4vtlqNV//2MUcRiUQikUgkbhNXVc+msakl5paPZRKRSCQSiUTiTrH9B+yYQiYnviqd014siUQikUgkEveKOatMR9fVu6m8fNVzZeSa7hKJRCKRSCRuE496656YAovlqqnW7fovIBKJRCKRSHxezF+Q4lZho6nGtm6nz0jZIBKJRCKRSHxeLHauo+6VZTucG7gPR2dEIpFIJBKJO8S6f1+dQ/Iy5q4zizmvTGpEIpFIJBKJ28SR6sjB7f763NxPvkskEolEIpG4TSwVezE2NecTMlrAakkkEolEIpG4TZzGpjJKrdj4eJTf/hhAJBKJRCKRuElcnbjuN/FetkanqFe1iUQikUgkEjeJEXI5o9rjM0W1Knutx8VhIpFIJBKJxKfFNkDlX008P2/bADU+RIxhY7S/ikgkEolEIvF5cSyI8uvCyuE8hpVl/Bk/fUEiEolEIpFIfEbMdY7SylH7OXNczFjxlkgkEolEIvHLxNXYNK64KTj6GflanAubSCQSiUQi8QvEqCvkvhqP1ZE2fK1TiEQikUgkEreJd0VIdpodh1d11LdEIpFIJBKJO8W1c1csQ6xJPwxacYRIJBKJRCJxmzimWzFUTcNSicvOb3eJRCKRSCQS94jlbH4UJw5Pjd6/cp11j0gkEolEInGb+K55Thqpjl8yx7J+nayIRCKRSCQS/2NxVYG9g1s/ee4q7TW7tkckEolEIpH4tDh63ZltLxr4hJTDR230lbsgEolEIpFI3CPG/utzq1yNzqKVyy5YO5djiUQikUgkEjeJV8yRxcj8sZXmRMr0txCJRCKRSCR+lfjK6RPbjpxTAJFIJBKJROL3ifGhKN+/l9O1EONGOUwkEolEIpG4TVw10IalXGedtlod0xEikUgkEonEPeLqwuvDts7uASofifTosUQRiUQikUgk7hCfKSKRSCQSiUQikUgkEolEIpFIJBKJRCKRSCQSiUQi8S8S/wEet/0ZWLqkcQAAAABJRU5ErkJggg==	\N	t	2026-07-27 23:30:22.580338	2026-07-27 23:30:23.732401	f	f	41ddc49f-fbb3-436e-afa2-4c6a1b8e5099	12000.00	\N	\N	\N	\N	\N	\N
05d91038-03d5-4743-80d9-34020b38e94a	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-09 00:00:00	08:30:00	100000.00	InProgress	100000.00	175	2026-09-08 13:29:49.382081	https://res.cloudinary.com/devu5qabc/image/upload/v1788874184/611de840-4b96-4f6c-9367-47e77172294f.png?cors=anonymous	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADhElEQVR4nO3ZQZLjIAwFUG7g+98yN/DU9BhLCNI9K5OqfixcNoj/tFQl7Xx4vRqRSCQSiUQikUgkEolEIpFIJBKJRCKRSCQSiUQikfgbxVbXkQ/+1i5P76R/e9fjTRSRSCQSiUTiHnEIuT678/V2PQYnsLi2jiISiUQikUjcKV75x/pzhZXMn+4SiUQikUgkfoYYY1O5+lWVT8snkUgkEolE4oeLea/lKeq/HkQikUgkEokfJK4auCam/rY6veNm4iQSiUQikUjcL5b1/dj0/WOOIhKJRCKRSNwmrtfrvjAPUKXupygikUgkEonEbWL5eShGpC7Gmti4O5wuOiMSiUQikUh8WpzuD8SVNGfGjfjM2FRHJBKJRCKR+LQYcXmo6s6qJDfae1ztEYlEIpFIJO4VI316e43sMHdNrUSjHSMSiUQikUjcK+a48yrLI9JZrw57MUC9brMMX0QikUgkEol7xRLX10rMmQNRiolEIpFIJBL3im25jvGgfxbn2uuNRj9TK0QikUgkEonPi1GbH+Vg6KyUrIhrj0gkEolEInGnGIcl83rr2LqfSAkxeiQSiUQikUjcKUZ6ENOFknnmHstnKSESiUQikUjcKL65UMarXFfGsPO2y0BGJBKJRCKRuFd8LZxVZkxMPX3qosxiRCKRSCQSiR8kXm+xF8NS/NL0Jvhtj0QikUgkEok7xBUWj7I3DVBvD4hEIpFIJBK3iwXLby3PU1N6a+2Oa/GZi2MRiUQikUgk7hRzSOmihLzu9GO8Gy2XRSQSiUQikfi82O41hRzr0zEpEd8UE4lEIpFIJG4TjzGk35qCVz8ynbm9KCESiUQikUjcKxZ2ChnGq5xUxI6VaYtIJBKJRCJxt3iMn+2OW89JKTM3MI9XRCKRSCQSidvEVtexaKBnTlikl4CW64hEIpFIJBJ3iKtVpqjr0Q9y3HBQusiJRCKRSCQSic+Lra6eVNYiZG7lIBKJRCKRSPwk8chHq4mpjE05vQxaxc7zGZFIJBKJROIWcZ10TCWra7mVWEQikUgkEokfKd5l6X7eW5WUupNIJBKJRCLxM8W3wXNTcZq5n/4pIxKJRCKRSHxGXDUQcdP9KI4uWrbjgEgkEolEInGj2NZl063raiJi5RkrAohEIpFIJBJ3is8sIpFIJBKJRCKRSCQSiUQikUgkEolEIpFIJBKJRCLxF4l/AInQzT0sx8LgAAAAAElFTkSuQmCC	0.00	f	2026-09-08 19:21:51.219818	2026-09-08 20:29:49.382081	f	f	df8e6759-dc4c-4689-8bae-1adc75fd50ae	83200.00	16800.00	\N	\N	\N	\N	\N
2993e6d6-b957-436d-a0f2-b5d9782eeddd	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	84cc584d-335e-40a7-90c3-abebfe573b01	618e9f63-8360-4cf4-b178-9457dd66761b	2026-08-12 18:00:27.129	10:00:00	350000.00	Completed	350000.00	232	2026-09-02 11:13:06.482316	\N	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADk0lEQVR4nO3ZQW7cMAwFUN3A97+lb+ACqRxSlDxBN6Mp8rgwLFn6j9kRk3a9uc5GJBKJRCKRSCQSiUQikUgkEolEIpFIJBKJRCKRSCT+RrHVOv6eaPG1L4e9+NDfHqOIRCKRSCQSt4mxfy/Hsy3e+tchszzWyUQikUgkEombxHI/RqmSVMTS4zqKSCQSiUQi8YPEKS6I1aBFJBKJRCKR+H+JsRfpGbsbWN8lEolEIpFI/AxxtSy3SgMZa3kMWycTiUQikUgk7hBLHRn7x8ccRSQSiUQikbhNXFXc6VPU1TPzW3wo6asiEolEIpFI3CGW34OOHBJsfytE+Z/ZcINIJBKJRCJxt1jseVmIcqk70UAsiUQikUgkEneKX2HTYBRJLX+IuLwX/VzrPSKRSCQSicSN4mz3txI3vBU7hwZGJBKJRCKRuFeM9Hv5uJcfpeXbLo0SiUQikUgk7hGPhR2TVZmx7vSytxq+chGJRCKRSCTuECPkXsZejivTVh6bhsnqGK8RiUQikUgk7hTj2DXWSgynzF2rlolEIpFIJBJ3i8GGHZlXHpuyfY53h/Z+mKyIRCKRSCQS3yGuQq5UETxnrprK54hEIpFIJBI3ifnsUL2BoamSOTl3K1ctIpFIJBKJxB1iJLXvs2WUijpGYuisXCMSiUQikUjcK+ahahiv4mvPPBdJD04etIhEIpFIJBI3if1EuTC8xdWplfkwkUgkEolE4qeI/dg5ni0NzHvTD0/3Mg9aRCKRSCQSiTvFwKYaPuRR6sin2qKmc0QikUgkEombxFwxRb0emx5rCCASiUQikUjcJpbgvhfLcwyZkuqPTLm9/FcRiUQikUgkvl+MY+07Paao4ztu2Asni2X4epqsiEQikUgkEt8j3sdePMq5odHYe1FEIpFIJBKJm8T+uCuS8txVxqaWW2m1YtAiEolEIpFI3CSuapWZB6hzDL7fyl0ikUgkEonEvWKrFekPvwzlpCuL0cB0jkgkEolEInGHWO7HrfIL0uPbkBJ7uYhEIpFIJBI3if1yTEczm88NTvkQo9nLyYpIJBKJRCLxM8S4Ojh9Wc6dOYpIJBKJRCLxo8SvWyXkGqs0lZ0oIpFIJBKJxO3ilJTPttVevlPEaCUfIRKJRCKRSHy/WOoYPxyL4Ii77XItT2VEIpFIJBKJm8T3FJFIJBKJRCKRSCQSiUQikUgkEolEIpFIJBKJRCKR+IvEP9wETVHrhdWEAAAAAElFTkSuQmCC	\N	t	\N	\N	f	f	\N	350000.00	\N	\N	\N	\N	\N	\N
88642819-451f-4089-ac79-9bc8ef7ed4c4	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	\N	2026-09-10 10:41:07.555	10:00:00	84000.00	Rejected	100000.00	175	2026-09-07 13:40:55.125134	\N	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADjElEQVR4nO3aQW7cMAwFUN/A97+lb+ACiV1SFCdpFx25yNNiMLKl/352xCDb+eZ1bEQikUgkEolEIpFIJBKJRCKRSCQSiUQikUgkEolE4k8Ut7r2zxOf6+Nbt72exeH9VRSRSCQSiUTiMjGe39tIv7b56tbdeBlFJBKJRCKRuFbcPlek36NUtj++xY29vztFEYlEIpFIJD5IzNw+nYttbkYkEolEIpH4f4iR3v1kVPocRCKRSCQSiY8V++2xbeVbvO3OfRlFJBKJRCKR+H6xrBiR/v5jjiISiUQikUhcJnbrOnvkfwaK4PyT0db/yDQtIpFIJBKJxBXii+ArLgqc+cUXpe4WRCKRSCQSiavFSIqQM4svM8s8lfts41sikUgkEonEFWJcvYN74vxdqis6PPt+liMSiUQikUj852LEXceGiamI5VkZtPJfsBGJRCKRSCSuFrMzrAiZtoPT1ZtGLiKRSCQSicT3i+VCFIg+X6wpeLhGJBKJRCKRuFLMmTEx3QXiRY6bq/Tl82EikUgkEonEd4vZOUbsdmIb57KzNVsikUgkEonEZ4h522WWiWmexQo7JROJRCKRSCS+X7zmqSDKL0PzZFXS89t5ICMSiUQikUhcK+Z1H/uDb7nFnqPOuohEIpFIJBJXiIOdW8zp3eEgMnZs5a8iEolEIpFIXCLmwWgfiaMJeTFAZbt0JBKJRCKRSFwh3venUerj/pFDphtBxOFjqkIkEolEIpG4UOywO2lKjzFsa4i+I5FIJBKJROK7xUjqbmV27/vkb1F++DOIRCKRSCQS14jxvFyNuD2zJTjXIxKJRCKRSHye+PJsKXXkPrnF3JZIJBKJRCJxtThcLWz+iLi5YxeVF5FIJBKJROIKcYibZqfIHEau68aZS5VtZolEIpFIJBIXiYXIH0NwnMs3uhUYkUgkEolE4iKxW9OcNMd1Ra8b5QiRSCQSiUTiIrGLmwaooUB5m5vd9eIFkUgkEolE4kJxz6/yKDUMS33w8KwcJhKJRCKRSHyAGMSUebT37z4xWcUaXhCJRCKRSCQ+URyGpS4ujuRflYZkIpFIJBKJxEeJJTO8HHxM23KYSCQSiUQi8QFiVyAy89ttDL7r5WtnrkwkEolEIpG4UCwrJqthdipzV5Qq9aY+RCKRSCQSiYvE9ywikUgkEolEIpFIJBKJRCKRSCQSiUQikUgkEolEIvEHib8Aae8SO4ZgnTAAAAAASUVORK5CYII=	-16000.00	f	\N	\N	f	f	\N	67200.00	16800.00	\N	\N	\N	\N	\N
0d0db69e-14d2-45fe-876d-31767e9f553c	c653b310-12c6-4a61-91b9-83b85af24635	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-08 00:00:00	19:38:44.453288	300000.00	CheckedIn	300000.00	170	\N	\N	\N	\N	\N	f	2026-09-08 19:38:44.453288	2026-09-08 19:38:44.453288	f	f	df8e6759-dc4c-4689-8bae-1adc75fd50ae	300000.00	\N	\N	\N	\N	\N	\N
0f7b2049-386c-4142-8f2d-8f768632b078	d99f9bee-3505-4f1a-9627-461402ca1afc	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-08 00:00:00	19:42:28.426575	427500.00	ServiceCompleted	450000.00	250	2026-09-08 13:22:10.841443	https://res.cloudinary.com/devu5qabc/image/upload/v1788871751/4e103b87-1db7-41e4-937e-10eb185d3e54.png?cors=anonymous	https://res.cloudinary.com/devu5qabc/image/upload/v1788873729/4cc1d41b-9725-4def-b3d9-7efabcc818c3.png?cors=anonymous	\N	-22500.00	f	2026-09-08 19:42:28.426575	2026-09-08 19:49:12.683557	f	f	5499758b-4f1c-47b9-86a9-207bed3c96f2	427500.00	\N	\N	\N	\N	\N	\N
58fc44bd-2ed3-40e1-9fbc-c5966335e604	0ddb8972-36cd-4b67-8887-829aadbdf942	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-07-24 13:49:02.978	11:00:00	70000.00	Cancelled	70000.00	22	2026-07-24 11:20:17.679876	\N	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADmElEQVR4nO3YQbKkOAwFQN+A+9+SGzAz3YBk2VXds8FE/PSCANt6qdopqh0Pr70RiUQikUgkEolEIpFIJBKJRCKRSCQSiUQikUgkEok/UWx1bfdB9xaP/+rb/PNDFJFIJBKJROIaMfYj/d91JpW3oz8dm5onE4lEIpFIJC4Sz8BtXvorPd76pDbUdp9EIpFIJBKJrxL/dqiK2i9RRCKRSCQSia8Sjz5uH+wyY5UGiEQikUgkEt8iDp8dEXvRQGnlSxSRSCQSiUTiMrGsGJH+/2OMIhKJRCKRSFwmztee7bOzKzNPVt3lr3lEIpFIJBKJz4vln6GtDxnjyhQVe/lK9EMkEolEIpG4SMz1IxaP+ZUj1w5lRCKRSCQSiSvFzIbdWtdU64ntPtrz5Y8BRCKRSCQSiQvFkhkhrc9sedAaxGLnzohEIpFIJBKfFvN+VzX8edQdDINW98dT+QVEIpFIJBKJa8R2Bhdint7uKzkuEcHmH0IkEolEIpH4vFgOy8SU7S59CNj7HocrRCKRSCQSiUvEYRwqe9fBbHYa0meLSCQSiUQi8Xmx/V7b/daGBopdRq5SMW+ASCQSiUQicZn4oSrXl4OtP+3WGUUkEolEIpG4UpzfjQFqm+/FZ6mN/olEIpFIJBLXivnalXQ+jqE+iNxPG4av3BmRSCQSiUTiIjFPTG1ekPfG9KGzceQiEolEIpFIXCMed9LepgeDXdg8QHVdfJqsiEQikUgkEp8QY4rKj6uB2Wd5y/fGWYxIJBKJRCJxmVjGoeji+xqI0m1ZRCKRSCQSic+L542LzRPTMWTOD9rdcveH0udZjkgkEolEIvEJMbNHf7eMXNe/RfnzQyu5eSKRSCQSicRl4pUUK5fmqhZJw2nptiwikUgkEonE58Vc3yb2nolc0fp70RSRSCQSiUTiO8QhZGwgD18d9qcGiEQikUgkEpeLQwNRut/BVz93SGXLvf4ykUgkEolE4tNiq2s74+bzVIxcpbYbyIhEIpFIJBJfIcb+cVddbH4LZ+sf0ehx97PnPSKRSCQSicRlYlQFMZxeSfOybsU9IpFIJBKJxFeJ8TmMSMfkctwL8bgPiEQikUgkEl8lzuK+veWRq2OJRCKRSCQS14pDA2Wv3enjlZy+D30TiUQikUgkLhTL6rASElNUH9fKg0gkEolEIvEF4jOLSCQSiUQikUgkEolEIpFIJBKJRCKRSCQSiUQikUj8QeI/1EVh5kK8VZ0AAAAASUVORK5CYII=	\N	f	\N	\N	f	f	\N	70000.00	\N	\N	\N	\N	\N	\N
37c9450a-fa86-4d51-b1b8-17cd94c18426	0ddb8972-36cd-4b67-8887-829aadbdf942	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-18 00:00:00	22:15:00	50000.00	Cancelled	50000.00	20	2026-08-18 15:30:07.784957	\N	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADnUlEQVR4nO3ZTdLbMAgAUN1A97+lb5BO+8UWILk/i1rp9LHwRBHwyI5x2uvhOBqRSCQSiUQikUgkEolEIpFIJBKJRCKRSCQSiUQikfg/iq1G//5d/0o7jy1++nGR26W81IpIJBKJRCJxo9hj9TiWizjemOccIA666kwkEolEIpG4SRydihi795zc3uvVVDG3IhKJRCKRSPww8fzuVhxHIpFIJBKJxH9DXDWOMf4pO6Y3SEQikUgkEokfJU4D9Lvblt8bHdOg685EIpFIJBKJO8QSqf4PH3MrIpFIJBKJxG3ibbwLzndE7y2qLY4j+TaIRCKRSCQSd4hn6YhyMY7vR5nsR/Srol8Xd5sVkUgkEolE4hNieQEU16Z+VR15nsKWinNGIpFIJBKJxL1irHrFgl/FEQeIZWm9IhKJRCKRSNwm5v0n7ETlscLKKhW7TMsXkUgkEolE4tPihLW4LMXjjV2SxwD5txCJRCKRSCQ+Lx4xreTG9WqklA2sXBxxRiKRSCQSicRtYltGv0ZJPcdFaTzyyrREIpFIJBKJ28RpExprU6pfz9hzl9sgEolEIpFIfF5sNRIx1qb3UO1aqkqDNG38BUQikUgkEok7xUXGuV7dblZllHScaolEIpFIJBKfF+PG9JqaTESbJpvYFvsRiUQikUgkfoR4xEcsHctSu27HKpVaxTwikUgkEonEnWIsHZ1aZOMor3XeaFU6E4lEIpFIJG4Tp/pXXowSG6PHaacZiUQikUgkEreLsf78NAri3jWPUj6VLkQikUgkEom7xdKudDofI+Jxnmc1PJFIJBKJROIecTQZ9Yl9p7wWt694jEQ5EolEIpFIJO4Qe71MxG30q7Yc+2QTiUQikUgkbhQTMdnn3hU/nSmx1Zk8OhOJRCKRSCTuFafuaYDVUPEirVLjIgaRSCQSiUTiNjF1mrajI7KRKBclfrpZEYlEIpFIJP5lcRWlyVU1H9MU0y8gEolEIpFI3Cm2GmeTeJu2qDJUtFfLF5FIJBKJROI2cXw/OsXcFi/SAKuKckskEolEIpG4VyzrUKtxfveu+I3kaW4ikUgkEonEzxKvgpbr719Blc5EIpFIJBKJHybObPy0GmoQ6Y8zIpFIJBKJxL3iNMD8KMlllDL3FEQikUgkEok7xBL9KyO9PBo9e/wu96yvm6JNJBKJRCKR+Lz4TBCJRCKRSCQSiUQikUgkEolEIpFIJBKJRCKRSCQSif+R+A1xURfYPcvifAAAAABJRU5ErkJggg==	\N	f	\N	\N	f	f	\N	49500.00	\N	\N	\N	\N	\N	\N
9c996c7a-7996-4e01-b4f6-626910de046e	0ddb8972-36cd-4b67-8887-829aadbdf942	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-01 00:00:00	10:00:00	36000.00	Cancelled	36000.00	36	2026-08-08 13:00:11.192847	\N	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADn0lEQVR4nO3ZQa7bMAwFQN3A979lbpCihW1SpBKgi0Yp/mgRxJL4htkRznh+eD0GkUgkEolEIpFIJBKJRCKRSCQSiUQikUgkEolEIpH4E8VR13HulSu/q0Z+zHGprEQRiUQikUgkbhSPXH0+TumNmJwsrqKIRCKRSCQSd4pn6tHEM27c6dNeZl9GEYlEIpFIJH6l+CZu+kYkEolEIpH4f4hjXnmAerT/zNa1RCKRSCQSid8hrhrIu/HKKK4c98Ex762SiUQikUgkEneIZU31f/nRo4hEIpFIJBK3iev1aNVn/fQGKb9LOl5FEYlEIpFIJG4Te3pOuh6DzfbznqeOO+qyX09WRCKRSCQSiZ8QS2Y7eC6IMkU9Rv8F7QqRSCQSiUTiFjEmppHWse5nzKt1+7j33kxWRCKRSCQSif9cLCEt88hXVhXlIMR5j0gkEolEIvHz4mMOPuYGpvq814k8n10fRCKRSCQSiXvFwEpcaWWdPtrIVX4GkUgkEolE4g5x3k8r10+d5WkrKlY2kUgkEolE4k4xH06Z63UFx2Nu4OUiEolEIpFI/LxYZqKWOdoAVdLjXVJr+c0sRyQSiUQikfgpcSrId5/5NK/e2ap5IpFIJBKJxI3iu6q1c7Sm2r0IIBKJRCKRSNwuRmkJKQext5rKyhUikUgkEonEbWKLK++SYh2tKL9a6tPWqzdIRCKRSCQSiZ8R/4Q1+/pop9HFReQrUwWRSCQSiUTiXrGkFyKnj7uVOD3yt7jXBi0ikUgkEonEHeLElvr145UZj609IpFIJBKJxO3ieTju8SruxhQ19RNjWD59VmIQiUQikUgk7hbHXBDsuIMjc+SkfG/VaCwikUgkEonEHWLcLY9tOkqtlL3WwKi1RCKRSCQSiVvE89vIM9b52DPPvbE+aI0SiUQikUgk7hUf8zuiyy5NNayX5ZXLiEQikUgkEj8trlYpbAfHPXxNH6tfQCQSiUQikbhNHHUd6+D2Lmmcb4vCybUjXyYSiUQikUjcI8Z+KZ3Sy71ooJxGe/NlIpFIJBKJxC1iVOWPcc9OMVRN/5S1vZ5CJBKJRCKR+I1iKS3D0vPem7qIe0QikUgkEonfKI48VJX6UpH3XjwSiUQikUgkbhNbA8dc9cjpMUqdFdHttdcWkUgkEolE4g6xrItYTVbltO31volEIpFIJBK3iZ9ZRCKRSCQSiUQikUgkEolEIpFIJBKJRCKRSCQSiUTiDxJ/AVMYd3dirmY2AAAAAElFTkSuQmCC	\N	f	\N	\N	f	f	\N	36000.00	\N	\N	\N	\N	\N	\N
3a81ba1b-9a1c-48db-9094-4d318ba065e4	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-26 12:35:15.72	17:00:00	79000.00	Completed	100000.00	175	2026-09-02 11:13:06.482316	\N	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAAE6ElEQVR4nO3da3LbMBAD4Nyg979lbtBOU6vkPig7bS26k08/Mo4jE5qdXQCCJeXt+8Xb+xtEVdU5pgPn4FXaQR+5Ds6Ke+SQnXc4t3L+6BxZ8iBdkSBJyaRkUjIpmZRMSiYlk5JJyaRkUjIpmZRMSiYl+4op2Vvevt3e+7nbr+3jVVriY7/yqi4FUVV1junAOXiVdtDHja5jvH/82v24HUB6b+CE98rKEFVV55gOnINXaQd93OQ60qeKn3ov9uq2S0XsDwCiquoc04Fz8CrtoI+v4joOKzX7qRQKDe90HApEVdU5pgPn4FXaQR//A9eRvjirbiulShBVVeeYDpyDV2kHfXxR19H9OsMeadEMUb84+4vrcyCqqs4xHTgHr9IO+vgM15G2xRVBD/2oS0FUVZ1jOnAOXqUd9HGb6+i2GfvIjR53Ud0GUVV1junAOXiVdtDHHa4jLXdAjJU6s9RBLHeGqKo6x3TgHLxKO+jjDtfxgIsqfihs/a1f6Rk/EFVV55gOnINXaQd93OE6BlhwTPN74aDmIxsG6uySaIiqqnNMB87Bq7SDPu5wHcM2FXeUaCmFTMNthQMo2BBVVeeYDpyDV2kHfdzkOmZ3tHBWM2x4r3uezyp9gqiqOsd04By8Sjvo49Wu455FCg9fXsZI6Rb3BxMkiKqqc0wHzsGrtIM+Xug6ZouUnFX1TjPicGXhkiKIqqpzTAfOwau0gz5udB2dgQpmaV4zHNSME17dcVYQVVXnmA6cg1dpB328zHWc3LCegqJj4eV3a90qEFVV55gOnINXaQd93OE6xkrF9oRLnU9uYq9fkt1/fg5EVdU5pgPn4FXaQR+f7jqS/+ifOFiNyfyq/2oMoqrqHNOBc/Aq7aCPm13H+UU+aZf5AKqBSqnSnecEQlRVnWM6cA5epR308cmuY17keFXyoPGHkSUtf4RdIKqqzjEdOAev0g76uM11zGAhPCrX/CTs5XN6hu8aG0RV1TmmA+fgVdpBH693HUtn1V3kM5br0qLlIUNUVZ1jOnAOXqUd9HGH60g+6d79VuGZPOWi53AAK2cFUVV1junAOXiVdtDHa1xHt2+hoLD1O3/m+hyIqqpzTAfOwau0gz5ucB0pLeq2dIdX+sOnnRVEVdU5pgPn4FXaQR+f5DqW7qjcw5W+M+u+6epMFURV1TmmA+fgVdpBHze5jvI4wf4ff9ZtIH4cd/fX0wQJoqrqHNOBc/Aq7aCPT3Yd5XLlYZbGcgE7fbYDaz4GUVV1junAOXiVdtDHq11Ht41Pl1/rf5VILqqERxBVVeeYDpyDV2kHfdzkOpbZz4BIK81/WNw5VbAhqqrOMR04B6/SDvq4w3UsHrMznFV6KGF0TO0DfCCqqs4xHTgHr9IO+vgqrqMzVeVrsJoMpcMrwRNEVdU5pgPn4FXaQR9fzXWUZw+GAygPZB6rp3uoZgcGUVV1junAOXiVdtDHV3Ad80rnFwN1huyP8xyIqqpzTAfOwau0gz7+e9fRmZDf+wafE5Yrhiw4I4iqqnNMB87Bq7SDPu52HWkLCdIAK3dz1f+8Na93+sQeiKqqc0wHzsGrtIM+XuE6rtkgqqrOMR04B6/SDvrIdXBW3COH7LzDuZXzR+fIkgfpigRJSiYlk5JJyaRkUjIpmZRMSiYlk5JJyaRkUjIp2RdKyX4Arl7nSc/p64MAAAAASUVORK5CYII=	-21000.00	t	\N	\N	f	f	\N	0.00	79000.00	\N	\N	\N	\N	\N
26d9fef8-cc52-4112-ad1a-50beb7a87a3a	0ddb8972-36cd-4b67-8887-829aadbdf942	84cc584d-335e-40a7-90c3-abebfe573b01	b53808e3-7219-4c65-899c-197f204e5581	2026-08-07 00:00:00	13:45:00	12000.00	Cancelled	12000.00	12	2026-08-08 13:00:11.192874	\N	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADoklEQVR4nO3ZQW7cMAwAQP1A///l/sAFUtukKG3aHmotkOHBsCWSw9yITTsejlcjEolEIpFIJBKJRCKRSCQSiUQikUgkEolEIpFIJBJ/othq9HxWCuN2bFfLohWRSCQSiUTiRrHn6vPzdff8ers+z8ZlqBBXrYhEIpFIJBJ3imdxn8Sz57E4G6b4phWRSCQSiUTi54mxXh13SpvYfEYkEolEIpH44WI+C2fetvJ9JxKJRCKRSPxEcRqg/05r8chn3w267kwkEolEIpG4Qywx1P/jY25FJBKJRCKRuE38izhLh0UrPyLlbRCJRCKRSCTuEF/TRhQXZ7tYuVp2yu3Zqt8X7zYrIpFIJBKJxCfEUlAe+aLMmNsNU5RaIpFIJBKJxE3iys65g1Om+Ir8NsxDJBKJRCKRuFdsd8R2lO02ng1EScmTTdMSiUQikUgkPi2WxSjvRKVxz6Pk7vPKlT+JRCKRSCQSN4kFe1vaasRkJV5jPyKRSCQSicRtYs9v5wCBtXuAYajpcdxDracgEolEIpFIfF680tabVYyS96RWys5BM1E+iUQikUgkEreIpXtuPHy+azfEVTHWEolEIpFIJG4Rp+3oladY5ZWlKmPr4YlEIpFIJBKfF/ui55vSVUp5C/5IQSQSiUQikfi8eB+17KywaNym2+gSkxGJRCKRSCTuFUvPt5+5Z4xXnCF5TCESiUQikUh8WsyX0a69e/tKbuNnzw3WexeRSCQSiUTi8+JqHTofF5adPhLHPWg0GPKIRCKRSCQSt4nreI3dwx4epXHGrjcikUgkEonEveLbfSoGeLtj5Ys+NSUSiUQikUjcJq6wiFig8j41XJQB/rxZEYlEIpFIJD4qruJycukwY9nAYpTIIxKJRCKRSNwr3ud1YyqL1rRj9XGAIw+fpyQSiUQikUjcJEZGKSjr1Tq557J89s1mRSQSiUQikfjfxVWUxtE93/aporzlIBKJRCKRSHxebDX62PP6BWmaJ4aa965oRSQSiUQikbhRjPNj6pT/KzZ0Py+Ocby5H5FIJBKJROJe8WzYx/phqcrikJzP4uJ6IxKJRCKRSPwwcVV6rVL57bhTriidiUQikUgkEj9UjO7RLqe0yT4XrU4kEolEIpH4EeI0wFCwPovuMe08BZFIJBKJROJGsUT/ndHu3FX9nFx+Verj30IkEolEIpH4vPhMEIlEIpFIJBKJRCKRSCQSiUQikUgkEolEIpFIJBKJP0j8BdhsXenvQYExAAAAAElFTkSuQmCC	\N	f	\N	\N	f	f	\N	12000.00	\N	\N	\N	\N	\N	\N
f4730e5d-340c-4121-bddf-923e4d400c6c	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-24 00:00:00	09:00:00	158400.00	Completed	160000.00	187	2026-09-02 11:13:06.482316	https://res.cloudinary.com/devu5qabc/image/upload/v1787535976/64536fcd-9c18-4895-aaf6-df5d0f43b93b.jpg?cors=anonymous	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADnklEQVR4nO3Ywa3bMAwAUG2g/bf0Binwa5sUpaToxUrRx0NgySQfcyPcXg/H0YhEIpFIJBKJRCKRSCQSiUQikUgkEolEIpFIJBKJxP9RbDX674zr6Sxo5ecn8tPbVkQikUgkEonbxLi/jnFXjhP7Nu9Vy4hEIpFIJBK3iLEJrQe4YsUWcd2KSCQSiUQi8TvEUn/mlZQYgEgkEolEIvHfEs9vRG1aoPIxgkgkEolEIvErxdWxjTENcIxvP7UiEolEIpFI3COWGNarv/yZWxGJRCKRSCRuE1fxk34+9fuu5+P0GanXJkMQiUQikUgk7hDL96BoPLyYhrpSzrfHXXsdiUQikUgkEneLb5p8GGDYseIYk02LFpFIJBKJROLzYvQuW1S5y417ro0pcr8+skQikUgkEonPi+fGVGJYr/JS1cbuxX6tj0QikUgkEok7xHMJWkXP82S7DHptVpHyh12OSCQSiUQi8QlximGLWmPhtLH7cQ/6IhKJRCKRSNwtlnZj2tyu3fMMeVGbNzUikUgkEonE7WLOCKeN4tA9s/MaliuIRCKRSCQSN4nZvpzc/cgjlfVqNXJg7zYrIpFIJBKJxGfEINZYaXI1nt5+Xq+IRCKRSCQSnxfPjGEdKgV55TqyuK44Wm1PJBKJRCKR+AXinXF1b9O2lcvafTdU5DIikUgkEonETWJUndGnAcqOFWzuH8vXlZe7EIlEIpFIJG4Sgy1TlGWpdMrOMTYgEolEIpFI3C6WyLvT/HN2j6GG8aI2xiMSiUQikUjcKB5Taf65pshv4+56USIPTyQSiUQikbhXHNah3CmWpaH7VNbGBlFLJBKJRCKRuElcvJzt2KyGiLuyfEUZkUgkEolE4m4xiKsq7Bjl9S4yMYxMJBKJRCKRuFHs01MW57s8aL9ffA4ikUgkEonETeJPdT72WtBKSnFajV7/AZFIJBKJROLT4irO7rFZld1pPub9jEgkEolEIvE7xFZjKD2fon6eohCljEgkEolEInGjGPdD98kuWHxVmjerxfJFJBKJRCKRuEU8i/t0zI1f07Gk5LLyX4hEIpFIJBK/RSx2m5yp8ZHFkkIkEolEIpH4LWJ5G9+DihNlq89IRCKRSCQSiV8gTgP0Rc9Xqh8atHuemLvlFCKRSCQSicQ9Yok+dYrPQ/lprs3r1bBoEYlEIpFIJO4QnwkikUgkEolEIpFIJBKJRCKRSCQSiUQikUgkEolEIvE/En8BoLa/5WsTT8wAAAAASUVORK5CYII=	-1600.00	t	\N	2026-08-24 08:46:18.266648	f	f	\N	110880.00	47520.00	\N	\N	\N	\N	\N
5418ce86-ced6-4f45-a492-dba029ec3329	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-10 00:00:00	09:15:00	310000.00	Cancelled	310000.00	242	2026-08-10 02:30:16.852239	\N	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADj0lEQVR4nO3YQY6kMAwFUG7A/W/JDZhFF9ixUz2aDUGa50UpAfyfe2f1dj5cx0YkEolEIpFIJBKJRCKRSCQSiUQikUgkEolEIpFIJP6P4lZr//liu7/d4idqjPtpm0cRiUQikUgkLhP33B2thYhR2nW/23oUkUgkEolE4loxQvJ1YAs2P/UrkUgkEolE4qvEz4uBzc/2n5Dhn0y/RRGJRCKRSCS+T/xln9onGJFIJBKJROLbxHYd1qZJf12lfokiEolEIpFIXCaW2u+kf//pUUQikUgkEonLxL/VcbcekwXqmKTPU4hEIpFIJBKfF6+ukt6cbUtteZXqWPmYSCQSiUQicYX4ad3zZ4Vtnxyjc9zPrrdtBCKRSCQSicQVYqn9Fod5IjMmmw81DE8kEolEIpG4UCyfRcgQPD8V8eqI4YlEIpFIJBKXibExRX+MUqok/T4okUgkEolE4lox9/dvP6dhlBg0n4Ydq3UQiUQikUgkPi9G66fhqvwiahivELGkxfBEIpFIJBKJbxHzsyO3RvpsgBKVT0QikUgkEomLxNL/IY477pi8PfOLPG08IxKJRCKRSHyLuOeuFtdPwZ6TmqxcRCKRSCQSiU+LhSjs7KeMV053ztfNikgkEolEIvEZMZagFjJUHuXMp096jLyNQxGJRCKRSCS+QLyqLFqzU+7Yx3lmIxOJRCKRSCSuEC/i69qU44bJAtvGKn8LkUgkEolE4kKxX0try7wWqDLPLJRIJBKJRCJxjRhxV2ureJGXpS1/vN951zxEIpFIJBKJa8Vgc8M5XiO9jBf2kNKuRCKRSCQSicvEISSCW3+sV8fYER+XDYxIJBKJRCJxmThrjS1qtiyVt51ovUQikUgkEonPi9GVf87aUDu2n9pb+nbX+FcRiUQikUgkPi22pH28bnmAnBT71LENVb4jEolEIpFIXC7O7LZ3nfd1OLXNah8TiUQikUgkEp8Xt1rDt/E4L1B9lDwjkUgkEolE4lvEQlxdLW7LpzGwblYtmUgkEolEInGRmOOujSmHfF2byjxXnbWIRCKRSCQS3yE2dtixPteh2ijnnUIkEolEIpH4KnHL/wWK693f7f7vprGDSCQSiUQicYnYBhg2q/Isp38ZJXcQiUQikUgkLhNL7feLTuTgrT2LK5FIJBKJROILxGeKSCQSiUQikUgkEolEIpFIJBKJRCKRSCQSiUQikUj8j8Q/bL72Qd1zIyYAAAAASUVORK5CYII=	\N	f	\N	\N	f	f	\N	310000.00	\N	\N	\N	\N	\N	\N
70eb03a9-e158-4428-a196-e4d81eb232d3	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-24 00:00:00	13:00:00	158400.00	Completed	160000.00	187	2026-09-02 11:13:06.482316	\N	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADlUlEQVR4nO3ZS47jMAwFQN1A97+lb5ABui2LouQ0ZjFWBl1cBP6Ir5gdkZTXw3UUIpFIJBKJRCKRSCQSiUQikUgkEolEIpFIJBKJRCLxN4olV71e1NDwXV9X6fa8ehdFJBKJRCKRuEfsz9ttb0gRXTyxOrbNUUQikUgkEol7xUjU7xNlZV/9gz2IUxSRSCQSiUTiB4mrrvh2OEIkEolEIpH4v4nnepWuXtfb13iOSCQSiUQi8SPF1W3s7yGN6B9xgJsoIpFIJBKJxD1iqr4i/f3HHEUkEolEIpG4TVzVefa4uo5lSIlOvckiEolEIpFI3CcOPwX1PWn1G9Fkpz/T2gBnL5FIJBKJROJOMS1L57NSlkciNryNWAolEolEIpFI3Cb2qvF2dWRyht401PhdiEQikUgkEp8W13XEkP5bUsTSxzzPmEckEolEIpG4RYzrUN+Thj/J0u4Uhxp+eIr1ZrMiEolEIpFI/Odifx9HaHFpd4rnjkVbn2xazYhEIpFIJBKfF9N2VKeuJJ5V3/RGjEgkEolEInG7OPWX61lsLaurr4ptpZSxjUgkEolEIvFpsbN9gXrlmjMnsY4z9m9AJBKJRCKRuE08zv4Ja89iyG1mm7sPRSQSiUQikbhXTE7csVpwxFrSbVtcueLwRCKRSCQSiZvF68SyelxnY8AxHiYSiUQikUjcLqZ96lyR+pGbVerWmVKIRCKRSCQSN4nx6jgH6Enx6hVCShqFSCQSiUQi8aPE2N9rJbb+/vb9oEQikUgkEom7xeNsuA2ZRmk1vW0VJyMSiUQikUj8AHEVslqWjolIHdN4RCKRSCQSic+LKSnuU/U6Mof0Z9NtGo9IJBKJRCJxr1gmJx5ZPWtYFNvcUweRSCQSiUTiDrEvSyX+ZzYNcEQiBqcX68NEIpFIJBKJW8TyXXUcoMcdpfQjKTMGpKpEIpFIJBKJG8VVfR1f7U79Rdqd4qDz8EQikUgkEok7xJJraJ2GGaZYbWXTtEQikUgkEonbxBoTU+sU0q7SevXzoEQikUgkEolbxJg+3L7PTM9S29vNikgkEolEInGTePMRO9IARymJJRKJRCKRSPw8cdqnVutVb1uJLY9IJBKJRCJxrzgNUGPmdK4NsB65T/EKHUQikUgkEonPi6nqlZk+GpamSOlpACKRSCQSicRt4jNFJBKJRCKRSCQSiUQikUgkEolEIpFIJBKJRCKRSCT+IvEPVvz6vOIN9WYAAAAASUVORK5CYII=	-1600.00	t	\N	\N	f	f	\N	0	158400.00	\N	\N	\N	\N	\N
2730e2f8-78c1-4296-a970-1174dc2c017d	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-10 00:00:00	08:15:00	310000.00	Cancelled	310000.00	242	2026-08-10 01:30:06.339543	\N	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADmUlEQVR4nO3YQXKjQAwF0L4B97+lb8AsApZaAleyiNtTeVpQGFr/yTsVY39zPQaRSCQSiUQikUgkEolEIpFIJBKJRCKRSCQSiUQikfgXxVFr+zrxVXHkuCtv+7kWRSQSiUQikbhMjOfnz/bsTG/B28s2IpFIJBKJxOVi9OeQs2KBKpWn2F5EEYlEIpFIJH6KmJ9FTecO7HHXRiQSiUQikfjfiNdfkMYFSyQSiUQikfhB4tXPK/vZP+ISL26jiEQikUgkEteIpcp69aNLjyISiUQikUhcJl7VcTa+DEX/o9219KsiEolEIpFIXCFOn4IiJL/d79am+IL0mHsfYxCJRCKRSCSuFvdn67kdhZj7I3PaovJ6NfKRfI5IJBKJRCLx/WI+MVWMki97TirO7TMikUgkEonEFWIOmb4lxV3+ebVUndO2vCgikUgkEonE94uBlfUqVqm7kKnjaj97scsRiUQikUgk/rqYM08npsjpI39BusLK8kUkEolEIpG4VmxJY0xvI/icpwTHPHmUeEskEolEIpG4XCwVRFmlsnPuU6WthRKJRCKRSCSuEGN3mnaitlTl/mnHioqAtnwRiUQikUgkvlss/fEsxLI7lSOlyj8gEolEIpFIXCYemf1srEgl7nXb3XhEIpFIJBKJ7xfj2COHHLtTJBWx7F3nJYpIJBKJRCJxrXgcm4gjc7SkMlSbbFy8JRKJRCKRSFwkHgen9SrSIyQma5dpMiKRSCQSicRPESM415ZHKf15vD2L8SL/DSKRSCQSicSVYr6bLgUr21aebKo8GZFIJBKJROJKsbTmKabgEF/UtJURiUQikUgkLhObc2L5xf4U+wC3Q83/ikgkEolEInGxeKxDj9kpU2x7rfylKZavb21WRCKRSCQSib8mTmdzww0WW1QZL1d5RiQSiUQikbhIvMLastTPtXlGCyASiUQikUhcJl5V7E75Z9jTPPntZOdEIpFIJBKJxPeLo9bU2ohIKkvVdj0okUgkEolE4kIxnu9zV3wt2mp/epHv9nmoPXUQiUQikUgkLhHHV8V2ND2LLapkxmS5+hREIpFIJBKJnyiWzOnIM+4bR4hEIpFIJBI/QcyL0blUlZBoy+LW8ohEIpFIJBLXim2AfDZVPMs905F2mEgkEolEInGZWCr2qZukElfa2h2RSCQSiUTiIvE9RSQSiUQikUgkEolEIpFIJBKJRCKRSCQSiUQikUgk/iHxHw/3j+P5tIk8AAAAAElFTkSuQmCC	\N	f	\N	\N	f	f	\N	310000.00	\N	\N	\N	\N	\N	\N
5864c2a3-6e31-4f7a-a143-c4afbc025327	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-25 00:00:00	09:00:00	158400.00	Rejected	160000.00	187	2026-08-25 11:40:47.76943	\N	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADoElEQVR4nO3YQc6kOAwG0Nwg978lN2CkbsCOk/rVsxhSo35eoALi77l2Fu18uY5GJBKJRCKRSCQSiUQikUgkEolEIpFIJBKJRCKRSCT+jWKr1a9nz9l2nevXbX57Ps/6OopIJBKJRCJxo9hz9zo42JZ/TYOuoohEIpFIJBJ3ih+7fhjlaG11ZB1FJBKJRCKR+C3i8WTen4fys3M6QiQSiUQikfj/ENtThW2pVrdEIpFIJBKJ3yauB2it5VfzFpVH6esoIpFIJBKJxI1iqaH/X17mKCKRSCQSicRt4rqO3LgaL85dL/ocMuQRiUQikUgkvi8O6RF3hQzfjeLwNEV/3vbnxafNikgkEolEIvENsXwFms4WZ7VKHW35D8ZnRCKRSCQSiW+LExsVm9WZF6g2VpknsM+7HJFIJBKJROJrYj6x2pP6+PZ4uHuA0rFIIRKJRCKRSNwkDiHR0D5VzHjdHk9vvPhxsyISiUQikUj8z8XYhIadKOzIXK1NY/DyCJFIJBKJROIOsQTHsrRy8rMzt11Rd8BiFCKRSCQSicT3xRJ8J+Vfx5M5zJPTW0uhOYBIJBKJRCJxkxiZEVK2owDiXPTmtoH9tFkRiUQikUgkviWu6rhC/miyI89T1jAikUgkEonEvWI05BXpyJe8Y5VRhhmficr2RiQSiUQikbhFjHUo/zrH7ei+DTavV0MvkUgkEolE4leI906Ut6Nyac+OtaqhbT0tkUgkEolE4g7xnIKn6rW/lcxynkgkEolEIvHLxA+XklnYKWA9I5FIJBKJROIW8eqPjenuKotWnCtEaSMSiUQikUj8PjH6o3JSy5Pl3rMSjUgkEolEInGv2FL18eywVEV/HqBPt58GIBKJRCKRSHxfvLvKZtVqrTJbW0aVGYlEIpFIJBI3ihE3YJE+OfHNqYyy+g5FJBKJRCKRuElcDZBvp/72ccar+phMJBKJRCKR+L64qmmKNi5V5zjFPUDuHZ4RiUQikUgk7hBbrXsnypkF6zm4PCspRCKRSCQSiRvFeD635hXpmG7z4b7OIxKJRCKRSNwrtt9VsDP/itt8eFVDHpFIJBKJROL3ibFA5X1qrhUayUQikUgkEolfJvanYRDXQw1ROZRIJBKJRCJxu/hxgNIfU/yqsnJNThSRSCQSiUTiDrFUZPb1ANNXpft2tYYRiUQikUgkbhPfKSKRSCQSiUQikUgkEolEIpFIJBKJRCKRSCQSiUQi8S8S/wFvz0CdubBGjwAAAABJRU5ErkJggg==	-1600.00	f	\N	\N	f	t	\N	110880.00	47520.00	\N	\N	\N	\N	\N
5ed810a0-f0ee-4046-85ba-62183a3a49da	0ddb8972-36cd-4b67-8887-829aadbdf942	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	53fca09d-2b87-41ae-95ea-640953f66815	2026-07-22 02:34:45.682	12:00:00	84000.00	Completed	84000.00	144	2026-09-02 11:13:06.482316	https://res.cloudinary.com/devu5qabc/image/upload/v1784468819/9d90e7d9-e239-4816-925b-fe99496e51c9.png?cors=anonymous	\N	iVBORw0KGgoAAAANSUhEUgAAA4QAAAOEAQAAAADRNcKWAAADkElEQVR4nO3ZTY6cQAwGUG7A/W/JDYiUAHbZ1RNlQ7WU5wVq6sfvY2fNbOfLdWxEIpFIJBKJRCKRSCQSiUQikUgkEolEIpFIJBKJROL/KG619j8n/lQ593sj7w7n5q2IRCKRSCQSl4mxfr9ej9nVwY6NuDbvTCQSiUQikbhIjMbXsaNhJUXMWPOgO5FIJBKJROI3ihkLexuJbYIRiUQikUgkfr0Ya7GxZaykIBKJRCKRSPxScfZ6/Tpa4/YronxoRSQSiUQikbhGLLWP49U/PXorIpFIJBKJxGXirJq9Pf8kO54ZK46U7rMiEolEIpFIXCHOmuzXRpmYSp7Z35dyPCKRSCQSicS14tmORbsc6t7IoWItUpztlUgkEolEInGZmI+F3aaje7c4M4xIJBKJRCJxuZidYS0aX9jwqz3uoPk1pyUSiUQikUh8Xyyd4uqW1+JcPOJwDjqEIhKJRCKRSFwmtvvnE2DLa23GGoh8LXaJRCKRSCQS14rB3k1yle5Dnhb5nqf+MlkRiUQikUgkviFeV2M6Op8L29NpCNBmrD5t5TxEIpFIJBKJa8Vw9nFt2M1DVZnFyjw1pCASiUQikUhcIear5ezQPSqu5Z7RYEhBJBKJRCKRuEyMUapMRz/MSXfPWYP4gvGriEQikUgkEt8Wo13uGaPU3roX5yLuRxSRSCQSiUTiWjGPQ4PT8gQbUfZPR9oXEIlEIpFIJL4v3pXZj+KHMSzSEolEIpFIJH6LOLNzinPEtifP0XZzxjhHJBKJRCKRuEzs89RHsWXcfwxKJBKJRCKRuFyMnrlJ/BVoGJtyxlIlMpFIJBKJROJKsWC55/nMTj1FXPt4bvwqIpFIJBKJxEXitb7l17vTbPgqR+KR57M4RyQSiUQikbhCLJuDE1g5l9eGjPMiEolEIpFIXCZurXGek7Zs5yglQO9CJBKJRCKRuEyc1YyIAPlxjvHCJhKJRCKRSFwubrX6PNVSxJG9Ee0ckUgkEolE4jIx1s/n1nC/1MfxKicrRSQSiUQikbhIvHr16Sifu9eiZ74WNcxdRCKRSCQSid8o9ipzV4syYEQikUgkEonfJ5ahqs1J55jseMRSRCKRSCQSicvF1qk8thZglqd44xEikUgkEonE98VS+9MkXsM5xnYDlnePepdIJBKJRCLxbfGdIhKJRCKRSCQSiUQikUgkEolEIpFIJBKJRCKRSCQS/yPxF43HDN25f7rpAAAAAElFTkSuQmCC	\N	t	2026-07-19 13:47:00.146954	\N	t	f	\N	84000.00	\N	\N	\N	\N	\N	\N
\.


--
-- Data for Name: Categories; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Categories" ("CategoryId", "Name", "CategoryTypeId", "Status") FROM stdin;
15	Hẹn hò	6	Active
17	Cắm trại	6	Active
19	Ngày lễ	6	Active
14	Tiệc tùng	6	Active
18	Văn phòng	6	Active
20	Mùa đông	1	Active
21	Mùa xuân	1	Active
22	Mùa hè	1	Active
23	Mùa thu	1	Active
24	Giáng sinh	1	Active
25	Tết Nguyên Đán	1	Active
26	Halloween	1	Active
27	Lễ tình nhân	1	Active
28	Dưới biển	1	Active
29	Vũ trụ	1	Active
30	Dễ thương	2	Active
8	Emo	2	Active
32	Sang trọng	2	Active
33	Bohemian	2	Active
34	Glamour	2	Active
35	Vintage	2	Active
36	Hiện đại	2	Active
37	Tự nhiên	2	Active
38	Họa tiết động vật	2	Active
39	Hoa lá	2	Active
10	Đen	3	Active
5	Da ngăm	3	Active
42	Trắng	3	Active
43	Trắng hồng	3	Active
44	Trắng ngà	3	Active
45	Da sáng	3	Active
46	Da trung bình	3	Active
47	Da nâu	3	Active
48	Ngắn	7	Active
49	Trung bình	7	Active
50	Dài	7	Active
51	Tròn	7	Active
52	Hạnh nhân	7	Active
53	Stiletto	7	Active
54	Quan tài	7	Active
55	Vuông tròn	7	Active
56	Sơn bóng	8	Active
57	Sơn mờ	8	Active
58	Gương	8	Active
59	Mắt mèo	8	Active
60	Cầu vồng	8	Active
61	Sơn thường	9	Active
62	Sơn gel	9	Active
63	Acrylic	9	Active
64	Đắp bột	9	Active
16	Photoshoots	6	Active
71	Đám cưới	6	Active
72	Học sinh/Sinh viên	6	Active
73	Phỏng vấn	6	Active
74	Du lịch	6	Active
75	Sơn đơn	11	Active
76	Sơn 2 màu	11	Active
77	Sơn ombre	11	Active
78	Dán họa tiết	11	Active
79	Vẽ họa tiết cơ bản	11	Active
80	Vẽ họa tiết nâng cao	11	Active
81	3D nghệ thuật	11	Active
82	Trang trí đá	11	Active
\.


--
-- Data for Name: CategoryTypes; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."CategoryTypes" ("CategoryTypeId", "Name", "Status") FROM stdin;
2	Style	Active
3	Skin Tone	Active
6	Occasion	Active
7	Length & Shape	Active
8	Finish	Active
9	Technique	Active
11	Service Type	Active
1	Theme	Inactive
\.


--
-- Data for Name: Chairs; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Chairs" ("ChairId", "SalonId", "ChairName", "Status") FROM stdin;
9b0ed177-2fa7-4e27-8627-2350acdc18bc	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	2	Active
706748a7-2bdc-4954-91a8-dcc3480cd0f7	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	100	Inactive
8e02951f-337b-4d56-b137-e092574ccad1	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	1	InActive
41ddc49f-fbb3-436e-afa2-4c6a1b8e5099	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	7B	Active
3502c060-1bf3-498b-857d-bb900c345cdc	118341cc-0df0-44cd-b40a-0a3f5d167c58	1A	Active
acda5525-dee6-424b-ab26-2bf24426d2d1	3bab09c5-20de-4a87-8415-60e2a62a16db	2B	Active
84961c2e-db18-482e-bf63-749372b3f179	118341cc-0df0-44cd-b40a-0a3f5d167c58	7B	Active
a1214399-7926-45b8-a280-aeeff85d30cc	118341cc-0df0-44cd-b40a-0a3f5d167c58	4C	InActive
5ff6909c-ec9d-42b5-8de8-868625c2c0b9	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	3A	Active
d5bd672d-7692-45cf-9e3b-902698000075	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	4A	Active
6906aa95-e06a-4743-b172-b8adb252ebde	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	5A	Active
2597eb8d-4fc7-4bdc-9295-a8ae1d44ac45	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	6A	Active
636023d0-bcff-4dbf-9d83-ded501505849	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	7A	Active
7f24ba6c-ccdd-4b84-88d8-d477773ddb06	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	8A	Active
5b79e9e3-a1a4-453f-ab58-be92b5093c37	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	9A	Active
9e51be5f-ae51-4fa0-9bc4-fa0e9343019b	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	10A	Active
2ddf99fe-a4ad-41b9-bf93-0b6c5a62fe46	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	1B	Active
61e05801-6b27-440d-9d2f-b5011806ddc9	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	2B	Active
682b8bd0-be58-4bce-a024-b5ac58cdc402	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	3B	Active
56c8138c-9f26-4ce5-9ef4-a3bb72adc257	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	4B	Active
3108d303-cfba-4ba9-94ed-a7479f8ed522	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	5B	Active
ab90b056-135f-41bd-92a8-7339292fada9	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	6B	Active
61aa59aa-c665-431a-8820-5864803b7b4e	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	8B	Active
ee5a7df6-a33b-4df1-b68b-57074560cc99	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	9B	Active
054928dc-9d2c-4fe2-a373-16cf2814d0be	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	10B	Active
df8e6759-dc4c-4689-8bae-1adc75fd50ae	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	1A	Active
5499758b-4f1c-47b9-86a9-207bed3c96f2	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	2A	Active
\.


--
-- Data for Name: Components; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Components" ("ComponentId", "Name", "ImageUrl", "ComponentType", "Price", "Duration", "Status") FROM stdin;
16	Flower 4	https://res.cloudinary.com/devu5qabc/image/upload/v1784193721/flower_1_zeszrn.png	1	10000.00	20	Active
17	Black Heart	https://res.cloudinary.com/devu5qabc/image/upload/v1784194728/black-heart_addfdf.png	1	10000.00	10	Active
18	Black Kitty	https://res.cloudinary.com/devu5qabc/image/upload/v1784194729/black-kitty_larwld.png	1	10000.00	5	Active
19	Black Star	https://res.cloudinary.com/devu5qabc/image/upload/v1784194729/black-star_b9lbel.png	1	10000.00	10	Active
20	White Kitty	https://res.cloudinary.com/devu5qabc/image/upload/v1784194729/white-kitty_dn19xm.png	1	10000.00	20	Active
21	White Heart	https://res.cloudinary.com/devu5qabc/image/upload/v1784194767/white-heart_cvijrm.png	1	10000.00	20	Active
22	White Star	https://res.cloudinary.com/devu5qabc/image/upload/v1784194768/white-star_bb8f70.png	1	10000.00	10	Active
4	Daisy	https://res.cloudinary.com/devu5qabc/image/upload/v1781056476/2c3f1304-b5db-43cb-be9e-5f35d9b6b1d7.png?cors=anonymous	2	10000.00	10	Active
5	Star	https://res.cloudinary.com/devu5qabc/image/upload/v1784193299/star_r8pv1c.png	0	10000.00	20	Active
1	Apple	https://res.cloudinary.com/devu5qabc/image/upload/v1780398676/ad189234-81f9-4bbb-af3d-3bf561afc8c6.png?cors=anonymous	1	10000.00	10	Active
2	Art 1	https://res.cloudinary.com/devu5qabc/image/upload/v1784193299/gem_2_tjpnd0.png	3	10000.00	30	Active
26	Blue Ruby	https://res.cloudinary.com/devu5qabc/image/upload/v1786545046/447cb26b-daf0-485e-8104-15ae61e30027.webp?cors=anonymous	0	120000.00	150	Active
23	Yellow Star 1	https://res.cloudinary.com/devu5qabc/image/upload/v1784194730/yellow-star2_woogvv.png	1	10000.00	10	Active
6	Art 2	https://res.cloudinary.com/devu5qabc/image/upload/v1784193299/gem_1_fn0tkl.png	3	10000.00	10	Active
8	Tear	https://res.cloudinary.com/devu5qabc/image/upload/v1784193299/tear_hijddu.png	0	10000.00	10	Active
9	Circle	https://res.cloudinary.com/devu5qabc/image/upload/v1784193300/circle_uipsfr.png	0	10000.00	10	Active
10	Square	https://res.cloudinary.com/devu5qabc/image/upload/v1784193300/square_zimiav.png	0	10000.00	10	Active
11	Moon	https://res.cloudinary.com/devu5qabc/image/upload/v1784193300/moon_eccmbe.png	0	10000.00	20	Active
12	Flower 1	https://res.cloudinary.com/devu5qabc/image/upload/v1784193722/flower_4_uroygs.png	1	10000.00	10	Active
14	Flower 2	https://res.cloudinary.com/devu5qabc/image/upload/v1784193721/flower_3_ll56kq.png	1	10000.00	10	Active
15	Flower 3	https://res.cloudinary.com/devu5qabc/image/upload/v1784193721/flower_2_k0twtb.png	1	10000.00	10	Active
24	Yellow Star 2	https://res.cloudinary.com/devu5qabc/image/upload/v1784194730/yellow-star1_xiaavu.png	1	10000.00	20	Active
\.


--
-- Data for Name: CustomerComponents; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."CustomerComponents" ("CustomerComponentId", "UserId", "Name", "ImageUrl", "ComponentType", "Price", "CreatedAt", "Status") FROM stdin;
10	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	Apple	https://res.cloudinary.com/devu5qabc/image/upload/v1780909248/2df3d09a-4443-4372-8ea4-1c419f673cc6.png?cors=anonymous	0	\N	2026-06-08 16:00:50.260638	Active
13	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	Daisy	https://res.cloudinary.com/devu5qabc/image/upload/v1782801535/b7bab4fc-31ec-4286-a1fb-6194006289f6.png?cors=anonymous	1	\N	2026-06-30 06:38:56.981418	Active
14	0ddb8972-36cd-4b67-8887-829aadbdf942	cây kéo 123	https://res.cloudinary.com/devu5qabc/image/upload/v1786891939/8400cf5c-ae23-43cd-af66-e1c990b2e642.jpg?cors=anonymous	0	\N	2026-08-16 14:52:20.255873	Active
15	0ddb8972-36cd-4b67-8887-829aadbdf942	kim cuong	https://res.cloudinary.com/devu5qabc/image/upload/v1787066648/dd41860c-fb9d-4de9-929d-1b53455aa6c2.jpg?cors=anonymous	0	\N	2026-08-18 15:23:44.024839	Active
16	0ddb8972-36cd-4b67-8887-829aadbdf942	doraemon	https://res.cloudinary.com/devu5qabc/image/upload/v1787066960/5f748574-9566-48bc-8b05-c611041cab50.png?cors=anonymous	1	\N	2026-08-18 15:29:21.027743	Active
17	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	aa		3	\N	2026-08-18 15:44:56.450452	Inactive
18	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	test2		1	\N	2026-08-18 16:59:35.660579	Inactive
19	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	Rose	https://res.cloudinary.com/devu5qabc/image/upload/v1787083221/c1682aba-4df1-47ba-9ee6-341c15e92d8f.jpg?cors=anonymous	2	\N	2026-08-18 20:00:21.971868	Inactive
20	0ddb8972-36cd-4b67-8887-829aadbdf942	jiiii	https://res.cloudinary.com/devu5qabc/image/upload/v1788883615/e1bc4d4a-96b8-45b2-b1e6-ff74ec8e2ccc.jpg?cors=anonymous	0	\N	2026-09-08 16:06:56.242048	Active
\.


--
-- Data for Name: CustomerNailComponents; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."CustomerNailComponents" ("CustomerNailComponentId", "CustomerNailId", "ComponentId", "CustomerComponentId", "PosX", "PosY", "FingerIndex", "ConfigJson") FROM stdin;
143	68	8	\N	0.15	0.33	0	{"scale":1.0854802836073956,"rotation":352.78767664702957}
144	68	6	\N	0.10	-0.24	0	{"scale":0.5,"rotation":0.0}
161	88	4	\N	0.00	0.00	1	{"scale":0.3,"rotation":0,"zIndex":10}
77	52	1	\N	49.14	48.52	2	{"scale":0.133,"rotation":0,"zIndex":10}
78	52	1	\N	0.00	0.00	0	{"scale":0.25,"rotation":0}
79	52	1	\N	0.00	0.00	1	{"scale":0.25,"rotation":0}
162	88	4	\N	0.00	0.00	3	{"scale":0.3,"rotation":0,"zIndex":10}
163	88	4	\N	0.00	0.00	0	{"scale":0.3,"rotation":0,"zIndex":10}
164	89	4	\N	0.00	0.00	3	{"scale":0.3,"rotation":0,"zIndex":10}
145	78	1	\N	0.00	0.00	1	{"scale":0.5,"rotation":0.0}
80	52	1	\N	0.00	0.00	0	{"scale":0.25,"rotation":0}
81	52	4	\N	0.00	-0.25	0	{"scale":0.2,"rotation":0}
82	52	1	\N	0.00	0.00	3	{"scale":0.25,"rotation":0}
146	78	1	\N	0.00	0.00	4	{"scale":0.45,"rotation":0.0}
147	78	9	\N	0.00	0.00	3	{"scale":0.4,"rotation":0.0}
165	89	4	\N	0.00	0.00	1	{"scale":0.3,"rotation":0,"zIndex":10}
166	89	9	\N	0.00	0.00	0	{"scale":0.3,"rotation":0,"zIndex":10}
167	89	4	\N	0.00	0.00	2	{"scale":0.3,"rotation":0,"zIndex":10}
168	89	4	\N	0.00	0.00	4	{"scale":0.3,"rotation":0,"zIndex":10}
148	81	1	\N	0.00	0.00	1	{"scale":0.5,"rotation":0.0}
169	89	4	\N	0.00	0.00	0	{"scale":0.3,"rotation":0,"zIndex":10}
30	32	1	\N	0.00	0.00	0	{"scale":0.35000000000000003,"rotation":0.0}
31	32	4	\N	0.00	0.00	1	{"scale":0.5,"rotation":0.0}
32	33	1	\N	-0.04	0.04	0	{"scale":0.30000000000000004,"rotation":0.0}
33	33	1	\N	0.00	0.00	1	{"scale":0.30000000000000004,"rotation":0.0}
34	33	1	\N	-0.04	0.00	2	{"scale":0.25000000000000006,"rotation":0.0}
35	33	1	\N	0.00	0.04	3	{"scale":0.35000000000000003,"rotation":0.0}
36	33	1	\N	0.00	-0.12	4	{"scale":0.3,"rotation":0.0}
175	31	6	\N	0.00	0.00	1	{"scale":0.5,"rotation":0.0}
176	31	6	\N	0.00	0.00	2	{"scale":0.5,"rotation":0.0}
177	31	6	\N	0.00	0.00	3	{"scale":0.5,"rotation":0.0}
178	31	6	\N	0.00	0.00	4	{"scale":0.5,"rotation":0.0}
179	31	8	\N	-0.08	-0.31	0	{"scale":0.9201233760640523,"rotation":357.8285962312768}
180	95	\N	16	-0.03	-0.01	0	{"scale":0.6817812750011311,"rotation":350.4685147263432}
181	95	8	\N	0.17	0.28	1	{"scale":0.9896363172643365,"rotation":32.64590228461417}
182	95	10	\N	-0.11	-0.32	2	{"scale":0.972845769424475,"rotation":11.652222828948823}
183	95	12	\N	-0.36	0.37	3	{"scale":0.489749523156096,"rotation":1.7267901175492857}
184	95	17	\N	0.00	0.00	4	{"scale":0.5,"rotation":0.0}
149	84	26	\N	-0.06	-0.04	0	{"scale":0.23552458423903438,"rotation":324.7993918002584}
187	103	8	\N	-0.03	-0.05	0	{"scale":0.31741738781973405,"rotation":351.36998033743464}
188	109	18	\N	-0.01	-0.02	0	{"scale":0.18467060266495106,"rotation":28.768917959499618}
150	85	23	\N	0.00	0.00	3	{"scale":0.3,"rotation":0,"zIndex":10}
151	86	23	\N	0.00	0.00	2	{"scale":0.3,"rotation":0,"zIndex":10}
152	87	4	\N	0.00	0.00	2	{"scale":0.3,"rotation":0,"zIndex":10}
59	48	4	\N	0.00	0.00	0	{"scale":0.3,"rotation":0}
60	48	4	\N	0.00	0.00	2	{"scale":0.3,"rotation":0}
61	48	4	\N	0.00	0.00	4	{"scale":0.3,"rotation":0}
62	48	4	\N	0.00	0.00	1	{"scale":0.3,"rotation":0}
63	48	4	\N	48.34	50.37	3	{"scale":0.187,"rotation":0,"zIndex":10}
64	49	4	\N	0.00	0.00	0	{"scale":0.2,"rotation":0}
65	49	2	\N	0.00	0.00	1	{"scale":0.35,"rotation":0}
66	49	4	\N	48.82	43.40	3	{"scale":0.155,"rotation":0,"zIndex":10}
153	87	4	\N	0.00	0.00	1	{"scale":0.3,"rotation":0,"zIndex":10}
154	87	4	\N	0.00	0.00	0	{"scale":0.3,"rotation":0,"zIndex":10}
155	87	9	\N	0.00	0.00	0	{"scale":0.3,"rotation":0,"zIndex":10}
156	87	4	\N	0.00	0.00	4	{"scale":0.3,"rotation":0,"zIndex":10}
157	87	4	\N	0.00	0.00	3	{"scale":0.3,"rotation":0,"zIndex":10}
159	88	4	\N	0.00	0.00	4	{"scale":0.3,"rotation":0,"zIndex":10}
158	88	9	\N	0.00	0.00	0	{"scale":0.3,"rotation":0,"zIndex":10}
160	88	4	\N	0.00	0.00	2	{"scale":0.3,"rotation":0,"zIndex":10}
\.


--
-- Data for Name: CustomerNailRequests; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."CustomerNailRequests" ("CustomerNailRequestId", "CustomerNailId", "SalonId", "Status", "RejectReason", "ApprovedArtistId", "Price", "Duration", "CreatedAt", "UpdatedAt", "IsCustomerRequest") FROM stdin;
31112ffd-dc19-430d-86ba-4f51633a6e44	31	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	Quoted	\N	53fca09d-2b87-41ae-95ea-640953f66815	10000.00	120	2026-08-18 15:44:23.107448	2026-08-18 15:46:53.055664	t
bd6239e3-21f4-4401-9db3-aabe995a8df0	95	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	Quoted	\N	53fca09d-2b87-41ae-95ea-640953f66815	0.00	0	2026-08-18 15:31:28.525423	2026-09-04 11:10:20.924973	t
9cc5682d-ca97-481c-962b-50a2b292a6d0	109	3bab09c5-20de-4a87-8415-60e2a62a16db	PendingReview	\N	\N	\N	\N	2026-09-08 16:07:47.286958	\N	t
\.


--
-- Data for Name: CustomerNails; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."CustomerNails" ("CustomerNailId", "UserId", "Name", "ImageUrl", "NailShapeId", "NailSurfaceId", "Price", "CustomColor", "Duration", "CreatedAt", "Status") FROM stdin;
31	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	White Daisy	https://res.cloudinary.com/devu5qabc/image/upload/v1782801559/3b8a7b93-fff5-461e-9268-4b91a7c9f78c.jpg?cors=anonymous	6	3	150000.00	{"mode":"perFinger","color":null,"gradient":null,"fingers":[{"fingerIndex":1,"color":"#FF0000","gradient":null},{"fingerIndex":2,"color":"#FF0000","gradient":null},{"fingerIndex":3,"color":"#FF0000","gradient":null},{"fingerIndex":4,"color":"#FF0000","gradient":null},{"fingerIndex":5,"color":"#FF0000","gradient":null}]}	120	2026-06-30 06:39:20.73043	Active
32	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	Pink Daisy	https://res.cloudinary.com/devu5qabc/image/upload/v1782815682/bb97ca17-860d-415f-86bf-10cca4179572.png?cors=anonymous	9	3	120000.00	{"mode":"perFinger","color":null,"gradient":null,"fingers":[{"fingerIndex":1,"color":"#FF0000","gradient":null},{"fingerIndex":2,"color":"#F5CBA7","gradient":null},{"fingerIndex":3,"color":"#0000FF","gradient":null},{"fingerIndex":4,"color":"#000000","gradient":null},{"fingerIndex":5,"color":"#FF0000","gradient":null}]}	22	2026-06-30 10:34:43.88552	Active
33	0ddb8972-36cd-4b67-8887-829aadbdf942	nauy	https://res.cloudinary.com/devu5qabc/image/upload/v1782815682/bb97ca17-860d-415f-86bf-10cca4179572.png?cors=anonymous	9	2	100000.00	{"mode":"perFinger","color":null,"gradient":null,"fingers":[{"fingerIndex":1,"color":"#9C27B0","gradient":null},{"fingerIndex":2,"color":"#9C27B0","gradient":null},{"fingerIndex":3,"color":"#9C27B0","gradient":null},{"fingerIndex":4,"color":"#9C27B0","gradient":null},{"fingerIndex":5,"color":"#9C27B0","gradient":null}]}	190	2026-06-30 14:23:45.566319	Active
83	8fb48656-7a6a-48eb-9c4e-6af6639e2cac	yeudoan		10	3	100000.00	\N	120	2026-08-10 07:40:31.588598	Active
74	8fb48656-7a6a-48eb-9c4e-6af6639e2cac	Thanh		1	5	100000.00	\N	120	2026-08-09 02:29:27.238355	Active
77	8fb48656-7a6a-48eb-9c4e-6af6639e2cac	Na Uy 		8	3	100000.00	\N	120	2026-08-10 02:38:51.818318	Active
80	8fb48656-7a6a-48eb-9c4e-6af6639e2cac	Thanh Thảo 229		9	3	100000.00	\N	120	2026-08-10 04:30:05.896776	Active
92	8fb48656-7a6a-48eb-9c4e-6af6639e2cac	Rose		\N	\N	0.00	\N	0	2026-08-18 03:39:53.126749	Active
95	0ddb8972-36cd-4b67-8887-829aadbdf942	trunghieu		8	3	140000.00	{"mode":"perFinger","color":null,"gradient":null,"fingers":[{"fingerIndex":1,"color":"#FF4081","gradient":null},{"fingerIndex":2,"color":"#FF4081","gradient":null},{"fingerIndex":3,"color":"#FF4081","gradient":null},{"fingerIndex":4,"color":"#FF4081","gradient":null},{"fingerIndex":5,"color":"#FF4081","gradient":null}]}	120	2026-08-18 15:22:55.723259	Active
106	0ddb8972-36cd-4b67-8887-829aadbdf942	Custom Nail		9	8	200000.00	{"mode":"perFinger","color":null,"gradient":null,"fingers":[{"fingerIndex":1,"color":"#881337","gradient":null},{"fingerIndex":2,"color":"#B7C9E2","gradient":null},{"fingerIndex":3,"color":"#D8C7F4","gradient":null},{"fingerIndex":4,"color":"#881337","gradient":null},{"fingerIndex":5,"color":"#B7C9E2","gradient":null}]}	10	2026-08-19 05:15:42.657321	Active
96	0ddb8972-36cd-4b67-8887-829aadbdf942	My Doraemon	https://res.cloudinary.com/devu5qabc/image/upload/v1787077906/02e30925-4b7c-4622-ab89-fc15ea4a3db5.png?cors=anonymous	9	8	200000.00	{"mode":"perFinger","color":null,"gradient":null,"fingers":[{"fingerIndex":1,"color":"#FB923C","gradient":{"enabled":true,"type":"linear","stops":["#FB923C","#B7C9E2","#E7E7EA"],"stopCount":3}},{"fingerIndex":2,"color":"#FB923C","gradient":{"enabled":true,"type":"linear","stops":["#FB923C","#B7C9E2","#E7E7EA"],"stopCount":3}},{"fingerIndex":3,"color":"#FB923C","gradient":{"enabled":true,"type":"linear","stops":["#FB923C","#B7C9E2","#E7E7EA"],"stopCount":3}},{"fingerIndex":4,"color":"#FB923C","gradient":{"enabled":true,"type":"linear","stops":["#FB923C","#B7C9E2","#E7E7EA"],"stopCount":3}},{"fingerIndex":5,"color":"#FB923C","gradient":{"enabled":true,"type":"linear","stops":["#FB923C","#B7C9E2","#E7E7EA"],"stopCount":3}}]}	10	2026-08-18 18:30:42.266465	Active
103	0ddb8972-36cd-4b67-8887-829aadbdf942	Custom Nail		9	3	110000.00	{"mode":"perFinger","color":null,"gradient":null,"fingers":[{"fingerIndex":1,"color":"#881337","gradient":null},{"fingerIndex":2,"color":"#FBCFE8","gradient":null},{"fingerIndex":3,"color":"#881337","gradient":null},{"fingerIndex":4,"color":"#FBCFE8","gradient":null},{"fingerIndex":5,"color":"#881337","gradient":null}]}	120	2026-08-19 05:07:36.600348	Active
43	ca83eb05-91ea-4865-b4fe-82948bf5dbbf	Cưới trắng nơ	https://res.cloudinary.com/devu5qabc/image/upload/v1783677756/8a842819-923b-4389-86f1-9478a4ae33ec.png?cors=anonymous	1	3	100000.00	{"mode":"perFinger","fingers":[{"fingerIndex":1,"mode":"solid","primaryColor":"#e6c1a2","secondaryColor":"#ef6daf"},{"fingerIndex":2,"mode":"solid","primaryColor":"#e6c1a2","secondaryColor":"#ef6daf"},{"fingerIndex":3,"mode":"solid","primaryColor":"#e6c1a2","secondaryColor":"#ef6daf"},{"fingerIndex":4,"mode":"solid","primaryColor":"#e6c1a2","secondaryColor":"#ef6daf"},{"fingerIndex":5,"mode":"solid","primaryColor":"#e6c1a2","secondaryColor":"#ef6daf"}]}	120	2026-07-10 10:02:38.499665	Active
48	ca83eb05-91ea-4865-b4fe-82948bf5dbbf	Vàng hoa cúc	https://res.cloudinary.com/devu5qabc/image/upload/v1784079144/fa447fa4-e5be-4542-8b2a-e33484ab0b15.png?cors=anonymous	1	2	100000.00	{"mode":"perFinger","fingers":[{"fingerIndex":1,"mode":"solid","primaryColor":"#1e010b","secondaryColor":"#ef6daf"},{"fingerIndex":2,"mode":"solid","primaryColor":"#1e010b","secondaryColor":"#ef6daf"},{"fingerIndex":3,"mode":"solid","primaryColor":"#1e010b","secondaryColor":"#ef6daf"},{"fingerIndex":4,"mode":"solid","primaryColor":"#1e010b","secondaryColor":"#ef6daf"},{"fingerIndex":5,"mode":"solid","primaryColor":"#1e010b","secondaryColor":"#ef6daf"}]}	120	2026-07-15 01:32:26.587442	Active
52	ca83eb05-91ea-4865-b4fe-82948bf5dbbf	Pháo hoa 3 màu	https://res.cloudinary.com/devu5qabc/image/upload/v1784108307/c7bc2f4a-52c5-4800-b780-c945493f5563.png?cors=anonymous	10	5	160000.00	{"mode":"perFinger","fingers":[{"fingerIndex":1,"mode":"solid","primaryColor":"#f3ecef","secondaryColor":"#ef6daf"},{"fingerIndex":2,"mode":"solid","primaryColor":"#f3ecef","secondaryColor":"#ef6daf"},{"fingerIndex":3,"mode":"solid","primaryColor":"#f3ecef","secondaryColor":"#ef6daf"},{"fingerIndex":4,"mode":"solid","primaryColor":"#f3ecef","secondaryColor":"#ef6daf"},{"fingerIndex":5,"mode":"solid","primaryColor":"#f3ecef","secondaryColor":"#ef6daf"}]}	180	2026-07-15 09:38:29.669372	Active
68	0ddb8972-36cd-4b67-8887-829aadbdf942	Thành		9	5	120000.00	{"mode":"perFinger","color":null,"gradient":null,"fingers":[{"fingerIndex":1,"color":"#007AFF","gradient":{"enabled":true,"type":"linear","stops":["#007AFF","#007AFF"],"stopCount":2}},{"fingerIndex":2,"color":"#007AFF","gradient":{"enabled":true,"type":"linear","stops":["#007AFF","#007AFF"],"stopCount":2}},{"fingerIndex":3,"color":"#007AFF","gradient":{"enabled":true,"type":"linear","stops":["#007AFF","#007AFF"],"stopCount":2}},{"fingerIndex":4,"color":"#007AFF","gradient":{"enabled":true,"type":"linear","stops":["#007AFF","#007AFF"],"stopCount":2}},{"fingerIndex":5,"color":"#007AFF","gradient":{"enabled":true,"type":"linear","stops":["#007AFF","#007AFF"],"stopCount":2}}]}	120	2026-08-04 15:00:09.839074	Active
75	8fb48656-7a6a-48eb-9c4e-6af6639e2cac	Thành Đẹp trai số 1 thế giới	https://res.cloudinary.com/devu5qabc/image/upload/v1786275001/b8ffb7ec-75bb-44da-929a-88e0246d009c.jpg?cors=anonymous	9	3	100000.00	\N	120	2026-08-09 11:30:01.32645	Active
78	8fb48656-7a6a-48eb-9c4e-6af6639e2cac	Thanh Thảo 229		8	3	130000.00	\N	120	2026-08-10 03:13:21.517876	Active
81	8fb48656-7a6a-48eb-9c4e-6af6639e2cac	TESTLANCUOI	https://res.cloudinary.com/devu5qabc/image/upload/v1786346571/0686df21-9cb1-4409-bad8-113ec6f34e65.jpg?cors=anonymous	10	3	110000.00	\N	120	2026-08-10 07:22:50.566223	Active
93	0ddb8972-36cd-4b67-8887-829aadbdf942	Nail		\N	\N	0.00	\N	0	2026-08-18 06:37:05.76805	Active
107	0ddb8972-36cd-4b67-8887-829aadbdf942	Custom Nail		9	9	200000.00	{"mode":"perFinger","color":null,"gradient":null,"fingers":[{"fingerIndex":1,"color":"#881337","gradient":null},{"fingerIndex":2,"color":"#D9E7E2","gradient":null},{"fingerIndex":3,"color":"#D8C7F4","gradient":null},{"fingerIndex":4,"color":"#881337","gradient":null},{"fingerIndex":5,"color":"#D9E7E2","gradient":null}]}	10	2026-08-19 05:22:21.680838	Active
108	0ddb8972-36cd-4b67-8887-829aadbdf942	Custom Nail		9	5	100000.00	{"mode":"perFinger","color":null,"gradient":null,"fingers":[{"fingerIndex":1,"color":"#1E293B","gradient":null},{"fingerIndex":2,"color":"#C7D8F4","gradient":null},{"fingerIndex":3,"color":"#B7C9E2","gradient":null},{"fingerIndex":4,"color":"#1E293B","gradient":null},{"fingerIndex":5,"color":"#C7D8F4","gradient":null}]}	120	2026-08-19 05:32:18.641555	Active
105	0ddb8972-36cd-4b67-8887-829aadbdf942	Custom Nail		9	3	100000.00	{"mode":"perFinger","color":null,"gradient":null,"fingers":[{"fingerIndex":1,"color":"#881337","gradient":null},{"fingerIndex":2,"color":"#B7C9E2","gradient":null},{"fingerIndex":3,"color":"#FBCFE8","gradient":null},{"fingerIndex":4,"color":"#881337","gradient":null},{"fingerIndex":5,"color":"#B7C9E2","gradient":null}]}	120	2026-08-19 05:12:52.899789	Inactive
76	8fb48656-7a6a-48eb-9c4e-6af6639e2cac	THANHDT	https://res.cloudinary.com/devu5qabc/image/upload/v1786297014/e8734c94-2c2d-4c02-94cf-952dfa9a54e8.jpg?cors=anonymous	9	3	100000.00	\N	120	2026-08-09 17:36:55.629635	Active
82	8fb48656-7a6a-48eb-9c4e-6af6639e2cac	TESTLANCUOI1	https://res.cloudinary.com/devu5qabc/image/upload/v1786347066/edf576b8-4d1e-4595-b22d-872cdd54b5d3.jpg?cors=anonymous	10	3	100000.00	\N	120	2026-08-10 07:31:05.083051	Active
84	0ddb8972-36cd-4b67-8887-829aadbdf942	móng của Mimh	https://res.cloudinary.com/devu5qabc/image/upload/v1786891443/e4c84496-1a29-450c-8dcc-9dffebd8ff7c.jpg?cors=anonymous	8	8	320000.00	{"mode":"perFinger","color":null,"gradient":null,"fingers":[{"fingerIndex":1,"color":"#FF4081","gradient":{"enabled":true,"type":"linear","stops":["#18D072","#BA0240"],"stopCount":2}},{"fingerIndex":2,"color":"#FF4081","gradient":null},{"fingerIndex":3,"color":"#FF4081","gradient":null},{"fingerIndex":4,"color":"#FF4081","gradient":null},{"fingerIndex":5,"color":"#FF4081","gradient":null}]}	160	2026-08-16 14:44:04.214549	Active
85	ca83eb05-91ea-4865-b4fe-82948bf5dbbf	Champagne Gold Mirror • Squoval • Cat Eyes	https://res.cloudinary.com/devu5qabc/image/upload/v1786960812/9f67d398-1ab7-4f9e-9541-781ca63ef5ba.png?cors=anonymous	8	8	210000.00	{"mode":"perFinger","fingers":[{"mode":"solid","primaryColor":"#ecdfe3","secondaryColor":"#ef6daf","gradientStops":["#ecdfe3","#ef6daf"],"fingerIndex":1,"gradient":{"stops":["#ecdfe3","#ef6daf"]}},{"mode":"solid","primaryColor":"#ecdfe3","secondaryColor":"#ef6daf","gradientStops":["#ecdfe3","#ef6daf"],"fingerIndex":2,"gradient":{"stops":["#ecdfe3","#ef6daf"]}},{"mode":"solid","primaryColor":"#ecdfe3","secondaryColor":"#ef6daf","gradientStops":["#ecdfe3","#ef6daf"],"fingerIndex":3,"gradient":{"stops":["#ecdfe3","#ef6daf"]}},{"mode":"solid","primaryColor":"#ecdfe3","secondaryColor":"#ef6daf","gradientStops":["#ecdfe3","#ef6daf"],"fingerIndex":4,"gradient":{"stops":["#ecdfe3","#ef6daf"]}},{"mode":"solid","primaryColor":"#ecdfe3","secondaryColor":"#ef6daf","gradientStops":["#ecdfe3","#ef6daf"],"fingerIndex":5,"gradient":{"stops":["#ecdfe3","#ef6daf"]}}]}	10	2026-08-17 10:00:14.477273	Active
94	0ddb8972-36cd-4b67-8887-829aadbdf942	hieu		\N	\N	0.00	\N	0	2026-08-18 15:18:17.380033	Active
49	4cbb0cf7-b545-4352-b3f4-8ffe69ea43b2	Pháo hoa 3 màu	https://res.cloudinary.com/devu5qabc/image/upload/v1784080088/1a80d205-4dbd-47b2-b191-6771fc2fa22e.png?cors=anonymous	9	3	130000.00	{"mode":"perFinger","fingers":[{"fingerIndex":1,"mode":"solid","primaryColor":"#FF69B4","secondaryColor":"#ef6daf"},{"fingerIndex":2,"mode":"solid","primaryColor":"#FF4081","secondaryColor":"#ef6daf"},{"fingerIndex":3,"mode":"solid","primaryColor":"#FF4081","secondaryColor":"#ef6daf"},{"fingerIndex":4,"mode":"solid","primaryColor":"#FF4081","secondaryColor":"#ef6daf"},{"fingerIndex":5,"mode":"solid","primaryColor":"#FF4081","secondaryColor":"#ef6daf"}]}	132	2026-07-15 01:48:10.360226	Active
104	0ddb8972-36cd-4b67-8887-829aadbdf942	Custom Nail		9	3	100000.00	{"mode":"perFinger","color":null,"gradient":null,"fingers":[{"fingerIndex":1,"color":"#FBCFE8","gradient":null},{"fingerIndex":2,"color":"#FFFFFF","gradient":null},{"fingerIndex":3,"color":"#B7C9E2","gradient":null},{"fingerIndex":4,"color":"#FBCFE8","gradient":null},{"fingerIndex":5,"color":"#FFFFFF","gradient":null}]}	120	2026-08-19 05:08:55.161411	Active
109	0ddb8972-36cd-4b67-8887-829aadbdf942	hi	https://res.cloudinary.com/devu5qabc/image/upload/v1788883477/e4558bf6-5abd-4278-ab3e-95da5de3c0bb.jpg?cors=anonymous	9	5	110000.00	{"mode":"perFinger","color":null,"gradient":null,"fingers":[{"fingerIndex":1,"color":"#000000","gradient":null},{"fingerIndex":2,"color":"#FF3B30","gradient":null},{"fingerIndex":3,"color":"#FF3B30","gradient":null},{"fingerIndex":4,"color":"#FF3B30","gradient":null},{"fingerIndex":5,"color":"#FF3B30","gradient":null}]}	35	2026-09-08 16:04:38.232212	Active
86	ca83eb05-91ea-4865-b4fe-82948bf5dbbf	Champagne Gold Mirror • Squoval • Cat Eyes	https://res.cloudinary.com/devu5qabc/image/upload/v1786961764/c01e973e-2153-47e7-8ec5-5fc77ce34198.png?cors=anonymous	8	8	210000.00	{"mode":"perFinger","fingers":[{"mode":"gradient","primaryColor":"#ecdfe3","secondaryColor":"#ef6daf","gradientStops":["#ecdfe3","#ef6daf"],"fingerIndex":1,"gradient":{"stops":["#ecdfe3","#ef6daf"]}},{"mode":"gradient","primaryColor":"#ecdfe3","secondaryColor":"#ef6daf","gradientStops":["#ecdfe3","#ef6daf"],"fingerIndex":2,"gradient":{"stops":["#ecdfe3","#ef6daf"]}},{"mode":"gradient","primaryColor":"#ecdfe3","secondaryColor":"#ef6daf","gradientStops":["#ecdfe3","#ef6daf"],"fingerIndex":3,"gradient":{"stops":["#ecdfe3","#ef6daf"]}},{"mode":"gradient","primaryColor":"#ecdfe3","secondaryColor":"#ef6daf","gradientStops":["#ecdfe3","#ef6daf"],"fingerIndex":4,"gradient":{"stops":["#ecdfe3","#ef6daf"]}},{"mode":"gradient","primaryColor":"#ecdfe3","secondaryColor":"#ef6daf","gradientStops":["#ecdfe3","#ef6daf"],"fingerIndex":5,"gradient":{"stops":["#ecdfe3","#ef6daf"]}}]}	10	2026-08-17 10:16:06.542107	Active
87	ca83eb05-91ea-4865-b4fe-82948bf5dbbf	Champagne Gold Mirror • Squoval • Cat Eyes	https://res.cloudinary.com/devu5qabc/image/upload/v1786965988/786fd5f8-9a1e-41bb-bcf1-c3621160845c.png?cors=anonymous	7	2	110000.00	{"mode":"perFinger","fingers":[{"mode":"solid","primaryColor":"#1e010b","secondaryColor":"#ef6daf","gradientStops":["#1e010b","#ef6daf"],"fingerIndex":1,"gradient":{"stops":["#1e010b","#ef6daf"]}},{"mode":"solid","primaryColor":"#1e010b","secondaryColor":"#ef6daf","gradientStops":["#1e010b","#ef6daf"],"fingerIndex":2,"gradient":{"stops":["#1e010b","#ef6daf"]}},{"mode":"solid","primaryColor":"#1e010b","secondaryColor":"#ef6daf","gradientStops":["#1e010b","#ef6daf"],"fingerIndex":3,"gradient":{"stops":["#1e010b","#ef6daf"]}},{"mode":"solid","primaryColor":"#1e010b","secondaryColor":"#ef6daf","gradientStops":["#1e010b","#ef6daf"],"fingerIndex":4,"gradient":{"stops":["#1e010b","#ef6daf"]}},{"mode":"solid","primaryColor":"#1e010b","secondaryColor":"#ef6daf","gradientStops":["#1e010b","#ef6daf"],"fingerIndex":5,"gradient":{"stops":["#1e010b","#ef6daf"]}}]}	120	2026-08-17 11:26:29.86844	Active
88	ca83eb05-91ea-4865-b4fe-82948bf5dbbf	Champagne Gold Mirror • Squoval • Cat Eyes	https://res.cloudinary.com/devu5qabc/image/upload/v1786966088/545cbc05-657a-45fa-ba36-ea670bd6dae6.png?cors=anonymous	7	2	110000.00	{"mode":"perFinger","fingers":[{"mode":"solid","primaryColor":"#1e010b","secondaryColor":"#ef6daf","gradientStops":["#1e010b","#ef6daf"],"fingerIndex":1,"gradient":{"stops":["#1e010b","#ef6daf"]}},{"mode":"solid","primaryColor":"#1e010b","secondaryColor":"#ef6daf","gradientStops":["#1e010b","#ef6daf"],"fingerIndex":2,"gradient":{"stops":["#1e010b","#ef6daf"]}},{"mode":"solid","primaryColor":"#1e010b","secondaryColor":"#ef6daf","gradientStops":["#1e010b","#ef6daf"],"fingerIndex":3,"gradient":{"stops":["#1e010b","#ef6daf"]}},{"mode":"solid","primaryColor":"#1e010b","secondaryColor":"#ef6daf","gradientStops":["#1e010b","#ef6daf"],"fingerIndex":4,"gradient":{"stops":["#1e010b","#ef6daf"]}},{"mode":"solid","primaryColor":"#1e010b","secondaryColor":"#ef6daf","gradientStops":["#1e010b","#ef6daf"],"fingerIndex":5,"gradient":{"stops":["#1e010b","#ef6daf"]}}]}	120	2026-08-17 11:28:11.126928	Active
89	ca83eb05-91ea-4865-b4fe-82948bf5dbbf	Champagne Gold Mirror • Squoval • Cat Eyes	https://res.cloudinary.com/devu5qabc/image/upload/v1786966145/5409d95d-00c6-48e6-a124-36cf167d8d3e.png?cors=anonymous	8	8	260000.00	{"mode":"perFinger","fingers":[{"mode":"solid","primaryColor":"#1e010b","secondaryColor":"#ef6daf","gradientStops":["#1e010b","#ef6daf"],"fingerIndex":1,"gradient":{"stops":["#1e010b","#ef6daf"]}},{"mode":"solid","primaryColor":"#1e010b","secondaryColor":"#ef6daf","gradientStops":["#1e010b","#ef6daf"],"fingerIndex":2,"gradient":{"stops":["#1e010b","#ef6daf"]}},{"mode":"solid","primaryColor":"#1e010b","secondaryColor":"#ef6daf","gradientStops":["#1e010b","#ef6daf"],"fingerIndex":3,"gradient":{"stops":["#1e010b","#ef6daf"]}},{"mode":"solid","primaryColor":"#1e010b","secondaryColor":"#ef6daf","gradientStops":["#1e010b","#ef6daf"],"fingerIndex":4,"gradient":{"stops":["#1e010b","#ef6daf"]}},{"mode":"solid","primaryColor":"#1e010b","secondaryColor":"#ef6daf","gradientStops":["#1e010b","#ef6daf"],"fingerIndex":5,"gradient":{"stops":["#1e010b","#ef6daf"]}}]}	10	2026-08-17 11:29:06.867882	Active
\.


--
-- Data for Name: CustomerQuizAnswers; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."CustomerQuizAnswers" ("CustomerQuizAnswerId", "CustomerId", "QuizQuestionId", "QuizOptionId", "CreatedAt") FROM stdin;
12c013ec-ff84-4e61-89aa-af7ad5b74892	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	22c4dbf5-ea82-45ed-9131-d4c0df76af9b	6028325b-41de-47e2-8d54-8defcc41579d	2026-09-06 19:26:06.503905
1a288576-5646-417f-853f-17d13d18c480	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	7203d821-5800-46cf-bec8-15fb088c0014	2d9b0f7b-6efe-4fdd-9b9a-aebf80316949	2026-09-06 19:26:06.504088
279f6b32-2391-4f16-b113-4d5cab06fda0	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	e2463373-6243-46e7-a3af-15e7784caf37	1990255d-a453-4385-b6f8-3eb483444d04	2026-09-06 19:26:06.504237
3cc1f9dd-a2f2-4138-bc66-f7f83465d7b4	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	966fe4df-abaf-4584-a157-3ca15e4890e3	7aa7498d-fb46-4214-89de-77056bf4dffa	2026-09-06 19:26:06.504102
56fe17fe-8095-494c-bd3f-f0e8898a0d5c	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	0dd4b558-8f84-442d-8189-66b792759ef9	77c19b0f-805a-495d-a7e4-92b240b7ce16	2026-09-06 19:26:06.503708
83b9b16b-42eb-4f4f-be29-54cf7475a49f	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	27e31006-46be-40e5-906c-4d331db42c6b	2b86d46c-cd45-4d7a-917f-6643af5bf217	2026-09-06 19:26:06.503928
a5518a16-98cb-498a-818a-eb819225fc53	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	6881e8b7-284c-475d-9798-151891f65833	953eafef-ae14-4a41-bb97-185b3c751810	2026-09-06 19:26:06.504074
b8ae7328-398b-42a2-b1a5-d064d7067176	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	b67f4f32-1355-46b1-872b-736a59df7aea	f80f8fa5-625e-4ab1-afb6-7b14fdc22052	2026-09-06 19:26:06.504115
d4f7ca83-a257-4dfc-b0ef-09b0bcc89723	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	dcb0547b-70dc-42da-863e-a8d79dbf64d9	8decbe4c-fc19-4fc6-b7f4-efba0b4d4d57	2026-09-06 19:26:06.50383
d7d6f37d-8053-4359-a8af-f9359e71277f	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	c6c7ca9d-39b7-4258-8d38-5773ddb1e6b9	1f420222-9e1d-49fd-b3b5-786530e12770	2026-09-06 19:26:06.504133
f29803b8-ad32-4f29-9345-16e983b57782	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	6881e8b7-284c-475d-9798-151891f65833	9a358da8-dc5b-4925-af9c-d09e5165c661	2026-09-06 19:26:06.504047
149f0e40-20d4-4a6a-82c8-6c52839c7858	0ddb8972-36cd-4b67-8887-829aadbdf942	c6c7ca9d-39b7-4258-8d38-5773ddb1e6b9	b9e1dd98-cca5-4512-acbc-4e7e39ba8be9	2026-09-08 16:15:06.070411
28203f80-8b87-447b-b3de-3da15a9a8d36	0ddb8972-36cd-4b67-8887-829aadbdf942	27e31006-46be-40e5-906c-4d331db42c6b	1cdc5792-b79c-4eaa-b2b0-4908f90f4533	2026-09-08 16:15:06.070287
6f0a3841-1c02-411f-a4f0-4d53fb75b8de	0ddb8972-36cd-4b67-8887-829aadbdf942	6881e8b7-284c-475d-9798-151891f65833	393ebc83-590c-4890-afe5-d24a665059d3	2026-09-08 16:15:06.07031
7671d12e-b109-47a1-a284-b8ec5f729906	0ddb8972-36cd-4b67-8887-829aadbdf942	dcb0547b-70dc-42da-863e-a8d79dbf64d9	bde5162e-7f15-4e98-8f8a-aba4549ff8f8	2026-09-08 16:15:06.070259
7ddd63bb-6330-455e-99d9-7fd324c7b7d9	0ddb8972-36cd-4b67-8887-829aadbdf942	966fe4df-abaf-4584-a157-3ca15e4890e3	384b8dfa-1b7b-4fbc-b574-ee36a815809c	2026-09-08 16:15:06.070379
81d44a3d-3a59-4728-a70b-f33f34bd7cdf	0ddb8972-36cd-4b67-8887-829aadbdf942	0dd4b558-8f84-442d-8189-66b792759ef9	361f7913-475a-4fc7-a56e-50dfbe885e9f	2026-09-08 16:15:06.058675
b78770ea-978a-4f21-9a67-8fe40b625142	0ddb8972-36cd-4b67-8887-829aadbdf942	b67f4f32-1355-46b1-872b-736a59df7aea	dbd533eb-910a-463f-b480-9965ee9318f5	2026-09-08 16:15:06.070396
d38cd5f5-58c0-480e-afc6-b0278e413ac5	0ddb8972-36cd-4b67-8887-829aadbdf942	7203d821-5800-46cf-bec8-15fb088c0014	6538ddde-5206-430a-92cb-02f61a76b48d	2026-09-08 16:15:06.070355
d39f1bb1-7d5b-4fe2-969e-0d868686377e	0ddb8972-36cd-4b67-8887-829aadbdf942	e2463373-6243-46e7-a3af-15e7784caf37	42efc7dd-d11a-4756-9ed7-a08e054327f9	2026-09-08 16:15:06.070426
ee378221-5670-44b9-95e9-08edfc076ff8	0ddb8972-36cd-4b67-8887-829aadbdf942	22c4dbf5-ea82-45ed-9131-d4c0df76af9b	55eb6b76-a85e-4aa6-a36b-bd7cb22964a7	2026-09-08 16:15:06.070176
\.


--
-- Data for Name: Customers; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Customers" ("UserId", "LoyaltyPoint", "SkinTone", "Occupation", "NailCondition", "LifetimePoints", "LoyaltyTierId", "PreferredColorsJson", "PreferredComplexity", "PreferredNailShapeId", "PreferredOccasionsJson", "PreferredStylesJson", "HandShape", "SkinShade") FROM stdin;
c6c3645a-d5a7-4dab-808c-c4fb099ad469	0	Cool	Accounting	Healthy	0	\N	["#FFFFFF", "#FFB6C1", "#DDA0DD"]	moderate	10	["Wedding", "Party"]	["French", "Floral Pattern", "Rhinestones"]	Long, slender fingers	Light
2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	345	5	Musician	Healthy	405	5	["#FFFFFF","#FCF2F2"]	complex	9	["22","14","71","18"]	["30","39"]	Long, slender fingers	Medium
c653b310-12c6-4a61-91b9-83b85af24635	0				0	\N			\N				
d99f9bee-3505-4f1a-9627-461402ca1afc	0				0	\N			\N				
c933ebef-a971-4e39-82b9-6b22465261cd	0				0	\N			\N				
0ddb8972-36cd-4b67-8887-829aadbdf942	224	10	Teacher	Soft	374	5	["#ECDFE3","#FFD700"]	complex	8	["20","17"]	["36","35","79"]	Short Fingers	Dark
2feceac3-bbde-4659-a3d5-4e7e10b0a260	10				10	\N			\N				
4ddd8b89-bc84-47f0-b02d-0db0aa414434	0				0	\N			\N				
39ee1410-5e30-4c82-9a43-1163444951da	0	Cool	Attorney	Healthy	0	\N	["#2C3E50", "#95A5A6", "#BDC3C7"]	simple	6	["Commute", "Daily"]	["Minimalist", "Solid", "Matte"]	Big hand	Light
3c63f226-f626-4757-9683-8e0376606b1a	0	Warm	Office	Healthy	0	\N	["#F5F5DC", "#D4A5A5", "#C9B1A0"]	simple	6	["Commute", "Daily"]	["Minimalist", "Solid"]	Elonged finger	Light
3d089065-f088-4508-b5b3-f89b47722125	0	Cool	Designer	Healthy	0	\N	["#FF1493", "#8B008B", "#FFD700"]	complex	7	["Party", "Birthday", "Wedding"]	["Rhinestone", "Ombre", "Pattern"]	Dolphin Finger	Medium
f9e14612-1973-4180-98a4-7e1b239d242c	0	Warm	Painter	Healthy	0	\N	["#FF6B6B", "#4ECDC4", "#FFE66D"]	complex	1	["Event", "Party"]	["Texture", "Ombre", "Geometric"]	Long finger	Medium
08db5518-d779-4024-a345-3a7acace4095	0	Cool	Student	Healthy	0	\N	["#ff0000"]	simple	1	["Commute", "Daily"]	["Minimalist", "Solid", "Matte"]\r\n	Small Hands	Dark
f296db48-0e31-42f5-8a15-b939728e9cb1	0	Warm	Accounting	Soft	0	\N	["#FF1493", "#8B008B", "#FFD700"]	complex	10	["Party", "Birthday", "Wedding"]	["Minimalist", "Solid"]	Dolphin Finger	Light
8fb48656-7a6a-48eb-9c4e-6af6639e2cac	0	Warm	Accounting	Soft	0	\N	["#FF1493", "#8B008B", "#FFD700"]	moderate	10	["Party", "Birthday", "Wedding"]	["Minimalist", "Solid"]	Dolphin Finger	Light
\.


--
-- Data for Name: FavoriteNails; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."FavoriteNails" ("FavoriteNailId", "UserId", "NailDesignId", "NailVariantId", "CreatedAt") FROM stdin;
1	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	1	\N	2026-08-06 20:13:56.459676
2	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	2	\N	2026-08-08 17:19:46.951301
3	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	5	\N	2026-08-08 17:19:48.213065
4	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	4	\N	2026-08-08 17:19:48.939807
5	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	6	\N	2026-08-08 17:19:50.353679
6	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	9	\N	2026-08-08 17:19:50.954806
9	0ddb8972-36cd-4b67-8887-829aadbdf942	2	\N	2026-09-08 15:46:04.364284
\.


--
-- Data for Name: LoyaltyTiers; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."LoyaltyTiers" ("LoyaltyTierId", "Name", "Description", "MinLifetimePoints", "MaxLifetimePoints", "DiscountRate", "ImageUrl", "BackgroundColor", "TextColor", "ColorJson", "Status", "SortOrder") FROM stdin;
2	Bạc	Bạc	51	100	0.002	https://res.cloudinary.com/dkmm5rh28/image/upload/v1781861251/2-removebg-preview_jj0ab7.png	#A8A9AD	#FFFFFF	{"primary": "#A8A9AD","gradientStart": "#A8A9AD","gradientEnd": "#7E7F83"}	Active	2
3	Vàng	Vàng	101	150	0.005	https://res.cloudinary.com/dkmm5rh28/image/upload/v1781861251/3-removebg-preview_gxjjoj.png	#F5C842	#FFFFFF	{"primary": "#F5C842","gradientStart": "#F5C842","gradientEnd": "#D4A820"}	Active	3
5	Platinum	Platinum	201	\N	0.01	https://res.cloudinary.com/dkmm5rh28/image/upload/v1781861251/5-removebg-preview_h4zvjg.png	#9B7BB8	#FFFFFF	{"primary": "#9B7BB8","gradientStart": "#9B7BB8","gradientEnd": "#7A5A97"}	Inactive	5
1	Đồng	Đồng	0	50	0	https://res.cloudinary.com/dkmm5rh28/image/upload/v1781861251/1-removebg-preview_mr4rce.png	#D48138	#FFFFFF	{"primary": "#D48138","gradientStart": "#D48138","gradientEnd": "#A86F3C"}	Inactive	1
4	Kim Cương	Kim Cương	151	500	0.007	https://res.cloudinary.com/dkmm5rh28/image/upload/v1781861251/4-removebg-preview_iqbguq.png	#5BC0EB	#FFFFFF	{"primary":"#5BC0EB","gradientStart":"#5BC0EB","gradientEnd":"#5BC0EB"}	Active	4
\.


--
-- Data for Name: LoyaltyTransactions; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."LoyaltyTransactions" ("LoyaltyTransactionId", "CustomerId", "BookingId", "Points", "TransactionType", "CreatedAt", "Description") FROM stdin;
87	0ddb8972-36cd-4b67-8887-829aadbdf942	8833af4e-bb29-486b-9a45-7bd33f4241e1	10	Earned	2026-08-19 07:10:36.587433	\N
88	0ddb8972-36cd-4b67-8887-829aadbdf942	5ed810a0-f0ee-4046-85ba-62183a3a49da	10	Earned	2026-08-19 07:10:37.85578	\N
89	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	ddc31886-f48e-4fcc-943c-d3cc9e14c8f5	10	Earned	2026-08-19 07:10:38.658347	\N
90	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	84853280-d3b7-4150-901a-4dd60e63d745	10	Earned	2026-08-19 07:10:39.444495	\N
91	0ddb8972-36cd-4b67-8887-829aadbdf942	0ecf1a61-9159-4bbd-9b2c-45a83bd42e4c	10	Earned	2026-08-19 07:10:40.214644	\N
92	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	f4b92bcb-3365-4354-9902-f9c084922767	10	Earned	2026-08-19 07:10:40.998487	\N
93	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	2993e6d6-b957-436d-a0f2-b5d9782eeddd	10	Earned	2026-08-19 07:10:41.876338	\N
94	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	571bda98-a035-4b14-9d2a-6e6a6bb2acf2	10	Earned	2026-08-19 07:10:42.698128	\N
95	0ddb8972-36cd-4b67-8887-829aadbdf942	bd3b6417-fa2b-4ef4-8ab7-3e33c61efff0	10	Earned	2026-08-19 07:10:43.465315	\N
96	0ddb8972-36cd-4b67-8887-829aadbdf942	4e3f981e-f1f3-42d5-b67b-299f07b178c2	10	Earned	2026-08-19 07:10:44.293103	\N
97	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	c1106f3d-8095-443e-b035-2df761fcac6d	10	Earned	2026-08-19 07:10:45.116343	\N
98	0ddb8972-36cd-4b67-8887-829aadbdf942	98a90c97-1cfd-4eb6-8c85-33d6ffbac9bf	10	Earned	2026-08-19 07:10:45.977507	\N
99	0ddb8972-36cd-4b67-8887-829aadbdf942	dabb9a56-e346-4f3b-9a8d-7e4c175ff1fc	10	Earned	2026-08-19 07:10:46.892406	\N
100	0ddb8972-36cd-4b67-8887-829aadbdf942	c2a91a31-1018-4a5e-a8e1-36e7249fecee	10	Earned	2026-08-19 07:10:47.661954	\N
101	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	931c5c48-eace-4ef1-a173-b791a22ca726	10	Earned	2026-08-19 07:10:48.431319	\N
102	0ddb8972-36cd-4b67-8887-829aadbdf942	bd7f9cab-5cc5-4002-a0f2-9e40d8212d34	10	Earned	2026-08-19 07:10:49.341589	\N
103	0ddb8972-36cd-4b67-8887-829aadbdf942	6f816baa-48a9-477d-9700-db60482ab272	10	Earned	2026-08-19 07:10:50.252315	\N
104	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	4b713e5d-d0da-4c9d-81ee-216f510cde31	10	Earned	2026-08-19 07:10:50.920019	\N
105	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	c13ad8a8-e179-48c0-9907-af31451e0565	10	Earned	2026-08-19 07:10:51.639021	\N
106	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	32060188-f6aa-42d5-b12c-60c30b579d58	10	Earned	2026-08-19 09:29:23.337333	\N
109	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	738529a7-3c13-4311-b5fc-2005f1aba573	10	Earned	2026-08-23 14:53:38.408062	\N
110	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	3a81ba1b-9a1c-48db-9094-4d318ba065e4	10	Earned	2026-08-23 14:55:34.754604	\N
111	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	f4730e5d-340c-4121-bddf-923e4d400c6c	10	Earned	2026-08-28 05:07:20.364556	\N
116	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	70eb03a9-e158-4428-a196-e4d81eb232d3	10	Earned	2026-08-28 06:56:49.753017	\N
117	0ddb8972-36cd-4b67-8887-829aadbdf942	\N	-100	Redeemed	2026-09-04 14:42:29.606877	Đổi voucher 'Giảm giá 2/9' (Trừ 100 điểm)
118	0ddb8972-36cd-4b67-8887-829aadbdf942	8a2e0d2e-8ecf-4f89-9fa3-354275e60f02	14	Earned	2026-09-04 16:05:32.335287	Tích điểm từ đơn hàng #8a2e0d2e-8ecf-4f89-9fa3-354275e60f02 (Thanh toán 148,500đ)
119	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	\N	-100	Redeemed	2026-09-06 07:03:03.671044	Đổi voucher 'Giảm giá 2/9' (Trừ 100 điểm)
120	0ddb8972-36cd-4b67-8887-829aadbdf942	\N	-50	Redeemed	2026-09-06 10:43:20.286788	Đổi voucher 'ThanhDT' (Trừ 50 điểm)
123	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	3a81ba1b-9a1c-48db-9094-4d318ba065e4	20	Refund	2026-09-07 14:51:04.186582	Hoàn 20 điểm vào ví. Lý do: Hoan tien
124	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	3a81ba1b-9a1c-48db-9094-4d318ba065e4	20	Refund	2026-09-07 15:00:19.248655	Hoàn 20 điểm vào ví. Lý do: Hoan tien
\.


--
-- Data for Name: NailArtistBreaks; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."NailArtistBreaks" ("NailArtistBreakId", "NailArtistId", "BreakDate", "StartTime", "EndTime", "Reason", "Status", "RejectReason") FROM stdin;
1ec096fe-075f-4b4e-bd6c-6588d4fd8491	53fca09d-2b87-41ae-95ea-640953f66815	2026-07-29 17:00:00	09:00:00	10:00:00	Toi bi benh	Approved	\N
cae73631-9f50-4c36-a486-5190540af1c7	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-08 00:00:00	09:00:00	17:00:00	[EMERGENCY OFF] Bị sốt đột xuất 08:00 sáng.	Approved	\N
\.


--
-- Data for Name: NailArtistSkills; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."NailArtistSkills" ("NailArtistSkillId", "NailArtistId", "SkillTypeId", "Level") FROM stdin;
34bbb52c-a25f-4c52-bae1-65893a8dcaa3	53fca09d-2b87-41ae-95ea-640953f66815	11e07623-b990-4c3d-bfa0-b8f0a72a901a	3
d49e6822-cd76-4d2a-8880-0051fcc52fe3	53fca09d-2b87-41ae-95ea-640953f66815	13363e83-677c-4e5b-befe-bbc018d68373	3
5eb5c5e2-82b5-47c4-885a-44907905ed3d	53fca09d-2b87-41ae-95ea-640953f66815	4a20f59e-2505-4b11-8eff-68ff16b39252	3
7d617f2f-03f3-49df-95ca-35f8c1e28589	2adce07e-6ef8-4f7d-8920-accc288417e9	4a20f59e-2505-4b11-8eff-68ff16b39252	5
8cbb81ac-c273-4467-b179-0aa266620d5d	2adce07e-6ef8-4f7d-8920-accc288417e9	11e07623-b990-4c3d-bfa0-b8f0a72a901a	5
d8a91a0c-6bb0-4b10-b28c-e5e6c6310fb7	2adce07e-6ef8-4f7d-8920-accc288417e9	13363e83-677c-4e5b-befe-bbc018d68373	5
5b59541f-847b-4fd9-bbdd-6654d7a6ae67	f3a2a4e5-ec22-40fc-91bd-e89c48c7c9ee	4a20f59e-2505-4b11-8eff-68ff16b39252	5
b8f7cc42-972c-4f17-9936-6ba2d2bcdddc	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	4a20f59e-2505-4b11-8eff-68ff16b39252	5
00c168cd-d670-437f-8f38-5b3b0b864e31	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	11e07623-b990-4c3d-bfa0-b8f0a72a901a	5
2ec9cad4-22d2-4f48-859a-a98ef499dd84	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	13363e83-677c-4e5b-befe-bbc018d68373	5
a47e674a-3057-4125-97b1-601e1c21a920	dba37f5a-970c-41da-be69-438732e98402	11e07623-b990-4c3d-bfa0-b8f0a72a901a	4
40f5a17c-3013-43bd-913c-74b47a68c22c	dba37f5a-970c-41da-be69-438732e98402	4a20f59e-2505-4b11-8eff-68ff16b39252	5
6dee3a96-3dd6-4ed0-bc61-600c1e9b813b	dba37f5a-970c-41da-be69-438732e98402	13363e83-677c-4e5b-befe-bbc018d68373	5
41ceb638-c682-4987-a422-08ed8b01aad4	dba37f5a-970c-41da-be69-438732e98402	525de8e8-1624-485c-afb2-b0f067156db1	4
9a4b4f1d-79d2-4872-a5e8-a2ed813e4177	60203747-3361-4008-8e0d-cded346095c8	13363e83-677c-4e5b-befe-bbc018d68373	3
ec567746-82d1-4be7-8ee1-668656f0e9d6	60203747-3361-4008-8e0d-cded346095c8	11e07623-b990-4c3d-bfa0-b8f0a72a901a	4
e67d0d3f-26d8-49e9-ba3c-4ab49f949b2c	60203747-3361-4008-8e0d-cded346095c8	4a20f59e-2505-4b11-8eff-68ff16b39252	4
63ea2ded-7083-4cc8-89d1-f8c329a4f71c	b53808e3-7219-4c65-899c-197f204e5581	4a20f59e-2505-4b11-8eff-68ff16b39252	5
1a900b5d-8ac7-4cf0-b793-d4e60ea765a4	b53808e3-7219-4c65-899c-197f204e5581	525de8e8-1624-485c-afb2-b0f067156db1	5
84d0fe4d-aff5-4eeb-a83e-4969ca310767	b53808e3-7219-4c65-899c-197f204e5581	11e07623-b990-4c3d-bfa0-b8f0a72a901a	4
20bb9bc0-aa6e-43ff-8507-ad58426bde02	b53808e3-7219-4c65-899c-197f204e5581	13363e83-677c-4e5b-befe-bbc018d68373	3
0a22a224-b20f-4dd3-9857-1430b05ba2e7	00eac1e0-417a-4377-b718-5fee2ca8aae1	525de8e8-1624-485c-afb2-b0f067156db1	3
1a267b79-4a43-4aba-a829-8509dc332841	00eac1e0-417a-4377-b718-5fee2ca8aae1	11e07623-b990-4c3d-bfa0-b8f0a72a901a	2
8dd062ae-e040-4e19-bf7c-b6cfa35abaae	00eac1e0-417a-4377-b718-5fee2ca8aae1	4a20f59e-2505-4b11-8eff-68ff16b39252	5
dfb64fac-abda-4965-aea8-8f6707e2636d	00eac1e0-417a-4377-b718-5fee2ca8aae1	13363e83-677c-4e5b-befe-bbc018d68373	5
\.


--
-- Data for Name: NailArtists; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."NailArtists" ("NailArtistId", "AccountId", "Status", "ConcurrentCapacity") FROM stdin;
618e9f63-8360-4cf4-b178-9457dd66761b	eb504fd5-28c0-47d7-bd06-982d9e497769	Active	1
c487ee7e-9e66-4c86-a015-1c22e9ef89cf	1ae6f279-6e5c-4870-9154-4267f1e13042	Active	1
00eac1e0-417a-4377-b718-5fee2ca8aae1	09e44380-4801-4715-8350-d2f3e2d69bc1	Active	1
b53808e3-7219-4c65-899c-197f204e5581	cfd8cd9e-ec60-4145-812e-5fc73ea63bc7	Active	1
88dfbcbd-c239-4f9a-8f5d-dd155d7ac99b	591c8035-6592-45d8-aef4-e7503681eb8c	Active	1
53fca09d-2b87-41ae-95ea-640953f66815	ca83eb05-91ea-4865-b4fe-82948bf5dbbf	Active	1
f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	4cbb0cf7-b545-4352-b3f4-8ffe69ea43b2	Active	2
60203747-3361-4008-8e0d-cded346095c8	8153236a-74a9-448d-bce8-c0e936d6df55	Inactive	1
066c5211-406e-4188-9461-023680ee6df2	deb5cc4a-2da1-4f54-bc50-8a284cc8f57c	Active	1
2adce07e-6ef8-4f7d-8920-accc288417e9	5dfc99cb-6a49-4873-b035-ffe9826b169b	Active	1
9e150ae6-7d66-4995-87ea-cd7d88cc83f8	c8f7b1b9-2a28-4d48-ae37-3e92a0d09723	Active	1
dba37f5a-970c-41da-be69-438732e98402	2657ccc3-6270-4d3c-a752-159afb5e9893	Active	1
f3a2a4e5-ec22-40fc-91bd-e89c48c7c9ee	6bcc2c98-c815-439e-a6b2-0878c10cb68d	Active	1
\.


--
-- Data for Name: NailCategories; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."NailCategories" ("NailCategoryId", "NailDesignId", "CategoryId") FROM stdin;
216	2	52
217	2	57
218	2	78
219	4	54
220	4	57
221	4	80
222	5	54
223	5	50
224	5	56
225	5	42
226	5	53
227	5	57
228	6	23
229	9	55
230	9	58
231	9	42
232	10	54
233	10	50
234	10	57
235	10	42
236	11	52
237	11	58
238	11	72
239	11	78
240	11	77
241	11	57
242	11	73
249	2	36
250	2	35
251	2	79
252	2	18
253	2	23
254	2	46
255	4	30
256	4	39
257	4	21
258	4	17
259	4	5
260	5	32
261	5	34
262	5	82
263	5	14
264	5	71
265	5	18
266	9	32
267	9	34
268	9	20
269	9	73
270	9	18
271	10	8
272	10	34
273	10	82
274	10	19
275	10	10
276	11	30
277	11	36
278	11	15
279	11	22
280	11	74
281	11	46
282	11	5
283	1	15
284	1	17
312	24	15
313	24	17
314	24	19
315	24	14
316	24	18
317	24	25
318	24	24
319	24	21
320	24	22
321	24	23
322	25	49
323	25	60
324	25	79
\.


--
-- Data for Name: NailComponents; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."NailComponents" ("NailComponentId", "ComponentId", "NailVariantId", "PosX", "PosY", "FingerIndex", "ConfigJson") FROM stdin;
30	1	17	0.00	-0.15	-1	{"scale":0.35,"rotation":0}
43	4	37	0.00	0.00	-1	{"scale":0.2,"rotation":0}
44	1	37	0.00	-0.35	2	{"scale":0.35,"rotation":0}
45	4	40	0.00	0.00	-1	{"scale":0.3,"rotation":0}
257	23	36	0.00	0.00	1	{"scale":0.35,"rotation":0}
258	23	36	-0.01	-0.26	1	{"scale":0.35,"rotation":0}
259	23	36	0.00	-0.25	2	{"scale":0.35,"rotation":0}
260	23	36	0.00	0.00	4	{"scale":0.35,"rotation":0}
261	23	36	0.00	0.00	2	{"scale":0.35,"rotation":0}
262	23	36	0.00	0.00	3	{"scale":0.35,"rotation":0}
263	23	36	-0.03	-0.17	5	{"scale":0.35,"rotation":0}
264	23	36	-0.01	-0.22	4	{"scale":0.35,"rotation":0}
265	23	36	-0.06	-0.24	3	{"scale":0.35,"rotation":0}
266	23	36	0.00	0.00	5	{"scale":0.35,"rotation":0}
194	9	32	-0.04	-0.33	4	{"scale":0.2,"rotation":0}
193	1	32	0.07	0.05	2	{"scale":0.24199981689453126,"rotation":0}
195	1	32	0.00	0.00	4	{"scale":0.25,"rotation":0}
196	1	32	0.00	0.00	3	{"scale":0.25,"rotation":0}
197	1	32	0.00	0.00	5	{"scale":0.25,"rotation":0}
198	1	32	0.00	0.00	1	{"scale":0.25,"rotation":0}
199	4	32	-0.02	-0.24	2	{"scale":0.2,"rotation":0}
200	9	32	0.00	0.00	5	{"scale":0.2,"rotation":0}
201	9	39	0.00	-0.14	1	{"scale":0.31098416323024536,"rotation":110.36000869719658}
202	11	39	-0.09	0.22	1	{"scale":0.49747587109636465,"rotation":77.04428484296136}
\.


--
-- Data for Name: NailDesigns; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."NailDesigns" ("NailDesignId", "Name", "MinPrice", "Description", "Status", "MaxPrice", "ImageUrl") FROM stdin;
24	Minh	0.00	Minh	Inactive	0.00	https://res.cloudinary.com/devu5qabc/image/upload/v1788878158/c3730992-d7cb-4a70-ad82-c0354d6223c1.webp?cors=anonymous
25	Minh123	0.00	123	Inactive	0.00	https://res.cloudinary.com/devu5qabc/image/upload/v1788879451/06032a72-737f-435e-8a66-7246498bab35.jpg?cors=anonymous
10	Đá Ngọc Lục Bảo Đen	50000.00	Nail đẹp cho đi tiệc, đám cưới,...	Active	50000.00	https://res.cloudinary.com/dkmm5rh28/image/upload/v1788871057/Screenshot_2026-09-08_193558_qtuneq.png?cors=anonymous
1	Pháo hoa 3 màu	180000.00	Mẫu móng đỏ thích hợp đi chơi.	Active	180000.00	https://res.cloudinary.com/dkmm5rh28/image/upload/v1788871058/Screenshot_2026-09-08_193720_t92hxf.png?cors=anonymous
2	Tím lấp lánh	110000.00	Mẫu móng đỏ thích hợp đi chơi.	Active	110000.00	https://res.cloudinary.com/dkmm5rh28/image/upload/v1788871058/Screenshot_2026-09-08_193707_prt3zq.png?cors=anonymous
4	Vàng hoa cúc	100000.00	Mẫu móng đỏ thích hợp đi chơi.	Active	100000.00	https://res.cloudinary.com/dkmm5rh28/image/upload/v1788871058/Screenshot_2026-09-08_193656_wjxuh5.png?cors=anonymous
5	Cưới trắng nơ	70000.00	Nail đẹp cho đi tiệc, đám cưới,...	Active	100000.00	https://res.cloudinary.com/dkmm5rh28/image/upload/v1788871057/Screenshot_2026-09-08_193613_z04e2q.png?cors=anonymous
6	Ribbon Balletcore	28000.00	Mẫu móng đỏ thích hợp đi chơi.	Active	28000.00	https://res.cloudinary.com/dkmm5rh28/image/upload/v1788871057/Screenshot_2026-09-08_193618_rd1ueq.png?cors=anonymous
9	Champagne Gold Mirror	100000.00	Mẫu móng đỏ thích hợp đi chơi.	Active	100000.00	https://res.cloudinary.com/dkmm5rh28/image/upload/v1788871058/Screenshot_2026-09-08_193701_m82qvf.png?cors=anonymous
11	Nail mắt mèo xanh	50000.00	Mẫu móng đỏ thích hợp đi chơi.	Active	150000.00	https://res.cloudinary.com/dkmm5rh28/image/upload/v1788871057/Screenshot_2026-09-08_193606_q4xej9.png?cors=anonymous
\.


--
-- Data for Name: NailProcedures; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."NailProcedures" ("NailProcedureId", "NailVariantId", "ProcedureId", "StepOrder", "Status", "CustomerNailId", "EstimatedMinutes", "IsCustomStep", "Name", "Note", "Price") FROM stdin;
50ff87e6-d644-4658-b56b-b8b653766a9b	18	82744afe-3799-4bc1-863b-72568f988644	1	InActive	\N	\N	f	\N	\N	\N
538663bc-21a1-4af2-ac68-cb2c48ce46b5	18	8b09327d-7887-4a36-9df9-2967c9ebdd59	2	InActive	\N	\N	f	\N	\N	\N
5c64e60d-5440-435e-90de-2492656069bc	18	da168e67-9448-4975-b3b2-542a4ff13a7a	4	InActive	\N	\N	f	\N	\N	\N
86df56c0-abe7-4ad6-84b8-8991542c4bc0	18	c77ce15e-e1c1-40f6-9a44-35a65fab2334	3	InActive	\N	\N	f	\N	\N	\N
b9edba0e-546a-4beb-bcd5-4e42fff4d562	18	e45527a6-cea6-465a-a1a9-d86166a0fe07	5	InActive	\N	\N	f	\N	\N	\N
4e8fead4-2b8a-4adb-878b-7feccbf516ef	18	0d109eb3-57de-4f52-a603-865e868b203f	3	Active	\N	\N	f	\N	\N	\N
ab64c0fa-5d1c-47cc-8f5b-a952a861e1ec	18	c77ce15e-e1c1-40f6-9a44-35a65fab2334	1	Active	\N	\N	f	\N	\N	\N
d3fb53dc-078d-4a4e-b55f-ac71ffac551d	18	ee1da04a-45f5-4b7f-8da8-66f8ef3c4240	2	Active	\N	\N	f	\N	\N	\N
8ed1f1fb-9b36-4d0f-ad88-6638fda13c96	\N	c77ce15e-e1c1-40f6-9a44-35a65fab2334	1	Active	32	\N	f	\N	\N	\N
0f0f6509-bb4d-4155-80a8-9a039a886d11	\N	\N	8	Active	74	20	t	AAAA	Bước kỹ thuật bổ sung do Thợ chỉ định	0
c277a29c-5bfb-4b61-95f9-006d6fb4bc06	\N	ee1da04a-45f5-4b7f-8da8-66f8ef3c4240	2	Active	32	\N	f	\N	\N	\N
6a3320fa-a5cd-4d12-9ab2-d679b6b6cb91	26	8b09327d-7887-4a36-9df9-2967c9ebdd59	2	Active	\N	\N	f	\N	\N	\N
71187f2f-f283-484d-95b1-1b998b92a10f	26	e45527a6-cea6-465a-a1a9-d86166a0fe07	5	Active	\N	\N	f	\N	\N	\N
7b61533f-4a13-4b34-a9be-76f2aa4dcaf1	26	82744afe-3799-4bc1-863b-72568f988644	1	Active	\N	\N	f	\N	\N	\N
99c4e2c1-3a80-43ae-8a41-fab72ff511b2	26	c77ce15e-e1c1-40f6-9a44-35a65fab2334	3	Active	\N	\N	f	\N	\N	\N
eee01278-204a-4f55-93a9-c50802176d88	26	da168e67-9448-4975-b3b2-542a4ff13a7a	4	Active	\N	\N	f	\N	\N	\N
262b2e97-9f1e-4bff-ad7a-40764cfb8e9c	\N	0d109eb3-57de-4f52-a603-865e868b203f	3	Active	32	\N	f	\N	\N	\N
6e15ba0d-9f73-42c1-80aa-308414763c02	\N	c77ce15e-e1c1-40f6-9a44-35a65fab2334	1	InActive	33	\N	f	\N	\N	\N
901bfa7c-005e-4b8c-8a8c-9d6e31709d14	\N	ee1da04a-45f5-4b7f-8da8-66f8ef3c4240	2	InActive	33	\N	f	\N	\N	\N
9a95dc71-f737-44df-9dbb-6e597c717db3	\N	0d109eb3-57de-4f52-a603-865e868b203f	3	InActive	33	\N	f	\N	\N	\N
8cda55e5-c5f3-424c-8a64-9d1986a52fab	\N	c77ce15e-e1c1-40f6-9a44-35a65fab2334	1	Active	33	\N	f	\N	\N	\N
b1c8f904-9151-47f0-92e1-ab91ed3a3cb5	\N	ee1da04a-45f5-4b7f-8da8-66f8ef3c4240	2	Active	33	\N	f	\N	\N	\N
ef51faf9-6683-4b7f-ad7f-9b453715d6dc	\N	0d109eb3-57de-4f52-a603-865e868b203f	3	Active	33	\N	f	\N	\N	\N
1ef60cd4-63b5-44a7-9364-6fc8dcc5b290	31	e45527a6-cea6-465a-a1a9-d86166a0fe07	1	Active	\N	\N	f	\N	\N	\N
bb9814e7-6269-445c-91b7-de8c3f6139ef	31	da168e67-9448-4975-b3b2-542a4ff13a7a	2	Active	\N	\N	f	\N	\N	\N
4bb6d942-c0f3-4ed5-a227-66ea89d08127	32	e45527a6-cea6-465a-a1a9-d86166a0fe07	1	Active	\N	\N	f	\N	\N	\N
7c25c163-3e70-4e35-9ee1-9f4cb5e79ac5	32	da168e67-9448-4975-b3b2-542a4ff13a7a	2	Active	\N	\N	f	\N	\N	\N
06374ccf-bfb2-4662-b43f-c5ca8da72482	36	da168e67-9448-4975-b3b2-542a4ff13a7a	2	Active	\N	\N	f	\N	\N	\N
17ea2be3-d03e-45c2-910a-579389d32b6e	36	96d1b102-8b31-4f23-ac99-c0766ee7a059	1	Active	\N	\N	f	\N	\N	\N
5f3ce007-50e3-4a75-99a8-091f65e010d6	37	82744afe-3799-4bc1-863b-72568f988644	4	Active	\N	\N	f	\N	\N	\N
746aec75-84fb-4dea-b898-fa11c863eca1	37	3ff78933-3817-4152-86f5-8ada4d33bf9f	3	Active	\N	\N	f	\N	\N	\N
b621f745-e05a-41ce-81ef-5222788cf8a6	37	c77ce15e-e1c1-40f6-9a44-35a65fab2334	2	Active	\N	\N	f	\N	\N	\N
df6753bd-8131-4d43-bb10-1138947cdff4	37	96d1b102-8b31-4f23-ac99-c0766ee7a059	1	Active	\N	\N	f	\N	\N	\N
3ad1a6f9-f28a-46bd-8a22-e0d0dd606fae	38	c77ce15e-e1c1-40f6-9a44-35a65fab2334	2	Active	\N	\N	f	\N	\N	\N
7a82d496-474a-48f1-ba75-5e7e6a59a588	38	1a26834a-9697-40da-adbb-5f98dfcee22a	1	Active	\N	\N	f	\N	\N	\N
c0d40bc6-0b92-48d6-9e34-b1871a8c8a2f	39	c77ce15e-e1c1-40f6-9a44-35a65fab2334	2	Active	\N	\N	f	\N	\N	\N
c2e9ca87-b1a7-4339-bb62-1ae4118c9a1b	39	96d1b102-8b31-4f23-ac99-c0766ee7a059	1	Active	\N	\N	f	\N	\N	\N
1dec5e2a-ab58-4577-aab9-5261f3414818	40	82744afe-3799-4bc1-863b-72568f988644	2	Active	\N	\N	f	\N	\N	\N
781ff913-5f5f-47dd-96fd-97d92eae3caa	40	96d1b102-8b31-4f23-ac99-c0766ee7a059	1	Active	\N	\N	f	\N	\N	\N
36ad4ca3-36ac-4f7f-bdb6-e13d54154ca4	17	0d109eb3-57de-4f52-a603-865e868b203f	1	InActive	\N	\N	f	\N	\N	\N
58dcab12-908d-416d-95d8-8eddd762dde0	17	3ff78933-3817-4152-86f5-8ada4d33bf9f	4	InActive	\N	\N	f	\N	\N	\N
6165c56b-b706-42c5-8e1c-b6b90d42f9bd	17	7c6b912e-5340-4cf6-917d-fa185477f4a5	5	InActive	\N	\N	f	\N	\N	\N
aac2fcb0-f60d-4760-8f4b-8184109869b1	17	da168e67-9448-4975-b3b2-542a4ff13a7a	2	InActive	\N	\N	f	\N	\N	\N
e83c8769-3184-4e42-b5cf-5efad4afb29b	17	3ff78933-3817-4152-86f5-8ada4d33bf9f	3	InActive	\N	\N	f	\N	\N	\N
04c0a1e4-0ced-413c-bbac-c97f285baf72	17	0d109eb3-57de-4f52-a603-865e868b203f	3	Active	\N	\N	f	\N	\N	\N
2e8e5bc2-b93f-4688-9b87-38b834befceb	17	ee1da04a-45f5-4b7f-8da8-66f8ef3c4240	2	Active	\N	\N	f	\N	\N	\N
586a6a28-024d-470d-8aac-3b7547dba796	17	c77ce15e-e1c1-40f6-9a44-35a65fab2334	1	Active	\N	\N	f	\N	\N	\N
79428306-05fb-4bd0-b63b-90825c512928	\N	\N	9	Active	74	15	t	abc	Bước kỹ thuật bổ sung do Thợ chỉ định	0
016410fd-874b-4768-a99a-dcf4f077c9b6	\N	7c6b912e-5340-4cf6-917d-fa185477f4a5	5	Inactive	74	\N	f	\N	Vẽ french tip	\N
18246558-0096-4173-b933-e2b534d6b7f9	\N	1a26834a-9697-40da-adbb-5f98dfcee22a	2	Active	78	\N	f	\N	Bề mặt bóng	\N
42e0a640-24aa-4013-90b1-5f08885e93c1	\N	8b09327d-7887-4a36-9df9-2967c9ebdd59	7	Inactive	74	\N	f	\N	Vẽ nghệ thuật	\N
75900e6b-2a71-4201-a55f-3ea802bf7766	\N	3ff78933-3817-4152-86f5-8ada4d33bf9f	4	Inactive	74	\N	f	\N	Thêm sticker/icon	\N
199c8c7c-face-46ab-9735-f7ee8a7c228a	\N	0d109eb3-57de-4f52-a603-865e868b203f	1	Active	78	\N	f	\N	Bề mặt mắt mèo	\N
88ab2a83-e67b-434f-ba51-ba33dc102cd8	\N	82744afe-3799-4bc1-863b-72568f988644	6	Inactive	74	\N	f	\N	Đính đá nhỏ	\N
94e513c3-0741-4742-820d-fc8fc536dbcb	\N	1a26834a-9697-40da-adbb-5f98dfcee22a	2	Inactive	74	\N	f	\N	Bề mặt bóng	\N
a852d12e-d386-4d31-b443-98c4fa268743	\N	1b13afac-166a-4d22-9632-3936911cc442	3	Inactive	74	\N	f	\N	Đính đá lớn	\N
ab037bb4-1f35-40c8-9029-66e0581545de	\N	82744afe-3799-4bc1-863b-72568f988644	3	Inactive	74	\N	f	\N	\N	\N
d26fbf25-536d-458d-acc8-571e59a7a8a8	\N	c77ce15e-e1c1-40f6-9a44-35a65fab2334	1	Inactive	74	\N	f	\N	\N	\N
d38daea0-7a06-4ac8-8ea5-27c299a95f91	\N	0d109eb3-57de-4f52-a603-865e868b203f	1	Inactive	74	\N	f	\N	Bề mặt mắt mèo	\N
d3a5a18f-073b-46e0-b64e-268cf2aa5bb0	\N	ee1da04a-45f5-4b7f-8da8-66f8ef3c4240	2	Inactive	74	\N	f	\N	\N	\N
88c4d09f-6513-44ac-b3a9-387b24d687b5	\N	3ff78933-3817-4152-86f5-8ada4d33bf9f	4	Active	74	\N	f	\N	Thêm sticker/icon	\N
894122c6-378b-4aa4-be19-422ef47553ca	\N	1a26834a-9697-40da-adbb-5f98dfcee22a	2	Active	74	\N	f	\N	Bề mặt bóng	\N
8f7e5cce-b96a-4cc5-bb6d-330d180888e3	\N	0d109eb3-57de-4f52-a603-865e868b203f	1	Active	74	\N	f	\N	Bề mặt mắt mèo	\N
93944c0f-4083-45cb-ae46-adb68388033c	\N	82744afe-3799-4bc1-863b-72568f988644	6	Active	74	\N	f	\N	Đính đá nhỏ	\N
a4993c0a-ecce-48df-b8ae-d001339905bf	\N	\N	8	Active	74	20	t	demo	Bước kỹ thuật bổ sung do Thợ chỉ định	0
b7959fc4-5f35-4b6b-b994-dcb83fc285fe	\N	1b13afac-166a-4d22-9632-3936911cc442	3	Active	74	\N	f	\N	Đính đá lớn	\N
d1594b1d-721d-42ad-81f5-cfe89c56f20c	\N	8b09327d-7887-4a36-9df9-2967c9ebdd59	7	Active	74	\N	f	\N	Vẽ nghệ thuật	\N
ff91cbeb-25dc-49e9-9d89-468db165545f	\N	7c6b912e-5340-4cf6-917d-fa185477f4a5	5	Active	74	\N	f	\N	Vẽ french tip	\N
a5490018-79fa-46b1-a5ba-035dcfd733a1	\N	\N	6	Active	78	15	t	THANHNHE	Bước kỹ thuật bổ sung do Thợ chỉ định	0
cc271d7a-0df0-4539-a14c-35ce43207808	\N	3ff78933-3817-4152-86f5-8ada4d33bf9f	4	Active	78	\N	f	\N	Thêm sticker/icon	\N
d9dc0563-bccf-4a63-8726-bed46d7827b2	\N	1b13afac-166a-4d22-9632-3936911cc442	3	Active	78	\N	f	\N	Đính đá lớn	\N
f2571153-6a0a-415d-93e3-f57cd0f558ac	\N	7c6b912e-5340-4cf6-917d-fa185477f4a5	5	Active	78	\N	f	\N	Vẽ french tip	\N
14ce5571-5c31-4a6c-8a54-a3d4f1d75cd8	\N	8b09327d-7887-4a36-9df9-2967c9ebdd59	7	Active	80	\N	f	\N	Vẽ nghệ thuật	\N
2055651a-5388-48de-9246-da7783a591c3	\N	1b13afac-166a-4d22-9632-3936911cc442	3	Active	80	\N	f	\N	Đính đá lớn	\N
8ec6327a-b36f-4278-98c1-c0b4c9e0c993	\N	ee1da04a-45f5-4b7f-8da8-66f8ef3c4240	2	Inactive	31	\N	f	\N	\N	\N
a1cebf53-2de4-440f-807a-c7acb0deea62	\N	c77ce15e-e1c1-40f6-9a44-35a65fab2334	1	Inactive	31	\N	f	\N	\N	\N
aa030bb1-3adf-4b12-ae7b-2bbdf606d456	\N	0d109eb3-57de-4f52-a603-865e868b203f	3	Inactive	31	\N	f	\N	\N	\N
35d77130-7fc9-4229-915b-d63d5187dfb8	\N	1a26834a-9697-40da-adbb-5f98dfcee22a	2	Active	80	\N	f	\N	Bề mặt bóng	\N
3cee4d67-dab0-4751-86f4-22f75d5bdb79	\N	0d109eb3-57de-4f52-a603-865e868b203f	1	Active	80	\N	f	\N	Bề mặt mắt mèo	\N
7e9b9f66-32de-47b6-b5c5-0446c58e7d36	\N	3ff78933-3817-4152-86f5-8ada4d33bf9f	4	Active	80	\N	f	\N	Thêm sticker/icon	\N
ce1b4900-0209-4ea8-8e37-bfbe1eb271b8	\N	7c6b912e-5340-4cf6-917d-fa185477f4a5	5	Active	80	\N	f	\N	Vẽ french tip	\N
d2524adc-81a8-4da5-a85e-b0460d3e5825	\N	82744afe-3799-4bc1-863b-72568f988644	6	Active	80	\N	f	\N	Đính đá nhỏ	\N
07c19b7f-53ab-414c-afcf-ff8bb416f452	\N	82744afe-3799-4bc1-863b-72568f988644	6	Inactive	77	\N	f	\N	Đính đá nhỏ	\N
1817fb12-4600-4d74-8f17-193b61b40e4f	\N	8b09327d-7887-4a36-9df9-2967c9ebdd59	7	Inactive	77	\N	f	\N	Vẽ nghệ thuật	\N
183039db-51a5-46ca-a438-0d904346e7cd	\N	\N	8	Inactive	77	15	t	ThanhDT Demo	Bước kỹ thuật bổ sung do Thợ chỉ định	0
6ecdc21b-d4f7-4811-b9ba-edf505e73a15	\N	7c6b912e-5340-4cf6-917d-fa185477f4a5	5	Inactive	77	\N	f	\N	Vẽ french tip	\N
8f9605fe-f57e-44bd-9f9a-480b63689e43	\N	1a26834a-9697-40da-adbb-5f98dfcee22a	2	Inactive	77	\N	f	\N	Bề mặt bóng	\N
ca8f78fa-e914-4be9-b2e6-f935ddb9210f	\N	1b13afac-166a-4d22-9632-3936911cc442	3	Inactive	77	\N	f	\N	Đính đá lớn	\N
cf306c2c-7cc4-43df-9ca3-1d1ffd40d248	\N	3ff78933-3817-4152-86f5-8ada4d33bf9f	4	Inactive	77	\N	f	\N	Thêm sticker/icon	\N
e762d86c-69d9-482f-b48d-92180f0d9ab9	\N	0d109eb3-57de-4f52-a603-865e868b203f	1	Inactive	77	\N	f	\N	Bề mặt mắt mèo	\N
2ffa2a91-2be4-4d0b-b377-7e7f7b9a98c6	\N	0d109eb3-57de-4f52-a603-865e868b203f	1	Active	77	\N	f	\N	Bề mặt mắt mèo	\N
47f4e14b-7069-4f5b-a651-6b94ae5bbdcd	\N	1b13afac-166a-4d22-9632-3936911cc442	3	Active	77	\N	f	\N	Đính đá lớn	\N
48143343-907d-41e9-9bda-87f0dc3d2193	\N	7c6b912e-5340-4cf6-917d-fa185477f4a5	5	Active	77	\N	f	\N	Vẽ french tip	\N
830a675f-d306-4e00-ada8-28ecdf0e8986	\N	3ff78933-3817-4152-86f5-8ada4d33bf9f	4	Active	77	\N	f	\N	Thêm sticker/icon	\N
ab3d1535-0fdb-4faa-8191-d4e25c5aae73	\N	\N	6	Active	77	15	t	ThanhDT Demo	Bước kỹ thuật bổ sung do Thợ chỉ định	0
c128b660-4721-4667-8652-4da16d27e527	\N	1a26834a-9697-40da-adbb-5f98dfcee22a	2	Active	77	\N	f	\N	Bề mặt bóng	\N
05747525-6d60-445a-a30e-1b458ce7e1f9	\N	1b13afac-166a-4d22-9632-3936911cc442	3	Active	81	\N	f	\N	Đính đá lớn	\N
14688fc9-1001-4f41-815e-256e7b22f030	\N	\N	6	Active	81	15	t	HELLO	Bước kỹ thuật bổ sung do Thợ chỉ định	0
7b775991-f962-47e4-bce8-d386f05948b1	\N	1a26834a-9697-40da-adbb-5f98dfcee22a	2	Active	81	\N	f	\N	Bề mặt bóng	\N
93d231d7-d489-4c5d-b3da-6efd41709bc3	\N	0d109eb3-57de-4f52-a603-865e868b203f	1	Active	81	\N	f	\N	Bề mặt mắt mèo	\N
9c0bb73b-fcbd-4294-957f-b95b1511d445	\N	3ff78933-3817-4152-86f5-8ada4d33bf9f	4	Active	81	\N	f	\N	Thêm sticker/icon	\N
a039a9cb-651b-4d7d-b1b3-4d310854bde6	\N	7c6b912e-5340-4cf6-917d-fa185477f4a5	5	Active	81	\N	f	\N	Vẽ french tip	\N
2ec96d1b-4052-4eda-8d3e-7f505d31215d	\N	0d109eb3-57de-4f52-a603-865e868b203f	1	Active	82	\N	f	\N	Bề mặt mắt mèo	\N
5b347326-59df-4bca-86d6-b3acd4b4f256	\N	1b13afac-166a-4d22-9632-3936911cc442	3	Active	82	\N	f	\N	Đính đá lớn	\N
7a22e68f-1a58-4b82-ad7d-65beebbcd0b1	\N	1a26834a-9697-40da-adbb-5f98dfcee22a	2	Active	82	\N	f	\N	Bề mặt bóng	\N
8a8f782e-dc8f-43c2-b6a7-9c4c9823ae5f	\N	3ff78933-3817-4152-86f5-8ada4d33bf9f	4	Active	82	\N	f	\N	Thêm sticker/icon	\N
c32d457b-6e0a-4cad-935d-003a6c52a328	\N	7c6b912e-5340-4cf6-917d-fa185477f4a5	5	Active	82	\N	f	\N	Vẽ french tip	\N
4eee69d5-9fb2-4060-a3ae-a8c4a901cd7d	\N	0d109eb3-57de-4f52-a603-865e868b203f	1	Active	83	\N	f	\N	Bề mặt mắt mèo	\N
5457d650-51f8-48e4-91d0-dd95c61511e7	\N	3ff78933-3817-4152-86f5-8ada4d33bf9f	4	Active	83	\N	f	\N	Thêm sticker/icon	\N
a65bc518-4ce9-4aa2-ade9-4e46e70216a5	\N	82744afe-3799-4bc1-863b-72568f988644	6	Active	83	\N	f	\N	Đính đá nhỏ	\N
e23ae574-7c3e-4b00-a26b-af31eb2ecfb2	\N	1b13afac-166a-4d22-9632-3936911cc442	3	Active	83	\N	f	\N	Đính đá lớn	\N
e4ca31c5-0b8c-4e07-b5d0-e711e0fc6f85	\N	7c6b912e-5340-4cf6-917d-fa185477f4a5	5	Active	83	\N	f	\N	Vẽ french tip	\N
e4f13bfe-79e5-4ecd-a846-92322b73a627	\N	1a26834a-9697-40da-adbb-5f98dfcee22a	2	Active	83	\N	f	\N	Bề mặt bóng	\N
37464670-7f72-418b-8d26-08a8fc865b61	\N	8b09327d-7887-4a36-9df9-2967c9ebdd59	3	Active	88	\N	f	\N	\N	\N
6b1474ed-0d6a-4ce0-8903-be7bf2dc683f	\N	c77ce15e-e1c1-40f6-9a44-35a65fab2334	5	Active	88	\N	f	\N	\N	\N
71ee04f6-9b98-4ced-a5ad-9e2a48653b7e	\N	7c6b912e-5340-4cf6-917d-fa185477f4a5	1	Active	88	\N	f	\N	\N	\N
e9c14755-dfa1-4c79-ac5c-28708b89658d	\N	da168e67-9448-4975-b3b2-542a4ff13a7a	6	Active	88	\N	f	\N	\N	\N
fae10f74-50b9-4a14-8507-a297b04ba352	\N	1a26834a-9697-40da-adbb-5f98dfcee22a	7	Active	88	\N	f	\N	\N	\N
ff7aa8ef-e11b-4a12-8b71-ed3a68dc6268	\N	3ff78933-3817-4152-86f5-8ada4d33bf9f	2	Active	88	\N	f	\N	\N	\N
fffee803-12c1-45fe-9834-21be7b146ecb	\N	82744afe-3799-4bc1-863b-72568f988644	4	Active	88	\N	f	\N	\N	\N
173de494-63b1-40d0-b7dd-55eb464253d0	\N	1b13afac-166a-4d22-9632-3936911cc442	3	Active	89	\N	f	\N	\N	\N
418ef671-b5a4-43e4-9690-85b41803af05	\N	da168e67-9448-4975-b3b2-542a4ff13a7a	6	Active	89	\N	f	\N	\N	\N
5d4c74e6-b529-4c5d-b0ca-6b9088552e26	\N	ee1da04a-45f5-4b7f-8da8-66f8ef3c4240	4	Active	89	\N	f	\N	\N	\N
64b86eb9-e97d-40e4-8ae2-b1da07d1233c	\N	3ff78933-3817-4152-86f5-8ada4d33bf9f	2	Active	89	\N	f	\N	\N	\N
8a145b48-7018-450e-aa0d-d596dd4e6f8e	\N	e45527a6-cea6-465a-a1a9-d86166a0fe07	5	Active	89	\N	f	\N	\N	\N
e0a0ea3d-856d-4239-a0ae-be857822043b	\N	7c6b912e-5340-4cf6-917d-fa185477f4a5	1	Active	89	\N	f	\N	\N	\N
0828c4f9-383a-4133-bb26-66573dc6643b	\N	ee1da04a-45f5-4b7f-8da8-66f8ef3c4240	2	Active	31	\N	f	\N	Đắp bột	\N
9e486e83-9c6c-497d-b67d-38ed6ffae331	\N	c77ce15e-e1c1-40f6-9a44-35a65fab2334	1	Active	31	\N	f	\N	Sơn màu gradient	\N
fdb250c7-e283-4b68-95af-9e8aa4c9f793	\N	0d109eb3-57de-4f52-a603-865e868b203f	3	Active	31	\N	f	\N	Bề mặt mắt mèo	\N
12a6edb5-d8df-4722-8256-d13ba8b12a0b	\N	82744afe-3799-4bc1-863b-72568f988644	6	Inactive	95	\N	f	\N	Đính đá nhỏ	\N
43704153-74ec-4ca3-8795-2ab7fba2abac	\N	82744afe-3799-4bc1-863b-72568f988644	6	Inactive	95	\N	f	\N	Đính đá nhỏ	\N
45ecfb90-b57e-4b3b-8d48-faaa5c2a18db	\N	7c6b912e-5340-4cf6-917d-fa185477f4a5	1	Inactive	95	\N	f	\N	Vẽ french tip	\N
47880fa0-1fe0-453d-b690-cacf1f4a32e6	\N	1a26834a-9697-40da-adbb-5f98dfcee22a	4	Inactive	95	\N	f	\N	Bề mặt bóng	\N
5ac26ab0-956c-4d34-903f-d3e37d2ee06c	\N	8b09327d-7887-4a36-9df9-2967c9ebdd59	7	Inactive	95	\N	f	\N	Vẽ nghệ thuật	\N
6a503e4e-8f84-4800-a9a2-43e53ba119a2	\N	1a26834a-9697-40da-adbb-5f98dfcee22a	4	Inactive	95	\N	f	\N	Bề mặt bóng	\N
7456d4b2-187b-4726-9f4e-cd5ed877edac	\N	\N	8	Inactive	95	5	t	Vẽ hình	Bước kỹ thuật bổ sung do Thợ chỉ định	0
890df397-571a-42ba-a222-8cb31b6b4e99	\N	7c6b912e-5340-4cf6-917d-fa185477f4a5	1	Inactive	95	\N	f	\N	Vẽ french tip	\N
9c42d5b4-4e5c-4ff2-b92e-3e805fe6c7b8	\N	0d109eb3-57de-4f52-a603-865e868b203f	5	Inactive	95	\N	f	\N	Bề mặt mắt mèo	\N
b096e187-0b6f-44b2-93e2-68351cb639c8	\N	0d109eb3-57de-4f52-a603-865e868b203f	5	Inactive	95	\N	f	\N	Bề mặt mắt mèo	\N
c6d70d4b-30ff-4f47-a940-23f1cba675e4	\N	\N	8	Inactive	95	5	t	Vẽ hình	Bước kỹ thuật bổ sung do Thợ chỉ định	0
c713b5e2-3131-430f-8568-c9d307241f44	\N	1b13afac-166a-4d22-9632-3936911cc442	3	Inactive	95	\N	f	\N	Đính đá lớn	\N
c82c2d9f-fe88-4378-aece-7517770bc5dc	\N	1b13afac-166a-4d22-9632-3936911cc442	3	Inactive	95	\N	f	\N	Đính đá lớn	\N
d1109d7c-bbb0-4be3-aa59-72d70802aadd	\N	8b09327d-7887-4a36-9df9-2967c9ebdd59	7	Inactive	95	\N	f	\N	Vẽ nghệ thuật	\N
eb979712-6a41-4401-b42a-ece8984f561c	\N	3ff78933-3817-4152-86f5-8ada4d33bf9f	2	Inactive	95	\N	f	\N	Thêm sticker/icon	\N
ec10f449-5a63-45d5-a4a3-07c61b7d2038	\N	3ff78933-3817-4152-86f5-8ada4d33bf9f	2	Inactive	95	\N	f	\N	Thêm sticker/icon	\N
01348524-4948-4826-93a3-092908005765	\N	82744afe-3799-4bc1-863b-72568f988644	6	Active	95	\N	f	\N	Đính đá nhỏ	\N
046d5d06-9cfe-4b7b-9f70-97d9e5898955	\N	\N	8	Active	95	5	t	Vẽ hình	Bước kỹ thuật bổ sung do Thợ chỉ định	0
08aa9b8d-7898-4370-9f11-f744fc8e5d9c	\N	82744afe-3799-4bc1-863b-72568f988644	6	Active	95	\N	f	\N	Đính đá nhỏ	\N
10d98206-4b30-42fc-879b-f2f2186960b3	\N	1b13afac-166a-4d22-9632-3936911cc442	3	Active	95	\N	f	\N	Đính đá lớn	\N
1710424c-316f-40d8-90fe-318ba52005df	\N	3ff78933-3817-4152-86f5-8ada4d33bf9f	2	Active	95	\N	f	\N	Thêm sticker/icon	\N
28eefed1-111b-4ddc-bff4-41be7a47038b	\N	3ff78933-3817-4152-86f5-8ada4d33bf9f	2	Active	95	\N	f	\N	Thêm sticker/icon	\N
57c01f34-0921-471b-8f4f-a60c39469493	\N	1b13afac-166a-4d22-9632-3936911cc442	3	Active	95	\N	f	\N	Đính đá lớn	\N
69562b5c-cf36-408b-ab3e-ce64f99f4648	\N	\N	8	Active	95	5	t	Vẽ hình	Bước kỹ thuật bổ sung do Thợ chỉ định	0
7580b79e-e7e4-40c4-b3bd-1d03722b6a9c	\N	0d109eb3-57de-4f52-a603-865e868b203f	5	Active	95	\N	f	\N	Bề mặt mắt mèo	\N
7d09594c-1576-44be-93cb-4273b16cacc9	\N	8b09327d-7887-4a36-9df9-2967c9ebdd59	7	Active	95	\N	f	\N	Vẽ nghệ thuật	\N
8bf25e4c-a3d8-4a13-9dcb-d50a99a6cc02	\N	1a26834a-9697-40da-adbb-5f98dfcee22a	4	Active	95	\N	f	\N	Bề mặt bóng	\N
8d13b51d-9f5e-49fa-b082-622ecd80e020	\N	0d109eb3-57de-4f52-a603-865e868b203f	5	Active	95	\N	f	\N	Bề mặt mắt mèo	\N
a3f9bcd2-2d17-4729-843d-a54b493e25da	\N	7c6b912e-5340-4cf6-917d-fa185477f4a5	1	Active	95	\N	f	\N	Vẽ french tip	\N
a435cdd7-2245-4248-9e1a-0e5fe9170053	\N	1a26834a-9697-40da-adbb-5f98dfcee22a	4	Active	95	\N	f	\N	Bề mặt bóng	\N
f705a090-8743-410f-b392-bab74527bae3	\N	8b09327d-7887-4a36-9df9-2967c9ebdd59	7	Active	95	\N	f	\N	Vẽ nghệ thuật	\N
fc722974-6466-4c45-9e25-21ec7361b6b6	\N	7c6b912e-5340-4cf6-917d-fa185477f4a5	1	Active	95	\N	f	\N	Vẽ french tip	\N
\.


--
-- Data for Name: NailRequiredSkills; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."NailRequiredSkills" ("NailRequiredSkillId", "NailVariantId", "SkillTypeId", "RequiredLevel") FROM stdin;
7df678f3-53cf-4705-b9b7-b4afd31598c8	17	11e07623-b990-4c3d-bfa0-b8f0a72a901a	3
6adf0d75-301a-4ac0-bac3-d2b3dce0d118	17	4a20f59e-2505-4b11-8eff-68ff16b39252	3
7eaa16b6-0f4f-4c7a-a3d6-93930f1d363f	17	13363e83-677c-4e5b-befe-bbc018d68373	3
c08866a6-df9e-4238-a0da-f9170c23ab7b	18	13363e83-677c-4e5b-befe-bbc018d68373	1
83a91b5d-f0f9-4fe2-9248-2c14a21a26b6	18	11e07623-b990-4c3d-bfa0-b8f0a72a901a	1
569374cc-9a45-494a-aaf7-941a77e9e3b0	18	4a20f59e-2505-4b11-8eff-68ff16b39252	1
\.


--
-- Data for Name: NailShapes; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."NailShapes" ("NailShapeId", "Name", "ImageUrl", "Status") FROM stdin;
8	Squoval	https://res.cloudinary.com/devu5qabc/image/upload/v1782271222/162357a6-467f-4488-89c5-6eed204e2096.png?cors=anonymous	Active
6	Round	https://res.cloudinary.com/devu5qabc/image/upload/v1788881297/Gemini_Generated_Image_7so0gq7so0gq7so0-removebg-preview_u2qflu.png?cors=anonymous	Active
1	Almond	https://res.cloudinary.com/devu5qabc/image/upload/v1788881297/Gemini_Generated_Image_x4a12ux4a12ux4a1-removebg-preview_pkpjf5.png?cors=anonymous	Active
7	Ballerina	https://res.cloudinary.com/devu5qabc/image/upload/v1788881297/Gemini_Generated_Image_djq6ckdjq6ckdjq6-removebg-preview_bdld5k.png?cors=anonymous	Active
9	Stiletto Dài	https://res.cloudinary.com/devu5qabc/image/upload/v1782271245/ed249509-3be8-476b-8f4c-b30bb2f2a8b7.png?cors=anonymous	Active
10	Ballerina Dài	https://res.cloudinary.com/devu5qabc/image/upload/v1782271258/58c46ea5-63ce-4927-836c-b1fc5a0eb5a7.png?cors=anonymous	Active
\.


--
-- Data for Name: NailSurfaces; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."NailSurfaces" ("NailSurfaceId", "Name", "ShaderParam", "Price", "Duration", "Status", "FinishType", "HueOffset", "LightnessOffset", "SaturationOffset") FROM stdin;
8	Cat Eyes	{"streak":0.9,"angle":90}	200000.00	30	Active	catEye	0	0.3	0.2
9	Holographic	{"rainbow":true,"intensity":0.85}	200000.00	30	Active	holographic	15	0.4	0.3
5	Chrome	{"reflectivity":0.95, "metallic":1.0}	100000.00	30	Active	chrome	10	0.6	-0.5
2	Matte	{"texture":{"type":"matte","roughness":0.85},"shine":{"enabled":false}}	50000.00	30	Active	matte	-310	10	-15
3	Glossy	{"texture":{"type":"glossy"},"shine":{"opacity":1}}	50000.00	30	Active	glossy	0	0	0
17	Thảo	{"shine":{"enabled":true,"position":"top-right","size":0.4,"opacity":0.6,"blur":20},"reflection":{"enabled":false,"intensity":0.5},"specular":{"enabled":false,"intensity":0.4},"texture":{"type":"glossy","roughness":0.3,"maskDataUrl":null}}	10000.00	10	Active	glossy	0	0	0
16	ThanhDT	{"shine":{"enabled":true,"position":"center","size":0.3,"opacity":0.7,"blur":15},"reflection":{"enabled":false,"intensity":0.85},"specular":{"enabled":true,"intensity":0.1},"stripe":{"enabled":false,"position":"center","width":60,"opacity":0.6,"blur":20,"magneticEffect":0.7},"gradient":{"enabled":true,"direction":"horizontal","intensity":0.5},"texture":{"type":"cat-eye","roughness":0.9,"maskDataUrl":null},"metalness":{"enabled":true,"intensity":0.4}}	99000.00	10	Inactive	glossy	-26	-1	-0.65
\.


--
-- Data for Name: NailVariants; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."NailVariants" ("NailVariantId", "Name", "NailShapeId", "NailSurfaceId", "NailDesignId", "Price", "Duration", "ImageUrl", "ColorJson", "Status") FROM stdin;
17	Pastel Rainbow	1	5	11	150000.00	130	https://res.cloudinary.com/devu5qabc/image/upload/v1780398933/5d261fed-127f-4d46-896e-cad4246d5d32.png?cors=anonymous	{"mode":"perFinger","fingers":[{"fingerIndex":1,"color":"#FF0000","gradient":null},{"fingerIndex":2,"color":"#F5CBA7","gradient":null},{"fingerIndex":3,"color":"#FF4081","gradient":null},{"fingerIndex":4,"color":"#0000FF","gradient":null},{"fingerIndex":5,"color":"#0000FF","gradient":null}]}	Active
18	Cat Eye Magnetic	1	2	11	50000.00	130	https://res.cloudinary.com/dkmm5rh28/image/upload/v1774027482/2a4bbc37-f961-401c-8723-ae8817fdf2c0.jpg?cors=anonymous	{"mode":"gradient","color":"#FF0000","gradient":{"enabled":true,"type":"linear","stops":["#00e1ff","#ff0066","#000000"],"stopCount":2}}	Active
26	Ombre Hồng Tím	1	2	11	50000.00	130	https://res.cloudinary.com/dkmm5rh28/image/upload/v1788871529/Screenshot_2026-09-08_193235_jwbnui.png?cors=anonymous	{"mode":"gradient","color":"#FF0000","gradient":{"enabled":true,"type":"linear","stops":["#00e1ff","#ff0066","#000000"],"stopCount":2}}	Active
31	Floral Spring	8	2	9	50000.00	30	https://res.cloudinary.com/dkmm5rh28/image/upload/v1788871529/Screenshot_2026-09-08_193452_lytbux.png?cors=anonymous	{"mode":"gradient","color":"#ecdfe3","gradient":{"enabled":true,"type":"linear","stops":["#ff4081","#ffd700","#ffd700"],"stopCount":2}}	Active
32	Dark Burgundy	10	5	1	180000.00	200	https://res.cloudinary.com/dkmm5rh28/image/upload/v1788871529/Screenshot_2026-09-08_193430_jbv8db.png?cors=anonymous	{"mode":"solid","color":"#f3ecef","gradient":null}	Active
36	Đá Ngọc Lục Bảo Trắng	10	2	10	150000.00	220	https://res.cloudinary.com/dkmm5rh28/image/upload/v1788871529/Screenshot_2026-08-16_204224_rozy7n.png?cors=anonymous	{"mode":"solid","color":"#fcf2f2","gradient":null}	Active
37	Black Minimal Line Art	1	2	2	110000.00	140	https://res.cloudinary.com/dkmm5rh28/image/upload/v1788871528/Screenshot_2026-09-08_193231_o4vgxw.png?cors=anonymous	{"mode":"gradient","color":"#FF4081","gradient":{"enabled":true,"type":"linear","stops":["#ff0000","#0a0000","#000000"],"stopCount":2}}	Active
38	Cưới Đen Nơ	10	3	5	100000.00	130	https://res.cloudinary.com/dkmm5rh28/image/upload/v1788871528/Screenshot_2026-09-08_193148_nuh9l4.png?cors=anonymous	{"mode":"gradient","color":"#FF4081","gradient":{"enabled":true,"type":"linear","stops":["#89f406","#ff0000","#000000"],"stopCount":2}}	Active
39	Cưới đỏ nơ	9	2	5	70000.00	150	https://res.cloudinary.com/dkmm5rh28/image/upload/v1788871528/Screenshot_2026-09-08_193226_aqfbgl.png?cors=anonymous	{"mode":"gradient","color":"#FF4081","gradient":{"enabled":true,"type":"linear","stops":["#ff0000","#ffffff","#000000"],"stopCount":2}}	Active
40	Đen hoa cúc	7	2	4	100000.00	130	https://res.cloudinary.com/dkmm5rh28/image/upload/v1788871058/Screenshot_2026-09-08_193720_t92hxf.png?cors=anonymous	{"mode":"solid","color":"#1e010b","gradient":null}	Active
63	thanh	8	8	1	200000.00	30	https://res.cloudinary.com/devu5qabc/image/upload/v1788856557/1de1a0f4-647a-466e-bba9-32faf272ac10.png?cors=anonymous	{"mode":"solid","color":"#FF4081","gradient":null}	Active
65	aaaa	8	8	24	200000.00	30	https://res.cloudinary.com/devu5qabc/image/upload/v1788878531/a16c17ff-aa4c-47b2-89d5-1e6039668808.jpg?cors=anonymous	{"mode":"solid","color":"#FF4081","gradient":null}	Active
\.


--
-- Data for Name: Procedures; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Procedures" ("ProcedureId", "Name", "Description", "Duration", "Status", "CreateAt", "IsRequired", "ActiveDuration", "CanOverlap", "PassiveDuration", "IsMainStep", "TransitionBuffer", "ProcedureType") FROM stdin;
82744afe-3799-4bc1-863b-72568f988644	Đính đá nhỏ	Đính đá nhỏ	5	Active	2026-06-18 07:58:33.311286	t	0	f	0	f	0	2
da168e67-9448-4975-b3b2-542a4ff13a7a	Sơn màu đơn	Sơn màu đơn	5	Active	2026-06-18 07:55:28.314191	t	0	f	0	f	0	2
e45527a6-cea6-465a-a1a9-d86166a0fe07	Bề mặt chrome	Bề mặt chrome	5	Active	2026-06-18 07:56:54.233257	t	0	f	0	f	0	2
ee1da04a-45f5-4b7f-8da8-66f8ef3c4240	Đắp bột	Đắp bột	5	Active	2026-06-18 07:56:14.581585	t	0	f	0	f	0	2
e85e0b2e-7f82-413d-9dea-f9b3587a3d33	Đính charm	Đính charm	5	Active	2026-06-18 07:58:48.291409	t	0	f	0	f	0	2
7c6b912e-5340-4cf6-917d-fa185477f4a5	Vẽ french tip	Vẽ french tip	5	Active	2026-06-18 07:57:33.70514	t	0	f	0	f	0	1
3ff78933-3817-4152-86f5-8ada4d33bf9f	Thêm sticker/icon	Thêm sticker/icon	5	Active	2026-06-18 07:57:47.20632	t	0	f	0	f	0	1
1b13afac-166a-4d22-9632-3936911cc442	Đính đá lớn	Đính đá lớn	5	Active	2026-06-18 07:58:40.052666	t	0	f	0	f	0	1
1a26834a-9697-40da-adbb-5f98dfcee22a	Bề mặt bóng	Bề mặt bóng	5	Active	2026-06-18 07:57:05.389637	t	0	f	0	f	0	1
96d1b102-8b31-4f23-ac99-c0766ee7a059	Bể mặt matte	Bể mặt matte	5	Active	2026-07-07 21:22:41	f	5	f	0	f	0	2
c77ce15e-e1c1-40f6-9a44-35a65fab2334	Sơn màu gradient	Sơn màu gradient	5	Inactive	2026-06-18 07:55:53.042318	t	0	f	0	f	0	2
0d109eb3-57de-4f52-a603-865e868b203f	Bề mặt mắt mèo	Bề mặt mắt mèo	12	Active	2026-06-18 07:56:39.559356	f	0	f	0	t	0	1
8b09327d-7887-4a36-9df9-2967c9ebdd59	Vẽ nghệ thuật	Vẽ nghệ thuật	5	Inactive	2026-06-18 07:58:08.386401	t	0	f	0	f	0	2
\.


--
-- Data for Name: Promotions; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Promotions" ("PromotionId", "Name", "Description", "Type", "Scope", "DiscountType", "DiscountValue", "CategoryId", "CategoryTypeId", "NailDesignId", "StartDate", "EndDate", "Status", "IsSelectable", "UsageLimit", "CurrentUsageCount", "UserLimit", "ImageUrl", "Situation", "PointsRequired") FROM stdin;
3	Champagne Gold Special	30% off Champagne Gold Mirror design - limited time only!	Discount	NailDesign	Percentage	30.00	\N	\N	9	2026-06-28 12:34:00	2026-09-30 03:11:00	Active	f	\N	3	1	https://res.cloudinary.com/devu5qabc/image/upload/v1782736730/8ff0bb96-07ff-4bf1-8903-88b9c1f4fae9.png?cors=anonymous		300
4	Summer Sale	10% off ALL nail designs and services - summer special!	Voucher	All	Percentage	10.00	\N	\N	\N	2026-06-28 12:34:40.688	2026-09-30 03:11:00	Active	t	\N	15	1	https://res.cloudinary.com/devu5qabc/image/upload/v1782736776/6a759654-1fd9-441c-916f-e3aece3332e7.png?cors=anonymous		10000
5	Welcome Gift	Enjoy 50,000đ off your first booking at Nailify! Welcome to the family!	Voucher	FirstTimeUser	FixedAmount	50000.00	\N	\N	\N	2026-06-28 12:34:40.688	2026-09-30 03:11:00	Active	t	\N	4	1	https://res.cloudinary.com/devu5qabc/image/upload/v1782736776/6a759654-1fd9-441c-916f-e3aece3332e7.png?cors=anonymous		50000
2	Perfect Match	15% off all SkinTone nail designs - find your perfect match!	Discount	CategoryType	Percentage	15.00	\N	3	\N	2026-06-28 12:34:40.688	2026-09-30 03:11:00	Active	f	\N	40	2	https://res.cloudinary.com/devu5qabc/image/upload/v1782736665/49ff2b2c-f1b3-4faf-8875-ddce5197c35e.png?cors=anonymous		200
1	Winter Wonderland	20% off all winter-themed nail designs	Discount	Category	Percentage	20.00	\N	\N	\N	2026-06-28 12:34:40.688	2026-09-30 03:11:00	Active	f	\N	45	3	https://res.cloudinary.com/devu5qabc/image/upload/v1782736562/5ac4fcbf-7587-4b48-9271-ec5f69e51aa0.png?cors=anonymous		100
8	Giảm giá 2/9	ThanhDT Test	Voucher	All	Percentage	20.00	\N	\N	\N	2026-07-20 18:06:56	2026-09-30 03:11:00	Active	t	\N	3	1	https://res.cloudinary.com/devu5qabc/image/upload/v1784467148/5806fe5e-b5f2-4fef-9aac-4bb9058a9245.png?cors=anonymous		100
9	ThanhDT	Test wallet\n	Voucher	All	Percentage	30.00	\N	\N	\N	2026-07-20 18:09:09	2026-09-30 03:11:00	Active	t	\N	1	1	https://res.cloudinary.com/devu5qabc/image/upload/v1784467148/5806fe5e-b5f2-4fef-9aac-4bb9058a9245.png?cors=anonymous		50
11	Thanh Thảo	Thanh Thảo	Voucher	All	Percentage	50.00	\N	\N	\N	2026-09-06 10:45:11.203	2026-09-22 10:45:11.203	Active	t	4	0	1			50
\.


--
-- Data for Name: QuizOptions; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."QuizOptions" ("QuizOptionId", "QuizQuestionId", "OptionValue", "Label", "Description") FROM stdin;
35e7c3fa-d15b-4ce5-b612-88b1a71ec041	0dd4b558-8f84-442d-8189-66b792759ef9	["#FF0000"]	Sự cởi mở, thân thiện, tràn đầy năng lượng tươi trẻ và dễ kết nối	\N
77c19b0f-805a-495d-a7e4-92b240b7ce16	0dd4b558-8f84-442d-8189-66b792759ef9	["#FFFFFF"]	Sự chuyên nghiệp, tinh tế, ngăn nắp và lịch sự chuẩn văn phòng	\N
81baa3b2-24c0-4106-a7cb-9cce3ddb3cc9	0dd4b558-8f84-442d-8189-66b792759ef9	["#1E010B"]	Sự quyết đoán, năng lực dẫn dắt và cá tính độc lập của bản thân	\N
361f7913-475a-4fc7-a56e-50dfbe885e9f	0dd4b558-8f84-442d-8189-66b792759ef9	["#ECDFE3"]	Sự điềm tĩnh, an toàn, tôn trọng và tạo độ tin cậy lớn cho đối tác	\N
55eb6b76-a85e-4aa6-a36b-bd7cb22964a7	22c4dbf5-ea82-45ed-9131-d4c0df76af9b	["20"]	Mùa đông lạnh giá, ấm áp với không khí lễ hội cuối năm	\N
bde5162e-7f15-4e98-8f8a-aba4549ff8f8	dcb0547b-70dc-42da-863e-a8d79dbf64d9	["#FFD700"]	Màu Vàng/Cam	Vui vẻ, sáng tạo và nhiều năng lượng tích cực
8decbe4c-fc19-4fc6-b7f4-efba0b4d4d57	dcb0547b-70dc-42da-863e-a8d79dbf64d9	["#FCF2F2"]	Màu Pastel/Hồng nude	Yên bình, ấm áp và sẵn sàng lắng nghe
57d50f56-67df-4892-8c97-42708be06440	dcb0547b-70dc-42da-863e-a8d79dbf64d9	["#1E010B"]	Màu Xanh navy/Ghi tối giản	Chuyên nghiệp, nghiêm túc và sâu sắc
b842fe7a-b941-4dcc-aafb-b07256a46bc4	e86238c5-decd-4d81-a931-c668c81aed0f	["#ff0000"]	Đỏ	\N
b394d828-6d33-4439-826c-ff704a6609ac	41ec57d4-19ee-4e11-bd8d-4b6c6c472e2a	["10"]	Đen	\N
749e517d-26c2-48be-aabb-958d27deb62b	41ec57d4-19ee-4e11-bd8d-4b6c6c472e2a	["9"]	Trắng	\N
39d18088-bf01-4031-93a0-291ea13ce2bd	e0d26d37-9bbe-418f-8d1a-24bf2c307bf6	["3"]	Trơn láng, bóng bẩy và phản chiếu ánh sáng cực tốt $\\rightarrow$ Liên kết mặt Bóng	\N
c9f6ed65-5a0e-4575-bfd0-53c60e7d2a20	e0d26d37-9bbe-418f-8d1a-24bf2c307bf6	["2"]	Mịn màng, không bóng, cảm giác mịn như nhung	\N
b775dff2-efa5-49fd-99b3-cc327efb72b8	e0d26d37-9bbe-418f-8d1a-24bf2c307bf6	["9"]	Lấp lánh và có hiệu ứng cầu vồng ảo diệu cuốn hút	\N
b6bc631b-b259-44ec-8290-244883232cd0	e0d26d37-9bbe-418f-8d1a-24bf2c307bf6	["5"]	Bóng loáng ánh kim loại tráng gương sang chảnh độc đáo	\N
e295c414-29a1-49fd-a501-13b4227af1ba	22c4dbf5-ea82-45ed-9131-d4c0df76af9b	["23"]	Mùa thu lãng mạn, nhẹ nhàng với các tone màu trầm loang ấm	\N
c51a6cae-24a6-46f6-bd1c-697ce1615e07	22c4dbf5-ea82-45ed-9131-d4c0df76af9b	["21"]	Mùa xuân tươi mới, ngập tràn hoa cỏ đâm chồi nảy lộc	\N
6028325b-41de-47e2-8d54-8defcc41579d	22c4dbf5-ea82-45ed-9131-d4c0df76af9b	["22"]	Mùa hè rực rỡ, năng động với những chuyến đi biển ngập nắng	\N
1cdc5792-b79c-4eaa-b2b0-4908f90f4533	27e31006-46be-40e5-906c-4d331db42c6b	["complex"]	Thường xuyên tham dự tiệc tùng, biểu diễn hoặc đứng trước máy quay	\N
8d6cd9b7-bdca-4e4f-9289-3221e5b77b7f	27e31006-46be-40e5-906c-4d331db42c6b	["moderate"]	Làm việc văn phòng nhiều, thường xuyên gõ bàn phím và sử dụng máy tính	\N
f69c149b-f291-4443-a15d-59cefe1839e4	27e31006-46be-40e5-906c-4d331db42c6b	["simple"]	Thực hiện trang điểm, dưỡng da hoặc làm nội trợ nấu ăn	\N
2b86d46c-cd45-4d7a-917f-6643af5bf217	27e31006-46be-40e5-906c-4d331db42c6b	["complex"]	Ít phải làm việc chân tay nặng nhọc, muốn bộ móng sang trọng, nổi bật nhất	\N
393ebc83-590c-4890-afe5-d24a665059d3	6881e8b7-284c-475d-9798-151891f65833	["17"]	Học tập, cắm trại dã ngoại và sinh hoạt tự do thường nhật	\N
9a358da8-dc5b-4925-af9c-d09e5165c661	6881e8b7-284c-475d-9798-151891f65833	["14","71"]	Dự tiệc cưới, sinh nhật, prom party hoặc sự kiện đặc biệt sang trọng	\N
953eafef-ae14-4a41-bb97-185b3c751810	6881e8b7-284c-475d-9798-151891f65833	["18"]	Đi làm văn phòng công sở chuyên nghiệp cần thanh lịch	\N
e52c056f-7994-445d-a01a-9764e8066e89	6881e8b7-284c-475d-9798-151891f65833	["74","15","19"]	Đi chơi, gặp gỡ bạn bè, đi du lịch nghỉ dưỡng hoặc hẹn hò lãng mạn	\N
aaafef05-e797-4368-b601-b01ac7a97820	7203d821-5800-46cf-bec8-15fb088c0014	["simple"]	Trơn láng, bóng bẩy và phản chiếu ánh sáng cực tốt	\N
6538ddde-5206-430a-92cb-02f61a76b48d	7203d821-5800-46cf-bec8-15fb088c0014	["complex"]	Bóng loáng ánh kim loại tráng gương sang chảnh độc đáo	\N
2d9b0f7b-6efe-4fdd-9b9a-aebf80316949	7203d821-5800-46cf-bec8-15fb088c0014	["simple"]	Mịn màng, không bóng, cảm giác mịn như nhung	\N
05e5293b-c379-4ff8-a6c2-eec05adfc919	7203d821-5800-46cf-bec8-15fb088c0014	["complex"]	Lấp lánh và có hiệu ứng cầu vồng ảo diệu cuốn hút	\N
35b6aaa4-096d-4678-82b9-36ab8d67c65b	966fe4df-abaf-4584-a157-3ca15e4890e3	["32","34","82"]	Sang trọng, quý phái, lấp lánh hoặc đính đá cầu kỳ	\N
384b8dfa-1b7b-4fbc-b574-ee36a815809c	966fe4df-abaf-4584-a157-3ca15e4890e3	["36","35","79"]	Các đường mảnh tinh tế tối giản phong cách cổ điển	\N
4bfa97e9-b6d7-4b93-8bec-8f44246d35b4	966fe4df-abaf-4584-a157-3ca15e4890e3	["8"]	Độc lạ, ma mị, phối màu đen/tối cá tính	\N
7aa7498d-fb46-4214-89de-77056bf4dffa	966fe4df-abaf-4584-a157-3ca15e4890e3	["30","39"]	Vẽ hình ngộ nghĩnh dễ thương, màu sắc ngọt ngào đáng yêu	\N
dbd533eb-910a-463f-b480-9965ee9318f5	b67f4f32-1355-46b1-872b-736a59df7aea	["8"]	Tuân thủ/Kỹ lưỡng	Tập trung vào tính chính xác, phân tích các dữ liệu và rủi ro trước khi quyết định
278e93d3-83ca-4284-b36d-c319a89c320b	b67f4f32-1355-46b1-872b-736a59df7aea	["6"]	Kiên định/Hòa nhã	Lắng nghe ý kiến của mọi người, lập kế hoạch chi tiết và tiến hành từng bước
9cd36f03-dedb-4679-91df-2c967d222263	b67f4f32-1355-46b1-872b-736a59df7aea	["1"]	Ảnh hưởng/Sáng tạo	Hào hứng kết nối nhóm, tìm kiếm sự ủng hộ và truyền cảm hứng
f80f8fa5-625e-4ab1-afb6-7b14fdc22052	b67f4f32-1355-46b1-872b-736a59df7aea	["9"]	Chủ động/Quyết đoán	Quyết tâm tiên phong, muốn tự mình dẫn dắt và hành động ngay lập tức
b9e1dd98-cca5-4512-acbc-4e7e39ba8be9	c6c7ca9d-39b7-4258-8d38-5773ddb1e6b9	["10"]	Da tối màu/Sẫm màu	\N
1f420222-9e1d-49fd-b3b5-786530e12770	c6c7ca9d-39b7-4258-8d38-5773ddb1e6b9	["5"]	Da ngăm/Bánh mật khỏe khoắn	\N
4a7d987b-5e09-433c-8082-65319b46684c	c6c7ca9d-39b7-4258-8d38-5773ddb1e6b9	["46"]	Da trung tính nhẹ nhàng	\N
89fe20b4-4ac3-415e-95ed-0d03eab72dff	c6c7ca9d-39b7-4258-8d38-5773ddb1e6b9	["42"]	Da trắng sáng	\N
ad0054bf-db6f-4b35-9020-0a144233d00f	dcb0547b-70dc-42da-863e-a8d79dbf64d9	["#FF0000"]	Màu Đỏ/Neon	Tràn đầy quyết đoán, nhiệt huyết và muốn dẫn đầu
42efc7dd-d11a-4756-9ed7-a08e054327f9	e2463373-6243-46e7-a3af-15e7784caf37	["8"]	Tối giản, gọn gàng, tinh tế và lịch sự	\N
1990255d-a453-4385-b6f8-3eb483444d04	e2463373-6243-46e7-a3af-15e7784caf37	["6"]	Nhẹ nhàng, thanh lịch, ưu tiên sự thoải mái tự nhiên	\N
a556ecbc-aa7e-488c-b0f4-0d55b63217af	e2463373-6243-46e7-a3af-15e7784caf37	["1"]	Thu hút, nhiều màu sắc, luôn cập nhật các xu hướng thời trang mới	\N
00a336f9-5206-42b0-85c5-af9a2661c8c9	e2463373-6243-46e7-a3af-15e7784caf37	["10"]	Nổi bật, cá tính, thời thượng và có phần phá cách	\N
\.


--
-- Data for Name: QuizQuestions; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."QuizQuestions" ("QuizQuestionId", "QuestionText", "Type", "Category", "IsActive") FROM stdin;
e0d26d37-9bbe-418f-8d1a-24bf2c307bf6	Bạn thích bề mặt móng tay của mình khi chạm vào hoặc quan sát có cảm giác thế nào?	Single	Complexity	f
41ec57d4-19ee-4e11-bd8d-4b6c6c472e2a	Bạn có màu da như nào?	Single	SkinTone	f
dcb0547b-70dc-42da-863e-a8d79dbf64d9	Tông màu nào phản ánh đúng nhất năng lượng tinh thần hiện tại của bạn?	Single	Color	t
0dd4b558-8f84-442d-8189-66b792759ef9	Khi tham gia một buổi gặp mặt giao tiếp quan trọng, bạn muốn màu sắc móng thể hiện điều gì ở bạn?	Single	Color	t
6881e8b7-284c-475d-9798-151891f65833	Bạn muốn diện bộ móng được thiết kế này vào những dịp nào dưới đây?	Multiple	Occasion	t
22c4dbf5-ea82-45ed-9131-d4c0df76af9b	Mùa nào trong năm mang lại cho bạn nhiều cảm hứng làm đẹp và thời trang nhất?	Single	Occasion	t
7203d821-5800-46cf-bec8-15fb088c0014	Bạn thích bề mặt móng tay của mình khi chạm vào hoặc quan sát có cảm giác thế nào?	Single	Complexity	t
27e31006-46be-40e5-906c-4d331db42c6b	Trong sinh hoạt hàng ngày, bạn thường xuyên làm các công việc nào sau đây?	Multiple	Complexity	t
966fe4df-abaf-4584-a157-3ca15e4890e3	Lối thiết kế nghệ thuật móng (Nail Art) nào làm bạn thấy ấn tượng và thu hút nhất?	Multiple	Style	t
b67f4f32-1355-46b1-872b-736a59df7aea	Khi đối mặt với một dự án hoặc thử thách mới, phản ứng đầu tiên của bạn là gì?	Single	Shape	t
e2463373-6243-46e7-a3af-15e7784caf37	Phong cách thời trang thường ngày mà bạn tự tin nhất là?	Multiple	Shape	t
e86238c5-decd-4d81-a931-c668c81aed0f	Khi đối mặt với một dự án hoặc thử thách mới, phản ứng đầu tiên của bạn là gì?	Single	Shape	f
c6c7ca9d-39b7-4258-8d38-5773ddb1e6b9	Tông da tay tự nhiên của bạn thuộc nhóm nào dưới đây để chọn màu móng phù hợp nhất?	Single	SkinTone	t
\.


--
-- Data for Name: SalonOffDates; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."SalonOffDates" ("SalonOffDateId", "SalonId", "StartDate", "EndDate", "Description") FROM stdin;
10c59c07-df6d-42d6-8bc2-927ae5aaaaad	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	2026-10-14 00:00:00	2026-10-15 00:00:00	hehe
e0073f06-4216-4781-9e3b-7951ff54fd90	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	2026-10-23 00:00:00	2026-10-24 00:00:00	hehe
ba334e7e-d13d-44a3-844d-11ecf1112c13	bc6e3435-7d2f-46e6-abe1-159befb1190e	2026-09-08 00:00:00	2026-09-11 00:00:00	aa
\.


--
-- Data for Name: SalonOperatingHours; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."SalonOperatingHours" ("OperatingHourId", "SalonId", "DayOfWeek", "OpenTime", "CloseTime", "IsClosed") FROM stdin;
5131f8ad-e3ee-4bc7-8a11-b777462ee2e5	84cc584d-335e-40a7-90c3-abebfe573b01	6	10:00:00	17:00:00	f
76c73ed0-c1c0-4619-9bc7-fac7a1b8fb8b	84cc584d-335e-40a7-90c3-abebfe573b01	0	10:00:00	17:00:00	f
97243f5a-9aac-446d-b6fd-bc619cb859a4	84cc584d-335e-40a7-90c3-abebfe573b01	1	08:00:00	19:00:00	f
a1f242c1-d1ce-4c04-88ed-8dc9607087b4	84cc584d-335e-40a7-90c3-abebfe573b01	4	08:00:00	19:00:00	f
ac34c9e9-c939-415a-b151-305e3bc6e98c	84cc584d-335e-40a7-90c3-abebfe573b01	3	08:00:00	19:00:00	f
cbab871d-c68b-4a43-a07f-8567b5b7a1cc	84cc584d-335e-40a7-90c3-abebfe573b01	5	08:00:00	19:00:00	f
d68bb78b-3ddd-4756-94de-512c89e3171a	84cc584d-335e-40a7-90c3-abebfe573b01	2	08:00:00	19:00:00	f
15ceed55-b918-4be4-a6e3-f15b5d375ce4	bc6e3435-7d2f-46e6-abe1-159befb1190e	4	08:00:00	19:00:00	f
415f1eeb-f521-48db-b9bb-379db89cbe06	bc6e3435-7d2f-46e6-abe1-159befb1190e	0	10:00:00	17:00:00	f
7b8d0c05-e203-446d-8423-7951b44f53a0	bc6e3435-7d2f-46e6-abe1-159befb1190e	2	08:00:00	19:00:00	f
bc0a4794-923a-4d55-ada1-670f4294e392	bc6e3435-7d2f-46e6-abe1-159befb1190e	1	08:00:00	19:00:00	f
0381023a-6b9f-47ee-9edd-ff1e2178f4ca	118341cc-0df0-44cd-b40a-0a3f5d167c58	0	13:00:00	17:00:00	f
217cd609-6a77-49f3-8047-36c6d008b70f	118341cc-0df0-44cd-b40a-0a3f5d167c58	1	13:00:00	17:00:00	f
38bd7c47-ef03-4c4a-9a6b-73fa1a55b3f1	118341cc-0df0-44cd-b40a-0a3f5d167c58	1	08:00:00	12:00:00	f
551b7273-9041-4b73-8bd8-cda161b6b14b	118341cc-0df0-44cd-b40a-0a3f5d167c58	0	10:00:00	12:00:00	f
55c4a872-291d-4b54-9c73-961b9f81faf5	118341cc-0df0-44cd-b40a-0a3f5d167c58	2	08:00:00	12:00:00	f
5e0dae71-8e77-4358-b758-2fe967cefa1b	118341cc-0df0-44cd-b40a-0a3f5d167c58	2	13:00:00	17:00:00	f
6d236872-e429-4120-bd27-dc8011fe642e	118341cc-0df0-44cd-b40a-0a3f5d167c58	3	08:00:00	12:00:00	f
8844e459-64dc-4e51-a665-943fb85ff728	118341cc-0df0-44cd-b40a-0a3f5d167c58	5	13:00:00	17:00:00	f
919757d8-113e-479a-9947-d9b94d32662d	118341cc-0df0-44cd-b40a-0a3f5d167c58	4	13:00:00	17:00:00	f
98ee4df6-4729-4def-9b64-bbd53d76c4e6	118341cc-0df0-44cd-b40a-0a3f5d167c58	3	13:00:00	17:00:00	f
bb0d3f73-f082-4742-b704-ceaae13646c0	118341cc-0df0-44cd-b40a-0a3f5d167c58	6	08:00:00	12:00:00	f
c840e604-56a1-4b1a-80e7-9e0c7eff5721	118341cc-0df0-44cd-b40a-0a3f5d167c58	5	08:00:00	12:00:00	f
dcdd2d80-6110-4b7e-af37-547a8ae8717c	118341cc-0df0-44cd-b40a-0a3f5d167c58	6	13:00:00	17:00:00	f
ddcbafc5-4d57-4ad3-befc-e64125407b37	118341cc-0df0-44cd-b40a-0a3f5d167c58	4	08:00:00	12:00:00	f
3a92d833-ac6e-4926-8301-a1ae4d54a52c	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	4	08:00:00	23:00:00	f
4803cba1-4f84-413e-abe8-85600b4c1db1	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	1	08:00:00	23:00:00	f
6264d5dd-c5ae-4d8f-880a-e2b53f609f76	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	2	08:00:00	23:00:00	f
8c900c34-db40-4188-a3c1-63800962a078	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	6	08:00:00	23:00:00	f
8e32d3c2-a96c-493f-bc53-67d31cd954e7	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	5	08:00:00	23:00:00	f
b0c0411a-7e8d-4f43-9a77-54685b7d13f0	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	3	08:00:00	23:00:00	f
c0c9500c-2325-4ca4-a169-5a3ce196ce67	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	0	08:00:00	23:00:00	f
bdb6fe1b-9d13-4d8b-afa6-4847e7667d3e	bc6e3435-7d2f-46e6-abe1-159befb1190e	5	08:00:00	19:00:00	f
c17eb0ef-ade2-4bb2-8191-5a2b7b878d80	bc6e3435-7d2f-46e6-abe1-159befb1190e	3	08:00:00	19:00:00	f
d0b11bc0-db21-4da0-b5ec-041c594654d8	bc6e3435-7d2f-46e6-abe1-159befb1190e	6	10:00:00	17:00:00	f
9bfd68e5-f191-42cc-bf2a-b0e3c7d55cd7	c2325bfa-edca-4803-92c1-9c2507f5b4a8	4	08:00:00	23:00:00	f
ad5b229d-885b-498c-82cc-2a6eb81ec404	c2325bfa-edca-4803-92c1-9c2507f5b4a8	1	08:00:00	23:00:00	f
b53172c0-caad-405d-bb88-a27d3228933e	c2325bfa-edca-4803-92c1-9c2507f5b4a8	2	08:00:00	23:00:00	f
c0cf0829-0f3d-4d32-81e2-ea62e3d6bfed	c2325bfa-edca-4803-92c1-9c2507f5b4a8	3	08:00:00	23:00:00	f
c3b52793-b97d-4d1d-8cb9-3b7025a69a87	c2325bfa-edca-4803-92c1-9c2507f5b4a8	5	08:00:00	23:00:00	f
26745976-b139-4358-9803-dc832652c90e	533bfd94-4f3f-4878-9fcc-5e2138d0b0d1	2	08:00:00	23:00:00	f
7f87a988-1bf2-40cc-982f-fbd3673c94c9	533bfd94-4f3f-4878-9fcc-5e2138d0b0d1	1	08:00:00	23:00:00	f
8f7a8ef0-785c-4289-81ca-196319968b92	533bfd94-4f3f-4878-9fcc-5e2138d0b0d1	5	08:00:00	23:00:00	f
dd0c904e-332e-4342-a821-935df48cabcf	533bfd94-4f3f-4878-9fcc-5e2138d0b0d1	4	08:00:00	23:00:00	f
e7f593e7-f9a9-40b9-b77e-887923e96d1c	533bfd94-4f3f-4878-9fcc-5e2138d0b0d1	3	08:00:00	23:00:00	f
2d385637-eead-4b91-8dc7-4a23078d84bc	533bfd94-4f3f-4878-9fcc-5e2138d0b0d1	6	08:00:00	23:00:00	f
41b1e9dd-9180-47b9-8ee4-d096cf3ecb17	c2325bfa-edca-4803-92c1-9c2507f5b4a8	0	08:00:00	23:00:00	f
610e261a-0430-47be-a0f5-1fe6a4ecde1f	533bfd94-4f3f-4878-9fcc-5e2138d0b0d1	0	08:00:00	23:00:00	f
ff2d6bcf-7005-41f9-92ee-101abb81bb1a	c2325bfa-edca-4803-92c1-9c2507f5b4a8	6	08:00:00	23:00:00	f
07e9669b-3461-4e66-a15c-92fcd8d3aa60	3bab09c5-20de-4a87-8415-60e2a62a16db	5	00:00:00	00:00:00	t
36ed4553-b53a-4710-b772-4f777f3b7c4d	3bab09c5-20de-4a87-8415-60e2a62a16db	0	00:00:00	00:00:00	t
43890626-e011-45d3-868e-6cc5d844477c	3bab09c5-20de-4a87-8415-60e2a62a16db	5	00:00:00	00:00:00	t
56c3b5ec-c06b-40c8-beef-2a8885ac2c0d	3bab09c5-20de-4a87-8415-60e2a62a16db	4	00:00:00	00:00:00	t
5e876df8-f9b8-4a4c-a384-131d7746eb42	3bab09c5-20de-4a87-8415-60e2a62a16db	6	00:00:00	00:00:00	t
65489246-b142-4f06-b431-e44b4628c4bf	3bab09c5-20de-4a87-8415-60e2a62a16db	3	00:00:00	00:00:00	t
7ecb332c-4568-4006-994a-cb82bb9b02e0	3bab09c5-20de-4a87-8415-60e2a62a16db	2	00:00:00	00:00:00	t
f50fcd93-561f-465c-b228-3a9383bcc0aa	3bab09c5-20de-4a87-8415-60e2a62a16db	1	00:00:00	00:00:00	t
\.


--
-- Data for Name: Salons; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Salons" ("SalonId", "Name", "Address", "Phone", "Latitude", "Longitude", "Status", "ImageUrl", "DepositConfig") FROM stdin;
484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	Salon Long Thành Mỹ	D2, Long Thành Mỹ, Phường Long Bình	0912025262	0	0	Open	https://res.cloudinary.com/devu5qabc/image/upload/v1780389921/8b85d913-90cc-4450-b080-3927d08b571d.png?cors=anonymous	0.2
533bfd94-4f3f-4878-9fcc-5e2138d0b0d1	Salon Đinh Tiên Hoàng	205 Đ. Đinh Tiên Hoàng, Phường Tân Định, Quận 1, TP.HCM	0912025262	0	0	Open	https://res.cloudinary.com/devu5qabc/image/upload/v1781836404/3b44f67f-e15a-4a84-9ae3-2c2078472713.png?cors=anonymous	0.4
84cc584d-335e-40a7-90c3-abebfe573b01	Salon Nguyễn Thông	55 Nguyễn Thông, Phường Nhiêu Lộc (Quận 3 cũ), TP.HCM	0946846177	0	0	Open	https://res.cloudinary.com/devu5qabc/image/upload/v1785572534/f915cbc1-b39f-4986-8773-17fd6da0363e.jpg?cors=anonymous	0.5
bc6e3435-7d2f-46e6-abe1-159befb1190e	MINH	s107, VINHOME GRAND PARK	0866679409	0	0	Closed	https://res.cloudinary.com/devu5qabc/image/upload/v1788833959/3a2b4dc1-c5e7-423f-95d0-02c6d0e59009.jpg?cors=anonymous	0.3
c2325bfa-edca-4803-92c1-9c2507f5b4a8	Salon Lê Thị Riêng	36b Đ. Lê Thị Riêng, Phường Phạm Ngũ Lão, Quận 1, TP.HCM	0396523147	10.52917	106.875	Open	https://res.cloudinary.com/devu5qabc/image/upload/v1781775702/5be8665b-bedb-4e6b-ac2d-30e4312aa0ec.jpg?cors=anonymous	0.6
118341cc-0df0-44cd-b40a-0a3f5d167c58	Salon Lê Thánh Tôn	15A/33 Lê Thánh Tôn, Bến Nghé, Quận 1, TP.HCM	0918273882	0	0	Open	https://res.cloudinary.com/devu5qabc/image/upload/v1783137881/e32b1db7-fdd4-4997-88ae-4203f9e11f4c.jpg?cors=anonymous	0.2
3bab09c5-20de-4a87-8415-60e2a62a16db	Salon Nguyễn Trãi	229 Nguyễn Trãi, Phường Nguyễn Cư Trinh, Quận 1, TP.HCM	0912025262	10.776461	106.703032	Open	https://res.cloudinary.com/devu5qabc/image/upload/v1781775702/5be8665b-bedb-4e6b-ac2d-30e4312aa0ec.jpg?cors=anonymous	0.2
\.


--
-- Data for Name: Schedules; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Schedules" ("ScheduleId", "NailArtistId", "WorkDate", "ShiftStart", "ShiftEnd", "Status") FROM stdin;
699f2de8-e5d8-4807-9a76-8bc2eae8b0af	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-04 00:00:00	09:00:00	17:00:00	Active
fbe99cba-a5ff-4457-a7f9-8866ddd2a70e	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-29 00:00:00	09:00:00	17:00:00	Active
18c95c18-381d-4e2a-a390-53970295fb4e	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-12 00:00:00	09:00:00	17:00:00	Active
e722c840-2f06-417c-b842-27f7e10609e8	c487ee7e-9e66-4c86-a015-1c22e9ef89cf	2026-08-10 00:00:00	08:00:00	23:30:00	Active
d36435e5-fc99-4fba-96b3-87603c902e65	2adce07e-6ef8-4f7d-8920-accc288417e9	2026-08-10 00:00:00	08:00:00	23:30:00	Active
36cc6e3e-10d3-4ad1-9145-9022990f206a	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-20 00:00:00	09:00:00	17:00:00	Active
564a397f-6d0e-4091-a1d9-894677b521c7	b53808e3-7219-4c65-899c-197f204e5581	2026-08-17 00:00:00	08:00:00	23:30:00	Active
a26fd812-1752-4e91-87a3-4755bf6b8873	618e9f63-8360-4cf4-b178-9457dd66761b	2026-08-17 00:00:00	08:00:00	23:30:00	Active
363057c5-157a-41ba-96c3-8779ee669fba	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-08-17 00:00:00	12:00:00	20:00:00	Active
8fef5bf7-0e8c-4383-b9a6-23d3d1b8e2b3	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-08-23 00:00:00	09:00:00	17:00:00	Active
65fa4b64-a361-40a0-a80e-73087f4efe84	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-08-20 00:00:00	09:00:00	17:00:00	Active
e93c393c-ecd2-4ecb-ae6b-c97ec4d8fb38	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-18 00:00:00	08:00:00	23:30:00	Active
7700a9f8-56c5-4135-b26d-363fb65102ac	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-19 00:00:00	08:00:00	23:30:00	Active
65030b96-6a9d-462b-9d64-0c3a6727f666	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-21 00:00:00	08:00:00	23:30:00	Active
04e83851-1f79-4539-a610-d93d3ea8df0c	066c5211-406e-4188-9461-023680ee6df2	2026-08-18 19:52:47	07:00:00	18:00:00	Active
0a91b28e-0527-476c-8d8a-68b966438134	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-18 19:52:59	07:00:00	21:00:00	Active
1375e75b-2eb9-4a07-862d-881ef30d08ef	dba37f5a-970c-41da-be69-438732e98402	2026-08-18 19:53:02	07:00:00	18:00:00	Active
19021407-9a33-4bb8-879c-5a23a2698cd0	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-18 19:53:04	08:00:00	23:30:00	Active
93d20d0a-301d-4fe8-84b1-495802cffeaa	b53808e3-7219-4c65-899c-197f204e5581	2026-09-09 00:00:00	14:00:00	17:00:00	Active
7916a688-da3b-474a-9dbd-9150effe1bc6	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-02 00:00:00	08:00:00	23:00:00	Active
20e0accb-ba50-4ffb-9025-c52f9eae656e	618e9f63-8360-4cf4-b178-9457dd66761b	2026-08-18 19:53:11	08:00:00	23:30:00	Active
2b045d6f-8864-43f5-ad6c-e6385b5c6cd8	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-18 19:53:16	08:00:00	23:30:00	Active
2f2c57ec-a3f7-4392-81c3-68431a7c1330	b53808e3-7219-4c65-899c-197f204e5581	2026-08-18 19:53:18	09:00:00	11:30:00	Active
097a5037-658d-4d29-90e4-3822ed7149fa	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-08-20 00:00:00	09:00:00	17:00:00	Active
be910429-83da-49da-8ccb-0f741737473e	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-08-23 00:00:00	09:00:00	17:00:00	Active
cca0e4fb-7ac6-47f1-ab30-338bd54a853f	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-27 00:00:00	09:00:00	17:00:00	Active
0d21defa-7d6a-4803-b624-2aa4e4e84827	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-08-24 00:00:00	09:00:00	17:00:00	Active
3fccadc5-cdd4-405c-ab82-5ff1bab346b0	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-05 00:00:00	08:00:00	23:30:00	Active
8630ed1d-7838-4416-8bb9-2ce3363ef93a	b53808e3-7219-4c65-899c-197f204e5581	2026-09-09 00:00:00	09:00:00	17:00:00	Active
b45c5ab7-c347-4013-8183-48c2abbe1be9	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-08-26 00:00:00	09:00:00	17:00:00	Active
1f741158-24cf-4a7e-a6a6-a71ec350950d	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-08-30 00:00:00	09:00:00	17:00:00	Active
b31c12ff-e64b-4592-a090-371a9ec70523	b53808e3-7219-4c65-899c-197f204e5581	2026-09-09 00:00:00	09:00:00	15:00:00	Active
6655db1a-07b8-4079-9004-2b7766469233	066c5211-406e-4188-9461-023680ee6df2	2026-08-05 00:00:00	07:00:00	18:00:00	Active
20c3e634-8c25-4e3a-a3f1-4282e62bad03	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-08-19 00:00:00	08:00:00	20:00:00	Active
4a3fd543-df2f-4e2a-873c-861ad4a01f47	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-08-28 00:00:00	08:00:00	23:00:00	Active
7df798cd-d62a-4685-bd73-4a537bf85638	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-08-05 00:00:00	07:00:00	19:00:00	Active
656072cd-1de2-4418-b6fb-185009502b6e	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-09 00:00:00	07:00:00	19:00:00	Active
8a06fbff-f539-42fe-8dde-199b8d1c3212	b53808e3-7219-4c65-899c-197f204e5581	2026-08-05 00:00:00	09:00:00	17:00:00	Active
8f153866-4019-4eaa-b380-6a11886db30b	b53808e3-7219-4c65-899c-197f204e5581	2026-08-05 00:00:00	09:00:00	17:00:00	Active
54132b03-e52a-4a50-9199-93d29139111c	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-09 00:00:00	08:00:00	23:30:00	Active
97d8efc4-584d-4b11-8aee-dc0576473a19	2adce07e-6ef8-4f7d-8920-accc288417e9	2026-08-05 00:00:00	07:00:00	19:00:00	Active
b8513ee3-eb47-4a50-b4ba-a3a9f3c116da	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-08-05 00:00:00	07:00:00	19:00:00	Active
4699322d-febe-467f-b7bf-ee2da587b5d6	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-08-24 00:00:00	08:00:00	23:00:00	Active
5e3e4304-238a-477c-8db2-f158fe941952	88dfbcbd-c239-4f9a-8f5d-dd155d7ac99b	2026-08-07 00:00:00	09:00:00	17:00:00	Active
b4ed74c4-6e55-4620-995e-05c42b6b99d6	618e9f63-8360-4cf4-b178-9457dd66761b	2026-08-07 00:00:00	08:00:00	23:00:00	Active
7b50f15a-bcc9-4e98-92a5-293f5114837b	00eac1e0-417a-4377-b718-5fee2ca8aae1	2026-08-07 00:00:00	08:00:00	23:00:00	Active
dc39609d-3133-40e6-84b9-e7b25347a9d5	2adce07e-6ef8-4f7d-8920-accc288417e9	2026-08-07 00:00:00	08:00:00	23:00:00	Active
41cf793a-4867-481f-a72a-a67bbd499392	066c5211-406e-4188-9461-023680ee6df2	2026-08-07 00:00:00	08:00:00	23:00:00	Active
508bb488-2aaa-4eb2-a8d1-3fbb6b54b3c3	618e9f63-8360-4cf4-b178-9457dd66761b	2026-08-08 00:00:00	08:00:00	23:00:00	Active
d15ba582-97c6-40ee-a28c-41718dca57a2	b53808e3-7219-4c65-899c-197f204e5581	2026-08-09 00:00:00	08:00:00	23:00:00	Active
47c59285-4cba-432c-a90c-ff8a46323a9c	c487ee7e-9e66-4c86-a015-1c22e9ef89cf	2026-08-09 00:00:00	08:00:00	23:00:00	Active
7bceec7b-2839-42e2-975d-c94ae63c1598	066c5211-406e-4188-9461-023680ee6df2	2026-08-09 00:00:00	08:00:00	23:00:00	Active
c4ad2777-d9e3-4702-908e-73a593401a5a	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-09 00:00:00	08:00:00	23:00:00	Active
f2c0c91e-68ba-4af8-a81e-60f5d3a2b2cf	618e9f63-8360-4cf4-b178-9457dd66761b	2026-08-11 00:00:00	08:00:00	23:00:00	Active
3a74b814-7cf7-4d6a-8fca-df62bceb2d61	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-08-18 00:00:00	08:00:00	23:30:00	Active
03b27f40-a11c-41fd-9625-e44d1193b52f	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-23 00:00:00	09:00:00	17:00:00	Active
83982fb3-e46b-415f-8464-4974c88a5a02	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-29 00:00:00	09:00:00	17:00:00	Active
3129babe-1a53-4af3-b440-da9fa9ed3c78	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-08-30 00:00:00	08:00:00	23:00:00	Active
49c29f98-caf3-4d79-8729-6b85ca54e8f8	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-08-25 00:00:00	08:00:00	23:00:00	Active
3f3010c0-c999-4b69-ab0a-757e423d098b	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-02 00:00:00	08:00:00	23:00:00	Active
1e4f4857-0db4-4b34-aa26-c93e186fa7ec	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-31 00:00:00	08:00:00	23:00:00	Active
d7736fe3-28b0-4527-b91d-e7662ab9349f	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-03 00:00:00	08:00:00	23:00:00	Active
70b0cb37-e3c5-478b-a9a0-0560b8c04796	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-06 00:00:00	08:00:00	23:00:00	Active
9848e17e-1d40-47e1-92ee-d4b470ecb36a	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-05 00:00:00	08:00:00	23:00:00	Active
4156017c-1325-47b3-ac82-6a5bc04c1504	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-01 00:00:00	08:00:00	23:00:00	Active
a49fc0d2-4dde-48f1-af90-ca6e8647b888	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-02 00:00:00	08:00:00	23:00:00	Active
a7cb8ba6-6ee0-459a-a190-55568ff06205	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-06 00:00:00	08:00:00	23:00:00	Active
19a567cc-6610-4fcf-9b84-94867d523bcb	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-05 00:00:00	08:00:00	23:00:00	Active
40120e36-45ea-423e-84a9-8b0205371b28	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-03 00:00:00	08:00:00	23:00:00	Active
74420564-8c61-4f86-8868-c75890a7d631	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-01 00:00:00	08:00:00	23:00:00	Active
b7e93997-8908-4b2d-a07b-956a63dfefa2	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-01 00:00:00	08:00:00	23:00:00	Active
36e1a45c-1647-42c2-978d-5b6807870a76	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-08-31 00:00:00	08:00:00	23:00:00	Active
ea6400c3-aa3e-4f2b-9fba-cf470d1341ac	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-01 00:00:00	09:00:00	17:00:00	Active
69b022f9-16a4-4bf0-8a2c-ac8ed7d59528	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-02 00:00:00	09:00:00	17:00:00	Active
ac9d231c-da11-49df-8cd9-2b349560b6a4	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-03 00:00:00	09:00:00	17:00:00	Active
f31244d3-da99-418a-ab3a-85f0b25dee39	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-30 00:00:00	09:00:00	17:00:00	Active
cec5d6a4-bbd7-4f72-9c31-2f1e0f540ee6	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-08-05 00:00:00	07:00:00	19:00:00	Active
1d53a34d-a130-4308-8161-182cf40718b0	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-28 00:00:00	09:00:00	17:00:00	Active
d7aed76a-3d1e-4fde-9557-5815667ca540	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-16 00:00:00	09:00:00	17:00:00	Active
c85aa185-a354-4a7c-bdbd-5d0406725ec4	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-17 00:00:00	09:00:00	17:00:00	Active
8d9bdc40-007b-461d-b861-f1254d78b37f	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-14 00:00:00	09:00:00	17:00:00	Active
da209fbc-3d40-4d73-9bd7-84926b5f55f5	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-13 00:00:00	09:00:00	17:00:00	Active
d566255a-632d-403d-9e9c-7bb418fc890d	b53808e3-7219-4c65-899c-197f204e5581	2026-08-05 00:00:00	09:00:00	17:00:00	Active
ed98bcaf-e63b-41ce-a8a5-f265842b716f	618e9f63-8360-4cf4-b178-9457dd66761b	2026-08-05 00:00:00	08:00:00	17:00:00	Active
f02e5fed-7932-4bb7-8842-9b24f61b4abd	b53808e3-7219-4c65-899c-197f204e5581	2026-08-05 00:00:00	09:00:00	11:30:00	Active
fc7b0ea1-a81b-458d-aa23-cb0dd3b01489	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-05 00:00:00	07:00:00	19:00:00	Active
ff63622c-7f44-476e-bb79-17d158dc3f1e	066c5211-406e-4188-9461-023680ee6df2	2026-08-05 00:00:00	07:00:00	23:00:00	Active
a7762aae-32fe-4752-aafa-4fa450ad3fe2	88dfbcbd-c239-4f9a-8f5d-dd155d7ac99b	2026-08-09 00:00:00	09:00:00	17:00:00	Active
ac3209a8-b9a9-4bd2-aafc-792157632bcc	b53808e3-7219-4c65-899c-197f204e5581	2026-08-07 00:00:00	08:00:00	23:00:00	Active
b667e174-c73f-42ff-a396-81106dbebcea	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-15 00:00:00	09:00:00	17:00:00	Active
a4d3fc7b-7c70-43eb-9f50-710d55443a5b	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-18 00:00:00	09:00:00	17:00:00	Active
2cab03c3-88fe-4574-a249-6a1281f5f2a3	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-08-21 00:00:00	09:00:00	17:00:00	Active
b84e41c1-eeba-43ac-96da-0c11e3d5a0ac	c487ee7e-9e66-4c86-a015-1c22e9ef89cf	2026-08-07 00:00:00	08:00:00	23:00:00	Active
d3338a5d-64b3-4d7a-b34b-e80704326ea4	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-08-22 00:00:00	09:00:00	17:00:00	Active
c3205961-4f40-4554-8cb0-77b6c887c737	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-25 00:00:00	09:00:00	17:00:00	Active
cd4f635c-a73e-4284-b653-9641fd6cb379	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-29 00:00:00	09:00:00	17:00:00	Active
73ce8da1-84a6-4939-baa1-a39059983688	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-26 00:00:00	09:00:00	17:00:00	Active
b5ddd911-e015-4ea3-9009-087d8ef5822d	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-08-07 00:00:00	08:00:00	23:00:00	Active
a3ce44de-c9b1-46f7-9499-35d2c4c5c994	b53808e3-7219-4c65-899c-197f204e5581	2026-08-08 00:00:00	08:00:00	23:00:00	Active
b8368393-6122-4765-a5d9-39016748e034	2adce07e-6ef8-4f7d-8920-accc288417e9	2026-08-09 00:00:00	08:00:00	23:00:00	Active
b07ffb6a-5038-4940-9711-3b72e6b7d8ab	00eac1e0-417a-4377-b718-5fee2ca8aae1	2026-08-09 00:00:00	08:00:00	23:00:00	Active
4dccd75e-143e-48c3-bd77-927b8c0e3ec4	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-08-09 00:00:00	08:00:00	23:00:00	Active
918ddac5-cb77-4841-9bc5-7caf88d18e48	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-10 00:00:00	08:00:00	23:30:00	Active
ce4a7d06-ef84-44e1-a471-e707c7f9fba3	b53808e3-7219-4c65-899c-197f204e5581	2026-08-10 00:00:00	08:00:00	23:30:00	Active
9f3eabc0-7faf-4170-9c59-b96443b84e89	00eac1e0-417a-4377-b718-5fee2ca8aae1	2026-08-10 00:00:00	08:00:00	23:30:00	Active
647510e6-dd3e-470c-a01b-7cc4d34a015f	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-08-10 00:00:00	08:00:00	23:30:00	Active
aedbd202-2fe3-4b26-be85-e0715c1ea34a	066c5211-406e-4188-9461-023680ee6df2	2026-08-10 00:00:00	08:00:00	23:30:00	Active
a0da8856-2ac7-4ad7-aa7a-ceed5ebdc3f0	b53808e3-7219-4c65-899c-197f204e5581	2026-08-11 00:00:00	08:00:00	23:30:00	Active
31f1dfe5-b7e3-4a5f-ba45-24bf8296d4d5	00eac1e0-417a-4377-b718-5fee2ca8aae1	2026-08-17 00:00:00	08:00:00	16:00:00	Active
7af82646-3481-473d-9e89-36a26b5de7e3	c487ee7e-9e66-4c86-a015-1c22e9ef89cf	2026-08-17 00:00:00	08:00:00	17:00:00	Active
d6ab423a-8043-43ce-af5a-1c2db06e8ac9	2adce07e-6ef8-4f7d-8920-accc288417e9	2026-08-17 00:00:00	08:00:00	23:30:00	Active
3228f8f0-1539-4c8c-9e5c-90c9c753de9f	066c5211-406e-4188-9461-023680ee6df2	2026-08-17 00:00:00	08:00:00	23:30:00	Active
b3d69a8d-cdcd-4c83-b551-74c0e353067f	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-17 00:00:00	08:00:00	23:30:00	Active
df60fa7c-8098-411b-b17b-c67b0f53b3bb	88dfbcbd-c239-4f9a-8f5d-dd155d7ac99b	2026-08-17 00:00:00	08:00:00	23:30:00	Active
afa1ae24-a81c-48fa-8d30-aa85b2846814	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-08-17 00:00:00	08:00:00	23:30:00	Active
55acb22c-c246-4978-8ea6-ec174e44cb66	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-08-21 00:00:00	09:00:00	17:00:00	Active
fa5c5324-7dcd-4fc8-a0d0-6cc038a41d34	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-08-18 00:00:00	09:00:00	17:00:00	Active
230f653a-e39d-4cd4-b371-34f2d9f01136	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-08-22 00:00:00	09:00:00	17:00:00	Active
6c9232b9-a723-4174-b125-47927e5c31d8	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-22 00:00:00	08:00:00	23:30:00	Active
8dec5145-8544-4f3a-b2b7-8cc4713bd7cc	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-23 00:00:00	08:00:00	23:30:00	Active
02d86b1d-3084-4191-94c0-08afe2f1e11e	88dfbcbd-c239-4f9a-8f5d-dd155d7ac99b	2026-08-18 19:52:44	09:00:00	17:00:00	Active
09f7894c-878c-4424-b7e8-59c50cf5cb78	88dfbcbd-c239-4f9a-8f5d-dd155d7ac99b	2026-08-18 19:52:51	09:00:00	17:00:00	Active
e8136b53-d852-4536-bae7-b0ceb19e80c6	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-21 00:00:00	09:00:00	17:00:00	Active
eff5b11e-93a3-4825-aeaa-2c7695ecc060	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-22 00:00:00	09:00:00	17:00:00	Active
b03aa071-ea7b-4f01-80f3-3f53be7d4256	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-26 00:00:00	09:00:00	17:00:00	Active
bd21997d-91a4-444b-807f-1323da7216b4	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-27 00:00:00	09:00:00	17:00:00	Active
e201c1f0-423d-4f74-8688-49e36eb0bf06	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-25 00:00:00	09:00:00	17:00:00	Active
7b55f8f1-2676-4de0-bb1a-16d4948596eb	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-24 00:00:00	09:00:00	17:00:00	Active
8f960830-3d7b-49cf-bdb7-c1aefb3dddcb	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-01 00:00:00	09:00:00	17:00:00	Active
3e5934b0-b30a-4635-80c8-6e48c3e04346	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-04 00:00:00	09:00:00	17:00:00	Active
129b3ab5-8ccf-4800-8678-9f5cb6dddb76	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-03 00:00:00	09:00:00	17:00:00	Active
1e4fd509-7ad3-4b12-ac96-f84733d423c3	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-28 00:00:00	09:00:00	17:00:00	Active
058d5dc5-86e3-4e42-9dc6-ed6572270761	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-30 00:00:00	09:00:00	17:00:00	Active
83103fa2-92ff-4372-9092-ee08061f2dc4	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-02 00:00:00	09:00:00	17:00:00	Active
91dfd486-2355-42c6-be24-76125e14cd21	53fca09d-2b87-41ae-95ea-640953f66815	2026-08-24 00:00:00	09:00:00	17:00:00	Active
cd4d94b6-497f-48d8-8dfe-51829c6e4d1f	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-08-25 00:00:00	09:00:00	17:00:00	Active
e665b72d-4ac2-4c7c-a285-5713d8e46e92	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-08-29 00:00:00	09:00:00	17:00:00	Active
1fffb13b-1774-4b27-a572-6540f4a5507b	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-08-27 00:00:00	09:00:00	17:00:00	Active
03be3317-e43b-4d0c-8dbd-718d490d6f3e	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-08-28 00:00:00	09:00:00	17:00:00	Active
49c0b6d7-8a21-45b6-b67d-4c2a05475be4	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-08-27 00:00:00	08:00:00	23:00:00	Active
50790758-fc89-4e36-9366-1141b6f25810	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-08-26 00:00:00	08:00:00	23:00:00	Active
f649f5ab-a25f-4908-899d-7338a67d12da	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-08-29 00:00:00	08:00:00	23:00:00	Active
f4bf21ed-de8e-41a3-9953-0cafa01ba35e	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-04 00:00:00	08:00:00	23:00:00	Active
83c0723f-1ab4-48d5-85b0-2129d892a500	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-08-31 00:00:00	08:00:00	23:00:00	Active
b9f50c4b-1d4d-463a-9941-36973e4953ec	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-04 00:00:00	08:00:00	23:00:00	Active
e0e01c31-516f-448a-b832-310e8fdc45d2	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-04 00:00:00	08:00:00	23:00:00	Active
c4a45425-5305-40e9-a5ad-dee81a56391d	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-07 00:00:00	09:00:00	17:00:00	Active
b0a40def-389d-4f3b-a09f-fac4937caf3c	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-11 00:00:00	09:00:00	17:00:00	Active
5fa8c5a4-85a8-4ff6-a221-98d2ff0bfcde	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-07 00:00:00	09:00:00	17:00:00	Active
db9098d3-6b12-4ef1-818b-5e8048cf3462	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-20 00:00:00	09:00:00	17:00:00	Active
5ac14c87-e708-47b7-a7a8-4b746cc50b91	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-16 00:00:00	09:00:00	17:00:00	Active
e3ab4882-0a11-4a9a-9d8a-bd8730a61e15	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-03 00:00:00	08:00:00	23:00:00	Active
1152e0ca-d730-49d2-9a45-9b8bf18aa4a1	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-05 00:00:00	08:00:00	23:00:00	Active
a1a4dd4c-3dd4-4143-8b85-d5dc6694e7c2	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-06 00:00:00	08:00:00	23:00:00	Active
050d8e30-6553-4e66-9cce-164ef93822b1	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-09 00:00:00	09:00:00	17:00:00	Active
0b736837-6265-43fd-a072-9042da703e6d	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-12 00:00:00	09:00:00	17:00:00	Active
d0c67493-20f7-439b-b750-76ca203d6740	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-10 00:00:00	09:00:00	17:00:00	Active
f21f60fa-005b-49da-ae4c-3a72e720af59	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-08 00:00:00	09:00:00	17:00:00	Active
1febb3cb-80a5-4c61-8a4c-a910df0da146	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-13 00:00:00	09:00:00	17:00:00	Active
a599c4bd-a554-41a1-b7a1-df981dd70d93	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-09 00:00:00	09:00:00	17:00:00	Active
b378c3bf-c51b-46f1-94c4-45a99873ad5b	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-12 00:00:00	09:00:00	17:00:00	Active
7fd736a3-907a-40d4-a043-535a9322ad3a	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-19 00:00:00	09:00:00	17:00:00	Active
93ffade6-3a1a-4865-aeb9-22a5a3124ed1	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-18 00:00:00	09:00:00	17:00:00	Active
f72a76ed-210c-4e66-ba2a-a67ccb9e6e1c	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-16 00:00:00	09:00:00	17:00:00	Active
06c368f9-a6da-436f-8ad0-860bc20d295c	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-18 00:00:00	09:00:00	17:00:00	Active
83161830-9513-49d0-824c-ab9c16eb9f9e	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-17 00:00:00	09:00:00	17:00:00	Active
8711b4ca-cc87-48c5-990e-274e14a8fbd4	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-13 00:00:00	09:00:00	17:00:00	Active
44ade9f7-966e-402e-bec1-7615ca7d9b13	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-08 00:00:00	09:00:00	17:00:00	Active
f64fa0d7-05ce-435c-86b1-112012b4b715	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-11 00:00:00	09:00:00	17:00:00	Active
4a3f6ce2-2e3f-4e8e-ad12-fedd86fecad4	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-09 00:00:00	09:00:00	17:00:00	Active
35c39510-5c16-4caa-bca1-b7d5879dd8d2	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-10 00:00:00	09:00:00	17:00:00	Active
c746961b-ae61-4fdd-98e5-8094225d4e4c	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-07 00:00:00	09:00:00	17:00:00	Active
d2d7f35a-4434-4629-96ee-b81df39b180d	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-12 00:00:00	09:00:00	17:00:00	Active
bd605438-d096-4b3b-8337-443f88cf313a	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-13 00:00:00	09:00:00	17:00:00	Active
bdf5bfd7-103f-49a8-a8f2-67fdde985075	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-10 00:00:00	09:00:00	17:00:00	Active
01ba0079-2c0b-4564-ac35-d96455ef4f69	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-11 00:00:00	09:00:00	17:00:00	Active
95ca5b9f-e968-458b-a6e1-aeb067dcab56	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-08 00:00:00	09:00:00	17:00:00	Active
05ab1d3f-4535-400d-b79a-0179a93637df	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-15 00:00:00	09:00:00	17:00:00	Active
0b5548e3-53f6-426f-bff5-9a81e0db1738	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-16 00:00:00	09:00:00	17:00:00	Active
f5dae517-82d6-4053-9310-ef2188cfeaa2	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-17 00:00:00	09:00:00	17:00:00	Active
9af3cd91-7114-4eec-9c86-2157617353d6	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-14 00:00:00	09:00:00	17:00:00	Active
4c5ffa40-0996-49e9-a7b6-26e163bd08b6	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-19 00:00:00	09:00:00	17:00:00	Active
77dadfd0-2128-4e94-87b1-b1fa488ff941	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-17 00:00:00	09:00:00	17:00:00	Active
c1e55af5-2d24-4e9f-add8-c05047096835	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-14 00:00:00	09:00:00	17:00:00	Active
048dca13-045b-4bb6-8620-ae9a9aad1b54	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-20 00:00:00	09:00:00	17:00:00	Active
598b9926-21b2-43ba-860f-714f1461d357	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-15 00:00:00	09:00:00	17:00:00	Active
7dca659f-9504-4291-b4ac-4d8e544fa813	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-14 00:00:00	09:00:00	17:00:00	Active
0cadd2c5-cc2f-4f89-9e93-387b8b42c9cf	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-15 00:00:00	09:00:00	17:00:00	Active
02ca7b95-c368-4a2d-8fbf-d2992ee3d962	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-20 00:00:00	09:00:00	17:00:00	Active
43e73a5d-e967-4c45-bd34-5787cc459552	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-19 00:00:00	09:00:00	17:00:00	Active
f7775cfe-9024-48b9-9352-9d069a33f6ff	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-18 00:00:00	09:00:00	17:00:00	Active
d22f5756-544a-42fb-87ac-85424f9899be	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-21 00:00:00	09:00:00	17:00:00	Active
e06b6302-e90c-472d-95f1-06781b49f64f	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-25 00:00:00	09:00:00	17:00:00	Active
bc7e4d7d-99a1-4d8e-b5ad-9c957de64f08	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-22 00:00:00	09:00:00	17:00:00	Active
c4afae03-617e-49d3-8d42-8cd12503c85f	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-26 00:00:00	09:00:00	17:00:00	Active
5769e338-b79a-4d2b-899d-443e95ee7110	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-27 00:00:00	09:00:00	17:00:00	Active
40a11fd8-500e-40ce-8cb1-7377e32a9e43	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-23 00:00:00	09:00:00	17:00:00	Active
78641b45-258c-4067-8263-ea532036de8b	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-09-24 00:00:00	09:00:00	17:00:00	Active
9d841e51-311f-4eb2-a53d-4732157ec8f9	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-21 00:00:00	09:00:00	17:00:00	Active
c3f9016e-9a6c-4d19-9407-1a1252763e34	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-25 00:00:00	09:00:00	17:00:00	Active
12623f55-b13c-4c25-be70-0ee0552fe58a	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-24 00:00:00	09:00:00	17:00:00	Active
b819be35-c49d-4718-9619-a5e6580fe5e3	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-22 00:00:00	09:00:00	17:00:00	Active
8a7abd28-6f77-46b7-960b-be753ee9c033	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-27 00:00:00	09:00:00	17:00:00	Active
b51f3b04-892f-4fc7-b7e6-8230f04aba95	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-26 00:00:00	09:00:00	17:00:00	Active
139d4f2b-38a4-48f3-af0f-9832d03c53ba	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-09-23 00:00:00	09:00:00	17:00:00	Active
62ac6df1-45ff-4d79-9078-050d2d8dad02	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-03 00:00:00	09:00:00	17:00:00	Active
1e960956-69f5-4ba3-bc26-8148077642a3	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-02 00:00:00	09:00:00	17:00:00	Active
515274ed-9ca9-45c0-9fa4-9ea0286a35d2	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-29 00:00:00	09:00:00	17:00:00	Active
8cabea4b-78d5-4fe3-9c80-492169f7c270	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-04 00:00:00	09:00:00	17:00:00	Active
44709ccf-d3e2-4ec8-9704-97a86e86b7cf	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-30 00:00:00	09:00:00	17:00:00	Active
1d0b4a59-e0f8-4c76-baca-d2a28a7ddc37	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-28 00:00:00	09:00:00	17:00:00	Active
22ad633a-670b-456c-a449-ef2e641ebffc	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-01 00:00:00	09:00:00	17:00:00	Active
7a330a1f-8086-4361-ab39-f1d3676cb9f2	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-05 00:00:00	09:00:00	17:00:00	Active
0d0535c0-2314-4dd8-8c68-abf41d3132d4	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-07 00:00:00	09:00:00	17:00:00	Active
bfb48c23-e298-4121-bfaf-83d7ccb7c5ab	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-10 00:00:00	09:00:00	17:00:00	Active
e13b2261-c49b-467d-8477-7c9f7d734c66	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-09 00:00:00	09:00:00	17:00:00	Active
01562cbd-521c-4150-8a05-c49aced251c5	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-06 00:00:00	09:00:00	17:00:00	Active
b518bdd8-e86f-4e70-a70d-57992f271cc2	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-11 00:00:00	09:00:00	17:00:00	Active
6ac187d4-f81a-4c2e-95f2-fc7431c28b95	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-08 00:00:00	09:00:00	17:00:00	Active
0e7ac673-fbc9-477c-b6a8-2b1195b6e83d	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-06 00:00:00	09:00:00	17:00:00	Active
087f6114-a4f5-4d50-b094-6345d835f7ce	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-09 00:00:00	09:00:00	17:00:00	Active
ef98f86b-73b8-4965-9390-1e31a4f1d9c3	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-11 00:00:00	09:00:00	17:00:00	Active
3f45269d-964d-4596-b82d-ecab00506e3c	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-10 00:00:00	09:00:00	17:00:00	Active
fe90a8bb-272d-46db-bd4f-3aa90aca3ede	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-08 00:00:00	09:00:00	17:00:00	Active
f595e707-b3ee-4832-ba61-b0ace34b8491	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-05 00:00:00	09:00:00	17:00:00	Active
aded73cf-edd6-4d6d-9d82-4b5849427e11	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-07 00:00:00	09:00:00	17:00:00	Active
a9ccf8a0-98b2-452d-bb44-24bfa2f47e7c	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-08 00:00:00	09:00:00	17:00:00	Active
a4c76ec0-05fc-453e-902f-b64ff53a2509	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-11 00:00:00	09:00:00	17:00:00	Active
f8bf8344-9d1b-43e9-8e83-5ca219bfaba7	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-09 00:00:00	09:00:00	17:00:00	Active
18e95325-d33e-4fa7-8bc5-5de554d9a167	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-07 00:00:00	09:00:00	17:00:00	Active
b7c283e6-8489-4699-b265-162acef15401	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-10 00:00:00	09:00:00	17:00:00	Active
d9c1a299-db3f-4926-b419-03dece144cdf	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-06 00:00:00	09:00:00	17:00:00	Active
5744b27d-3380-4ff8-ac5f-b109dca00981	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-05 00:00:00	09:00:00	17:00:00	Active
0e704668-bd77-4811-b7ed-b23636348dd2	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-12 00:00:00	09:00:00	17:00:00	Active
0d69e841-8dfe-49f3-b581-dd29f9578e0b	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-17 00:00:00	09:00:00	17:00:00	Active
e476603f-bdfe-458f-a273-01ce71deb3a5	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-18 00:00:00	09:00:00	17:00:00	Active
5d2e84bb-c7d7-427b-ac4b-08617e4b1452	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-15 00:00:00	09:00:00	17:00:00	Active
a2d70692-f864-4bee-bf23-f85f75c33324	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-16 00:00:00	09:00:00	17:00:00	Active
38014603-0d0e-43d7-8f40-84629400c2f4	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-13 00:00:00	09:00:00	17:00:00	Active
f5ba8c3b-cf56-43cb-af1b-781dea5e4b8f	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-14 00:00:00	09:00:00	17:00:00	Active
a060eb74-a422-4446-8eda-de02d2be8f26	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-13 00:00:00	09:00:00	17:00:00	Active
e5f1875f-8f67-4fbf-9282-c9fc32949bca	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-14 00:00:00	09:00:00	17:00:00	Active
0e0b1c15-231c-4f97-ad92-169dd75c9d32	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-27 00:00:00	09:00:00	17:00:00	Active
c0950f50-966c-449a-90f3-49c420a15488	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-23 00:00:00	09:00:00	17:00:00	Active
4c0a04f3-ccc0-4159-8a79-26cfc85813f7	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-24 00:00:00	09:00:00	17:00:00	Active
89593cb5-26a3-425a-a34f-c0c4a8df5deb	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-15 00:00:00	09:00:00	17:00:00	Active
73830b73-24f9-4f43-9ef5-1dcd8e2e4ea2	53fca09d-2b87-41ae-95ea-640953f66815	2026-11-01 00:00:00	09:00:00	17:00:00	Active
8fc13db7-5fe0-4b54-8265-c4a9fadcde1c	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-19 00:00:00	09:00:00	17:00:00	Active
bcb52129-8c4f-428b-89e0-974b12b62034	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-22 00:00:00	09:00:00	17:00:00	Active
fd5d9192-7aa8-4815-84eb-a7bc8b01445c	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-16 00:00:00	09:00:00	17:00:00	Active
05a1f7a3-b5b5-4ff1-878d-4017f677b2db	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-26 00:00:00	09:00:00	17:00:00	Active
c9d92bd4-9603-4863-9f61-b2398ceba25f	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-24 00:00:00	09:00:00	17:00:00	Active
d5de039e-2fee-47d9-93a5-57ffddcb720d	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-21 00:00:00	09:00:00	17:00:00	Active
277e0245-9e42-4a25-a99f-09b757432947	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-18 00:00:00	09:00:00	17:00:00	Active
4016fb58-8172-4cf0-a79d-fe93a2735331	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-28 00:00:00	09:00:00	17:00:00	Active
136de040-bec6-434b-9dae-14e6b704949e	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-22 00:00:00	09:00:00	17:00:00	Active
738a68e9-a062-412d-b2d5-5db6e7ad75b0	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-19 00:00:00	09:00:00	17:00:00	Active
643210a4-e840-4702-8c33-4dd1da29806e	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-12 00:00:00	09:00:00	17:00:00	Active
c6e657d9-e749-4fc3-90db-0cce89f94ffd	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-31 00:00:00	09:00:00	17:00:00	Active
c01f5f5c-b8a2-47ce-b49b-ca7b3af7fe47	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-21 00:00:00	09:00:00	17:00:00	Active
1579c312-4853-4e96-acdb-32ca93770eb3	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-23 00:00:00	09:00:00	17:00:00	Active
48730447-cbe5-4b12-9263-0cfa6b21f399	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-17 00:00:00	09:00:00	17:00:00	Active
b77eb3be-30b0-47e2-9dc4-c3486e7c41e9	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-30 00:00:00	09:00:00	17:00:00	Active
e533f5e2-80d1-4faa-9dde-9a70790f6f13	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-20 00:00:00	09:00:00	17:00:00	Active
2fa3ecef-e1dc-4ee3-aaa0-25746d9b0fc8	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-20 00:00:00	09:00:00	17:00:00	Active
b5415968-6fe8-4809-8eec-b54133af385e	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-29 00:00:00	09:00:00	17:00:00	Active
2356c45d-58cf-49d8-8691-484e1efda166	53fca09d-2b87-41ae-95ea-640953f66815	2026-10-25 00:00:00	09:00:00	17:00:00	Active
fb68ffaf-96f6-4714-b2e8-d1bc3de84e0e	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-25 00:00:00	09:00:00	17:00:00	Active
bbe8c2c2-9867-4a6c-bec7-78549a800e2a	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-19 00:00:00	09:00:00	17:00:00	Active
da48560f-54a7-4bd7-ad8e-83281c5bd344	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-23 00:00:00	09:00:00	17:00:00	Active
9069a6a5-b8c9-490d-b1dc-bff64f37c8cd	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-25 00:00:00	09:00:00	17:00:00	Active
0f3ceae9-a4e8-4c36-911d-e11dda527507	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-24 00:00:00	09:00:00	17:00:00	Active
06bbc3b0-58b4-4a59-bc82-aafec5da63a8	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-20 00:00:00	09:00:00	17:00:00	Active
b4ccac34-f78e-4a8c-be90-6611a0980cb8	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-21 00:00:00	09:00:00	17:00:00	Active
d6dd6cf5-6e14-4cd4-8495-fae3c0c4f8fd	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-22 00:00:00	09:00:00	17:00:00	Active
d8aa316d-e275-4d91-8db2-f708d2736389	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-31 00:00:00	09:00:00	17:00:00	Active
e0be73a3-7e80-4f10-b76a-fba1d7364533	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-30 00:00:00	09:00:00	17:00:00	Active
db7b465b-0dc6-472d-8905-0bf73d5170f6	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-27 00:00:00	09:00:00	17:00:00	Active
c90d73f7-0f97-4408-b609-666c94a38858	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-26 00:00:00	09:00:00	17:00:00	Active
2c894595-1862-4c9f-83d3-7885eef58c9c	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-29 00:00:00	09:00:00	17:00:00	Active
de36eade-8065-4a45-b4d4-138bac92ac9a	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-11-01 00:00:00	09:00:00	17:00:00	Active
c8ae98d5-de91-4ae1-b3e4-43b92b7b23ea	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	2026-10-28 00:00:00	09:00:00	17:00:00	Active
57ef2468-243a-4ce4-a025-0ee7ab4c5a9c	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-28 00:00:00	09:00:00	17:00:00	Active
d4afafef-030c-45a3-b96f-22d364e98d8b	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-26 00:00:00	09:00:00	17:00:00	Active
fd09bcff-95d1-4a1f-b4dd-4f8112852a22	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-27 00:00:00	09:00:00	17:00:00	Active
fe3c6a21-eb39-4aa6-9fdd-0d0285161d46	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-30 00:00:00	09:00:00	17:00:00	Active
d390816e-0c8b-4e0e-a2cc-dc0a2872f46c	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-29 00:00:00	09:00:00	17:00:00	Active
d9a5afb2-6190-418e-b500-642ea86cc2f5	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-11-01 00:00:00	09:00:00	17:00:00	Active
253a9942-0390-4945-95b3-69e66d90b626	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-10-31 00:00:00	09:00:00	17:00:00	Active
19729dcb-145a-46d2-8357-f2ac87261374	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-08-19 00:00:00	09:00:00	23:00:00	Active
2f7e5fd1-2629-4a6c-8886-7c9c0bf166a8	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	2026-08-19 00:00:00	00:00:00	00:00:00	Off
f2b672a8-9a90-426d-99f5-c44753fd3cc8	53fca09d-2b87-41ae-95ea-640953f66815	2026-09-09 00:00:00	07:00:00	19:00:00	Active
\.


--
-- Data for Name: Services; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Services" ("ServiceId", "Name", "Description", "Price", "Duration", "Status", "CreateAt") FROM stdin;
387fdc42-a732-46cb-abe0-3bbe4fa11906	Ngâm chân thảo mộc	Kết hợp các loại thảo mộc tự nhiên như gừng, sả, hương nhu, lá lốt và muối khoáng trong nước ấm, giúp kháng khuẩn, giảm đau nhức, tan mỡ máu và giải độc cơ thể qua da chân. Cảm giác thư giãn sâu, dễ chịu, đặc biệt tốt cho người thường xuyên đứng lâu hoặc đi giày cao gót.	50000.00	20	Active	2026-06-17 07:26:44
39850d34-91e5-4973-8a0f-644476dfe48a	Cắt da – chân\t	Sử dụng dụng cụ chuyên dụng đã được khử trùng, kỹ thuật viên nhẹ nhàng cắt bỏ phần da chết, vết chai sần hoặc da thừa ở gót chân và các cạnh bàn chân, giúp bàn chân trở nên mềm mại, sạch sẽ và ngăn ngừa nứt nẻ hiệu quả.	50000.00	20	Active	2026-06-18 15:33:09
f29a17c3-f99e-4b5d-a0ff-3e01b2e6a264	Cắt da – tay\t	Loại bỏ nhẹ nhàng phần da khô, xơ và xước ở vùng quanh móng và kẽ tay. Giúp đôi tay sạch sẽ, gọn gàng, hạn chế tình trạng da thừa gây vướng víu, tạo nền tảng hoàn hảo để móng và da tay trông tươi tắn, khỏe khoắn hơn.	50000.00	20	Active	2026-06-18 15:32:37
f512b732-231c-4584-b3f0-647603b1f167	Chà gót chân	Sử dụng đá bọt hoặc dụng cụ chà chuyên dụng kết hợp với hỗn hợp muối tẩy tế bào chết và dầu dưỡng, tác động nhẹ nhàng lên vùng gót chân để loại bỏ lớp da sừng hóa cứng đầu. Sau liệu trình, gót chân trở nên phẳng mịn, hồng hào và dễ dàng hấp thụ dưỡng chất.	50000.00	20	Active	2026-06-12 05:27:59.503
51870a0e-cae5-4f0f-9991-f330bb35462f	Vệ sinh móng	Làm sạch bụi bẩn, vi khuẩn và cặn bã dưới móng tay/chân. Cắt, dũa tạo dáng móng tự nhiên, đẩy lớp biểu bì và làm sạch vùng da quanh móng, giúp móng thông thoáng, chắc khỏe và sẵn sàng cho các bước sơn hoặc dưỡng tiếp theo.	50000.00	20	Active	2026-06-17 07:26:19
3135047c-f048-4f8b-926c-d5675e3387c3	Dưỡng dầu / massage tay\t	Sử dụng tinh dầu thiên nhiên kết hợp với kỹ thuật massage Nhật Bản, giúp kích thích tuần hoàn máu, giảm căng thẳng gân cốt và làm mềm da tay. Dưỡng chất thẩm thấu sâu, nuôi dưỡng móng và vùng da khô ráp, mang lại đôi tay mịn màng, săn chắc và thơm nhẹ nhàng.	50000.00	20	Inactive	2026-06-18 15:33:32
\.


--
-- Data for Name: ShapeMethodConfigs; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."ShapeMethodConfigs" ("ShapeMethodConfigId", "NailShapeId", "Name", "Price", "Duration", "Status") FROM stdin;
1	1	Sơn thường	50000.00	45	Active
2	1	Sơn gel	100000.00	60	Active
3	1	Đắp bột + Sơn thường	180000.00	75	Active
4	1	Đắp bột + Sơn gel	220000.00	85	Active
5	1	Acrylic + Sơn thường	200000.00	90	Active
6	1	Acrylic + Sơn gel	240000.00	100	Active
7	6	Sơn thường	50000.00	40	Active
8	6	Sơn gel	100000.00	55	Active
9	6	Đắp bột + Sơn thường	180000.00	70	Active
10	6	Đắp bột + Sơn gel	220000.00	80	Active
11	6	Acrylic + Sơn thường	200000.00	85	Active
12	6	Acrylic + Sơn gel	240000.00	95	Active
13	8	Sơn thường	60000.00	45	Active
14	8	Sơn gel	120000.00	60	Active
15	8	Đắp bột + Sơn thường	200000.00	80	Active
16	8	Đắp bột + Sơn gel	240000.00	90	Active
19	7	Đắp bột + Sơn thường	280000.00	100	Active
20	7	Đắp bột + Sơn gel	320000.00	110	Active
21	7	Acrylic + Sơn thường	320000.00	120	Active
22	7	Acrylic + Sơn gel	360000.00	130	Active
23	9	Đắp bột + Sơn thường	350000.00	120	Active
24	9	Đắp bột + Sơn gel	390000.00	130	Active
25	9	Acrylic + Sơn thường	400000.00	140	Active
26	9	Acrylic + Sơn gel	440000.00	150	Active
27	10	Đắp bột + Sơn thường	400000.00	130	Active
28	10	Đắp bột + Sơn gel	440000.00	140	Active
29	10	Acrylic + Sơn thường	450000.00	150	Active
30	10	Acrylic + Sơn gel	490000.00	160	Active
18	8	Acrylic + Sơn gel	260000.00	105	Inactive
17	8	Acrylic + Sơn thường	220000.00	95	Inactive
\.


--
-- Data for Name: SkillTypes; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."SkillTypes" ("SkillTypeId", "Name", "Description", "Status") FROM stdin;
13363e83-677c-4e5b-befe-bbc018d68373	Color	Kỹ năng lựa chọn, phối hợp và sử dụng màu sắc trong thiết kế móng, thể hiện khả năng tạo ra sự hài hòa, chuyển màu và phối màu phù hợp với phong cách và yêu cầu của khách hàng.	Active
0d810e8a-5d0e-43b4-8b2e-862af568b299	Form	Kỹ năng tạo hình và định dạng móng, thể hiện khả năng tạo dáng, cân đối và điều chỉnh form móng phù hợp với hình dáng móng tự nhiên, phong cách thiết kế và yêu cầu của khách hàng.	Active
525de8e8-1624-485c-afb2-b0f067156db1	Precision	Kỹ năng thực hiện các thao tác làm móng với độ chính xác và tỉ mỉ cao, bao gồm việc vẽ chi tiết, tạo đường nét sắc sảo, căn chỉnh cân đối và hoàn thiện thiết kế một cách tinh tế, đảm bảo chất lượng và tính thẩm mỹ của bộ móng.	Active
4a20f59e-2505-4b11-8eff-68ff16b39252	Speed	Kỹ năng thực hiện các công đoạn làm móng một cách nhanh chóng và hiệu quả, đồng thời duy trì chất lượng, độ chính xác và tính thẩm mỹ của bộ móng trong thời gian phù hợp.	Active
ffe0d973-f068-4c8d-9b18-6b0dbf395ea3	Minh	123	Inactive
11e07623-b990-4c3d-bfa0-b8f0a72a901a	Art	Kỹ năng nghệ thuật và trang trí móng của Nail Artist, thể hiện khả năng thực hiện các kỹ thuật, họa tiết và phong cách thiết kế nail khác nhau theo yêu cầu của khách hàng.	Inactive
\.


--
-- Data for Name: StaffTransfers; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."StaffTransfers" ("StaffTransferId", "NailArtistId", "FromSalonId", "ToSalonId", "StartDate", "EndDate", "Status", "Reason", "CreatedBy", "CreatedAt", "UpdatedAt") FROM stdin;
\.


--
-- Data for Name: Transactions; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Transactions" ("TransactionId", "BookingId", "OrderCode", "Amount", "Reference", "PaymentLinkId", "CheckoutUrl", "QrCode", "Status", "CreatedAt", "PaidAt", "ExpiresAt", "WebhookPayload", "Policy") FROM stdin;
155	8833af4e-bb29-486b-9a45-7bd33f4241e1	914210	12000.00	\N	\N			Paid	2026-07-17 23:12:09.324761	2026-07-17 23:12:09.324761	2026-07-17 23:27:09.324761		
156	5ed810a0-f0ee-4046-85ba-62183a3a49da	726686	84000.00	\N	\N			Paid	2026-07-22 01:15:11.617281	2026-07-22 01:15:11.617281	2026-07-22 01:30:11.617281		
157	ddc31886-f48e-4fcc-943c-d3cc9e14c8f5	644593	160000.00	\N	\N			Paid	2026-07-22 12:20:54.544854	2026-07-22 12:20:54.544854	2026-07-22 12:35:54.544854		
158	84853280-d3b7-4150-901a-4dd60e63d745	257875	160000.00	\N	\N			Paid	2026-07-26 09:52:52.281758	2026-07-26 09:52:52.281758	2026-07-26 10:07:52.281758		
159	0ecf1a61-9159-4bbd-9b2c-45a83bd42e4c	838439	12000.00	\N	\N			Paid	2026-07-27 16:56:04.885461	2026-07-27 16:56:04.885461	2026-07-27 17:11:04.885461		
160	f4b92bcb-3365-4354-9902-f9c084922767	434719	160000.00	\N	\N			Paid	2026-07-23 03:08:52.365204	2026-07-23 03:08:52.365204	2026-07-23 03:23:52.365204		
161	2993e6d6-b957-436d-a0f2-b5d9782eeddd	329252	350000.00	\N	\N			Paid	2026-08-10 10:06:43.096096	2026-08-10 10:06:43.096096	2026-08-10 10:21:43.096096		
162	571bda98-a035-4b14-9d2a-6e6a6bb2acf2	233326	160000.00	\N	\N			Paid	2026-07-26 09:52:27.484956	2026-07-26 09:52:27.484956	2026-07-26 10:07:27.484956		
163	bd3b6417-fa2b-4ef4-8ab7-3e33c61efff0	851147	94000.00	\N	\N			Paid	2026-07-27 16:53:22.737664	2026-07-27 16:53:22.737664	2026-07-27 17:08:22.737664		
164	4e3f981e-f1f3-42d5-b67b-299f07b178c2	579883	94000.00	\N	\N			Paid	2026-07-18 10:24:54.370971	2026-07-18 10:24:54.370971	2026-07-18 10:39:54.370971		
165	c1106f3d-8095-443e-b035-2df761fcac6d	198026	160000.00	\N	\N			Paid	2026-07-22 12:28:21.912076	2026-07-22 12:28:21.912076	2026-07-22 12:43:21.912076		
166	98a90c97-1cfd-4eb6-8c85-33d6ffbac9bf	689114	94000.00	\N	\N			Paid	2026-07-21 15:26:53.961392	2026-07-21 15:26:53.961392	2026-07-21 15:41:53.961392		
167	dabb9a56-e346-4f3b-9a8d-7e4c175ff1fc	729646	94000.00	\N	\N			Paid	2026-07-19 13:35:52.628813	2026-07-19 13:35:52.628813	2026-07-19 13:50:52.628813		
168	c2a91a31-1018-4a5e-a8e1-36e7249fecee	904450	12000.00	\N	\N			Paid	2026-07-22 14:35:30.015248	2026-07-22 14:35:30.015248	2026-07-22 14:50:30.015248		
169	931c5c48-eace-4ef1-a173-b791a22ca726	784315	160000.00	\N	\N			Paid	2026-07-21 17:27:25.729277	2026-07-21 17:27:25.729277	2026-07-21 17:42:25.729277		
170	bd7f9cab-5cc5-4002-a0f2-9e40d8212d34	146060	142000.00	\N	\N			Paid	2026-07-12 08:16:41.867704	2026-07-12 08:16:41.867704	2026-07-12 08:31:41.867704		
171	6f816baa-48a9-477d-9700-db60482ab272	827485	94000.00	\N	\N			Paid	2026-07-22 13:15:33.157893	2026-07-22 13:15:33.157893	2026-07-22 13:30:33.157893		
172	4b713e5d-d0da-4c9d-81ee-216f510cde31	140860	350000.00	\N	\N			Paid	2026-08-10 10:03:45.748189	2026-08-10 10:03:45.748189	2026-08-10 10:18:45.748189		
173	c13ad8a8-e179-48c0-9907-af31451e0565	922395	160000.00	\N	\N			Paid	2026-07-22 12:19:42.207296	2026-07-22 12:19:42.207296	2026-07-22 12:34:42.207296		
178	32060188-f6aa-42d5-b12c-60c30b579d58	151456	166320.00	FT26231JGRV4	bfdef232d5014e7fa9c962e600cc87b3	https://pay.payos.vn/web/bfdef232d5014e7fa9c962e600cc87b3	00020101021238620010A000000727013200069704480118CAS0651000204630090208QRIBFTTA530370454061663205802VN62370833CS5CP3K15D6 Thanh toan don 15145663047CBE	Paid	2026-08-19 09:29:07.162533	2026-08-19 09:29:22.996747	2026-08-19 09:44:07.162533	{"code":"00","desc":"success","success":true,"data":{"orderCode":151456,"amount":166320,"description":"CS5CP3K15D6 Thanh toan don 151456","accountNumber":"0065100020463009","reference":"FT26231JGRV4","transactionDateTime":"2026-08-19 16:29:00","currency":"VND","paymentLinkId":"bfdef232d5014e7fa9c962e600cc87b3","code":"00","desc":"success","counterAccountBankId":"","counterAccountBankName":"","counterAccountName":null,"counterAccountNumber":null,"virtualAccountName":"","virtualAccountNumber":"CAS065100020463009"},"signature":"152d97af4379e2a6a202024f8c100ccf26cf5418962569a9015895c77c59b94f","orderCode":0,"amount":0}	
187	738529a7-3c13-4311-b5fc-2005f1aba573	664625	55300.00	FT262369Z2J0	ed2e62ffd9c344ef8ac735a087fa7be4	https://pay.payos.vn/web/ed2e62ffd9c344ef8ac735a087fa7be4	00020101021238620010A000000727013200069704480118CAS0651000204630090208QRIBFTTA53037045405553005802VN62370833CS89LB67OT7 Thanh toan don 6646256304527C	Paid	2026-08-23 14:53:12.276437	2026-08-23 14:53:38.113866	2026-08-23 15:08:12.276437	{"code":"00","desc":"success","success":true,"data":{"orderCode":664625,"amount":55300,"description":"CS89LB67OT7 Thanh toan don 664625","accountNumber":"0065100020463009","reference":"FT262369Z2J0","transactionDateTime":"2026-08-23 21:53:00","currency":"VND","paymentLinkId":"ed2e62ffd9c344ef8ac735a087fa7be4","code":"00","desc":"success","counterAccountBankId":"","counterAccountBankName":"","counterAccountName":null,"counterAccountNumber":null,"virtualAccountName":"","virtualAccountNumber":"CAS065100020463009"},"signature":"b95431e8eea6e2e83e41c408cddb1ff26dbee57ae9777eb02558c90b7654ec89","orderCode":0,"amount":0}	
186	738529a7-3c13-4311-b5fc-2005f1aba573	121033	23700.00	FT26236XSYYQ	702b9ee688ef4b3bb4f3963220928e4b	https://pay.payos.vn/web/702b9ee688ef4b3bb4f3963220928e4b	00020101021238620010A000000727013200069704480118CAS0651000204630090208QRIBFTTA53037045405237005802VN62370833CSPKLUKQZO7 Thanh toan don 12103363046029	Paid	2026-08-23 14:52:05.624865	2026-08-23 14:52:28.051522	2026-08-23 15:07:05.624866	{"code":"00","desc":"success","success":true,"data":{"orderCode":121033,"amount":23700,"description":"CSPKLUKQZO7 Thanh toan don 121033","accountNumber":"0065100020463009","reference":"FT26236XSYYQ","transactionDateTime":"2026-08-23 21:52:00","currency":"VND","paymentLinkId":"702b9ee688ef4b3bb4f3963220928e4b","code":"00","desc":"success","counterAccountBankId":"","counterAccountBankName":"","counterAccountName":null,"counterAccountNumber":null,"virtualAccountName":"","virtualAccountNumber":"CAS065100020463009"},"signature":"38bbb70e0f6af0c381401185fbcb18bfb6b4a4c746fff3b2a2c125ba09a9f194","orderCode":0,"amount":0}	Cọc 30%
188	3a81ba1b-9a1c-48db-9094-4d318ba065e4	977035	79000.00	FT26236W8Y17	199ceb22f523424b8a236489de395d57	https://pay.payos.vn/web/199ceb22f523424b8a236489de395d57	00020101021238620010A000000727013200069704480118CAS0651000204630090208QRIBFTTA53037045405790005802VN62370833CSA2RHY4970 Thanh toan don 97703563043A38	Paid	2026-08-23 14:55:16.97731	2026-08-23 14:55:34.469366	2026-08-23 15:10:16.977312	{"code":"00","desc":"success","success":true,"data":{"orderCode":977035,"amount":79000,"description":"CSA2RHY4970 Thanh toan don 977035","accountNumber":"0065100020463009","reference":"FT26236W8Y17","transactionDateTime":"2026-08-23 21:55:00","currency":"VND","paymentLinkId":"199ceb22f523424b8a236489de395d57","code":"00","desc":"success","counterAccountBankId":"","counterAccountBankName":"","counterAccountName":null,"counterAccountNumber":null,"virtualAccountName":"","virtualAccountNumber":"CAS065100020463009"},"signature":"c59831d20974f9cd3124e3642d418eba1961ecfdd9724ff374b829b5ba18ab9f","orderCode":0,"amount":0}	
202	\N	361297	66300.00	\N	c7a0fb0dba5f4dccb4169a51b2368ee1	https://pay.payos.vn/web/c7a0fb0dba5f4dccb4169a51b2368ee1	00020101021238620010A000000727013200069704480118CAS0651000204630090208QRIBFTTA53037045405663005802VN62300826CSUN3RUIHF7 Coc don 3612976304F3EB	Overdue	2026-09-08 15:49:37.546572	\N	2026-09-08 16:04:37.546572		Cọc 20%
193	5864c2a3-6e31-4f7a-a143-c4afbc025327	RF-365114	47520.00	batch_a364d32fdaa64951927b16e62903138b	a42871ccaeb64321a853cd4eae337e51			Refunded	2026-08-25 11:41:15.650182	2026-08-25 11:41:15.650217	2026-08-25 11:41:15.650248	{"code":"00","desc":"success","data":{"id":"batch_a364d32fdaa64951927b16e62903138b","referenceId":"transaction_192_20260825114111","transactions":[{"id":"batch_txn_0f5fbd7c8cad4287a1c8e50493da75a6","referenceId":"transaction_192_20260825114111","amount":47520,"description":"Refund transaction 192","toBin":"970436","toAccountNumber":"9901697330","toAccountName":"PHAN DO GIA TUE","reference":"105553544","transactionDatetime":"2026-08-25T18:41:15+07:00","errorMessage":null,"errorCode":null,"state":"SUCCEEDED"}],"category":["refund"],"approvalState":"COMPLETED","createdAt":"2026-08-25T18:41:11+07:00"}}	Hoàn tiền toàn bộ do Salon hủy lịch.
192	5864c2a3-6e31-4f7a-a143-c4afbc025327	365114	47520.00	FT2623695DN2	a42871ccaeb64321a853cd4eae337e51	https://pay.payos.vn/web/a42871ccaeb64321a853cd4eae337e51	00020101021238620010A000000727013200069704480118CAS0651000204630090208QRIBFTTA53037045405475205802VN62300826CSIKBJZIY18 Coc don 3651146304C202	Paid	2026-08-24 05:08:24.387101	2026-08-24 05:08:48.180803	2026-08-24 05:23:24.387102	{"code":"00","desc":"success","success":true,"data":{"orderCode":365114,"amount":47520,"description":"CSIKBJZIY18 Coc don 365114","accountNumber":"0065100020463009","reference":"FT2623695DN2","transactionDateTime":"2026-08-24 12:08:00","currency":"VND","paymentLinkId":"a42871ccaeb64321a853cd4eae337e51","code":"00","desc":"success","counterAccountBankId":"","counterAccountBankName":"","counterAccountName":null,"counterAccountNumber":null,"virtualAccountName":"","virtualAccountNumber":"CAS065100020463009"},"signature":"88b9ee05724cc63726eedfe2da783708d69676a377996673c18d6b00ae9f9424","orderCode":0,"amount":0}	Cọc 30%
197	70eb03a9-e158-4428-a196-e4d81eb232d3	704459	110880.00	\N	\N			Paid	2026-08-28 06:56:48.596541	2026-08-28 06:56:48.596541	2026-08-28 06:56:48.596541		
198	\N	195012	29700.00	FT262508YSHP	8bc2494066254ea591c5c258e4de6e8c	https://pay.payos.vn/web/8bc2494066254ea591c5c258e4de6e8c	00020101021238620010A000000727013200069704480118CAS0651000204630090208QRIBFTTA53037045405297005802VN62300826CS8AU3QJEK4 Coc don 1950126304D3C6	Paid	2026-09-06 12:41:57.438863	2026-09-06 12:43:23.820556	2026-09-06 12:56:57.438886	{"code":"00","desc":"success","success":true,"data":{"orderCode":195012,"amount":29700,"description":"CS8AU3QJEK4 Coc don 195012","accountNumber":"0065100020463009","reference":"FT262508YSHP","transactionDateTime":"2026-09-06 19:42:00","currency":"VND","paymentLinkId":"8bc2494066254ea591c5c258e4de6e8c","code":"00","desc":"success","counterAccountBankId":"","counterAccountBankName":"","counterAccountName":null,"counterAccountNumber":null,"virtualAccountName":"","virtualAccountNumber":"CAS065100020463009"},"signature":"4a9a02512ee7315307493376dee95642fb43a1d02b98dc289aac2433f753c53d","orderCode":0,"amount":0}	Cọc 20%
200	88642819-451f-4089-ac79-9bc8ef7ed4c4	752175	16800.00	FT26250F55J0	1d1764ab3b244d7a93312bac0f2de67f	https://pay.payos.vn/web/1d1764ab3b244d7a93312bac0f2de67f	00020101021238620010A000000727013200069704480118CAS0651000204630090208QRIBFTTA53037045405168005802VN62370833CSDM2YRTLT7 Thanh toan don 7521756304DCF4	Paid	2026-09-07 10:48:14.361264	2026-09-07 10:49:15.73995	2026-09-07 11:03:14.361264	{"code":"00","desc":"success","success":true,"data":{"orderCode":752175,"amount":16800,"description":"CSDM2YRTLT7 Thanh toan don 752175","accountNumber":"0065100020463009","reference":"FT26250F55J0","transactionDateTime":"2026-09-07 17:49:00","currency":"VND","paymentLinkId":"1d1764ab3b244d7a93312bac0f2de67f","code":"00","desc":"success","counterAccountBankId":"","counterAccountBankName":"","counterAccountName":null,"counterAccountNumber":null,"virtualAccountName":"","virtualAccountNumber":"CAS065100020463009"},"signature":"92797b5d79ad2fa7dabef50a925a3211da12f8666ebcc1c8dc7ddc7132660382","orderCode":0,"amount":0}	Cọc 20%
199	05d91038-03d5-4743-80d9-34020b38e94a	656577	16800.00	FT26250DFXYX	72336c0fb6c44ce3bc0c3346cd0cc75d	https://pay.payos.vn/web/72336c0fb6c44ce3bc0c3346cd0cc75d	00020101021238620010A000000727013200069704480118CAS0651000204630090208QRIBFTTA53037045405168005802VN62370833CSJVC5WIRJ7 Thanh toan don 6565776304F455	Paid	2026-09-07 10:44:52.32993	2026-09-07 10:46:45.092134	2026-09-07 10:59:52.329948	{"code":"00","desc":"success","success":true,"data":{"orderCode":656577,"amount":16800,"description":"CSJVC5WIRJ7 Thanh toan don 656577","accountNumber":"0065100020463009","reference":"FT26250DFXYX","transactionDateTime":"2026-09-07 17:45:00","currency":"VND","paymentLinkId":"72336c0fb6c44ce3bc0c3346cd0cc75d","code":"00","desc":"success","counterAccountBankId":"","counterAccountBankName":"","counterAccountName":null,"counterAccountNumber":null,"virtualAccountName":"","virtualAccountNumber":"CAS065100020463009"},"signature":"5faa0b6ffca6bba55e0c4ac0f12c217f281861b908fa45905e03282dcf968180","orderCode":0,"amount":0}	Cọc 20%
203	\N	459799	49300.00	\N	e22a91bb5117480db5e5870e1966cf4f	https://pay.payos.vn/web/e22a91bb5117480db5e5870e1966cf4f	00020101021238620010A000000727013200069704480118CAS0651000204630090208QRIBFTTA53037045405493005802VN62300826CS037A6Q057 Coc don 459799630465D6	Overdue	2026-09-08 15:51:40.149943	\N	2026-09-08 16:06:40.149943		Cọc 20%
201	\N	667296	20300.00	\N	f9f625ac58834513bad162c407222688	https://pay.payos.vn/web/f9f625ac58834513bad162c407222688	00020101021238620010A000000727013200069704480118CAS0651000204630090208QRIBFTTA53037045405203005802VN62300826CS69BXDHJ04 Coc don 6672966304EF96	Paid	2026-09-08 15:04:11.724159	2026-09-08 15:09:28.388536	2026-09-08 15:19:11.724188		Cọc 20%
204	\N	953350	59500.00	\N	20cdc081ca2246fdb870ac3eefa90725	https://pay.payos.vn/web/20cdc081ca2246fdb870ac3eefa90725	00020101021238620010A000000727013200069704480118CAS0651000204630090208QRIBFTTA53037045405595005802VN62300826CS43TO4W505 Coc don 9533506304A959	Overdue	2026-09-08 16:01:05.16026	\N	2026-09-08 16:16:05.160261		Cọc 20%
\.


--
-- Data for Name: UserPromotionUsages; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."UserPromotionUsages" ("UserPromotionUsageId", "UserId", "PromotionId", "UsageCount", "LastUsedDate", "ReceivedCount") FROM stdin;
36	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	1	7	2026-08-23 14:54:27.701415	\N
37	0ddb8972-36cd-4b67-8887-829aadbdf942	8	1	2026-09-04 15:44:11.711861	1
38	0ddb8972-36cd-4b67-8887-829aadbdf942	1	3	2026-09-04 15:44:11.0478	\N
39	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	8	0	2026-09-06 07:03:03.37182	1
40	0ddb8972-36cd-4b67-8887-829aadbdf942	9	0	2026-09-06 10:43:20.170817	1
41	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	2	2	2026-09-07 10:47:48.912834	\N
\.


--
-- Data for Name: Users; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Users" ("UserId", "Email", "Password", "Phone", "FirstName", "LastName", "AvatarUrl", "Status", "Role", "SalonId", "CreatedAt") FROM stdin;
f9e14612-1973-4180-98a4-7e1b239d242c	manager@gmail.com	$2a$11$XF9nVC8OGdAMp1KafzLoiOjPyjPbPRaSrbzCxpC0nrJJ/xqBM2iZe	0987654321	Minh	Long	https://res.cloudinary.com/devu5qabc/image/upload/v1780398933/5d261fed-127f-4d46-896e-cad4246d5d32.png?cors=anonymous	Active	Manager	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	2026-07-18 16:18:57
3c63f226-f626-4757-9683-8e0376606b1a	tuedo4@gmail.com	$2a$11$w2V/.XqHD2HPip8lh0rQnenRtb5Tz5eutt1awIzqLg3X7ruwMABZ6	0971923112	Mai	Võ Ngọc	https://res.cloudinary.com/devu5qabc/image/upload/v1780398933/5d261fed-127f-4d46-896e-cad4246d5d32.png?cors=anonymous	Active	Manager	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	2026-07-18 16:18:27
3d089065-f088-4508-b5b3-f89b47722125	recep@gmail.com	$2a$11$w2V/.XqHD2HPip8lh0rQnenRtb5Tz5eutt1awIzqLg3X7ruwMABZ6	0917429332	Trần Hoàng	Nam	https://res.cloudinary.com/devu5qabc/image/upload/v1780398933/5d261fed-127f-4d46-896e-cad4246d5d32.png?cors=anonymous	Active	Receptionist	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	2026-07-18 16:18:29
c6c3645a-d5a7-4dab-808c-c4fb099ad469	nauy@gmail.com	$2a$11$o8bUmYjLGqpS.08yNxumiemSL63n5Zf/o0.kWVlcBhBkaGUYbxs9q	0910202093	Vy	Dương Khánh	https://res.cloudinary.com/devu5qabc/image/upload/v1780398933/5d261fed-127f-4d46-896e-cad4246d5d32.png?cors=anonymous	Active	Customer	\N	2026-07-18 16:18:44
08db5518-d779-4024-a345-3a7acace4095	kietvo.01@gmail.com	$2a$11$aB9hxTVmN6tWNSpaUp7CpOCL567cr8C9NjcdN6hE2wDNJnwDS2IbG	0912345679	Linh	Bùi Khánh	https://res.cloudinary.com/devu5qabc/image/upload/v1780398933/5d261fed-127f-4d46-896e-cad4246d5d32.png?cors=anonymous	Active	Customer	\N	2026-07-18 16:18:14
39ee1410-5e30-4c82-9a43-1163444951da	minh1@gmail.com	$2a$11$llbN.fp/281SuOXTggPiTeE7LoLlMw/KKncjbPUESP3p3CAFx1ewO	0988879555	Minh	Nguyen	https://res.cloudinary.com/devu5qabc/image/upload/v1780398933/5d261fed-127f-4d46-896e-cad4246d5d32.png?cors=anonymous	Active	Customer	\N	2026-07-18 16:18:25
c653b310-12c6-4a61-91b9-83b85af24635	admin@nailify.com	$2a$11$iB/bN7Yjb55HmFHbCwAQsOQXTBgKpFlQ8l2xKCOJyylOPyyN70TiG	0966340303	Bảo	Hồ Gia	https://res.cloudinary.com/devu5qabc/image/upload/v1780398933/5d261fed-127f-4d46-896e-cad4246d5d32.png?cors=anonymous	Active	Admin	\N	2026-07-18 16:18:42
8f21bc41-e10f-4b37-a123-a0800ec41aba	hieu1@gmail.com	$2a$11$nvbHxKvP9ZsHBRjCEXT9AuZweOV4hePxhNFhN5KeBZk4WLzykGKJq	0977927965	nguyen	hieu	https://res.cloudinary.com/devu5qabc/image/upload/v1780398933/5d261fed-127f-4d46-896e-cad4246d5d32.png?cors=anonymous	Active	Manager	c2325bfa-edca-4803-92c1-9c2507f5b4a8	2026-07-18 16:18:39
2feceac3-bbde-4659-a3d5-4e7e10b0a260	aaaaaaaaaaa@nailify.com	$2a$11$LWyCMxxNKF6tFRNAZWJrg.sHoR5V08iUyZK0e1kwG9w1uNi7ldMfm	0866645987	Việt ANh	Trần	\N	Inactive	Customer	\N	2026-08-19 01:03:17.754124
09e44380-4801-4715-8350-d2f3e2d69bc1	hieudai1@gmailc.om	$2a$11$KfxEGVEegdjv8B7NVB7RyuRGhlGsMxAbUeMjqzeJRi/oZwQssUGrq	0892161657	Thanh 123	Doan	https://res.cloudinary.com/devu5qabc/image/upload/v1780398933/5d261fed-127f-4d46-896e-cad4246d5d32.png?cors=anonymous	Inactive	Staff_Artist	3bab09c5-20de-4a87-8415-60e2a62a16db	2026-07-18 16:18:15
c933ebef-a971-4e39-82b9-6b22465261cd	walkin_0988765435_b9dddf@nailify.com	$2a$11$F3R7UxX2pFXnZ1fZ2AD07eqN4yEZKh2/pJCuWS8Ayf/l7LxtgDIWa	0988765435	nè	thành	\N	Active	Customer	\N	2026-09-08 19:43:00.400878
2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	tuedo51@gmail.com	$2a$11$5CQJ/xxMwGj4XwQAqL3zpu2GEbZGQAbRyZqt7wWfZooWSIRqzj5hq	0937294332	Vy	Nguyễn	https://res.cloudinary.com/devu5qabc/image/upload/v1780398933/5d261fed-127f-4d46-896e-cad4246d5d32.png?cors=anonymous	Active	Customer	\N	2026-07-18 16:18:23
ca83eb05-91ea-4865-b4fe-82948bf5dbbf	nailartist@gmail.com	$2a$11$w2V/.XqHD2HPip8lh0rQnenRtb5Tz5eutt1awIzqLg3X7ruwMABZ6	0941792114	Tue	Do	https://res.cloudinary.com/devu5qabc/image/upload/v1780398933/5d261fed-127f-4d46-896e-cad4246d5d32.png?cors=anonymous	Active	Staff_Artist	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	2026-07-18 16:18:49
8153236a-74a9-448d-bce8-c0e936d6df55	stevejobsvn@gmail.com	$2a$11$OVpk62x9MiuUWn.33phbIOlGzsCoaUwQHlre1LqXGPVuftNKoqBk.	0354912841	Anh	Nguyễn Minh	https://res.cloudinary.com/devu5qabc/image/upload/v1780398933/5d261fed-127f-4d46-896e-cad4246d5d32.png?cors=anonymous	Active	Staff_Artist	3bab09c5-20de-4a87-8415-60e2a62a16db	2026-07-18 16:18:37
c8f7b1b9-2a28-4d48-ae37-3e92a0d09723	artist1@gmail.com	$2a$11$k8A5HdyKr/9UmchLbWNHxOycvZWIykOf.MkYhO8RUBV6f.TKN9qWS	0338857821	Trung	Hieu	https://res.cloudinary.com/devu5qabc/image/upload/v1780398933/5d261fed-127f-4d46-896e-cad4246d5d32.png?cors=anonymous	Active	Staff_Artist	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	2026-07-18 16:18:46
1ae6f279-6e5c-4870-9154-4267f1e13042	hieuhieu@gmail.com	$2a$11$/qvlijaebxOJZMJ2TyHzzOq79LouKy4acXO9McdKpk5RaobXUwLIC	0945151827	Thanh Long	Nguyen	https://res.cloudinary.com/devu5qabc/image/upload/v1780398933/5d261fed-127f-4d46-896e-cad4246d5d32.png?cors=anonymous	Active	Staff_Artist	533bfd94-4f3f-4878-9fcc-5e2138d0b0d1	2026-07-18 16:18:20
cfd8cd9e-ec60-4145-812e-5fc73ea63bc7	stevejobsvn1@gmail.com	$2a$11$B2Jy.aJ2evN8SYhZRNJX2OZfZyh77do2Xa6Mya.m3uAS2qP33E.TC	0389515945	Long	Hoang	https://res.cloudinary.com/devu5qabc/image/upload/v1783932933/77d053bb-01d7-4578-87ec-a9201935eb2b.jpg?cors=anonymous	Active	Staff_Artist	84cc584d-335e-40a7-90c3-abebfe573b01	2026-07-18 16:18:47
eb504fd5-28c0-47d7-bd06-982d9e497769	minh2@gmail.com	$2a$11$f.z48eWrca.a5LJ3ABdLcOUoYP7DmtcdWI7F8o1E7Tt5L1wmTDixm	0909093288	Nguyên	Lê Thảo	https://res.cloudinary.com/devu5qabc/image/upload/v1783763775/261f75a0-d3a9-424d-83c7-702803b04350.jpg?cors=anonymous	Active	Staff_Artist	84cc584d-335e-40a7-90c3-abebfe573b01	2026-07-18 16:18:55
3e6b83a6-5e22-467a-9753-d90b4f10cef9	adwdasd@gmail.com	$2a$11$vxoJfc3h2zVfYw75RxbofOkwk3q6pGXgjdbAaVWANPvqx8PlZIx9O	0358719817	Nguyễn	Hiếu	https://res.cloudinary.com/devu5qabc/image/upload/v1780398933/5d261fed-127f-4d46-896e-cad4246d5d32.png?cors=anonymous	Active	Receptionist	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	2026-07-18 16:18:30
8fb48656-7a6a-48eb-9c4e-6af6639e2cac	thanhdtse185105@fpt.edu.vn	$2a$11$KdrTLVaeAL3sT8Ww80idhe.qGncWXtewMS1TtkFhcRpB4CllQMQam	0912025262	Thanh	Doan Trung	https://res.cloudinary.com/devu5qabc/image/upload/v1780398933/5d261fed-127f-4d46-896e-cad4246d5d32.png?cors=anonymous	Active	Customer	\N	2026-08-08 04:14:10.108948
0ddb8972-36cd-4b67-8887-829aadbdf942	customer@gmail.com	$2a$11$w2V/.XqHD2HPip8lh0rQnenRtb5Tz5eutt1awIzqLg3X7ruwMABZ6	0912345679	Minh	Hồ Nhật	https://res.cloudinary.com/devu5qabc/image/upload/v1780398933/5d261fed-127f-4d46-896e-cad4246d5d32.png?cors=anonymous	Active	Customer	\N	2026-07-18 16:18:17
f296db48-0e31-42f5-8a15-b939728e9cb1	phandogiatue51@gmail.com	$2a$11$8BiRREfP6m5iKGLNfCCzMu3xJUmjizlSAISGK1DexWWaV7KauUdzu	0901239456	Gia Tue	Phan Do	https://res.cloudinary.com/devu5qabc/image/upload/v1780398933/5d261fed-127f-4d46-896e-cad4246d5d32.png?cors=anonymous	Active	Customer	\N	2026-08-02 18:04:12.046318
4ddd8b89-bc84-47f0-b02d-0db0aa414434	walkin_0899908765_2c75d1@nailify.com	$2a$11$1Km2YezBEn.HssgVDMVfi.UdQ4V2vfgpJMSWHb8mr0Ura3N/3LLEC	0899908765	Gia Minh	Ho	https://res.cloudinary.com/devu5qabc/image/upload/v1780398933/5d261fed-127f-4d46-896e-cad4246d5d32.png?cors=anonymous	Inactive	Customer	\N	2026-08-18 19:51:19.048928
6bcc2c98-c815-439e-a6b2-0878c10cb68d	hieunguyen@gmail.com	$2a$11$ySYGRcKH8HaO/JHNx5XXRuJojuT5HrKQMg1BBLkZg8eIQmI3CN04K	0398745615	Hieu	Nguyen	https://res.cloudinary.com/devu5qabc/image/upload/v1780398933/5d261fed-127f-4d46-896e-cad4246d5d32.png?cors=anonymous	Active	Staff_Artist	118341cc-0df0-44cd-b40a-0a3f5d167c58	2026-07-18 16:18:33
2657ccc3-6270-4d3c-a752-159afb5e9893	vonguyennauy@gmail.com	$2a$11$Kw2oVmKS2K7J/P7.Pt7/3.xxu7YA18njGJ4sxnjHy5zo9FdoUN0DC	0988746512	Kiệt	Đặng Tuấn	https://res.cloudinary.com/devu5qabc/image/upload/v1780398933/5d261fed-127f-4d46-896e-cad4246d5d32.png?cors=anonymous	Inactive	Staff_Artist	118341cc-0df0-44cd-b40a-0a3f5d167c58	2026-07-18 16:18:22
d99f9bee-3505-4f1a-9627-461402ca1afc	walkin_0912020202_b1f214@nailify.com	$2a$11$fQQb//mXwg3J6L4xfG/UauMhGecmiqhrjDAtLqiz9v9fMTLMA5wX2	0912020202	nè	Dev	\N	Active	Customer	\N	2026-09-08 19:40:48.303987
4cbb0cf7-b545-4352-b3f4-8ffe69ea43b2	hieu@gmail.com	$2a$11$wZxJo1yquuhF1AZPQY2RbuR8tUDT0zF6Yr22mFn9TsgsSypqZYGs6	0358778574	Trung	Hậu	https://res.cloudinary.com/devu5qabc/image/upload/v1780398933/5d261fed-127f-4d46-896e-cad4246d5d32.png?cors=anonymous	Active	Staff_Artist	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	2026-07-18 16:18:31
591c8035-6592-45d8-aef4-e7503681eb8c	thanhdt@gmail.com	$2a$11$cBCskR4XaQvvZyz.MEOOne7Ro3dJoMDuxi2cNgPELHvD8cqaWxJvm	0915034629	Khanh Hung	Pham	https://res.cloudinary.com/devu5qabc/image/upload/v1780398933/5d261fed-127f-4d46-896e-cad4246d5d32.png?cors=anonymous	Active	Staff_Artist	533bfd94-4f3f-4878-9fcc-5e2138d0b0d1	2026-07-19 13:33:30.92073
5dfc99cb-6a49-4873-b035-ffe9826b169b	hieutrung@gmail.com	$2a$11$Ab5Ekx7luJDRyq/QCN7fiOR/kyF1n.VZzEiOijm6P2lePcX6N5IjW	0593784612	Hieu	Trung	https://res.cloudinary.com/devu5qabc/image/upload/v1780398933/5d261fed-127f-4d46-896e-cad4246d5d32.png?cors=anonymous	Active	Staff_Artist	c2325bfa-edca-4803-92c1-9c2507f5b4a8	2026-07-18 16:18:32
deb5cc4a-2da1-4f54-bc50-8a284cc8f57c	staff@nailify.com	$2a$11$iP91TzIOS6UzBrdB70JAPuf8Bbma9PRszjn1e.5AXBaqFFp11q6L2	0987654321	Thao	Thanh	https://res.cloudinary.com/devu5qabc/image/upload/v1780398933/5d261fed-127f-4d46-896e-cad4246d5d32.png?cors=anonymous	Active	Staff_Artist	c2325bfa-edca-4803-92c1-9c2507f5b4a8	2026-07-18 16:18:52
\.


--
-- Data for Name: WaitlistItems; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."WaitlistItems" ("WaitlistItemId", "WaitlistId", "NailVariantId", "ServiceId", "CustomerNailId", "Quantity") FROM stdin;
\.


--
-- Data for Name: WalkInQueues; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."WalkInQueues" ("QueueId", "SalonId", "CustomerId", "OriginalBookingId", "GuestName", "GuestPhone", "QueuePosition", "Status", "ArrivalTime", "CalledTime", "ServiceStartTime", "AssignedNailArtistId", "RequestNote", "EstimatedWait", "ChairId", "SelectedItemsJson") FROM stdin;
b578340b-3559-4cee-9708-f39b28df0954	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	0ddb8972-36cd-4b67-8887-829aadbdf942	\N	Khách Hàng2	0912345679	1	Waiting	2026-07-17 14:26:16.453258	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	Khách hàng đến muộn -> Tự động chuyển xuống hàng chờ.	0	\N	\N
e07e627c-15d5-475f-aca7-5c8acfd052aa	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	39ee1410-5e30-4c82-9a43-1163444951da	\N	Minh Nguyen	0988879555	1	Done	2026-07-27 08:52:27.353002	\N	2026-07-27 16:00:36.403448	53fca09d-2b87-41ae-95ea-640953f66815	Vệ sinh móng x1	0	\N	\N
5322fc42-78a7-4198-8f53-d1c4ab3a98b5	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	\N	\N	ThanhDT	0966340303	1	Waiting	2026-08-07 17:37:51.971299	\N	\N	b53808e3-7219-4c65-899c-197f204e5581	Pastel Rainbow x1 + Dưỡng dầu / massage tay\t x1 + Vệ sinh móng x2 + Cắt da – tay\t x1	442	\N	\N
41656bfb-bdbc-4ed6-8a66-c32302e1badd	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	\N	\N	AAA	0966340303	1	Waiting	2026-08-07 17:38:45.223456	\N	\N	b53808e3-7219-4c65-899c-197f204e5581	Pastel Rainbow x1 + Dưỡng dầu / massage tay\t x1 + Vệ sinh móng x1	441	\N	\N
68c144e9-ccbd-4d3b-a2cc-d9183957f87a	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	\N	\N	Thành	0966340303	1	Done	2026-07-11 10:39:10.546525	2026-07-11 10:40:39.455746	\N	618e9f63-8360-4cf4-b178-9457dd66761b	\N	60	\N	\N
52387f1c-b51a-4c75-87f0-cbd147ba0aea	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	\N	\N	aa	0966340303	1	Called	2026-07-14 13:49:36.322009	2026-07-14 13:50:03.090533	\N	\N	\N	60	\N	\N
6098461c-1514-4ec1-a13d-2f0a447a9892	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	\N	\N	aa	0966340303	2	Waiting	2026-07-14 13:49:38.767767	\N	\N	\N	\N	60	\N	\N
679ce820-5f45-4381-b9d7-50ce40a939f0	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	0ddb8972-36cd-4b67-8887-829aadbdf942	98a90c97-1cfd-4eb6-8c85-33d6ffbac9bf	Khách Hàng2	0912345679	3	Waiting	2026-07-17 14:35:27.357881	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	Khách hàng đến muộn -> Tự động chuyển xuống hàng chờ.	0	\N	\N
1faf9b9f-3089-4941-bf55-89b5991bf24d	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	\N	\N	aa	0966340303	1	Waiting	2026-07-15 01:16:25.295009	\N	\N	\N	\N	60	\N	\N
8b372c52-0431-4eb0-8375-0ae0a4e41fcd	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	\N	\N	aa	0966340303	1	Waiting	2026-07-15 01:16:14.832496	\N	\N	\N	\N	60	\N	\N
b65d5a76-9a0d-474b-a2e7-f8d627d09f91	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	\N	\N	Na Uy	0966340303	2	Done	2026-07-11 10:39:57.880759	2026-07-11 11:16:34.375876	\N	53fca09d-2b87-41ae-95ea-640953f66815	\N	60	\N	\N
848733e7-ceb6-4a93-93f2-8e219990a4b8	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	\N	\N	Vân Khánh	0966340303	3	Left	2026-07-11 11:18:55.979138	2026-07-11 13:36:51.338949	\N	\N	\N	60	\N	\N
23f4285b-1b76-4fb3-90da-151d627d5994	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	2feceac3-bbde-4659-a3d5-4e7e10b0a260	\N	Việt ANh Trần	0866645987	2	Done	2026-08-19 01:12:07.202093	\N	2026-08-19 01:13:10.613313	53fca09d-2b87-41ae-95ea-640953f66815	Cat Eye Magnetic x1 + Cắt da – chân\t x1	377	5499758b-4f1c-47b9-86a9-207bed3c96f2	[{"NailVariantId":null,"ServiceId":"39850d34-91e5-4973-8a0f-644476dfe48a","ShapeMethodConfigId":null,"CustomerNailId":null,"CustomerNailRequestId":null,"Quantity":1},{"NailVariantId":18,"ServiceId":null,"ShapeMethodConfigId":null,"CustomerNailId":null,"CustomerNailRequestId":null,"Quantity":1}]
60959c9f-8341-4686-86dd-82478a7d061d	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	4ddd8b89-bc84-47f0-b02d-0db0aa414434	\N	Ho Gia Minh	0899908765	6	Done	2026-08-18 19:51:18.001035	2026-08-18 12:57:35.254052	2026-08-18 19:57:43.850945	53fca09d-2b87-41ae-95ea-640953f66815	Pastel Rainbow x1 + Ngâm chân thảo mộc x1	30	df8e6759-dc4c-4689-8bae-1adc75fd50ae	[{"NailVariantId":null,"ServiceId":"387fdc42-a732-46cb-abe0-3bbe4fa11906","ShapeMethodConfigId":null,"CustomerNailId":null,"CustomerNailRequestId":null,"Quantity":1},{"NailVariantId":17,"ServiceId":null,"ShapeMethodConfigId":null,"CustomerNailId":null,"CustomerNailRequestId":null,"Quantity":1}]
4ea1e07e-63fe-426b-90d8-3a3f94608231	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	\N	\N	NaUy	0966340303	9	Waiting	2026-07-11 15:20:22.007092	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	\N	60	\N	\N
7aaefec1-1c4f-47ba-aec4-275a324c947c	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	\N	\N	Tue Do	0909281223	11	Waiting	2026-07-11 16:40:04.317392	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	\N	60	\N	\N
1f1343dd-d870-492c-8952-a43a200eb66a	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	\N	\N	Huy Phạm Gia	0901372442	1	Waiting	2026-08-17 19:48:49.934125	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	Khách hàng đến muộn -> Tự động chuyển xuống hàng chờ.	0	\N	\N
9cb172c5-8986-4ce0-bf77-52215629e36e	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	\N	\N	NaUy	0966340303	10	Waiting	2026-07-11 15:20:45.197779	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	\N	60	\N	\N
a548c966-904e-4b17-80c5-2bbfce337cea	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	\N	\N	Huy Phạm Gia	0901372442	1	Done	2026-08-18 10:40:21.25877	\N	2026-08-18 11:32:02.000187	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	Khách hàng đến muộn -> Tự động chuyển xuống hàng chờ.	35	\N	\N
a7cd4dbf-3951-4fd0-ac0c-873c4bde421d	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	\N	\N	Quân	0966340303	4	Called	2026-07-11 11:19:18.304971	2026-07-11 15:06:02.582902	\N	\N	\N	60	\N	\N
e50d3735-f834-4f6b-bf96-57866fe9fdc5	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	\N	\N	Messi	0966340303	6	Called	2026-07-11 13:36:37.160897	2026-07-11 15:05:27.698299	\N	53fca09d-2b87-41ae-95ea-640953f66815	\N	60	\N	\N
1c926272-6db0-4a7a-865e-9395a3490857	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	\N	Minh Dep Trai Nguyen	0937294332	1	Waiting	2026-08-08 13:32:34.346376	\N	\N	b53808e3-7219-4c65-899c-197f204e5581	Pastel Rainbow x1 + Vệ sinh móng x1	0	\N	\N
6278fea1-2c5b-4429-880b-701a5ae4e1f1	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	\N	\N	Nguyen Van An	0919979409	1	Called	2026-08-10 19:35:30.621529	2026-08-10 12:42:29.221451	\N	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	Ruby Đỏ x1 + Dưỡng dầu / massage tay\t x1 + Cắt da – chân\t x1	0	\N	\N
bb965170-2f0c-4dfa-adc7-bc53bee7619f	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	\N	\N	Minh	0866678789	3	Done	2026-08-18 11:56:12.449992	2026-08-18 04:56:58.312028	2026-08-18 12:07:59.816063	53fca09d-2b87-41ae-95ea-640953f66815	Floral Spring x1 + Ngâm chân thảo mộc x1	0	3108d303-cfba-4ba9-94ed-a7479f8ed522	\N
3a505c64-170c-4486-8a8a-ed0398de208f	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	\N	\N	Nguyễn Ngọc Minh	0866679409	2	Done	2026-08-18 11:52:40.560394	2026-08-18 04:54:24.024558	2026-08-18 11:54:31.472565	\N	Pastel Rainbow x1 + Ngâm chân thảo mộc x1 + Cắt da – chân\t x1	30	\N	\N
a1ca2d96-2e4b-4b1c-9ea2-ea5a3b4059c0	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	\N	\N	282	09523424442	1	Called	2026-07-28 02:08:50.400858	2026-07-28 02:09:54.388379	\N	53fca09d-2b87-41ae-95ea-640953f66815	Cat Eye Magnetic x1 + Vệ sinh móng x1	30	\N	\N
c4eabdba-7477-425d-8716-e6d6f878d791	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	\N	\N	MINH MOI ME	0912121212	2	Called	2026-07-27 09:02:04.674837	2026-07-27 09:14:46.089389	\N	53fca09d-2b87-41ae-95ea-640953f66815	Floral Spring x1 + Vệ sinh móng x1 + Chà gót chân x1	0	\N	\N
9923e859-983f-4acd-acb5-8b68fb6d44f7	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	\N	\N	aaa	09090909090	1	Done	2026-07-26 10:18:37.275969	\N	2026-07-26 17:19:25.605651	53fca09d-2b87-41ae-95ea-640953f66815	Dưỡng dầu / massage tay\t x1 + Vệ sinh móng x1	60	\N	\N
063a9561-e297-4330-af53-5c354a4ec6dd	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	08db5518-d779-4024-a345-3a7acace4095	\N	k v	0912345679	1	Waiting	2026-07-10 07:13:25.691692	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	Khách hàng đến muộn -> Tự động chuyển xuống hàng chờ.	60	\N	\N
bba8e783-d761-4e85-8b5c-a95b2dcc1b23	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	0ddb8972-36cd-4b67-8887-829aadbdf942	\N	Khách Hàng2	0912345679	3	Waiting	2026-07-14 15:58:07.506942	\N	\N	\N	Khách hàng đến muộn -> Tự động chuyển xuống hàng chờ.	60	\N	\N
205e5bb1-cd28-4ed5-b054-5d78045154fc	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	0ddb8972-36cd-4b67-8887-829aadbdf942	\N	Khách Hàng2	0912345679	2	Waiting	2026-07-17 14:32:21.084342	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	Khách hàng đến muộn -> Tự động chuyển xuống hàng chờ.	0	\N	\N
7c3d0401-9794-49e5-8f9f-0fe42c87f263	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	\N	hehe gam	0937294332	1	Waiting	2026-07-20 03:34:16.037993	\N	\N	f8c47d70-4d2e-4fdd-a59a-f9b3b53e80a8	Khách hàng đến muộn -> Tự động chuyển xuống hàng chờ.	205	\N	\N
ebabbfd7-f898-4cac-b248-f14ed5e1f4f2	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	0ddb8972-36cd-4b67-8887-829aadbdf942	\N	Khách Hàng2	0912345679	2	Waiting	2026-07-16 12:53:39.880006	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	Khách hàng đến muộn -> Tự động chuyển xuống hàng chờ.	60	\N	\N
89989e98-bd96-4589-a6cf-14781f4b078e	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	0ddb8972-36cd-4b67-8887-829aadbdf942	\N	Khách Hàng2	0912345679	3	Waiting	2026-07-16 12:55:26.51837	\N	\N	\N	Khách hàng đến muộn -> Tự động chuyển xuống hàng chờ.	60	\N	\N
2ba7e628-d81c-4c62-8c0e-9b8c2df4f5b5	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	0ddb8972-36cd-4b67-8887-829aadbdf942	\N	Khách Hàng2	0912345679	2	Waiting	2026-07-10 10:16:16.971882	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	Khách hàng đến muộn -> Tự động chuyển xuống hàng chờ.	60	\N	\N
ba28b07a-418e-46de-8514-3254db452527	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	2feceac3-bbde-4659-a3d5-4e7e10b0a260	\N	Trần Việt ANh	0866645987	1	Done	2026-08-19 01:03:16.615777	2026-08-18 18:04:28.263055	2026-08-19 01:04:47.879659	53fca09d-2b87-41ae-95ea-640953f66815	Cat Eye Magnetic x1 + Ngâm chân thảo mộc x1	386	df8e6759-dc4c-4689-8bae-1adc75fd50ae	[{"NailVariantId":null,"ServiceId":"387fdc42-a732-46cb-abe0-3bbe4fa11906","ShapeMethodConfigId":null,"CustomerNailId":null,"CustomerNailRequestId":null,"Quantity":1},{"NailVariantId":18,"ServiceId":null,"ShapeMethodConfigId":null,"CustomerNailId":null,"CustomerNailRequestId":null,"Quantity":1}]
d2ddf68d-d1f2-47d1-b664-934c6265c863	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	2698a9d9-4ae7-49b0-a78d-4d3b76ca0d33	\N	Vy Nguyễn	0937294332	4	Waiting	2026-08-19 13:08:16.2471	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	Khách hàng đến muộn -> Tự động chuyển xuống hàng chờ.	0	\N	[{"NailVariantId":null,"ServiceId":null,"ShapeMethodConfigId":null,"CustomerNailId":null,"CustomerNailRequestId":"31112ffd-dc19-430d-86ba-4f51633a6e44","Quantity":1}]
8f4f07d2-d185-4d6d-bca6-657b5c2cc1a7	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	8fb48656-7a6a-48eb-9c4e-6af6639e2cac	\N	Thanh Doan Trung	0912025262	3	Waiting	2026-08-19 11:15:55.884801	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	Khách hàng đến muộn -> Tự động chuyển xuống hàng chờ.	0	\N	[{"NailVariantId":null,"ServiceId":"3135047c-f048-4f8b-926c-d5675e3387c3","ShapeMethodConfigId":null,"CustomerNailId":null,"CustomerNailRequestId":null,"Quantity":1},{"NailVariantId":17,"ServiceId":null,"ShapeMethodConfigId":null,"CustomerNailId":null,"CustomerNailRequestId":null,"Quantity":1}]
935b9a56-64e2-4d74-bd9c-7251c3ccfa14	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	c653b310-12c6-4a61-91b9-83b85af24635	0d0db69e-14d2-45fe-876d-31767e9f553c	Thanh Thao Dev	0966340303	1	Done	2026-09-08 19:36:37.265802	2026-09-08 12:38:37.269446	2026-09-08 19:38:44.453288	9e150ae6-7d66-4995-87ea-cd7d88cc83f8	Pastel Rainbow x1 + Cắt da – chân\t x2 + Chà gót chân x1	60	df8e6759-dc4c-4689-8bae-1adc75fd50ae	[{"NailVariantId":null,"ServiceId":"39850d34-91e5-4973-8a0f-644476dfe48a","ShapeMethodConfigId":null,"CustomerNailId":null,"CustomerNailRequestId":null,"Quantity":2},{"NailVariantId":null,"ServiceId":"f512b732-231c-4584-b3f0-647603b1f167","ShapeMethodConfigId":null,"CustomerNailId":null,"CustomerNailRequestId":null,"Quantity":1},{"NailVariantId":17,"ServiceId":null,"ShapeMethodConfigId":null,"CustomerNailId":null,"CustomerNailRequestId":null,"Quantity":1}]
70d0272f-89f5-4f9f-bb6e-64ac0560eef3	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	0ddb8972-36cd-4b67-8887-829aadbdf942	\N	Khách Hàng2	0912345679	1	Waiting	2026-07-16 10:38:02.794393	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	Khách hàng đến muộn -> Tự động chuyển xuống hàng chờ.	60	\N	\N
9a537bea-f26a-40eb-bcca-8d4b4f9ae73f	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	0ddb8972-36cd-4b67-8887-829aadbdf942	\N	Khách Hàng2	0912345679	1	Waiting	2026-07-11 14:05:10.23626	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	Khách hàng đến muộn -> Tự động chuyển xuống hàng chờ.	60	\N	\N
a37eaae8-33fe-494c-a9bf-a182c918098c	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	0ddb8972-36cd-4b67-8887-829aadbdf942	\N	Khách Hàng2	0912345679	7	Waiting	2026-07-11 14:05:13.033704	\N	\N	\N	Khách hàng đến muộn -> Tự động chuyển xuống hàng chờ.	60	\N	\N
89f8617b-8d65-4a46-9263-95673809cfa1	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	0ddb8972-36cd-4b67-8887-829aadbdf942	\N	Khách Hàng2	0912345679	8	Waiting	2026-07-11 14:20:03.592287	\N	\N	618e9f63-8360-4cf4-b178-9457dd66761b	Khách hàng đến muộn -> Tự động chuyển xuống hàng chờ.	60	\N	\N
ac8a1593-a08e-4d47-939d-f84e9dd452e0	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	d99f9bee-3505-4f1a-9627-461402ca1afc	0f7b2049-386c-4142-8f2d-8f768632b078	Dev nè	0912020202	2	Done	2026-09-08 19:40:47.241449	\N	2026-09-08 19:42:28.426575	53fca09d-2b87-41ae-95ea-640953f66815	Pastel Rainbow x1 + Ngâm chân thảo mộc x1 + Cắt da – chân\t x1	60	5499758b-4f1c-47b9-86a9-207bed3c96f2	[{"NailVariantId":null,"ServiceId":"387fdc42-a732-46cb-abe0-3bbe4fa11906","ShapeMethodConfigId":null,"CustomerNailId":null,"CustomerNailRequestId":null,"Quantity":1},{"NailVariantId":null,"ServiceId":"39850d34-91e5-4973-8a0f-644476dfe48a","ShapeMethodConfigId":null,"CustomerNailId":null,"CustomerNailRequestId":null,"Quantity":1},{"NailVariantId":17,"ServiceId":null,"ShapeMethodConfigId":null,"CustomerNailId":null,"CustomerNailRequestId":null,"Quantity":1}]
05f7ef2e-2c44-45cb-801c-fd7a1fb1cef3	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	\N	\N	Huy Phạm Gia	0901372442	1	Done	2026-08-18 10:40:18.364954	2026-08-18 04:32:14.95831	2026-08-18 11:32:21.964517	53fca09d-2b87-41ae-95ea-640953f66815	Khách hàng đến muộn -> Tự động chuyển xuống hàng chờ.	35	\N	\N
df7763fa-f5c4-4d85-896d-1e774c3cc5c9	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	c933ebef-a971-4e39-82b9-6b22465261cd	\N	thành nè	0988765435	3	Waiting	2026-09-08 19:42:59.429478	\N	\N	53fca09d-2b87-41ae-95ea-640953f66815	Pastel Rainbow x1 + Ngâm chân thảo mộc x2	60	\N	[{"NailVariantId":null,"ServiceId":"387fdc42-a732-46cb-abe0-3bbe4fa11906","ShapeMethodConfigId":null,"CustomerNailId":null,"CustomerNailRequestId":null,"Quantity":2},{"NailVariantId":17,"ServiceId":null,"ShapeMethodConfigId":null,"CustomerNailId":null,"CustomerNailRequestId":null,"Quantity":1}]
7d7b0bcd-e3ab-4a55-8511-c00c68ba5909	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	\N	\N	NGÔ HỒNG NGỌC	0908456123	4	Done	2026-08-18 12:14:28.883177	2026-08-18 05:14:43.985422	2026-08-18 12:14:55.777107	53fca09d-2b87-41ae-95ea-640953f66815	Pastel Rainbow x1 + Ngâm chân thảo mộc x1	0	ab90b056-135f-41bd-92a8-7339292fada9	\N
58b2e1b4-cd9e-4b71-8e24-6bcae453b6e5	484c3aef-3ae1-4ad6-8aba-6b0bc6df586d	\N	\N	Hạnh Tỏi	0907899234	5	Done	2026-08-18 12:20:02.529944	2026-08-18 05:20:33.621218	2026-08-18 12:21:12.51523	53fca09d-2b87-41ae-95ea-640953f66815	Pastel Rainbow x1 + Ngâm chân thảo mộc x1	30	df8e6759-dc4c-4689-8bae-1adc75fd50ae	\N
\.


--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20260529041738_ThanhDT	8.0.0
20260530122203_TuePDG-AddNailCategory	8.0.0
20260530123308_TuePDG-AddNailCategory	8.0.0
20260601085301_ThanhDT-Init-Salon-Staff-Schedule-Core	8.0.0
20260601155658_ThanhDT-Add-ImagesUrl-Salon	8.0.0
20260602044252_ThanhDT-AddNailArtistSalonRelationship	8.0.0
20260602094005_TuePDG-AddNailImage	8.0.0
20260602102543_TuePDG-AddNailVariantCrudModels	8.0.0
20260602110425_TuePDG-AddNailVariantImageUrl	8.0.0
20260601085906_KietVA-AddRoleToUsers	8.0.0
20260603081735_TuePDG-RemoveSkillInVariant	8.0.0
20260603083855_TuePDG-DesignPriceRangeVariantComputed	8.0.0
20260603092034_TuePDG-AddCustomerNailCustomizationCrud	8.0.0
20260606100545_TuePDG-UpdateNullableVariant	8.0.0
20260606110523_TuePDG-AddNailColorJson	8.0.0
20260604111235_KietVA-AddCustomerAndMapNailArtist	8.0.0
20260606121503_ThanhDT-UpdateAvatarUrlToNullable	8.0.0
20260607100037_TuePDG-AddDuration	8.0.0
20260608063916_TuePDG-RemoveJsonInCustomerNail	8.0.0
20260608064500_TuePDG-RemoveMaterialCustomerNail	8.0.0
20260608072836_TuePDG-NullableCustomerNail	8.0.0
20260608073148_TuePDG-NullableCustomerNail2	8.0.0
20260609084032_TuePDG-RemoveNailVariantInCustomer	8.0.0
20260609041959_ThanhDT-RecreateNailRequiredSkillTable-NailVariant	8.0.0
20260612092914_ThanhDT-Rename-ExpectedTime-StartTime	8.0.0
20260613070828_TuePDG-AddStatus	8.0.0
20260613073600_TuePDG-DefaultStatusActive	8.0.0
20260615091256_ThanhDT-Add-NailProcedure-Procedure-BookingProcedure	8.0.0
20260617133506_ThanhDT-Fix-RelationShip-Users-Salon-NailArtist	8.0.0
20260619080028_TuePDG-FavoriteAndLoyalty	8.0.0
20260619082737_TuePDG-FavoriteAndLoyalty	8.0.0
20260619145217_ThanhDT-AddRequired-Procedure	8.0.0
20260621061752_ThanhDT-AddStatusAndApprovalToCustomerNail	8.0.0
20260623021212_ThanhDT-AddSalonIdToCustomerNail	8.0.0
20260623122326_TuePDG-AddDiscountInBooking	8.0.0
20260623133542_ThanhDT-AddCustomerNailRequest	8.0.0
20260626115541_TuePDG-AddBookingRating	8.0.0
20260629105802_TuePDG-AddPromotion	8.0.0
20260629110545_TuePDG-AddImageInPromotion	8.0.0
20260629131844_TuePDG-RemoveSalonInCustomerNail	8.0.0
20260629171711_TuePDG-AddIsRatedInBooking	8.0.0
20260630143710_TuePDG-AdjustNailSurface	8.0.0
20260701044637_ThanhDT-AddBookingActualTimesAndLateArrival-AddWalkInAndWaitlistTables	8.0.0
20260701085058_ThanhDT-FixRequestedDate	8.0.0
20260702084748_TuePDG-AddTransaction	8.0.0
20260704100034_TuePDG-AddRefundTransaction	8.0.0
20260704111023_TuePDG-UpdateRefundTransaction	8.0.0
20260705092924_TuePDG-FixBookingItems	8.0.0
20260703152603_ThanhDT-addFieldConcurrentCapacity-NailArtist	8.0.0
20260705093954_ThanhDT-AddFieldToBookingProcedure-Procedure-BookWailist	8.0.0
20260705174114_ThanhDT-AddChairTable	8.0.0
20260706133629_TuePDG-AdjustNailShapeNailVariant	8.0.0
20260706095653_ThanhDT-AddIsMainStepColumn	8.0.0
20260709064425_TuePDG-AddAmountDueInBooking	8.0.0
20260709120621_TuePDG-FixBookingItemPrice	8.0.0
20260709025315_ThanhDT-AddNailArtistBreakTable	8.0.0
20260709161833_ThanhDT-AddQuiz-QuizOptionTable	8.0.0
20260710062724_ThanhDT-AddSalonOffDayTable	8.0.0
20260715134155_ThanhDT-AddCustomerFields	8.0.0
20260715143645_ThanhDT-AddRejectReasonToArtistBreak	8.0.0
20260716115150_TuePDG-FixLoyaltyTierRule	8.0.0
20260718091428_TuePDG-FixUserNailDesignImage	8.0.0
20260721171556_ThanhDT-addProcedure-BookingProcedure	8.0.0
20260723093447_TuePDG-FixPromotion	8.0.0
20260727134534_ThanhDT-AddChairIdToWalkInQueue	8.0.0
20260729061756_ThanhDT-AddStaffTransfer	8.0.0
20260803075344_TuePDG-RemoveIsPublic	8.0.0
20260809072151_ThanhDT-AddProcedureType-ToProcedure	8.0.0
20260809085159_ThanhDT-SupportCustomNailProcedure	8.0.0
20260810123821_ThanhDT-AddBookingProperty	8.0.0
20260818113116_ThanhDT-AddJson	8.0.0
20260818114521_TuePDG-FixCustomerNailRequest	8.0.0
20260822083856_TuePDG-AddSalonConfig	8.0.0
20260904035602_ThanhDT-AddVirtualWalletAndPromotions	8.0.0
20260907144857_RemoveUniqueConstraintFromLoyaltyTransactionBookingId	8.0.0
\.


--
-- Name: aggregatedcounter_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.aggregatedcounter_id_seq', 13364, true);


--
-- Name: counter_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.counter_id_seq', 15667, true);


--
-- Name: hash_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.hash_id_seq', 29, true);


--
-- Name: job_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.job_id_seq', 5170, true);


--
-- Name: jobparameter_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.jobparameter_id_seq', 14999, true);


--
-- Name: jobqueue_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.jobqueue_id_seq', 5340, true);


--
-- Name: list_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.list_id_seq', 1, false);


--
-- Name: set_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.set_id_seq', 5558, true);


--
-- Name: state_id_seq; Type: SEQUENCE SET; Schema: hangfire; Owner: postgres
--

SELECT pg_catalog.setval('hangfire.state_id_seq', 16479, true);


--
-- Name: BookingDiscounts_BookingDiscountId_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."BookingDiscounts_BookingDiscountId_seq"', 276, true);


--
-- Name: Categories_CategoryId_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Categories_CategoryId_seq"', 82, true);


--
-- Name: CategoryTypes_CategoryTypeId_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."CategoryTypes_CategoryTypeId_seq"', 12, true);


--
-- Name: Components_ComponentId_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Components_ComponentId_seq"', 26, true);


--
-- Name: CustomerComponents_CustomerComponentId_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."CustomerComponents_CustomerComponentId_seq"', 20, true);


--
-- Name: CustomerNailComponents_CustomerNailComponentId_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."CustomerNailComponents_CustomerNailComponentId_seq"', 188, true);


--
-- Name: CustomerNails_CustomerNailId_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."CustomerNails_CustomerNailId_seq"', 109, true);


--
-- Name: FavoriteNails_FavoriteNailId_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."FavoriteNails_FavoriteNailId_seq"', 9, true);


--
-- Name: LoyaltyTiers_LoyaltyTierId_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."LoyaltyTiers_LoyaltyTierId_seq"', 24, true);


--
-- Name: LoyaltyTransactions_LoyaltyTransactionId_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."LoyaltyTransactions_LoyaltyTransactionId_seq"', 124, true);


--
-- Name: NailCategories_NailCategoryId_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."NailCategories_NailCategoryId_seq"', 324, true);


--
-- Name: NailComponents_NailComponentId_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."NailComponents_NailComponentId_seq"', 281, true);


--
-- Name: NailDesigns_NailDesignId_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."NailDesigns_NailDesignId_seq"', 25, true);


--
-- Name: NailShapes_NailShapeId_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."NailShapes_NailShapeId_seq"', 15, true);


--
-- Name: NailSurfaces_NailSurfaceId_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."NailSurfaces_NailSurfaceId_seq"', 17, true);


--
-- Name: NailVariants_NailVariantId_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."NailVariants_NailVariantId_seq"', 65, true);


--
-- Name: Promotions_PromotionId_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Promotions_PromotionId_seq"', 11, true);


--
-- Name: ShapeMethodConfigs_ShapeMethodConfigId_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."ShapeMethodConfigs_ShapeMethodConfigId_seq"', 36, true);


--
-- Name: Transactions_TransactionId_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Transactions_TransactionId_seq"', 204, true);


--
-- Name: UserPromotionUsages_UserPromotionUsageId_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."UserPromotionUsages_UserPromotionUsageId_seq"', 41, true);


--
-- Name: aggregatedcounter aggregatedcounter_key_key; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.aggregatedcounter
    ADD CONSTRAINT aggregatedcounter_key_key UNIQUE (key);


--
-- Name: aggregatedcounter aggregatedcounter_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.aggregatedcounter
    ADD CONSTRAINT aggregatedcounter_pkey PRIMARY KEY (id);


--
-- Name: counter counter_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.counter
    ADD CONSTRAINT counter_pkey PRIMARY KEY (id);


--
-- Name: hash hash_key_field_key; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.hash
    ADD CONSTRAINT hash_key_field_key UNIQUE (key, field);


--
-- Name: hash hash_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.hash
    ADD CONSTRAINT hash_pkey PRIMARY KEY (id);


--
-- Name: job job_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.job
    ADD CONSTRAINT job_pkey PRIMARY KEY (id);


--
-- Name: jobparameter jobparameter_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.jobparameter
    ADD CONSTRAINT jobparameter_pkey PRIMARY KEY (id);


--
-- Name: jobqueue jobqueue_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.jobqueue
    ADD CONSTRAINT jobqueue_pkey PRIMARY KEY (id);


--
-- Name: list list_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.list
    ADD CONSTRAINT list_pkey PRIMARY KEY (id);


--
-- Name: lock lock_resource_key; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.lock
    ADD CONSTRAINT lock_resource_key UNIQUE (resource);

ALTER TABLE ONLY hangfire.lock REPLICA IDENTITY USING INDEX lock_resource_key;


--
-- Name: schema schema_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.schema
    ADD CONSTRAINT schema_pkey PRIMARY KEY (version);


--
-- Name: server server_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.server
    ADD CONSTRAINT server_pkey PRIMARY KEY (id);


--
-- Name: set set_key_value_key; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.set
    ADD CONSTRAINT set_key_value_key UNIQUE (key, value);


--
-- Name: set set_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.set
    ADD CONSTRAINT set_pkey PRIMARY KEY (id);


--
-- Name: state state_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.state
    ADD CONSTRAINT state_pkey PRIMARY KEY (id);


--
-- Name: BookingDiscounts PK_BookingDiscounts; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BookingDiscounts"
    ADD CONSTRAINT "PK_BookingDiscounts" PRIMARY KEY ("BookingDiscountId");


--
-- Name: BookingHistories PK_BookingHistories; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BookingHistories"
    ADD CONSTRAINT "PK_BookingHistories" PRIMARY KEY ("BookingHistoryId");


--
-- Name: BookingItems PK_BookingItems; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BookingItems"
    ADD CONSTRAINT "PK_BookingItems" PRIMARY KEY ("BookingItemId");


--
-- Name: BookingProcedures PK_BookingProcedures; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BookingProcedures"
    ADD CONSTRAINT "PK_BookingProcedures" PRIMARY KEY ("BookingProcedureId");


--
-- Name: BookingRatings PK_BookingRatings; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BookingRatings"
    ADD CONSTRAINT "PK_BookingRatings" PRIMARY KEY ("BookingRatingId");


--
-- Name: BookingWaitlists PK_BookingWaitlists; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BookingWaitlists"
    ADD CONSTRAINT "PK_BookingWaitlists" PRIMARY KEY ("WailistId");


--
-- Name: Bookings PK_Bookings; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Bookings"
    ADD CONSTRAINT "PK_Bookings" PRIMARY KEY ("BookingId");


--
-- Name: Categories PK_Categories; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Categories"
    ADD CONSTRAINT "PK_Categories" PRIMARY KEY ("CategoryId");


--
-- Name: CategoryTypes PK_CategoryTypes; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CategoryTypes"
    ADD CONSTRAINT "PK_CategoryTypes" PRIMARY KEY ("CategoryTypeId");


--
-- Name: Chairs PK_Chairs; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Chairs"
    ADD CONSTRAINT "PK_Chairs" PRIMARY KEY ("ChairId");


--
-- Name: Components PK_Components; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Components"
    ADD CONSTRAINT "PK_Components" PRIMARY KEY ("ComponentId");


--
-- Name: CustomerComponents PK_CustomerComponents; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CustomerComponents"
    ADD CONSTRAINT "PK_CustomerComponents" PRIMARY KEY ("CustomerComponentId");


--
-- Name: CustomerNailComponents PK_CustomerNailComponents; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CustomerNailComponents"
    ADD CONSTRAINT "PK_CustomerNailComponents" PRIMARY KEY ("CustomerNailComponentId");


--
-- Name: CustomerNailRequests PK_CustomerNailRequests; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CustomerNailRequests"
    ADD CONSTRAINT "PK_CustomerNailRequests" PRIMARY KEY ("CustomerNailRequestId");


--
-- Name: CustomerNails PK_CustomerNails; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CustomerNails"
    ADD CONSTRAINT "PK_CustomerNails" PRIMARY KEY ("CustomerNailId");


--
-- Name: CustomerQuizAnswers PK_CustomerQuizAnswers; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CustomerQuizAnswers"
    ADD CONSTRAINT "PK_CustomerQuizAnswers" PRIMARY KEY ("CustomerQuizAnswerId");


--
-- Name: Customers PK_Customers; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Customers"
    ADD CONSTRAINT "PK_Customers" PRIMARY KEY ("UserId");


--
-- Name: FavoriteNails PK_FavoriteNails; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."FavoriteNails"
    ADD CONSTRAINT "PK_FavoriteNails" PRIMARY KEY ("FavoriteNailId");


--
-- Name: LoyaltyTiers PK_LoyaltyTiers; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."LoyaltyTiers"
    ADD CONSTRAINT "PK_LoyaltyTiers" PRIMARY KEY ("LoyaltyTierId");


--
-- Name: LoyaltyTransactions PK_LoyaltyTransactions; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."LoyaltyTransactions"
    ADD CONSTRAINT "PK_LoyaltyTransactions" PRIMARY KEY ("LoyaltyTransactionId");


--
-- Name: NailArtistBreaks PK_NailArtistBreaks; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NailArtistBreaks"
    ADD CONSTRAINT "PK_NailArtistBreaks" PRIMARY KEY ("NailArtistBreakId");


--
-- Name: NailArtistSkills PK_NailArtistSkills; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NailArtistSkills"
    ADD CONSTRAINT "PK_NailArtistSkills" PRIMARY KEY ("NailArtistSkillId");


--
-- Name: NailArtists PK_NailArtists; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NailArtists"
    ADD CONSTRAINT "PK_NailArtists" PRIMARY KEY ("NailArtistId");


--
-- Name: NailCategories PK_NailCategories; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NailCategories"
    ADD CONSTRAINT "PK_NailCategories" PRIMARY KEY ("NailCategoryId");


--
-- Name: NailComponents PK_NailComponents; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NailComponents"
    ADD CONSTRAINT "PK_NailComponents" PRIMARY KEY ("NailComponentId");


--
-- Name: NailDesigns PK_NailDesigns; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NailDesigns"
    ADD CONSTRAINT "PK_NailDesigns" PRIMARY KEY ("NailDesignId");


--
-- Name: NailProcedures PK_NailProcedures; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NailProcedures"
    ADD CONSTRAINT "PK_NailProcedures" PRIMARY KEY ("NailProcedureId");


--
-- Name: NailRequiredSkills PK_NailRequiredSkills; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NailRequiredSkills"
    ADD CONSTRAINT "PK_NailRequiredSkills" PRIMARY KEY ("NailRequiredSkillId");


--
-- Name: NailShapes PK_NailShapes; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NailShapes"
    ADD CONSTRAINT "PK_NailShapes" PRIMARY KEY ("NailShapeId");


--
-- Name: NailSurfaces PK_NailSurfaces; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NailSurfaces"
    ADD CONSTRAINT "PK_NailSurfaces" PRIMARY KEY ("NailSurfaceId");


--
-- Name: NailVariants PK_NailVariants; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NailVariants"
    ADD CONSTRAINT "PK_NailVariants" PRIMARY KEY ("NailVariantId");


--
-- Name: Procedures PK_Procedures; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Procedures"
    ADD CONSTRAINT "PK_Procedures" PRIMARY KEY ("ProcedureId");


--
-- Name: Promotions PK_Promotions; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Promotions"
    ADD CONSTRAINT "PK_Promotions" PRIMARY KEY ("PromotionId");


--
-- Name: QuizOptions PK_QuizOptions; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."QuizOptions"
    ADD CONSTRAINT "PK_QuizOptions" PRIMARY KEY ("QuizOptionId");


--
-- Name: QuizQuestions PK_QuizQuestions; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."QuizQuestions"
    ADD CONSTRAINT "PK_QuizQuestions" PRIMARY KEY ("QuizQuestionId");


--
-- Name: SalonOffDates PK_SalonOffDates; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SalonOffDates"
    ADD CONSTRAINT "PK_SalonOffDates" PRIMARY KEY ("SalonOffDateId");


--
-- Name: SalonOperatingHours PK_SalonOperatingHours; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SalonOperatingHours"
    ADD CONSTRAINT "PK_SalonOperatingHours" PRIMARY KEY ("OperatingHourId");


--
-- Name: Salons PK_Salons; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Salons"
    ADD CONSTRAINT "PK_Salons" PRIMARY KEY ("SalonId");


--
-- Name: Schedules PK_Schedules; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Schedules"
    ADD CONSTRAINT "PK_Schedules" PRIMARY KEY ("ScheduleId");


--
-- Name: Services PK_Services; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Services"
    ADD CONSTRAINT "PK_Services" PRIMARY KEY ("ServiceId");


--
-- Name: ShapeMethodConfigs PK_ShapeMethodConfigs; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ShapeMethodConfigs"
    ADD CONSTRAINT "PK_ShapeMethodConfigs" PRIMARY KEY ("ShapeMethodConfigId");


--
-- Name: SkillTypes PK_SkillTypes; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SkillTypes"
    ADD CONSTRAINT "PK_SkillTypes" PRIMARY KEY ("SkillTypeId");


--
-- Name: StaffTransfers PK_StaffTransfers; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."StaffTransfers"
    ADD CONSTRAINT "PK_StaffTransfers" PRIMARY KEY ("StaffTransferId");


--
-- Name: Transactions PK_Transactions; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Transactions"
    ADD CONSTRAINT "PK_Transactions" PRIMARY KEY ("TransactionId");


--
-- Name: UserPromotionUsages PK_UserPromotionUsages; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."UserPromotionUsages"
    ADD CONSTRAINT "PK_UserPromotionUsages" PRIMARY KEY ("UserPromotionUsageId");


--
-- Name: Users PK_Users; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Users"
    ADD CONSTRAINT "PK_Users" PRIMARY KEY ("UserId");


--
-- Name: WaitlistItems PK_WaitlistItems; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."WaitlistItems"
    ADD CONSTRAINT "PK_WaitlistItems" PRIMARY KEY ("WaitlistItemId");


--
-- Name: WalkInQueues PK_WalkInQueues; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."WalkInQueues"
    ADD CONSTRAINT "PK_WalkInQueues" PRIMARY KEY ("QueueId");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: Categories UQ_Categories_CategoryTypeId_Name; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Categories"
    ADD CONSTRAINT "UQ_Categories_CategoryTypeId_Name" UNIQUE ("CategoryTypeId", "Name");


--
-- Name: ix_hangfire_counter_expireat; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_counter_expireat ON hangfire.counter USING btree (expireat);


--
-- Name: ix_hangfire_counter_key; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_counter_key ON hangfire.counter USING btree (key);


--
-- Name: ix_hangfire_hash_expireat; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_hash_expireat ON hangfire.hash USING btree (expireat);


--
-- Name: ix_hangfire_job_expireat; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_job_expireat ON hangfire.job USING btree (expireat);


--
-- Name: ix_hangfire_job_statename; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_job_statename ON hangfire.job USING btree (statename);


--
-- Name: ix_hangfire_job_statename_is_not_null; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_job_statename_is_not_null ON hangfire.job USING btree (statename) INCLUDE (id) WHERE (statename IS NOT NULL);


--
-- Name: ix_hangfire_jobparameter_jobidandname; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_jobparameter_jobidandname ON hangfire.jobparameter USING btree (jobid, name);


--
-- Name: ix_hangfire_jobqueue_fetchedat_queue_jobid; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_jobqueue_fetchedat_queue_jobid ON hangfire.jobqueue USING btree (fetchedat NULLS FIRST, queue, jobid);


--
-- Name: ix_hangfire_jobqueue_jobidandqueue; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_jobqueue_jobidandqueue ON hangfire.jobqueue USING btree (jobid, queue);


--
-- Name: ix_hangfire_jobqueue_queueandfetchedat; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_jobqueue_queueandfetchedat ON hangfire.jobqueue USING btree (queue, fetchedat);


--
-- Name: ix_hangfire_list_expireat; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_list_expireat ON hangfire.list USING btree (expireat);


--
-- Name: ix_hangfire_set_expireat; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_set_expireat ON hangfire.set USING btree (expireat);


--
-- Name: ix_hangfire_set_key_score; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_set_key_score ON hangfire.set USING btree (key, score);


--
-- Name: ix_hangfire_state_jobid; Type: INDEX; Schema: hangfire; Owner: postgres
--

CREATE INDEX ix_hangfire_state_jobid ON hangfire.state USING btree (jobid);


--
-- Name: IX_BookingDiscounts_BookingId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_BookingDiscounts_BookingId" ON public."BookingDiscounts" USING btree ("BookingId");


--
-- Name: IX_BookingDiscounts_LoyaltyTierId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_BookingDiscounts_LoyaltyTierId" ON public."BookingDiscounts" USING btree ("LoyaltyTierId");


--
-- Name: IX_BookingDiscounts_LoyaltyTransactionId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_BookingDiscounts_LoyaltyTransactionId" ON public."BookingDiscounts" USING btree ("LoyaltyTransactionId");


--
-- Name: IX_BookingDiscounts_PromotionId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_BookingDiscounts_PromotionId" ON public."BookingDiscounts" USING btree ("PromotionId");


--
-- Name: IX_BookingHistories_ActorId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_BookingHistories_ActorId" ON public."BookingHistories" USING btree ("ActorId");


--
-- Name: IX_BookingHistories_BookingId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_BookingHistories_BookingId" ON public."BookingHistories" USING btree ("BookingId");


--
-- Name: IX_BookingItems_BookingId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_BookingItems_BookingId" ON public."BookingItems" USING btree ("BookingId");


--
-- Name: IX_BookingItems_CustomerNailRequestId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_BookingItems_CustomerNailRequestId" ON public."BookingItems" USING btree ("CustomerNailRequestId");


--
-- Name: IX_BookingItems_NailVariantId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_BookingItems_NailVariantId" ON public."BookingItems" USING btree ("NailVariantId");


--
-- Name: IX_BookingItems_ServiceId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_BookingItems_ServiceId" ON public."BookingItems" USING btree ("ServiceId");


--
-- Name: IX_BookingItems_ShapeMethodConfigId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_BookingItems_ShapeMethodConfigId" ON public."BookingItems" USING btree ("ShapeMethodConfigId");


--
-- Name: IX_BookingProcedures_AssignedArtistId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_BookingProcedures_AssignedArtistId" ON public."BookingProcedures" USING btree ("AssignedArtistId");


--
-- Name: IX_BookingProcedures_BookingItemId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_BookingProcedures_BookingItemId" ON public."BookingProcedures" USING btree ("BookingItemId");


--
-- Name: IX_BookingProcedures_CompletedById; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_BookingProcedures_CompletedById" ON public."BookingProcedures" USING btree ("CompletedById");


--
-- Name: IX_BookingProcedures_ProcedureId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_BookingProcedures_ProcedureId" ON public."BookingProcedures" USING btree ("ProcedureId");


--
-- Name: IX_BookingRatings_BookingId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX "IX_BookingRatings_BookingId" ON public."BookingRatings" USING btree ("BookingId");


--
-- Name: IX_BookingRatings_CreatedAt; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_BookingRatings_CreatedAt" ON public."BookingRatings" USING btree ("CreatedAt");


--
-- Name: IX_BookingRatings_CustomerId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_BookingRatings_CustomerId" ON public."BookingRatings" USING btree ("CustomerId");


--
-- Name: IX_BookingWaitlists_ConvertedBookingId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_BookingWaitlists_ConvertedBookingId" ON public."BookingWaitlists" USING btree ("ConvertedBookingId");


--
-- Name: IX_BookingWaitlists_CustomerId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_BookingWaitlists_CustomerId" ON public."BookingWaitlists" USING btree ("CustomerId");


--
-- Name: IX_BookingWaitlists_PreferredNailArtistId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_BookingWaitlists_PreferredNailArtistId" ON public."BookingWaitlists" USING btree ("PreferredNailArtistId");


--
-- Name: IX_BookingWaitlists_SalonId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_BookingWaitlists_SalonId" ON public."BookingWaitlists" USING btree ("SalonId");


--
-- Name: IX_Bookings_ChairId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Bookings_ChairId" ON public."Bookings" USING btree ("ChairId");


--
-- Name: IX_Bookings_CustomerId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Bookings_CustomerId" ON public."Bookings" USING btree ("CustomerId");


--
-- Name: IX_Bookings_NailArtistId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Bookings_NailArtistId" ON public."Bookings" USING btree ("NailArtistId");


--
-- Name: IX_Bookings_SalonId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Bookings_SalonId" ON public."Bookings" USING btree ("SalonId");


--
-- Name: IX_Bookings_WarrantyForBookingId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Bookings_WarrantyForBookingId" ON public."Bookings" USING btree ("WarrantyForBookingId");


--
-- Name: IX_Categories_CategoryTypeId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Categories_CategoryTypeId" ON public."Categories" USING btree ("CategoryTypeId");


--
-- Name: IX_Chairs_SalonId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Chairs_SalonId" ON public."Chairs" USING btree ("SalonId");


--
-- Name: IX_CustomerComponents_UserId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_CustomerComponents_UserId" ON public."CustomerComponents" USING btree ("UserId");


--
-- Name: IX_CustomerNailComponents_ComponentId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_CustomerNailComponents_ComponentId" ON public."CustomerNailComponents" USING btree ("ComponentId");


--
-- Name: IX_CustomerNailComponents_CustomerComponentId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_CustomerNailComponents_CustomerComponentId" ON public."CustomerNailComponents" USING btree ("CustomerComponentId");


--
-- Name: IX_CustomerNailComponents_CustomerNailId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_CustomerNailComponents_CustomerNailId" ON public."CustomerNailComponents" USING btree ("CustomerNailId");


--
-- Name: IX_CustomerNailRequests_ApprovedArtistId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_CustomerNailRequests_ApprovedArtistId" ON public."CustomerNailRequests" USING btree ("ApprovedArtistId");


--
-- Name: IX_CustomerNailRequests_CustomerNailId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_CustomerNailRequests_CustomerNailId" ON public."CustomerNailRequests" USING btree ("CustomerNailId");


--
-- Name: IX_CustomerNailRequests_SalonId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_CustomerNailRequests_SalonId" ON public."CustomerNailRequests" USING btree ("SalonId");


--
-- Name: IX_CustomerNails_NailShapeId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_CustomerNails_NailShapeId" ON public."CustomerNails" USING btree ("NailShapeId");


--
-- Name: IX_CustomerNails_NailSurfaceId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_CustomerNails_NailSurfaceId" ON public."CustomerNails" USING btree ("NailSurfaceId");


--
-- Name: IX_CustomerNails_UserId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_CustomerNails_UserId" ON public."CustomerNails" USING btree ("UserId");


--
-- Name: IX_CustomerQuizAnswers_CustomerId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_CustomerQuizAnswers_CustomerId" ON public."CustomerQuizAnswers" USING btree ("CustomerId");


--
-- Name: IX_CustomerQuizAnswers_QuizOptionId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_CustomerQuizAnswers_QuizOptionId" ON public."CustomerQuizAnswers" USING btree ("QuizOptionId");


--
-- Name: IX_CustomerQuizAnswers_QuizQuestionId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_CustomerQuizAnswers_QuizQuestionId" ON public."CustomerQuizAnswers" USING btree ("QuizQuestionId");


--
-- Name: IX_Customers_LoyaltyTierId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Customers_LoyaltyTierId" ON public."Customers" USING btree ("LoyaltyTierId");


--
-- Name: IX_FavoriteNails_NailDesignId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_FavoriteNails_NailDesignId" ON public."FavoriteNails" USING btree ("NailDesignId");


--
-- Name: IX_FavoriteNails_NailVariantId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_FavoriteNails_NailVariantId" ON public."FavoriteNails" USING btree ("NailVariantId");


--
-- Name: IX_FavoriteNails_UserId_NailDesignId_NailVariantId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX "IX_FavoriteNails_UserId_NailDesignId_NailVariantId" ON public."FavoriteNails" USING btree ("UserId", "NailDesignId", "NailVariantId");


--
-- Name: IX_LoyaltyTransactions_BookingId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_LoyaltyTransactions_BookingId" ON public."LoyaltyTransactions" USING btree ("BookingId");


--
-- Name: IX_LoyaltyTransactions_CustomerId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_LoyaltyTransactions_CustomerId" ON public."LoyaltyTransactions" USING btree ("CustomerId");


--
-- Name: IX_NailArtistBreaks_NailArtistId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_NailArtistBreaks_NailArtistId" ON public."NailArtistBreaks" USING btree ("NailArtistId");


--
-- Name: IX_NailArtistSkills_NailArtistId_SkillTypeId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX "IX_NailArtistSkills_NailArtistId_SkillTypeId" ON public."NailArtistSkills" USING btree ("NailArtistId", "SkillTypeId");


--
-- Name: IX_NailArtistSkills_SkillTypeId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_NailArtistSkills_SkillTypeId" ON public."NailArtistSkills" USING btree ("SkillTypeId");


--
-- Name: IX_NailArtists_AccountId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX "IX_NailArtists_AccountId" ON public."NailArtists" USING btree ("AccountId");


--
-- Name: IX_NailCategories_CategoryId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_NailCategories_CategoryId" ON public."NailCategories" USING btree ("CategoryId");


--
-- Name: IX_NailCategories_NailDesignId_CategoryId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX "IX_NailCategories_NailDesignId_CategoryId" ON public."NailCategories" USING btree ("NailDesignId", "CategoryId");


--
-- Name: IX_NailComponents_ComponentId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_NailComponents_ComponentId" ON public."NailComponents" USING btree ("ComponentId");


--
-- Name: IX_NailComponents_NailVariantId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_NailComponents_NailVariantId" ON public."NailComponents" USING btree ("NailVariantId");


--
-- Name: IX_NailProcedures_CustomerNailId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_NailProcedures_CustomerNailId" ON public."NailProcedures" USING btree ("CustomerNailId");


--
-- Name: IX_NailProcedures_NailVariantId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_NailProcedures_NailVariantId" ON public."NailProcedures" USING btree ("NailVariantId");


--
-- Name: IX_NailProcedures_ProcedureId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_NailProcedures_ProcedureId" ON public."NailProcedures" USING btree ("ProcedureId");


--
-- Name: IX_NailRequiredSkills_NailVariantId_SkillTypeId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX "IX_NailRequiredSkills_NailVariantId_SkillTypeId" ON public."NailRequiredSkills" USING btree ("NailVariantId", "SkillTypeId");


--
-- Name: IX_NailRequiredSkills_SkillTypeId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_NailRequiredSkills_SkillTypeId" ON public."NailRequiredSkills" USING btree ("SkillTypeId");


--
-- Name: IX_NailVariants_NailDesignId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_NailVariants_NailDesignId" ON public."NailVariants" USING btree ("NailDesignId");


--
-- Name: IX_NailVariants_NailShapeId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_NailVariants_NailShapeId" ON public."NailVariants" USING btree ("NailShapeId");


--
-- Name: IX_NailVariants_NailSurfaceId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_NailVariants_NailSurfaceId" ON public."NailVariants" USING btree ("NailSurfaceId");


--
-- Name: IX_Promotions_CategoryId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Promotions_CategoryId" ON public."Promotions" USING btree ("CategoryId");


--
-- Name: IX_Promotions_CategoryTypeId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Promotions_CategoryTypeId" ON public."Promotions" USING btree ("CategoryTypeId");


--
-- Name: IX_Promotions_NailDesignId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Promotions_NailDesignId" ON public."Promotions" USING btree ("NailDesignId");


--
-- Name: IX_Promotions_Name; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX "IX_Promotions_Name" ON public."Promotions" USING btree ("Name");


--
-- Name: IX_Promotions_StartDate_EndDate; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Promotions_StartDate_EndDate" ON public."Promotions" USING btree ("StartDate", "EndDate");


--
-- Name: IX_Promotions_Status; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Promotions_Status" ON public."Promotions" USING btree ("Status");


--
-- Name: IX_QuizOptions_QuizQuestionId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_QuizOptions_QuizQuestionId" ON public."QuizOptions" USING btree ("QuizQuestionId");


--
-- Name: IX_SalonOffDates_SalonId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SalonOffDates_SalonId" ON public."SalonOffDates" USING btree ("SalonId");


--
-- Name: IX_SalonOperatingHours_SalonId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SalonOperatingHours_SalonId" ON public."SalonOperatingHours" USING btree ("SalonId");


--
-- Name: IX_Schedules_NailArtistId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Schedules_NailArtistId" ON public."Schedules" USING btree ("NailArtistId");


--
-- Name: IX_ShapeMethodConfigs_NailShapeId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_ShapeMethodConfigs_NailShapeId" ON public."ShapeMethodConfigs" USING btree ("NailShapeId");


--
-- Name: IX_StaffTransfers_FromSalonId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_StaffTransfers_FromSalonId" ON public."StaffTransfers" USING btree ("FromSalonId");


--
-- Name: IX_StaffTransfers_NailArtistId_StartDate_EndDate; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_StaffTransfers_NailArtistId_StartDate_EndDate" ON public."StaffTransfers" USING btree ("NailArtistId", "StartDate", "EndDate");


--
-- Name: IX_StaffTransfers_ToSalonId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_StaffTransfers_ToSalonId" ON public."StaffTransfers" USING btree ("ToSalonId");


--
-- Name: IX_Transactions_BookingId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Transactions_BookingId" ON public."Transactions" USING btree ("BookingId");


--
-- Name: IX_Transactions_OrderCode; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX "IX_Transactions_OrderCode" ON public."Transactions" USING btree ("OrderCode");


--
-- Name: IX_UserPromotionUsages_PromotionId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_UserPromotionUsages_PromotionId" ON public."UserPromotionUsages" USING btree ("PromotionId");


--
-- Name: IX_UserPromotionUsages_UserId_PromotionId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX "IX_UserPromotionUsages_UserId_PromotionId" ON public."UserPromotionUsages" USING btree ("UserId", "PromotionId");


--
-- Name: IX_Users_SalonId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Users_SalonId" ON public."Users" USING btree ("SalonId");


--
-- Name: IX_WaitlistItems_CustomerNailId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_WaitlistItems_CustomerNailId" ON public."WaitlistItems" USING btree ("CustomerNailId");


--
-- Name: IX_WaitlistItems_NailVariantId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_WaitlistItems_NailVariantId" ON public."WaitlistItems" USING btree ("NailVariantId");


--
-- Name: IX_WaitlistItems_ServiceId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_WaitlistItems_ServiceId" ON public."WaitlistItems" USING btree ("ServiceId");


--
-- Name: IX_WaitlistItems_WaitlistId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_WaitlistItems_WaitlistId" ON public."WaitlistItems" USING btree ("WaitlistId");


--
-- Name: IX_WalkInQueues_AssignedNailArtistId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_WalkInQueues_AssignedNailArtistId" ON public."WalkInQueues" USING btree ("AssignedNailArtistId");


--
-- Name: IX_WalkInQueues_ChairId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_WalkInQueues_ChairId" ON public."WalkInQueues" USING btree ("ChairId");


--
-- Name: IX_WalkInQueues_CustomerId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_WalkInQueues_CustomerId" ON public."WalkInQueues" USING btree ("CustomerId");


--
-- Name: IX_WalkInQueues_OriginalBookingId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_WalkInQueues_OriginalBookingId" ON public."WalkInQueues" USING btree ("OriginalBookingId");


--
-- Name: IX_WalkInQueues_SalonId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_WalkInQueues_SalonId" ON public."WalkInQueues" USING btree ("SalonId");


--
-- Name: jobparameter jobparameter_jobid_fkey; Type: FK CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.jobparameter
    ADD CONSTRAINT jobparameter_jobid_fkey FOREIGN KEY (jobid) REFERENCES hangfire.job(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: state state_jobid_fkey; Type: FK CONSTRAINT; Schema: hangfire; Owner: postgres
--

ALTER TABLE ONLY hangfire.state
    ADD CONSTRAINT state_jobid_fkey FOREIGN KEY (jobid) REFERENCES hangfire.job(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: BookingDiscounts FK_BookingDiscounts_Bookings_BookingId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BookingDiscounts"
    ADD CONSTRAINT "FK_BookingDiscounts_Bookings_BookingId" FOREIGN KEY ("BookingId") REFERENCES public."Bookings"("BookingId") ON DELETE CASCADE;


--
-- Name: BookingDiscounts FK_BookingDiscounts_LoyaltyTiers_LoyaltyTierId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BookingDiscounts"
    ADD CONSTRAINT "FK_BookingDiscounts_LoyaltyTiers_LoyaltyTierId" FOREIGN KEY ("LoyaltyTierId") REFERENCES public."LoyaltyTiers"("LoyaltyTierId") ON DELETE SET NULL;


--
-- Name: BookingDiscounts FK_BookingDiscounts_LoyaltyTransactions_LoyaltyTransactionId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BookingDiscounts"
    ADD CONSTRAINT "FK_BookingDiscounts_LoyaltyTransactions_LoyaltyTransactionId" FOREIGN KEY ("LoyaltyTransactionId") REFERENCES public."LoyaltyTransactions"("LoyaltyTransactionId") ON DELETE SET NULL;


--
-- Name: BookingDiscounts FK_BookingDiscounts_Promotions_PromotionId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BookingDiscounts"
    ADD CONSTRAINT "FK_BookingDiscounts_Promotions_PromotionId" FOREIGN KEY ("PromotionId") REFERENCES public."Promotions"("PromotionId") ON DELETE SET NULL;


--
-- Name: BookingHistories FK_BookingHistories_Bookings_BookingId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BookingHistories"
    ADD CONSTRAINT "FK_BookingHistories_Bookings_BookingId" FOREIGN KEY ("BookingId") REFERENCES public."Bookings"("BookingId") ON DELETE CASCADE;


--
-- Name: BookingHistories FK_BookingHistories_Users_ActorId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BookingHistories"
    ADD CONSTRAINT "FK_BookingHistories_Users_ActorId" FOREIGN KEY ("ActorId") REFERENCES public."Users"("UserId");


--
-- Name: BookingItems FK_BookingItems_Bookings_BookingId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BookingItems"
    ADD CONSTRAINT "FK_BookingItems_Bookings_BookingId" FOREIGN KEY ("BookingId") REFERENCES public."Bookings"("BookingId") ON DELETE CASCADE;


--
-- Name: BookingItems FK_BookingItems_CustomerNailRequests_CustomerNailRequestId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BookingItems"
    ADD CONSTRAINT "FK_BookingItems_CustomerNailRequests_CustomerNailRequestId" FOREIGN KEY ("CustomerNailRequestId") REFERENCES public."CustomerNailRequests"("CustomerNailRequestId") ON DELETE SET NULL;


--
-- Name: BookingItems FK_BookingItems_NailVariants_NailVariantId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BookingItems"
    ADD CONSTRAINT "FK_BookingItems_NailVariants_NailVariantId" FOREIGN KEY ("NailVariantId") REFERENCES public."NailVariants"("NailVariantId") ON DELETE RESTRICT;


--
-- Name: BookingItems FK_BookingItems_Services_ServiceId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BookingItems"
    ADD CONSTRAINT "FK_BookingItems_Services_ServiceId" FOREIGN KEY ("ServiceId") REFERENCES public."Services"("ServiceId") ON DELETE RESTRICT;


--
-- Name: BookingItems FK_BookingItems_ShapeMethodConfigs_ShapeMethodConfigId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BookingItems"
    ADD CONSTRAINT "FK_BookingItems_ShapeMethodConfigs_ShapeMethodConfigId" FOREIGN KEY ("ShapeMethodConfigId") REFERENCES public."ShapeMethodConfigs"("ShapeMethodConfigId") ON DELETE RESTRICT;


--
-- Name: BookingProcedures FK_BookingProcedures_BookingItems_BookingItemId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BookingProcedures"
    ADD CONSTRAINT "FK_BookingProcedures_BookingItems_BookingItemId" FOREIGN KEY ("BookingItemId") REFERENCES public."BookingItems"("BookingItemId") ON DELETE CASCADE;


--
-- Name: BookingProcedures FK_BookingProcedures_NailArtists_AssignedArtistId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BookingProcedures"
    ADD CONSTRAINT "FK_BookingProcedures_NailArtists_AssignedArtistId" FOREIGN KEY ("AssignedArtistId") REFERENCES public."NailArtists"("NailArtistId") ON DELETE RESTRICT;


--
-- Name: BookingProcedures FK_BookingProcedures_NailArtists_CompletedById; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BookingProcedures"
    ADD CONSTRAINT "FK_BookingProcedures_NailArtists_CompletedById" FOREIGN KEY ("CompletedById") REFERENCES public."NailArtists"("NailArtistId") ON DELETE RESTRICT;


--
-- Name: BookingProcedures FK_BookingProcedures_Procedures_ProcedureId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BookingProcedures"
    ADD CONSTRAINT "FK_BookingProcedures_Procedures_ProcedureId" FOREIGN KEY ("ProcedureId") REFERENCES public."Procedures"("ProcedureId") ON DELETE SET NULL;


--
-- Name: BookingRatings FK_BookingRatings_Bookings_BookingId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BookingRatings"
    ADD CONSTRAINT "FK_BookingRatings_Bookings_BookingId" FOREIGN KEY ("BookingId") REFERENCES public."Bookings"("BookingId") ON DELETE CASCADE;


--
-- Name: BookingRatings FK_BookingRatings_Customers_CustomerId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BookingRatings"
    ADD CONSTRAINT "FK_BookingRatings_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES public."Customers"("UserId") ON DELETE RESTRICT;


--
-- Name: BookingWaitlists FK_BookingWaitlists_Bookings_ConvertedBookingId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BookingWaitlists"
    ADD CONSTRAINT "FK_BookingWaitlists_Bookings_ConvertedBookingId" FOREIGN KEY ("ConvertedBookingId") REFERENCES public."Bookings"("BookingId") ON DELETE SET NULL;


--
-- Name: BookingWaitlists FK_BookingWaitlists_Customers_CustomerId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BookingWaitlists"
    ADD CONSTRAINT "FK_BookingWaitlists_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES public."Customers"("UserId") ON DELETE CASCADE;


--
-- Name: BookingWaitlists FK_BookingWaitlists_NailArtists_PreferredNailArtistId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BookingWaitlists"
    ADD CONSTRAINT "FK_BookingWaitlists_NailArtists_PreferredNailArtistId" FOREIGN KEY ("PreferredNailArtistId") REFERENCES public."NailArtists"("NailArtistId") ON DELETE SET NULL;


--
-- Name: BookingWaitlists FK_BookingWaitlists_Salons_SalonId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BookingWaitlists"
    ADD CONSTRAINT "FK_BookingWaitlists_Salons_SalonId" FOREIGN KEY ("SalonId") REFERENCES public."Salons"("SalonId") ON DELETE RESTRICT;


--
-- Name: Bookings FK_Bookings_Bookings_WarrantyForBookingId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Bookings"
    ADD CONSTRAINT "FK_Bookings_Bookings_WarrantyForBookingId" FOREIGN KEY ("WarrantyForBookingId") REFERENCES public."Bookings"("BookingId") ON DELETE RESTRICT;


--
-- Name: Bookings FK_Bookings_Chairs_ChairId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Bookings"
    ADD CONSTRAINT "FK_Bookings_Chairs_ChairId" FOREIGN KEY ("ChairId") REFERENCES public."Chairs"("ChairId") ON DELETE RESTRICT;


--
-- Name: Bookings FK_Bookings_Customers_CustomerId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Bookings"
    ADD CONSTRAINT "FK_Bookings_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES public."Customers"("UserId") ON DELETE CASCADE;


--
-- Name: Bookings FK_Bookings_NailArtists_NailArtistId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Bookings"
    ADD CONSTRAINT "FK_Bookings_NailArtists_NailArtistId" FOREIGN KEY ("NailArtistId") REFERENCES public."NailArtists"("NailArtistId");


--
-- Name: Bookings FK_Bookings_Salons_SalonId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Bookings"
    ADD CONSTRAINT "FK_Bookings_Salons_SalonId" FOREIGN KEY ("SalonId") REFERENCES public."Salons"("SalonId") ON DELETE CASCADE;


--
-- Name: Categories FK_Categories_CategoryTypes_CategoryTypeId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Categories"
    ADD CONSTRAINT "FK_Categories_CategoryTypes_CategoryTypeId" FOREIGN KEY ("CategoryTypeId") REFERENCES public."CategoryTypes"("CategoryTypeId") ON DELETE CASCADE;


--
-- Name: Chairs FK_Chairs_Salons_SalonId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Chairs"
    ADD CONSTRAINT "FK_Chairs_Salons_SalonId" FOREIGN KEY ("SalonId") REFERENCES public."Salons"("SalonId") ON DELETE CASCADE;


--
-- Name: CustomerComponents FK_CustomerComponents_Users_UserId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CustomerComponents"
    ADD CONSTRAINT "FK_CustomerComponents_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("UserId") ON DELETE RESTRICT;


--
-- Name: CustomerNailComponents FK_CustomerNailComponents_Components_ComponentId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CustomerNailComponents"
    ADD CONSTRAINT "FK_CustomerNailComponents_Components_ComponentId" FOREIGN KEY ("ComponentId") REFERENCES public."Components"("ComponentId") ON DELETE RESTRICT;


--
-- Name: CustomerNailComponents FK_CustomerNailComponents_CustomerComponents_CustomerComponent~; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CustomerNailComponents"
    ADD CONSTRAINT "FK_CustomerNailComponents_CustomerComponents_CustomerComponent~" FOREIGN KEY ("CustomerComponentId") REFERENCES public."CustomerComponents"("CustomerComponentId") ON DELETE RESTRICT;


--
-- Name: CustomerNailComponents FK_CustomerNailComponents_CustomerNails_CustomerNailId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CustomerNailComponents"
    ADD CONSTRAINT "FK_CustomerNailComponents_CustomerNails_CustomerNailId" FOREIGN KEY ("CustomerNailId") REFERENCES public."CustomerNails"("CustomerNailId") ON DELETE CASCADE;


--
-- Name: CustomerNailRequests FK_CustomerNailRequests_CustomerNails_CustomerNailId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CustomerNailRequests"
    ADD CONSTRAINT "FK_CustomerNailRequests_CustomerNails_CustomerNailId" FOREIGN KEY ("CustomerNailId") REFERENCES public."CustomerNails"("CustomerNailId") ON DELETE CASCADE;


--
-- Name: CustomerNailRequests FK_CustomerNailRequests_NailArtists_ApprovedArtistId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CustomerNailRequests"
    ADD CONSTRAINT "FK_CustomerNailRequests_NailArtists_ApprovedArtistId" FOREIGN KEY ("ApprovedArtistId") REFERENCES public."NailArtists"("NailArtistId") ON DELETE RESTRICT;


--
-- Name: CustomerNailRequests FK_CustomerNailRequests_Salons_SalonId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CustomerNailRequests"
    ADD CONSTRAINT "FK_CustomerNailRequests_Salons_SalonId" FOREIGN KEY ("SalonId") REFERENCES public."Salons"("SalonId") ON DELETE RESTRICT;


--
-- Name: CustomerNails FK_CustomerNails_NailShapes_NailShapeId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CustomerNails"
    ADD CONSTRAINT "FK_CustomerNails_NailShapes_NailShapeId" FOREIGN KEY ("NailShapeId") REFERENCES public."NailShapes"("NailShapeId") ON DELETE RESTRICT;


--
-- Name: CustomerNails FK_CustomerNails_NailSurfaces_NailSurfaceId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CustomerNails"
    ADD CONSTRAINT "FK_CustomerNails_NailSurfaces_NailSurfaceId" FOREIGN KEY ("NailSurfaceId") REFERENCES public."NailSurfaces"("NailSurfaceId") ON DELETE RESTRICT;


--
-- Name: CustomerNails FK_CustomerNails_Users_UserId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CustomerNails"
    ADD CONSTRAINT "FK_CustomerNails_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("UserId") ON DELETE RESTRICT;


--
-- Name: CustomerQuizAnswers FK_CustomerQuizAnswers_Customers_CustomerId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CustomerQuizAnswers"
    ADD CONSTRAINT "FK_CustomerQuizAnswers_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES public."Customers"("UserId") ON DELETE CASCADE;


--
-- Name: CustomerQuizAnswers FK_CustomerQuizAnswers_QuizOptions_QuizOptionId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CustomerQuizAnswers"
    ADD CONSTRAINT "FK_CustomerQuizAnswers_QuizOptions_QuizOptionId" FOREIGN KEY ("QuizOptionId") REFERENCES public."QuizOptions"("QuizOptionId") ON DELETE RESTRICT;


--
-- Name: CustomerQuizAnswers FK_CustomerQuizAnswers_QuizQuestions_QuizQuestionId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CustomerQuizAnswers"
    ADD CONSTRAINT "FK_CustomerQuizAnswers_QuizQuestions_QuizQuestionId" FOREIGN KEY ("QuizQuestionId") REFERENCES public."QuizQuestions"("QuizQuestionId") ON DELETE RESTRICT;


--
-- Name: Customers FK_Customers_LoyaltyTiers_LoyaltyTierId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Customers"
    ADD CONSTRAINT "FK_Customers_LoyaltyTiers_LoyaltyTierId" FOREIGN KEY ("LoyaltyTierId") REFERENCES public."LoyaltyTiers"("LoyaltyTierId") ON DELETE SET NULL;


--
-- Name: Customers FK_Customers_Users_UserId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Customers"
    ADD CONSTRAINT "FK_Customers_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("UserId") ON DELETE CASCADE;


--
-- Name: FavoriteNails FK_FavoriteNails_NailDesigns_NailDesignId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."FavoriteNails"
    ADD CONSTRAINT "FK_FavoriteNails_NailDesigns_NailDesignId" FOREIGN KEY ("NailDesignId") REFERENCES public."NailDesigns"("NailDesignId") ON DELETE CASCADE;


--
-- Name: FavoriteNails FK_FavoriteNails_NailVariants_NailVariantId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."FavoriteNails"
    ADD CONSTRAINT "FK_FavoriteNails_NailVariants_NailVariantId" FOREIGN KEY ("NailVariantId") REFERENCES public."NailVariants"("NailVariantId") ON DELETE CASCADE;


--
-- Name: FavoriteNails FK_FavoriteNails_Users_UserId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."FavoriteNails"
    ADD CONSTRAINT "FK_FavoriteNails_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("UserId") ON DELETE CASCADE;


--
-- Name: LoyaltyTransactions FK_LoyaltyTransactions_Bookings_BookingId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."LoyaltyTransactions"
    ADD CONSTRAINT "FK_LoyaltyTransactions_Bookings_BookingId" FOREIGN KEY ("BookingId") REFERENCES public."Bookings"("BookingId") ON DELETE RESTRICT;


--
-- Name: LoyaltyTransactions FK_LoyaltyTransactions_Customers_CustomerId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."LoyaltyTransactions"
    ADD CONSTRAINT "FK_LoyaltyTransactions_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES public."Customers"("UserId") ON DELETE CASCADE;


--
-- Name: NailArtistBreaks FK_NailArtistBreaks_NailArtists_NailArtistId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NailArtistBreaks"
    ADD CONSTRAINT "FK_NailArtistBreaks_NailArtists_NailArtistId" FOREIGN KEY ("NailArtistId") REFERENCES public."NailArtists"("NailArtistId") ON DELETE CASCADE;


--
-- Name: NailArtistSkills FK_NailArtistSkills_NailArtists_NailArtistId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NailArtistSkills"
    ADD CONSTRAINT "FK_NailArtistSkills_NailArtists_NailArtistId" FOREIGN KEY ("NailArtistId") REFERENCES public."NailArtists"("NailArtistId") ON DELETE CASCADE;


--
-- Name: NailArtistSkills FK_NailArtistSkills_SkillTypes_SkillTypeId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NailArtistSkills"
    ADD CONSTRAINT "FK_NailArtistSkills_SkillTypes_SkillTypeId" FOREIGN KEY ("SkillTypeId") REFERENCES public."SkillTypes"("SkillTypeId") ON DELETE RESTRICT;


--
-- Name: NailArtists FK_NailArtists_Users_AccountId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NailArtists"
    ADD CONSTRAINT "FK_NailArtists_Users_AccountId" FOREIGN KEY ("AccountId") REFERENCES public."Users"("UserId") ON DELETE RESTRICT;


--
-- Name: NailCategories FK_NailCategories_Categories_CategoryId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NailCategories"
    ADD CONSTRAINT "FK_NailCategories_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES public."Categories"("CategoryId") ON DELETE RESTRICT;


--
-- Name: NailCategories FK_NailCategories_NailDesigns_NailDesignId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NailCategories"
    ADD CONSTRAINT "FK_NailCategories_NailDesigns_NailDesignId" FOREIGN KEY ("NailDesignId") REFERENCES public."NailDesigns"("NailDesignId") ON DELETE CASCADE;


--
-- Name: NailComponents FK_NailComponents_Components_ComponentId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NailComponents"
    ADD CONSTRAINT "FK_NailComponents_Components_ComponentId" FOREIGN KEY ("ComponentId") REFERENCES public."Components"("ComponentId") ON DELETE RESTRICT;


--
-- Name: NailComponents FK_NailComponents_NailVariants_NailVariantId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NailComponents"
    ADD CONSTRAINT "FK_NailComponents_NailVariants_NailVariantId" FOREIGN KEY ("NailVariantId") REFERENCES public."NailVariants"("NailVariantId") ON DELETE CASCADE;


--
-- Name: NailProcedures FK_NailProcedures_CustomerNails_CustomerNailId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NailProcedures"
    ADD CONSTRAINT "FK_NailProcedures_CustomerNails_CustomerNailId" FOREIGN KEY ("CustomerNailId") REFERENCES public."CustomerNails"("CustomerNailId") ON DELETE CASCADE;


--
-- Name: NailProcedures FK_NailProcedures_NailVariants_NailVariantId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NailProcedures"
    ADD CONSTRAINT "FK_NailProcedures_NailVariants_NailVariantId" FOREIGN KEY ("NailVariantId") REFERENCES public."NailVariants"("NailVariantId") ON DELETE CASCADE;


--
-- Name: NailProcedures FK_NailProcedures_Procedures_ProcedureId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NailProcedures"
    ADD CONSTRAINT "FK_NailProcedures_Procedures_ProcedureId" FOREIGN KEY ("ProcedureId") REFERENCES public."Procedures"("ProcedureId") ON DELETE RESTRICT;


--
-- Name: NailRequiredSkills FK_NailRequiredSkills_NailVariants_NailVariantId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NailRequiredSkills"
    ADD CONSTRAINT "FK_NailRequiredSkills_NailVariants_NailVariantId" FOREIGN KEY ("NailVariantId") REFERENCES public."NailVariants"("NailVariantId") ON DELETE CASCADE;


--
-- Name: NailRequiredSkills FK_NailRequiredSkills_SkillTypes_SkillTypeId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NailRequiredSkills"
    ADD CONSTRAINT "FK_NailRequiredSkills_SkillTypes_SkillTypeId" FOREIGN KEY ("SkillTypeId") REFERENCES public."SkillTypes"("SkillTypeId") ON DELETE RESTRICT;


--
-- Name: NailVariants FK_NailVariants_NailDesigns_NailDesignId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NailVariants"
    ADD CONSTRAINT "FK_NailVariants_NailDesigns_NailDesignId" FOREIGN KEY ("NailDesignId") REFERENCES public."NailDesigns"("NailDesignId") ON DELETE SET NULL;


--
-- Name: NailVariants FK_NailVariants_NailShapes_NailShapeId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NailVariants"
    ADD CONSTRAINT "FK_NailVariants_NailShapes_NailShapeId" FOREIGN KEY ("NailShapeId") REFERENCES public."NailShapes"("NailShapeId") ON DELETE RESTRICT;


--
-- Name: NailVariants FK_NailVariants_NailSurfaces_NailSurfaceId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NailVariants"
    ADD CONSTRAINT "FK_NailVariants_NailSurfaces_NailSurfaceId" FOREIGN KEY ("NailSurfaceId") REFERENCES public."NailSurfaces"("NailSurfaceId") ON DELETE RESTRICT;


--
-- Name: Promotions FK_Promotions_Categories_CategoryId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Promotions"
    ADD CONSTRAINT "FK_Promotions_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES public."Categories"("CategoryId") ON DELETE SET NULL;


--
-- Name: Promotions FK_Promotions_CategoryTypes_CategoryTypeId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Promotions"
    ADD CONSTRAINT "FK_Promotions_CategoryTypes_CategoryTypeId" FOREIGN KEY ("CategoryTypeId") REFERENCES public."CategoryTypes"("CategoryTypeId") ON DELETE SET NULL;


--
-- Name: Promotions FK_Promotions_NailDesigns_NailDesignId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Promotions"
    ADD CONSTRAINT "FK_Promotions_NailDesigns_NailDesignId" FOREIGN KEY ("NailDesignId") REFERENCES public."NailDesigns"("NailDesignId") ON DELETE SET NULL;


--
-- Name: QuizOptions FK_QuizOptions_QuizQuestions_QuizQuestionId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."QuizOptions"
    ADD CONSTRAINT "FK_QuizOptions_QuizQuestions_QuizQuestionId" FOREIGN KEY ("QuizQuestionId") REFERENCES public."QuizQuestions"("QuizQuestionId") ON DELETE CASCADE;


--
-- Name: SalonOffDates FK_SalonOffDates_Salons_SalonId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SalonOffDates"
    ADD CONSTRAINT "FK_SalonOffDates_Salons_SalonId" FOREIGN KEY ("SalonId") REFERENCES public."Salons"("SalonId") ON DELETE CASCADE;


--
-- Name: SalonOperatingHours FK_SalonOperatingHours_Salons_SalonId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SalonOperatingHours"
    ADD CONSTRAINT "FK_SalonOperatingHours_Salons_SalonId" FOREIGN KEY ("SalonId") REFERENCES public."Salons"("SalonId") ON DELETE CASCADE;


--
-- Name: Schedules FK_Schedules_NailArtists_NailArtistId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Schedules"
    ADD CONSTRAINT "FK_Schedules_NailArtists_NailArtistId" FOREIGN KEY ("NailArtistId") REFERENCES public."NailArtists"("NailArtistId") ON DELETE CASCADE;


--
-- Name: ShapeMethodConfigs FK_ShapeMethodConfigs_NailShapes_NailShapeId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ShapeMethodConfigs"
    ADD CONSTRAINT "FK_ShapeMethodConfigs_NailShapes_NailShapeId" FOREIGN KEY ("NailShapeId") REFERENCES public."NailShapes"("NailShapeId") ON DELETE RESTRICT;


--
-- Name: StaffTransfers FK_StaffTransfers_NailArtists_NailArtistId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."StaffTransfers"
    ADD CONSTRAINT "FK_StaffTransfers_NailArtists_NailArtistId" FOREIGN KEY ("NailArtistId") REFERENCES public."NailArtists"("NailArtistId") ON DELETE RESTRICT;


--
-- Name: StaffTransfers FK_StaffTransfers_Salons_FromSalonId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."StaffTransfers"
    ADD CONSTRAINT "FK_StaffTransfers_Salons_FromSalonId" FOREIGN KEY ("FromSalonId") REFERENCES public."Salons"("SalonId") ON DELETE RESTRICT;


--
-- Name: StaffTransfers FK_StaffTransfers_Salons_ToSalonId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."StaffTransfers"
    ADD CONSTRAINT "FK_StaffTransfers_Salons_ToSalonId" FOREIGN KEY ("ToSalonId") REFERENCES public."Salons"("SalonId") ON DELETE RESTRICT;


--
-- Name: Transactions FK_Transactions_Bookings_BookingId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Transactions"
    ADD CONSTRAINT "FK_Transactions_Bookings_BookingId" FOREIGN KEY ("BookingId") REFERENCES public."Bookings"("BookingId") ON DELETE RESTRICT;


--
-- Name: UserPromotionUsages FK_UserPromotionUsages_Promotions_PromotionId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."UserPromotionUsages"
    ADD CONSTRAINT "FK_UserPromotionUsages_Promotions_PromotionId" FOREIGN KEY ("PromotionId") REFERENCES public."Promotions"("PromotionId") ON DELETE CASCADE;


--
-- Name: UserPromotionUsages FK_UserPromotionUsages_Users_UserId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."UserPromotionUsages"
    ADD CONSTRAINT "FK_UserPromotionUsages_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("UserId") ON DELETE CASCADE;


--
-- Name: Users FK_Users_Salons_SalonId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Users"
    ADD CONSTRAINT "FK_Users_Salons_SalonId" FOREIGN KEY ("SalonId") REFERENCES public."Salons"("SalonId") ON DELETE RESTRICT;


--
-- Name: WaitlistItems FK_WaitlistItems_BookingWaitlists_WaitlistId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."WaitlistItems"
    ADD CONSTRAINT "FK_WaitlistItems_BookingWaitlists_WaitlistId" FOREIGN KEY ("WaitlistId") REFERENCES public."BookingWaitlists"("WailistId") ON DELETE CASCADE;


--
-- Name: WaitlistItems FK_WaitlistItems_CustomerNails_CustomerNailId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."WaitlistItems"
    ADD CONSTRAINT "FK_WaitlistItems_CustomerNails_CustomerNailId" FOREIGN KEY ("CustomerNailId") REFERENCES public."CustomerNails"("CustomerNailId") ON DELETE RESTRICT;


--
-- Name: WaitlistItems FK_WaitlistItems_NailVariants_NailVariantId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."WaitlistItems"
    ADD CONSTRAINT "FK_WaitlistItems_NailVariants_NailVariantId" FOREIGN KEY ("NailVariantId") REFERENCES public."NailVariants"("NailVariantId") ON DELETE RESTRICT;


--
-- Name: WaitlistItems FK_WaitlistItems_Services_ServiceId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."WaitlistItems"
    ADD CONSTRAINT "FK_WaitlistItems_Services_ServiceId" FOREIGN KEY ("ServiceId") REFERENCES public."Services"("ServiceId") ON DELETE RESTRICT;


--
-- Name: WalkInQueues FK_WalkInQueues_Bookings_OriginalBookingId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."WalkInQueues"
    ADD CONSTRAINT "FK_WalkInQueues_Bookings_OriginalBookingId" FOREIGN KEY ("OriginalBookingId") REFERENCES public."Bookings"("BookingId") ON DELETE SET NULL;


--
-- Name: WalkInQueues FK_WalkInQueues_Chairs_ChairId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."WalkInQueues"
    ADD CONSTRAINT "FK_WalkInQueues_Chairs_ChairId" FOREIGN KEY ("ChairId") REFERENCES public."Chairs"("ChairId");


--
-- Name: WalkInQueues FK_WalkInQueues_Customers_CustomerId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."WalkInQueues"
    ADD CONSTRAINT "FK_WalkInQueues_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES public."Customers"("UserId") ON DELETE SET NULL;


--
-- Name: WalkInQueues FK_WalkInQueues_NailArtists_AssignedNailArtistId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."WalkInQueues"
    ADD CONSTRAINT "FK_WalkInQueues_NailArtists_AssignedNailArtistId" FOREIGN KEY ("AssignedNailArtistId") REFERENCES public."NailArtists"("NailArtistId") ON DELETE SET NULL;


--
-- Name: WalkInQueues FK_WalkInQueues_Salons_SalonId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."WalkInQueues"
    ADD CONSTRAINT "FK_WalkInQueues_Salons_SalonId" FOREIGN KEY ("SalonId") REFERENCES public."Salons"("SalonId") ON DELETE RESTRICT;


--
-- Name: BookingHistories; Type: ROW SECURITY; Schema: public; Owner: postgres
--

ALTER TABLE public."BookingHistories" ENABLE ROW LEVEL SECURITY;

--
-- Name: BookingItems; Type: ROW SECURITY; Schema: public; Owner: postgres
--

ALTER TABLE public."BookingItems" ENABLE ROW LEVEL SECURITY;

--
-- Name: Bookings; Type: ROW SECURITY; Schema: public; Owner: postgres
--

ALTER TABLE public."Bookings" ENABLE ROW LEVEL SECURITY;

--
-- Name: Categories; Type: ROW SECURITY; Schema: public; Owner: postgres
--

ALTER TABLE public."Categories" ENABLE ROW LEVEL SECURITY;

--
-- Name: CategoryTypes; Type: ROW SECURITY; Schema: public; Owner: postgres
--

ALTER TABLE public."CategoryTypes" ENABLE ROW LEVEL SECURITY;

--
-- Name: Components; Type: ROW SECURITY; Schema: public; Owner: postgres
--

ALTER TABLE public."Components" ENABLE ROW LEVEL SECURITY;

--
-- Name: CustomerComponents; Type: ROW SECURITY; Schema: public; Owner: postgres
--

ALTER TABLE public."CustomerComponents" ENABLE ROW LEVEL SECURITY;

--
-- Name: CustomerNailComponents; Type: ROW SECURITY; Schema: public; Owner: postgres
--

ALTER TABLE public."CustomerNailComponents" ENABLE ROW LEVEL SECURITY;

--
-- Name: CustomerNails; Type: ROW SECURITY; Schema: public; Owner: postgres
--

ALTER TABLE public."CustomerNails" ENABLE ROW LEVEL SECURITY;

--
-- Name: Customers; Type: ROW SECURITY; Schema: public; Owner: postgres
--

ALTER TABLE public."Customers" ENABLE ROW LEVEL SECURITY;

--
-- Name: NailArtistSkills; Type: ROW SECURITY; Schema: public; Owner: postgres
--

ALTER TABLE public."NailArtistSkills" ENABLE ROW LEVEL SECURITY;

--
-- Name: NailArtists; Type: ROW SECURITY; Schema: public; Owner: postgres
--

ALTER TABLE public."NailArtists" ENABLE ROW LEVEL SECURITY;

--
-- Name: NailCategories; Type: ROW SECURITY; Schema: public; Owner: postgres
--

ALTER TABLE public."NailCategories" ENABLE ROW LEVEL SECURITY;

--
-- Name: NailComponents; Type: ROW SECURITY; Schema: public; Owner: postgres
--

ALTER TABLE public."NailComponents" ENABLE ROW LEVEL SECURITY;

--
-- Name: NailDesigns; Type: ROW SECURITY; Schema: public; Owner: postgres
--

ALTER TABLE public."NailDesigns" ENABLE ROW LEVEL SECURITY;

--
-- Name: NailRequiredSkills; Type: ROW SECURITY; Schema: public; Owner: postgres
--

ALTER TABLE public."NailRequiredSkills" ENABLE ROW LEVEL SECURITY;

--
-- Name: NailShapes; Type: ROW SECURITY; Schema: public; Owner: postgres
--

ALTER TABLE public."NailShapes" ENABLE ROW LEVEL SECURITY;

--
-- Name: NailSurfaces; Type: ROW SECURITY; Schema: public; Owner: postgres
--

ALTER TABLE public."NailSurfaces" ENABLE ROW LEVEL SECURITY;

--
-- Name: NailVariants; Type: ROW SECURITY; Schema: public; Owner: postgres
--

ALTER TABLE public."NailVariants" ENABLE ROW LEVEL SECURITY;

--
-- Name: SalonOperatingHours; Type: ROW SECURITY; Schema: public; Owner: postgres
--

ALTER TABLE public."SalonOperatingHours" ENABLE ROW LEVEL SECURITY;

--
-- Name: Salons; Type: ROW SECURITY; Schema: public; Owner: postgres
--

ALTER TABLE public."Salons" ENABLE ROW LEVEL SECURITY;

--
-- Name: Schedules; Type: ROW SECURITY; Schema: public; Owner: postgres
--

ALTER TABLE public."Schedules" ENABLE ROW LEVEL SECURITY;

--
-- Name: Services; Type: ROW SECURITY; Schema: public; Owner: postgres
--

ALTER TABLE public."Services" ENABLE ROW LEVEL SECURITY;

--
-- Name: SkillTypes; Type: ROW SECURITY; Schema: public; Owner: postgres
--

ALTER TABLE public."SkillTypes" ENABLE ROW LEVEL SECURITY;

--
-- Name: Users; Type: ROW SECURITY; Schema: public; Owner: postgres
--

ALTER TABLE public."Users" ENABLE ROW LEVEL SECURITY;

--
-- Name: __EFMigrationsHistory; Type: ROW SECURITY; Schema: public; Owner: postgres
--

ALTER TABLE public."__EFMigrationsHistory" ENABLE ROW LEVEL SECURITY;

--
-- Name: SCHEMA public; Type: ACL; Schema: -; Owner: pg_database_owner
--

GRANT USAGE ON SCHEMA public TO postgres;
GRANT USAGE ON SCHEMA public TO anon;
GRANT USAGE ON SCHEMA public TO authenticated;
GRANT USAGE ON SCHEMA public TO service_role;


--
-- Name: TABLE "BookingDiscounts"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."BookingDiscounts" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."BookingDiscounts" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."BookingDiscounts" TO service_role;


--
-- Name: TABLE "BookingHistories"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."BookingHistories" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."BookingHistories" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."BookingHistories" TO service_role;


--
-- Name: TABLE "BookingItems"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."BookingItems" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."BookingItems" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."BookingItems" TO service_role;


--
-- Name: TABLE "BookingProcedures"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."BookingProcedures" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."BookingProcedures" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."BookingProcedures" TO service_role;


--
-- Name: TABLE "BookingRatings"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."BookingRatings" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."BookingRatings" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."BookingRatings" TO service_role;


--
-- Name: TABLE "BookingWaitlists"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."BookingWaitlists" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."BookingWaitlists" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."BookingWaitlists" TO service_role;


--
-- Name: TABLE "Bookings"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Bookings" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Bookings" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Bookings" TO service_role;


--
-- Name: TABLE "Categories"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Categories" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Categories" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Categories" TO service_role;


--
-- Name: TABLE "CategoryTypes"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."CategoryTypes" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."CategoryTypes" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."CategoryTypes" TO service_role;


--
-- Name: TABLE "Chairs"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Chairs" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Chairs" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Chairs" TO service_role;


--
-- Name: TABLE "Components"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Components" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Components" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Components" TO service_role;


--
-- Name: TABLE "CustomerComponents"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."CustomerComponents" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."CustomerComponents" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."CustomerComponents" TO service_role;


--
-- Name: TABLE "CustomerNailComponents"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."CustomerNailComponents" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."CustomerNailComponents" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."CustomerNailComponents" TO service_role;


--
-- Name: TABLE "CustomerNailRequests"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."CustomerNailRequests" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."CustomerNailRequests" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."CustomerNailRequests" TO service_role;


--
-- Name: TABLE "CustomerNails"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."CustomerNails" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."CustomerNails" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."CustomerNails" TO service_role;


--
-- Name: TABLE "CustomerQuizAnswers"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."CustomerQuizAnswers" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."CustomerQuizAnswers" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."CustomerQuizAnswers" TO service_role;


--
-- Name: TABLE "Customers"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Customers" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Customers" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Customers" TO service_role;


--
-- Name: TABLE "FavoriteNails"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."FavoriteNails" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."FavoriteNails" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."FavoriteNails" TO service_role;


--
-- Name: TABLE "LoyaltyTiers"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."LoyaltyTiers" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."LoyaltyTiers" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."LoyaltyTiers" TO service_role;


--
-- Name: TABLE "LoyaltyTransactions"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."LoyaltyTransactions" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."LoyaltyTransactions" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."LoyaltyTransactions" TO service_role;


--
-- Name: TABLE "NailArtistBreaks"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailArtistBreaks" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailArtistBreaks" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailArtistBreaks" TO service_role;


--
-- Name: TABLE "NailArtistSkills"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailArtistSkills" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailArtistSkills" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailArtistSkills" TO service_role;


--
-- Name: TABLE "NailArtists"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailArtists" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailArtists" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailArtists" TO service_role;


--
-- Name: TABLE "NailCategories"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailCategories" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailCategories" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailCategories" TO service_role;


--
-- Name: TABLE "NailComponents"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailComponents" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailComponents" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailComponents" TO service_role;


--
-- Name: TABLE "NailDesigns"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailDesigns" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailDesigns" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailDesigns" TO service_role;


--
-- Name: TABLE "NailProcedures"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailProcedures" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailProcedures" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailProcedures" TO service_role;


--
-- Name: TABLE "NailRequiredSkills"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailRequiredSkills" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailRequiredSkills" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailRequiredSkills" TO service_role;


--
-- Name: TABLE "NailShapes"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailShapes" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailShapes" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailShapes" TO service_role;


--
-- Name: TABLE "NailSurfaces"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailSurfaces" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailSurfaces" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailSurfaces" TO service_role;


--
-- Name: TABLE "NailVariants"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailVariants" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailVariants" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."NailVariants" TO service_role;


--
-- Name: TABLE "Procedures"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Procedures" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Procedures" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Procedures" TO service_role;


--
-- Name: TABLE "Promotions"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Promotions" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Promotions" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Promotions" TO service_role;


--
-- Name: TABLE "QuizOptions"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."QuizOptions" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."QuizOptions" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."QuizOptions" TO service_role;


--
-- Name: TABLE "QuizQuestions"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."QuizQuestions" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."QuizQuestions" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."QuizQuestions" TO service_role;


--
-- Name: TABLE "SalonOffDates"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."SalonOffDates" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."SalonOffDates" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."SalonOffDates" TO service_role;


--
-- Name: TABLE "SalonOperatingHours"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."SalonOperatingHours" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."SalonOperatingHours" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."SalonOperatingHours" TO service_role;


--
-- Name: TABLE "Salons"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Salons" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Salons" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Salons" TO service_role;


--
-- Name: TABLE "Schedules"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Schedules" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Schedules" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Schedules" TO service_role;


--
-- Name: TABLE "Services"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Services" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Services" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Services" TO service_role;


--
-- Name: TABLE "ShapeMethodConfigs"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."ShapeMethodConfigs" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."ShapeMethodConfigs" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."ShapeMethodConfigs" TO service_role;


--
-- Name: TABLE "SkillTypes"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."SkillTypes" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."SkillTypes" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."SkillTypes" TO service_role;


--
-- Name: TABLE "StaffTransfers"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."StaffTransfers" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."StaffTransfers" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."StaffTransfers" TO service_role;


--
-- Name: TABLE "Transactions"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Transactions" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Transactions" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Transactions" TO service_role;


--
-- Name: TABLE "UserPromotionUsages"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."UserPromotionUsages" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."UserPromotionUsages" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."UserPromotionUsages" TO service_role;


--
-- Name: TABLE "Users"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Users" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Users" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."Users" TO service_role;


--
-- Name: TABLE "WaitlistItems"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."WaitlistItems" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."WaitlistItems" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."WaitlistItems" TO service_role;


--
-- Name: TABLE "WalkInQueues"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."WalkInQueues" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."WalkInQueues" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."WalkInQueues" TO service_role;


--
-- Name: TABLE "__EFMigrationsHistory"; Type: ACL; Schema: public; Owner: postgres
--

GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."__EFMigrationsHistory" TO anon;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."__EFMigrationsHistory" TO authenticated;
GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLE public."__EFMigrationsHistory" TO service_role;


--
-- Name: DEFAULT PRIVILEGES FOR SEQUENCES; Type: DEFAULT ACL; Schema: public; Owner: postgres
--

ALTER DEFAULT PRIVILEGES FOR ROLE postgres IN SCHEMA public GRANT ALL ON SEQUENCES TO postgres;


--
-- Name: DEFAULT PRIVILEGES FOR SEQUENCES; Type: DEFAULT ACL; Schema: public; Owner: supabase_admin
--

ALTER DEFAULT PRIVILEGES FOR ROLE supabase_admin IN SCHEMA public GRANT ALL ON SEQUENCES TO postgres;
ALTER DEFAULT PRIVILEGES FOR ROLE supabase_admin IN SCHEMA public GRANT ALL ON SEQUENCES TO anon;
ALTER DEFAULT PRIVILEGES FOR ROLE supabase_admin IN SCHEMA public GRANT ALL ON SEQUENCES TO authenticated;
ALTER DEFAULT PRIVILEGES FOR ROLE supabase_admin IN SCHEMA public GRANT ALL ON SEQUENCES TO service_role;


--
-- Name: DEFAULT PRIVILEGES FOR FUNCTIONS; Type: DEFAULT ACL; Schema: public; Owner: postgres
--

ALTER DEFAULT PRIVILEGES FOR ROLE postgres IN SCHEMA public GRANT ALL ON FUNCTIONS TO postgres;


--
-- Name: DEFAULT PRIVILEGES FOR FUNCTIONS; Type: DEFAULT ACL; Schema: public; Owner: supabase_admin
--

ALTER DEFAULT PRIVILEGES FOR ROLE supabase_admin IN SCHEMA public GRANT ALL ON FUNCTIONS TO postgres;
ALTER DEFAULT PRIVILEGES FOR ROLE supabase_admin IN SCHEMA public GRANT ALL ON FUNCTIONS TO anon;
ALTER DEFAULT PRIVILEGES FOR ROLE supabase_admin IN SCHEMA public GRANT ALL ON FUNCTIONS TO authenticated;
ALTER DEFAULT PRIVILEGES FOR ROLE supabase_admin IN SCHEMA public GRANT ALL ON FUNCTIONS TO service_role;


--
-- Name: DEFAULT PRIVILEGES FOR TABLES; Type: DEFAULT ACL; Schema: public; Owner: postgres
--

ALTER DEFAULT PRIVILEGES FOR ROLE postgres IN SCHEMA public GRANT ALL ON TABLES TO postgres;
ALTER DEFAULT PRIVILEGES FOR ROLE postgres IN SCHEMA public GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLES TO anon;
ALTER DEFAULT PRIVILEGES FOR ROLE postgres IN SCHEMA public GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLES TO authenticated;
ALTER DEFAULT PRIVILEGES FOR ROLE postgres IN SCHEMA public GRANT REFERENCES,TRIGGER,TRUNCATE,MAINTAIN ON TABLES TO service_role;


--
-- Name: DEFAULT PRIVILEGES FOR TABLES; Type: DEFAULT ACL; Schema: public; Owner: supabase_admin
--

ALTER DEFAULT PRIVILEGES FOR ROLE supabase_admin IN SCHEMA public GRANT ALL ON TABLES TO postgres;
ALTER DEFAULT PRIVILEGES FOR ROLE supabase_admin IN SCHEMA public GRANT ALL ON TABLES TO anon;
ALTER DEFAULT PRIVILEGES FOR ROLE supabase_admin IN SCHEMA public GRANT ALL ON TABLES TO authenticated;
ALTER DEFAULT PRIVILEGES FOR ROLE supabase_admin IN SCHEMA public GRANT ALL ON TABLES TO service_role;


--
-- PostgreSQL database dump complete
--

\unrestrict GRIK32g67YKcZkqS2RrmhNgTOAhnhDTNljowt4O8OEnM1lYkCrTwaSRCnY92hKt

