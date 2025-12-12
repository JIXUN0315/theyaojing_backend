-- 啟用 UUID 產生 extension
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- 建立資料表
CREATE TABLE IF NOT EXISTS public.form_responses (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    answers JSONB NOT NULL,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT now()
);
-- 建立精簡版 admin_users 資料表

CREATE TABLE IF NOT EXISTS public.admin_users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    username VARCHAR(50) NOT NULL UNIQUE,
    email VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL
    );

-- 建立索引以提升查詢效能
CREATE INDEX IF NOT EXISTS idx_admin_users_username ON public.admin_users(username);
CREATE INDEX IF NOT EXISTS idx_admin_users_email ON public.admin_users(email);


-- public.blog_posts definition
CREATE TABLE public.blog_posts (
                                   id uuid DEFAULT gen_random_uuid() NOT NULL,
                                   title varchar(200) NOT NULL,
                                   "content" text NOT NULL,
                                   excerpt text NULL,
                                   featured_image_url varchar(500) NULL,
                                   category varchar(50) NULL,
                                   is_published bool DEFAULT false NULL,
                                   created_at timestamp DEFAULT now() NOT NULL,
                                   updated_at timestamp DEFAULT now() NOT NULL,
                                   CONSTRAINT blog_posts_pkey PRIMARY KEY (id)
);
CREATE INDEX idx_blog_posts_category ON public.blog_posts USING btree (category);
CREATE INDEX idx_blog_posts_created_at ON public.blog_posts USING btree (created_at DESC);
CREATE INDEX idx_blog_posts_published ON public.blog_posts USING btree (is_published);

-- Table Triggers

create trigger update_blog_posts_updated_at before
    update
    on
        public.blog_posts for each row execute function update_updated_at_column();