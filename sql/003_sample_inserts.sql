INSERT INTO public.tbl_restaurants
(
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
    'Udupi Bhavan',
    'udupi-bhavan',
    'Ameerpet',
    'Main Market Road, Ameerpet, Hyderabad',
    '/uploads/logos/udupi.svg',
    '#1D7A46',
    '#F7D154',
    'fresh-leaf',
    'INR',
    now(),
    now()
);

INSERT INTO public.tbl_menu_items
(
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
    currval(pg_get_serial_sequence('public.tbl_restaurants', 'Id')),
    'Masala Dosa',
    149.00,
    1,
    'MainCourse',
    'Veg',
    'Active',
    now(),
    now()
);

INSERT INTO public.tbl_menu_item_images
(
    "MenuItemId",
    "ImagePath",
    "SortOrder",
    "CreatedAt",
    "UpdatedAt"
)
VALUES
(
    currval(pg_get_serial_sequence('public.tbl_menu_items', 'Id')),
    '/uploads/menu-items/masala-dosa-1.svg',
    0,
    now(),
    now()
),
(
    currval(pg_get_serial_sequence('public.tbl_menu_items', 'Id')),
    '/uploads/menu-items/masala-dosa-2.svg',
    1,
    now(),
    now()
);
