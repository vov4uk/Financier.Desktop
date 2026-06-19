using System.ComponentModel;
using Financier.Common.Attribute;
using Financier.Converters;

namespace Financier.Common.Entities
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum Mcc
    {
        [LocalizedMccDescription("mcc_none")]
        [MccCodes(0)]
        none,

        [LocalizedMccDescription("mcc_accessories")]
        [MccCodes(5699)]
        accessories,

        [LocalizedMccDescription("mcc_accounting_audit")]
        [MccCodes(8931)]
        accounting_audit,

        [LocalizedMccDescription("mcc_advertising")]
        [MccCodes(7311)]
        advertising,

        [LocalizedMccDescription("mcc_agricultural_co_operatives")]
        [MccCodes(763)]
        agricultural_co_operatives,

        [LocalizedMccDescription("mcc_airlines")]
        [MccCodes(3000, 3001, 3002, 3003, 3004, 3005, 3006, 3007, 3008, 3009, 3010, 3011, 3012, 3013, 3014, 3015, 3016, 3017, 3018, 3019, 3020, 3021, 3022, 3023, 3024, 3025, 3026, 3027, 3028, 3029, 3030, 3031, 3032, 3033, 3034, 3035, 3036, 3037, 3038, 3039, 3040, 3041, 3042, 3043, 3044, 3045, 3046, 3047, 3048, 3049, 3050, 3051, 3052, 3053, 3054, 3055, 3056, 3057, 3058, 3059, 3060, 3061, 3062, 3063, 3064, 3065, 3066, 3067, 3068, 3069, 3070, 3071, 3072, 3073, 3074, 3075, 3076, 3077, 3078, 3079, 3080, 3081, 3082, 3083, 3084, 3085, 3086, 3087, 3088, 3089, 3090, 3091, 3092, 3093, 3094, 3095, 3096, 3097, 3098, 3099, 3100, 3101, 3102, 3103, 3104, 3105, 3106, 3107, 3108, 3109, 3110, 3111, 3112, 3113, 3114, 3115, 3116, 3117, 3118, 3119, 3120, 3121, 3122, 3123, 3124, 3125, 3126, 3127, 3128, 3129, 3130, 3131, 3132, 3133, 3134, 3135, 3136, 3137, 3138, 3139, 3140, 3141, 3142, 3143, 3144, 3145, 3146, 3147, 3148, 3149, 3150, 3151, 3152, 3153, 3154, 3155, 3156, 3157, 3158, 3159, 3160, 3161, 3162, 3163, 3164, 3165, 3166, 3167, 3168, 3169, 3170, 3171, 3172, 3173, 3174, 3175, 3176, 3177, 3178, 3179, 3180, 3181, 3182, 3183, 3184, 3185, 3186, 3187, 3188, 3189, 3190, 3191, 3192, 3193, 3194, 3195, 3196, 3197, 3198, 3199, 3200, 3201, 3202, 3203, 3204, 3205, 3206, 3207, 3208, 3209, 3210, 3211, 3212, 3213, 3214, 3215, 3216, 3217, 3218, 3219, 3220, 3221, 3222, 3223, 3224, 3225, 3226, 3227, 3228, 3229, 3230, 3231, 3232, 3233, 3234, 3235, 3236, 3237, 3238, 3239, 3240, 3241, 3242, 3243, 3244, 3245, 3246, 3247, 3248, 3249, 3250, 3251, 3252, 3253, 3254, 3255, 3256, 3257, 3258, 3259, 3260, 3261, 3262, 3263, 3264, 3265, 3266, 3267, 3268, 3269, 3270, 3271, 3272, 3273, 3274, 3275, 3276, 3277, 3278, 3279, 3280, 3281, 3282, 3283, 3284, 3285, 3286, 3287, 3288, 3289, 3290, 3291, 3292, 3293, 3294, 3295, 3296, 3297, 3298, 3299, 3300, 3301, 3302, 4511)]
        airlines,

        [LocalizedMccDescription("mcc_airports")]
        [MccCodes(4582)]
        airports,

        [LocalizedMccDescription("mcc_alcohol")]
        [MccCodes(5715, 5921)]
        alcohol,

        [LocalizedMccDescription("mcc_ambulance")]
        [MccCodes(4119)]
        ambulance,

        [LocalizedMccDescription("mcc_antiques")]
        [MccCodes(5832, 5932)]
        antiques,

        [LocalizedMccDescription("mcc_applications")]
        [MccCodes(5817)]
        applications,

        [LocalizedMccDescription("mcc_aquariums_dolphinariums")]
        [MccCodes(7998)]
        aquariums_dolphinariums,

        [LocalizedMccDescription("mcc_architects")]
        [MccCodes(8911)]
        architects,

        [LocalizedMccDescription("mcc_ard_replenishment")]
        [MccCodes(6529, 6530)]
        ard_replenishment,

        [LocalizedMccDescription("mcc_art_goods")]
        [MccCodes(5970)]
        art_goods,

        [LocalizedMccDescription("mcc_ashier_s_office")]
        [MccCodes(6010, 6011)]
        ashier_s_office,

        [LocalizedMccDescription("mcc_ashing")]
        [MccCodes(3882)]
        ashing,

        [LocalizedMccDescription("mcc_atelier")]
        [MccCodes(5697)]
        atelier,

        [LocalizedMccDescription("mcc_auto_parts")]
        [MccCodes(5013, 5531)]
        auto_parts,

        [LocalizedMccDescription("mcc_auto_repair")]
        [MccCodes(7531)]
        auto_repair,

        [LocalizedMccDescription("mcc_auto_shops")]
        [MccCodes(5533)]
        auto_shops,

        [LocalizedMccDescription("mcc_autoclub")]
        [MccCodes(8675)]
        autoclub,

        [LocalizedMccDescription("mcc_baby_clothes")]
        [MccCodes(5641)]
        baby_clothes,

        [LocalizedMccDescription("mcc_babysitters")]
        [MccCodes(7295)]
        babysitters,

        [LocalizedMccDescription("mcc_bakeries")]
        [MccCodes(5462)]
        bakeries,

        [LocalizedMccDescription("mcc_banks")]
        [MccCodes(6022, 6023, 6025, 6026, 6028)]
        banks,

        [LocalizedMccDescription("mcc_bars")]
        [MccCodes(5813)]
        bars,

        [LocalizedMccDescription("mcc_bicycles")]
        [MccCodes(5940)]
        bicycles,

        [LocalizedMccDescription("mcc_billiard")]
        [MccCodes(7932)]
        billiard,

        [LocalizedMccDescription("mcc_boat_rentals")]
        [MccCodes(4457)]
        boat_rentals,

        [LocalizedMccDescription("mcc_boats")]
        [MccCodes(5551)]
        boats,

        [LocalizedMccDescription("mcc_bonds")]
        [MccCodes(6760)]
        bonds,

        [LocalizedMccDescription("mcc_book_stores")]
        [MccCodes(5942)]
        book_stores,

        [LocalizedMccDescription("mcc_books_press")]
        [MccCodes(5192)]
        books_press,

        [LocalizedMccDescription("mcc_bowling_clubs")]
        [MccCodes(7933)]
        bowling_clubs,

        [LocalizedMccDescription("mcc_building_materials")]
        [MccCodes(5039, 5211)]
        building_materials,

        [LocalizedMccDescription("mcc_business_services")]
        [MccCodes(7389, 7399)]
        business_services,

        [LocalizedMccDescription("mcc_cafe_restaurants")]
        [MccCodes(5812)]
        cafe_restaurants,

        [LocalizedMccDescription("mcc_campgrounds")]
        [MccCodes(7033)]
        campgrounds,

        [LocalizedMccDescription("mcc_car_dealerships")]
        [MccCodes(5511, 5521, 5561, 5571, 5592, 5598, 5599)]
        car_dealerships,

        [LocalizedMccDescription("mcc_car_dump")]
        [MccCodes(5935)]
        car_dump,

        [LocalizedMccDescription("mcc_car_paints")]
        [MccCodes(7535)]
        car_paints,

        [LocalizedMccDescription("mcc_car_rent")]
        [MccCodes(3351, 3352, 3353, 3354, 3355, 3356, 3357, 3358, 3359, 3360, 3361, 3362, 3363, 3364, 3365, 3366, 3367, 3368, 3369, 3370, 3371, 3372, 3373, 3374, 3375, 3376, 3377, 3378, 3379, 3380, 3381, 3382, 3383, 3384, 3385, 3386, 3387, 3388, 3389, 3390, 3391, 3392, 3393, 3394, 3395, 3396, 3397, 3398, 3399, 3400, 3401, 3402, 3403, 3404, 3405, 3406, 3407, 3408, 3409, 3410, 3411, 3412, 3413, 3414, 3415, 3416, 3417, 3418, 3419, 3420, 3421, 3422, 3423, 3424, 3425, 3426, 3427, 3428, 3429, 3430, 3431, 3432, 3433, 3434, 3435, 3436, 3437, 3438, 3439, 3440, 3441, 7512, 7519)]
        car_rent,

        [LocalizedMccDescription("mcc_car_washes")]
        [MccCodes(7542)]
        car_washes,

        [LocalizedMccDescription("mcc_caregiver_nurse")]
        [MccCodes(8050)]
        caregiver_nurse,

        [LocalizedMccDescription("mcc_carpentry_contractors")]
        [MccCodes(1750)]
        carpentry_contractors,

        [LocalizedMccDescription("mcc_cashback")]
        [MccCodes(9700)]
        cashback,

        [LocalizedMccDescription("mcc_caterers")]
        [MccCodes(5811)]
        caterers,

        [LocalizedMccDescription("mcc_champagne_producers")]
        [MccCodes(744)]
        champagne_producers,

        [LocalizedMccDescription("mcc_chancery")]
        [MccCodes(5111)]
        chancery,

        [LocalizedMccDescription("mcc_charging_stations")]
        [MccCodes(5552)]
        charging_stations,

        [LocalizedMccDescription("mcc_charity")]
        [MccCodes(8398)]
        charity,

        [LocalizedMccDescription("mcc_chemicals")]
        [MccCodes(5169)]
        chemicals,

        [LocalizedMccDescription("mcc_chiropractors")]
        [MccCodes(8041)]
        chiropractors,

        [LocalizedMccDescription("mcc_church_shops")]
        [MccCodes(5973)]
        church_shops,

        [LocalizedMccDescription("mcc_cinemas")]
        [MccCodes(7832, 7833)]
        cinemas,

        [LocalizedMccDescription("mcc_cleaning")]
        [MccCodes(7210, 7217)]
        cleaning,

        [LocalizedMccDescription("mcc_cleaning_and_maintenance")]
        [MccCodes(7349)]
        cleaning_and_maintenance,

        [LocalizedMccDescription("mcc_clock")]
        [MccCodes(5944)]
        clock,

        [LocalizedMccDescription("mcc_clothes")]
        [MccCodes(5651, 5691)]
        clothes,

        [LocalizedMccDescription("mcc_clothing")]
        [MccCodes(5137)]
        clothing,

        [LocalizedMccDescription("mcc_clothing_rental")]
        [MccCodes(7296)]
        clothing_rental,

        [LocalizedMccDescription("mcc_clothing_repair")]
        [MccCodes(7251)]
        clothing_repair,

        [LocalizedMccDescription("mcc_clothing_stores")]
        [MccCodes(5631)]
        clothing_stores,

        [LocalizedMccDescription("mcc_collection_agencies")]
        [MccCodes(7322)]
        collection_agencies,

        [LocalizedMccDescription("mcc_computer_repair")]
        [MccCodes(7379)]
        computer_repair,

        [LocalizedMccDescription("mcc_computer_software")]
        [MccCodes(5734)]
        computer_software,

        [LocalizedMccDescription("mcc_computers_and_software")]
        [MccCodes(5045)]
        computers_and_software,

        [LocalizedMccDescription("mcc_concrete_work_contractors")]
        [MccCodes(1771)]
        concrete_work_contractors,

        [LocalizedMccDescription("mcc_consultation")]
        [MccCodes(7277)]
        consultation,

        [LocalizedMccDescription("mcc_consulting_pr")]
        [MccCodes(7392)]
        consulting_pr,

        [LocalizedMccDescription("mcc_copy_centers")]
        [MccCodes(7332, 7338)]
        copy_centers,

        [LocalizedMccDescription("mcc_cosmetics")]
        [MccCodes(5977)]
        cosmetics,

        [LocalizedMccDescription("mcc_court")]
        [MccCodes(9211)]
        court,

        [LocalizedMccDescription("mcc_credit_bureaus")]
        [MccCodes(7321)]
        credit_bureaus,

        [LocalizedMccDescription("mcc_cruise_lines")]
        [MccCodes(4411)]
        cruise_lines,

        [LocalizedMccDescription("mcc_crystal_glassware")]
        [MccCodes(5950)]
        crystal_glassware,

        [LocalizedMccDescription("mcc_curtains")]
        [MccCodes(5714)]
        curtains,

        [LocalizedMccDescription("mcc_dance_studios_dance_schools")]
        [MccCodes(7911)]
        dance_studios_dance_schools,

        [LocalizedMccDescription("mcc_dating_escort")]
        [MccCodes(7273)]
        dating_escort,

        [LocalizedMccDescription("mcc_delivery_service")]
        [MccCodes(4215)]
        delivery_service,

        [LocalizedMccDescription("mcc_dentistry")]
        [MccCodes(8021)]
        dentistry,

        [LocalizedMccDescription("mcc_department_stores")]
        [MccCodes(5311)]
        department_stores,

        [LocalizedMccDescription("mcc_detective_agencies")]
        [MccCodes(7393)]
        detective_agencies,

        [LocalizedMccDescription("mcc_digital_goods")]
        [MccCodes(5815, 5818)]
        digital_goods,

        [LocalizedMccDescription("mcc_discounters")]
        [MccCodes(5310)]
        discounters,

        [LocalizedMccDescription("mcc_disinfecting")]
        [MccCodes(7342)]
        disinfecting,

        [LocalizedMccDescription("mcc_document_flow")]
        [MccCodes(9751, 9752)]
        document_flow,

        [LocalizedMccDescription("mcc_drug_stores")]
        [MccCodes(5912)]
        drug_stores,

        [LocalizedMccDescription("mcc_drugs")]
        [MccCodes(5122)]
        drugs,

        [LocalizedMccDescription("mcc_dry_cleaners")]
        [MccCodes(7216)]
        dry_cleaners,

        [LocalizedMccDescription("mcc_duty_free")]
        [MccCodes(5309)]
        duty_free,

        [LocalizedMccDescription("mcc_education")]
        [MccCodes(8249, 8299)]
        education,

        [LocalizedMccDescription("mcc_education_business")]
        [MccCodes(8244)]
        education_business,

        [LocalizedMccDescription("mcc_education_university")]
        [MccCodes(8220)]
        education_university,

        [LocalizedMccDescription("mcc_electrical_contractors")]
        [MccCodes(1731)]
        electrical_contractors,

        [LocalizedMccDescription("mcc_electronics")]
        [MccCodes(5065)]
        electronics,

        [LocalizedMccDescription("mcc_emergency_services")]
        [MccCodes(9702)]
        emergency_services,

        [LocalizedMccDescription("mcc_employment")]
        [MccCodes(7361)]
        employment,

        [LocalizedMccDescription("mcc_entertainment")]
        [MccCodes(7996)]
        entertainment,

        [LocalizedMccDescription("mcc_entertainment_and_sport")]
        [MccCodes(7997, 7999)]
        entertainment_and_sport,

        [LocalizedMccDescription("mcc_equipment")]
        [MccCodes(5046, 5072)]
        equipment,

        [LocalizedMccDescription("mcc_equipment_rental")]
        [MccCodes(7394)]
        equipment_rental,

        [LocalizedMccDescription("mcc_escort")]
        [MccCodes(7272)]
        escort,

        [LocalizedMccDescription("mcc_farm_goods")]
        [MccCodes(5451)]
        farm_goods,

        [LocalizedMccDescription("mcc_fast_food")]
        [MccCodes(5814)]
        fast_food,

        [LocalizedMccDescription("mcc_financial_services")]
        [MccCodes(6012)]
        financial_services,

        [LocalizedMccDescription("mcc_fines")]
        [MccCodes(9222)]
        fines,

        [LocalizedMccDescription("mcc_fireplaces")]
        [MccCodes(5718)]
        fireplaces,

        [LocalizedMccDescription("mcc_floor_coverings")]
        [MccCodes(5713)]
        floor_coverings,

        [LocalizedMccDescription("mcc_florists")]
        [MccCodes(5992)]
        florists,

        [LocalizedMccDescription("mcc_flowers")]
        [MccCodes(5193)]
        flowers,

        [LocalizedMccDescription("mcc_food_stores")]
        [MccCodes(5499)]
        food_stores,

        [LocalizedMccDescription("mcc_footwear")]
        [MccCodes(5139)]
        footwear,

        [LocalizedMccDescription("mcc_fuel")]
        [MccCodes(5983)]
        fuel,

        [LocalizedMccDescription("mcc_funeral_services")]
        [MccCodes(7261)]
        funeral_services,

        [LocalizedMccDescription("mcc_fur")]
        [MccCodes(5681)]
        fur,

        [LocalizedMccDescription("mcc_furniture")]
        [MccCodes(5021, 5712, 5719)]
        furniture,

        [LocalizedMccDescription("mcc_furniture_repair")]
        [MccCodes(7641)]
        furniture_repair,

        [LocalizedMccDescription("mcc_galleries")]
        [MccCodes(5971)]
        galleries,

        [LocalizedMccDescription("mcc_gambling")]
        [MccCodes(7995)]
        gambling,

        [LocalizedMccDescription("mcc_garden_accessories")]
        [MccCodes(5261)]
        garden_accessories,

        [LocalizedMccDescription("mcc_gas_sales")]
        [MccCodes(5299)]
        gas_sales,

        [LocalizedMccDescription("mcc_gas_station")]
        [MccCodes(5542)]
        gas_station,

        [LocalizedMccDescription("mcc_general_contractors")]
        [MccCodes(1520)]
        general_contractors,

        [LocalizedMccDescription("mcc_golf")]
        [MccCodes(7992)]
        golf,

        [LocalizedMccDescription("mcc_goods")]
        [MccCodes(5099, 5199)]
        goods,

        [LocalizedMccDescription("mcc_goods_by_mail")]
        [MccCodes(5961, 5964, 5965, 5966, 5969)]
        goods_by_mail,

        [LocalizedMccDescription("mcc_government_owned_lottery")]
        [MccCodes(7800, 9406)]
        government_owned_lottery,

        [LocalizedMccDescription("mcc_government_procurement")]
        [MccCodes(9405)]
        government_procurement,

        [LocalizedMccDescription("mcc_government_services")]
        [MccCodes(9399, 9411)]
        government_services,

        [LocalizedMccDescription("mcc_grocery")]
        [MccCodes(5411)]
        grocery,

        [LocalizedMccDescription("mcc_haberdashery")]
        [MccCodes(5131)]
        haberdashery,

        [LocalizedMccDescription("mcc_hardware_stores")]
        [MccCodes(5251)]
        hardware_stores,

        [LocalizedMccDescription("mcc_health_and_beauty")]
        [MccCodes(7298)]
        health_and_beauty,

        [LocalizedMccDescription("mcc_hearing_aids")]
        [MccCodes(5975)]
        hearing_aids,

        [LocalizedMccDescription("mcc_heating_plumbing_a_c")]
        [MccCodes(1711)]
        heating_plumbing_a_c,

        [LocalizedMccDescription("mcc_horse_dog_racing")]
        [MccCodes(7802, 9754)]
        horse_dog_racing,

        [LocalizedMccDescription("mcc_horticultural_and_landscaping")]
        [MccCodes(780)]
        horticultural_and_landscaping,

        [LocalizedMccDescription("mcc_hospitals")]
        [MccCodes(7280, 8062)]
        hospitals,

        [LocalizedMccDescription("mcc_hotels_and_resorts")]
        [MccCodes(3501, 3502, 3503, 3504, 3505, 3506, 3507, 3508, 3509, 3510, 3511, 3512, 3513, 3514, 3515, 3516, 3517, 3518, 3519, 3520, 3521, 3522, 3523, 3524, 3525, 3526, 3527, 3528, 3529, 3530, 3531, 3532, 3533, 3534, 3535, 3536, 3537, 3538, 3539, 3540, 3541, 3542, 3543, 3544, 3545, 3546, 3547, 3548, 3549, 3550, 3551, 3552, 3553, 3554, 3555, 3556, 3557, 3558, 3559, 3560, 3561, 3562, 3563, 3564, 3565, 3566, 3567, 3568, 3569, 3570, 3571, 3572, 3573, 3574, 3575, 3576, 3577, 3578, 3579, 3580, 3581, 3582, 3583, 3584, 3585, 3586, 3587, 3588, 3589, 3590, 3591, 3592, 3593, 3594, 3595, 3596, 3597, 3598, 3599, 3600, 3601, 3602, 3603, 3604, 3605, 3606, 3607, 3608, 3609, 3610, 3611, 3612, 3613, 3614, 3615, 3616, 3617, 3618, 3619, 3620, 3621, 3622, 3623, 3624, 3625, 3626, 3627, 3628, 3629, 3630, 3631, 3632, 3633, 3634, 3635, 3636, 3637, 3638, 3639, 3640, 3641, 3642, 3643, 3644, 3645, 3646, 3647, 3648, 3649, 3650, 3651, 3652, 3653, 3654, 3655, 3656, 3657, 3658, 3659, 3660, 3661, 3662, 3663, 3664, 3665, 3666, 3667, 3668, 3669, 3670, 3671, 3672, 3673, 3674, 3675, 3676, 3677, 3678, 3679, 3680, 3681, 3682, 3683, 3684, 3685, 3686, 3687, 3688, 3689, 3690, 3691, 3692, 3693, 3694, 3695, 3696, 3697, 3698, 3699, 3700, 3701, 3702, 3703, 3704, 3705, 3706, 3707, 3708, 3709, 3710, 3711, 3712, 3713, 3714, 3715, 3716, 3717, 3718, 3719, 3720, 3721, 3722, 3723, 3724, 3725, 3726, 3727, 3728, 3729, 3730, 3731, 3732, 3733, 3734, 3735, 3736, 3737, 3738, 3739, 3740, 3741, 3742, 3743, 3744, 3745, 3746, 3747, 3748, 3749, 3750, 3751, 3752, 3753, 3754, 3755, 3756, 3757, 3758, 3759, 3760, 3761, 3762, 3763, 3764, 3765, 3766, 3767, 3768, 3769, 3770, 3771, 3772, 3773, 3774, 3775, 3776, 3777, 3778, 3779, 3780, 3781, 3782, 3783, 3784, 3785, 3786, 3787, 3788, 3789, 3790, 3791, 3792, 3793, 3794, 3795, 3796, 3797, 3798, 3799, 3800, 3801, 3802, 3803, 3804, 3805, 3806, 3807, 3808, 3809, 3810, 3811, 3812, 3813, 3814, 3815, 3816, 3817, 3818, 3819, 3820, 3821, 3822, 3823, 3824, 3825, 3826, 3827, 3828, 3829, 3830, 3831, 3832, 3833, 3834, 3835, 3836, 3837, 3838, 7011)]
        hotels_and_resorts,

        [LocalizedMccDescription("mcc_household_appliance")]
        [MccCodes(5722, 5732)]
        household_appliance,

        [LocalizedMccDescription("mcc_household_products")]
        [MccCodes(5200)]
        household_products,

        [LocalizedMccDescription("mcc_hvac_equipment_repair")]
        [MccCodes(7623)]
        hvac_equipment_repair,

        [LocalizedMccDescription("mcc_i_purchasing_pilot")]
        [MccCodes(9034, 9401)]
        i_purchasing_pilot,

        [LocalizedMccDescription("mcc_in_company_purchases")]
        [MccCodes(9950)]
        in_company_purchases,

        [LocalizedMccDescription("mcc_industry")]
        [MccCodes(5085)]
        industry,

        [LocalizedMccDescription("mcc_information_services")]
        [MccCodes(4816, 5967, 7375)]
        information_services,

        [LocalizedMccDescription("mcc_insurance")]
        [MccCodes(5960, 6300, 6381, 6399)]
        insurance,

        [LocalizedMccDescription("mcc_jewelry")]
        [MccCodes(5094)]
        jewelry,

        [LocalizedMccDescription("mcc_kindergarten")]
        [MccCodes(8351)]
        kindergarten,

        [LocalizedMccDescription("mcc_laundry")]
        [MccCodes(7211)]
        laundry,

        [LocalizedMccDescription("mcc_lawyers")]
        [MccCodes(8110, 8111)]
        lawyers,

        [LocalizedMccDescription("mcc_leather_products")]
        [MccCodes(5948)]
        leather_products,

        [LocalizedMccDescription("mcc_mail")]
        [MccCodes(9402)]
        mail,

        [LocalizedMccDescription("mcc_maintenance_stations")]
        [MccCodes(7538)]
        maintenance_stations,

        [LocalizedMccDescription("mcc_marketplaces")]
        [MccCodes(5262)]
        marketplaces,

        [LocalizedMccDescription("mcc_masonry_stonework_and_plaster")]
        [MccCodes(1740)]
        masonry_stonework_and_plaster,

        [LocalizedMccDescription("mcc_massage")]
        [MccCodes(7297)]
        massage,

        [LocalizedMccDescription("mcc_meat")]
        [MccCodes(5422)]
        meat,

        [LocalizedMccDescription("mcc_medical_equipment")]
        [MccCodes(5047)]
        medical_equipment,

        [LocalizedMccDescription("mcc_medical_services")]
        [MccCodes(8099)]
        medical_services,

        [LocalizedMccDescription("mcc_medicine")]
        [MccCodes(8011, 8031)]
        medicine,

        [LocalizedMccDescription("mcc_medicine_and_dentistry")]
        [MccCodes(8071)]
        medicine_and_dentistry,

        [LocalizedMccDescription("mcc_mens_clothing")]
        [MccCodes(5611)]
        mens_clothing,

        [LocalizedMccDescription("mcc_merchandise_stores")]
        [MccCodes(5399)]
        merchandise_stores,

        [LocalizedMccDescription("mcc_metal_processing")]
        [MccCodes(5051)]
        metal_processing,

        [LocalizedMccDescription("mcc_miscellaneous")]
        [MccCodes(4304, 5292, 5295, 5415, 5999, 7299, 8664, 9999)]
        miscellaneous,

        [LocalizedMccDescription("mcc_miscellaneous_publishing_and_printing")]
        [MccCodes(2741, 2744)]
        miscellaneous_publishing_and_printing,

        [LocalizedMccDescription("mcc_mobile_connection")]
        [MccCodes(4814)]
        mobile_connection,

        [LocalizedMccDescription("mcc_mobile_homes")]
        [MccCodes(5271)]
        mobile_homes,

        [LocalizedMccDescription("mcc_money_transfer")]
        [MccCodes(4829, 6531, 6532, 6533, 6534, 6535, 6536, 6537, 6538, 6539, 6540, 6611)]
        money_transfer,

        [LocalizedMccDescription("mcc_music_bands_orchestras")]
        [MccCodes(7929)]
        music_bands_orchestras,

        [LocalizedMccDescription("mcc_musical_instruments")]
        [MccCodes(5733)]
        musical_instruments,

        [LocalizedMccDescription("mcc_newspapers_magazines")]
        [MccCodes(5994)]
        newspapers_magazines,

        [LocalizedMccDescription("mcc_office_equipment")]
        [MccCodes(5044)]
        office_equipment,

        [LocalizedMccDescription("mcc_online_casino")]
        [MccCodes(7801)]
        online_casino,

        [LocalizedMccDescription("mcc_optics")]
        [MccCodes(8042, 8043, 8044)]
        optics,

        [LocalizedMccDescription("mcc_organizations_membership")]
        [MccCodes(8699)]
        organizations_membership,

        [LocalizedMccDescription("mcc_organizations_political")]
        [MccCodes(8651)]
        organizations_political,

        [LocalizedMccDescription("mcc_organizations_religious")]
        [MccCodes(8661)]
        organizations_religious,

        [LocalizedMccDescription("mcc_paints")]
        [MccCodes(5198)]
        paints,

        [LocalizedMccDescription("mcc_parking")]
        [MccCodes(7511, 7523, 7524)]
        parking,

        [LocalizedMccDescription("mcc_passenger_railways")]
        [MccCodes(4112)]
        passenger_railways,

        [LocalizedMccDescription("mcc_passenger_transportation")]
        [MccCodes(4111)]
        passenger_transportation,

        [LocalizedMccDescription("mcc_pawn_shops")]
        [MccCodes(5933)]
        pawn_shops,

        [LocalizedMccDescription("mcc_payouts_bonds")]
        [MccCodes(9223)]
        payouts_bonds,

        [LocalizedMccDescription("mcc_pet_supplies")]
        [MccCodes(5995)]
        pet_supplies,

        [LocalizedMccDescription("mcc_petroleum")]
        [MccCodes(5172)]
        petroleum,

        [LocalizedMccDescription("mcc_philatelicism")]
        [MccCodes(5972)]
        philatelicism,

        [LocalizedMccDescription("mcc_photo_goods")]
        [MccCodes(5946)]
        photo_goods,

        [LocalizedMccDescription("mcc_photo_printing")]
        [MccCodes(7395)]
        photo_printing,

        [LocalizedMccDescription("mcc_photographic_studios")]
        [MccCodes(7221)]
        photographic_studios,

        [LocalizedMccDescription("mcc_photography_and_art")]
        [MccCodes(7333)]
        photography_and_art,

        [LocalizedMccDescription("mcc_plumbing")]
        [MccCodes(5074)]
        plumbing,

        [LocalizedMccDescription("mcc_podiatrists")]
        [MccCodes(8049)]
        podiatrists,

        [LocalizedMccDescription("mcc_printing_machines")]
        [MccCodes(5978)]
        printing_machines,

        [LocalizedMccDescription("mcc_professional_services")]
        [MccCodes(8999)]
        professional_services,

        [LocalizedMccDescription("mcc_programming")]
        [MccCodes(7372)]
        programming,

        [LocalizedMccDescription("mcc_prostheses")]
        [MccCodes(5976)]
        prostheses,

        [LocalizedMccDescription("mcc_public_organizations")]
        [MccCodes(8641)]
        public_organizations,

        [LocalizedMccDescription("mcc_quasi_cash")]
        [MccCodes(6050, 6051)]
        quasi_cash,

        [LocalizedMccDescription("mcc_railway")]
        [MccCodes(4011, 4789)]
        railway,

        [LocalizedMccDescription("mcc_razors")]
        [MccCodes(5997)]
        razors,

        [LocalizedMccDescription("mcc_record_shops")]
        [MccCodes(5735)]
        record_shops,

        [LocalizedMccDescription("mcc_recreation")]
        [MccCodes(7032)]
        recreation,

        [LocalizedMccDescription("mcc_renovation")]
        [MccCodes(5231)]
        renovation,

        [LocalizedMccDescription("mcc_rental_property")]
        [MccCodes(6513)]
        rental_property,

        [LocalizedMccDescription("mcc_repair_of_equipment")]
        [MccCodes(7622, 7629)]
        repair_of_equipment,

        [LocalizedMccDescription("mcc_repair_of_watches_and_jewelry")]
        [MccCodes(7631)]
        repair_of_watches_and_jewelry,

        [LocalizedMccDescription("mcc_repairs")]
        [MccCodes(7699)]
        repairs,

        [LocalizedMccDescription("mcc_reproduction_stores")]
        [MccCodes(5937)]
        reproduction_stores,

        [LocalizedMccDescription("mcc_retail_outlets_with_telephony")]
        [MccCodes(4813)]
        retail_outlets_with_telephony,

        [LocalizedMccDescription("mcc_retail_stores")]
        [MccCodes(5297, 5298)]
        retail_stores,

        [LocalizedMccDescription("mcc_roofing_siding_sheet_metal")]
        [MccCodes(1761)]
        roofing_siding_sheet_metal,

        [LocalizedMccDescription("mcc_rubber_stamp")]
        [MccCodes(5974)]
        rubber_stamp,

        [LocalizedMccDescription("mcc_salesmen")]
        [MccCodes(5963)]
        salesmen,

        [LocalizedMccDescription("mcc_school")]
        [MccCodes(8211)]
        school,

        [LocalizedMccDescription("mcc_schools_correspondence")]
        [MccCodes(8241)]
        schools_correspondence,

        [LocalizedMccDescription("mcc_second_hand")]
        [MccCodes(5931)]
        second_hand,

        [LocalizedMccDescription("mcc_securities")]
        [MccCodes(6211, 6236)]
        securities,

        [LocalizedMccDescription("mcc_service_stations")]
        [MccCodes(5541)]
        service_stations,

        [LocalizedMccDescription("mcc_sewing_supplies")]
        [MccCodes(5949)]
        sewing_supplies,

        [LocalizedMccDescription("mcc_shoes")]
        [MccCodes(5661)]
        shoes,

        [LocalizedMccDescription("mcc_shopping")]
        [MccCodes(7278)]
        shopping,

        [LocalizedMccDescription("mcc_souvenirs")]
        [MccCodes(5947)]
        souvenirs,

        [LocalizedMccDescription("mcc_special_trade_contractors")]
        [MccCodes(1799)]
        special_trade_contractors,

        [LocalizedMccDescription("mcc_specialty_cleaning")]
        [MccCodes(2842)]
        specialty_cleaning,

        [LocalizedMccDescription("mcc_sports_clubs")]
        [MccCodes(7941)]
        sports_clubs,

        [LocalizedMccDescription("mcc_sports_goods")]
        [MccCodes(5941)]
        sports_goods,

        [LocalizedMccDescription("mcc_sportswear")]
        [MccCodes(5655)]
        sportswear,

        [LocalizedMccDescription("mcc_stationery")]
        [MccCodes(5943)]
        stationery,

        [LocalizedMccDescription("mcc_stenography")]
        [MccCodes(7339)]
        stenography,

        [LocalizedMccDescription("mcc_storage")]
        [MccCodes(4225)]
        storage,

        [LocalizedMccDescription("mcc_subscriptions")]
        [MccCodes(5968)]
        subscriptions,

        [LocalizedMccDescription("mcc_sweets")]
        [MccCodes(5441)]
        sweets,

        [LocalizedMccDescription("mcc_swimming_pools")]
        [MccCodes(5996)]
        swimming_pools,

        [LocalizedMccDescription("mcc_taxes")]
        [MccCodes(7276, 9311)]
        taxes,

        [LocalizedMccDescription("mcc_taxi")]
        [MccCodes(4121)]
        taxi,

        [LocalizedMccDescription("mcc_telecommunication_equipment")]
        [MccCodes(4812)]
        telecommunication_equipment,

        [LocalizedMccDescription("mcc_telegraph")]
        [MccCodes(4821)]
        telegraph,

        [LocalizedMccDescription("mcc_telemarketing")]
        [MccCodes(4761)]
        telemarketing,

        [LocalizedMccDescription("mcc_telephone_services")]
        [MccCodes(4815)]
        telephone_services,

        [LocalizedMccDescription("mcc_tents")]
        [MccCodes(5998)]
        tents,

        [LocalizedMccDescription("mcc_testing_laboratories")]
        [MccCodes(8734, 8743)]
        testing_laboratories,

        [LocalizedMccDescription("mcc_the_beauty")]
        [MccCodes(7230)]
        the_beauty,

        [LocalizedMccDescription("mcc_the_television")]
        [MccCodes(4899)]
        the_television,

        [LocalizedMccDescription("mcc_tickets")]
        [MccCodes(7922)]
        tickets,

        [LocalizedMccDescription("mcc_timeshares")]
        [MccCodes(7012)]
        timeshares,

        [LocalizedMccDescription("mcc_tire_service")]
        [MccCodes(7534)]
        tire_service,

        [LocalizedMccDescription("mcc_tires")]
        [MccCodes(5532)]
        tires,

        [LocalizedMccDescription("mcc_tobacco_products")]
        [MccCodes(5993)]
        tobacco_products,

        [LocalizedMccDescription("mcc_toll_roads")]
        [MccCodes(4784)]
        toll_roads,

        [LocalizedMccDescription("mcc_tour_operators")]
        [MccCodes(4723)]
        tour_operators,

        [LocalizedMccDescription("mcc_tourism")]
        [MccCodes(4722, 7991)]
        tourism,

        [LocalizedMccDescription("mcc_tow_truck")]
        [MccCodes(7549)]
        tow_truck,

        [LocalizedMccDescription("mcc_toys")]
        [MccCodes(5945)]
        toys,

        [LocalizedMccDescription("mcc_transportation_bus")]
        [MccCodes(4131)]
        transportation_bus,

        [LocalizedMccDescription("mcc_transportation_delivery")]
        [MccCodes(4214)]
        transportation_delivery,

        [LocalizedMccDescription("mcc_transportation_services")]
        [MccCodes(4729)]
        transportation_services,

        [LocalizedMccDescription("mcc_travels")]
        [MccCodes(5962)]
        travels,

        [LocalizedMccDescription("mcc_truck_rental")]
        [MccCodes(7513)]
        truck_rental,

        [LocalizedMccDescription("mcc_typesetting_plate_making")]
        [MccCodes(2791)]
        typesetting_plate_making,

        [LocalizedMccDescription("mcc_utilities")]
        [MccCodes(4900)]
        utilities,

        [LocalizedMccDescription("mcc_variety_stores")]
        [MccCodes(5331)]
        variety_stores,

        [LocalizedMccDescription("mcc_veterinary_services")]
        [MccCodes(742)]
        veterinary_services,

        [LocalizedMccDescription("mcc_video_rental")]
        [MccCodes(7829, 7841)]
        video_rental,

        [LocalizedMccDescription("mcc_videogames")]
        [MccCodes(7993, 7994)]
        videogames,

        [LocalizedMccDescription("mcc_visa")]
        [MccCodes(9701)]
        visa,

        [LocalizedMccDescription("mcc_welding_works")]
        [MccCodes(7692)]
        welding_works,

        [LocalizedMccDescription("mcc_wholesalers")]
        [MccCodes(5300)]
        wholesalers,

        [LocalizedMccDescription("mcc_wigs")]
        [MccCodes(5698)]
        wigs,

        [LocalizedMccDescription("mcc_wine_producers")]
        [MccCodes(743)]
        wine_producers,

        [LocalizedMccDescription("mcc_womens_clothing")]
        [MccCodes(5621)]
        womens_clothing,

        [LocalizedMccDescription("mcc_yachting_service")]
        [MccCodes(4468)]
        yachting_service
    }
}
