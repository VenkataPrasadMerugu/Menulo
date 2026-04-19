INSERT INTO public.tbl_users
(
    "Id",
    "FullName",
    "CreatedAt",
    "UpdatedAt",
    "UserName",
    "NormalizedUserName",
    "Email",
    "NormalizedEmail",
    "EmailConfirmed",
    "PasswordHash",
    "SecurityStamp",
    "ConcurrencyStamp",
    "PhoneNumber",
    "PhoneNumberConfirmed",
    "TwoFactorEnabled",
    "LockoutEnd",
    "LockoutEnabled",
    "AccessFailedCount"
)
VALUES
(
    1,
    'Demo Owner',
    '2026-03-31T04:30:00Z',
    '2026-03-31T04:30:00Z',
    'owner@menulo.local',
    'OWNER@MENULO.LOCAL',
    'owner@menulo.local',
    'OWNER@MENULO.LOCAL',
    TRUE,
    'AQAAAAIAAYagAAAAEIzw2pL209z+Onxl7VKotUoCuDeO+idlBCtNBXtobaINFW96udptCUgb+p6wLzDq6A==',
    'demo-security-stamp',
    'demo-concurrency-stamp',
    NULL,
    FALSE,
    FALSE,
    NULL,
    TRUE,
    0
)
ON CONFLICT ("Id") DO NOTHING;

SELECT setval(pg_get_serial_sequence('public.tbl_users', 'Id'), GREATEST((SELECT COALESCE(MAX("Id"), 0) FROM public.tbl_users), 1), TRUE);

INSERT INTO public.tbl_restaurants
(
    "Id",
    "OwnerUserId",
    "Name",
    "Slug",
    "BranchName",
    "Address",
    "LogoPath",
    "PrimaryColor",
    "SecondaryColor",
    "PaletteKey",
    "CurrencyCode",
    "CreatedAt",
    "UpdatedAt"
)
VALUES
(
    1,
    1,
    'Paradise Spice House',
    'paradise-spice-house',
    'Banjara Hills',
    'Road No. 12, Hyderabad',
    '/uploads/logos/demo-logo.svg',
    '#8D1B3D',
    '#F2B544',
    'spice-house',
    'INR',
    '2026-03-31T04:31:00Z',
    '2026-03-31T04:31:00Z'
),
(
    2,
    1,
    'Chefs Village By Cuisine Voyage',
    'chefs-village-by-cuisine-voyage-2',
    'Madhapur',
    'HiTech City Main Road, Hyderabad',
    '/uploads/logos/demo-logo.svg',
    '#1D7A46',
    '#F7D154',
    'fresh-leaf',
    'INR',
    '2026-03-31T04:32:00Z',
    '2026-03-31T04:32:00Z'
)
ON CONFLICT ("Id") DO NOTHING;

SELECT setval(pg_get_serial_sequence('public.tbl_restaurants', 'Id'), GREATEST((SELECT COALESCE(MAX("Id"), 0) FROM public.tbl_restaurants), 1), TRUE);

INSERT INTO public.tbl_menu_items
(
    "Id",
    "RestaurantId",
    "Name",
    "Price",
    "Serves",
    "Category",
    "FoodType",
    "Status",
    "CreatedAt",
    "UpdatedAt"
)
VALUES
(
    1,
    1,
    'Hyderabadi Dum Biryani',
    349.00,
    2,
    'RiceAndBiryani',
    'NonVeg',
    'Active',
    '2026-03-31T04:33:00Z',
    '2026-03-31T04:33:00Z'
),
(
    2,
    1,
    'Paneer Tikka',
    259.00,
    1,
    'Starters',
    'Veg',
    'OutOfStock',
    '2026-03-31T04:34:00Z',
    '2026-03-31T04:34:00Z'
),
(
    3,
    1,
    'Mango Lassi',
    129.00,
    1,
    'Beverages',
    'Veg',
    'Inactive',
    '2026-03-31T04:35:00Z',
    '2026-03-31T04:35:00Z'
),
(
    4,
    2,
    'Veg Meals',
    199.00,
    1,
    'Meals',
    'Veg',
    'Active',
    '2026-03-31T04:36:00Z',
    '2026-03-31T04:36:00Z'
)
ON CONFLICT ("Id") DO NOTHING;

SELECT setval(pg_get_serial_sequence('public.tbl_menu_items', 'Id'), GREATEST((SELECT COALESCE(MAX("Id"), 0) FROM public.tbl_menu_items), 1), TRUE);

INSERT INTO public.tbl_menu_item_images
(
    "Id",
    "MenuItemId",
    "ImagePath",
    "SortOrder",
    "CreatedAt",
    "UpdatedAt"
)
VALUES
(
    1,
    1,
    '/uploads/menu-items/demo-biryani-1.svg',
    0,
    '2026-03-31T04:37:00Z',
    '2026-03-31T04:37:00Z'
),
(
    2,
    1,
    '/uploads/menu-items/demo-biryani-2.svg',
    1,
    '2026-03-31T04:37:00Z',
    '2026-03-31T04:37:00Z'
),
(
    3,
    2,
    '/uploads/menu-items/demo-paneer-1.svg',
    0,
    '2026-03-31T04:38:00Z',
    '2026-03-31T04:38:00Z'
),
(
    4,
    4,
    '/uploads/menu-items/demo-meals-1.svg',
    0,
    '2026-03-31T04:39:00Z',
    '2026-03-31T04:39:00Z'
),
(
    5,
    4,
    '/uploads/menu-items/demo-meals-2.svg',
    1,
    '2026-03-31T04:39:00Z',
    '2026-03-31T04:39:00Z'
)
ON CONFLICT ("Id") DO NOTHING;

SELECT setval(pg_get_serial_sequence('public.tbl_menu_item_images', 'Id'), GREATEST((SELECT COALESCE(MAX("Id"), 0) FROM public.tbl_menu_item_images), 1), TRUE);
