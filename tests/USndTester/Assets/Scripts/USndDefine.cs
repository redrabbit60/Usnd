// USndDefine generate:9/24/2015 11:37:12 AM


namespace USnd {
  public class USndDefine {

      public enum Master {
          BGM = 0,
          SE = 1,
          VOICE = 2,

          Master_Max = 3,
      }

      static public readonly string[] MasterName = new string[] {
          "BGM",
          "SE",
          "VOICE",
      };

      public enum Category {
          BGM = 0,
          SE_DUEL = 1,
          SE_EFX = 2,
          SE_UI = 3,
          VOICE = 4,

          Category_Max = 5,
      }

      static public readonly string[] CategoryName = new string[] {
          "BGM",
          "SE_DUEL",
          "SE_EFX",
          "SE_UI",
          "VOICE",
      };

      public enum Label {
          NON = -1,
          bgm_cl_battle = 0,
          BGM_TEST = 1,
          se_attack = 2,
          se_card_break = 3,
          se_card_move_01 = 4,
          se_card_set = 5,
          se_cut_in_01 = 6,
          se_cut_in_02 = 7,
          se_draw = 8,
          se_field_card_up = 9,
          se_lp_count_loop = 10,
          se_lp_count_stop = 11,
          se_lp_zero = 12,
          se_player_damage = 13,
          se_summon = 14,
          se_turn_change = 15,
          VA02_01_00_00_0 = 16,
          VA02_01_00_00_1 = 17,
          VA02_02_00_00_0 = 18,
          VA02_03_00_00_0 = 19,
          VA02_03_00_00_1 = 20,
          VA02_03_00_01_0 = 21,
          VA02_03_00_01_1 = 22,
          VA02_03_00_02_0 = 23,
          VA02_03_00_02_1 = 24,
          VA02_03_00_03_0 = 25,
          VA02_03_00_03_1 = 26,
          VA02_03_00_04_0 = 27,
          VA02_03_00_04_1 = 28,
          VA02_03_00_05_0 = 29,
          VA02_03_00_05_1 = 30,
          VA02_04_00_00_0 = 31,
          VA02_04_00_01_0 = 32,
          VA02_04_00_02_0 = 33,
          VA02_05_00_00_0 = 34,
          VA02_05_00_00_1 = 35,
          VA02_06_01_00_0 = 36,
          VA02_06_01_01_0 = 37,
          VA02_06_01_02_0 = 38,
          VA02_06_01_03_0 = 39,
          VA02_07_00_00_0 = 40,
          VA02_07_01_00_0 = 41,
          VA02_07_02_00_0 = 42,
          VA02_07_03_00_0 = 43,
          VA02_07_04_00_0 = 44,
          VA02_07_05_00_0 = 45,
          VA02_07_06_00_0 = 46,
          VA02_07_07_00_0 = 47,
          VA02_07_08_00_0 = 48,
          VA02_07_09_00_0 = 49,
          VA02_07_09_01_0 = 50,
          VA02_07_10_00_0 = 51,
          VA02_07_11_00_0 = 52,
          VA02_08_00_00_0 = 53,
          VA02_08_00_01_0 = 54,
          VA02_08_00_01_1 = 55,
          VA02_08_00_02_0 = 56,
          VA02_08_00_03_0 = 57,
          VA02_08_00_03_1 = 58,
          VA02_08_00_04_0 = 59,
          VA02_08_00_04_1 = 60,
          VA02_08_00_05_0 = 61,
          VA02_08_00_06_0 = 62,
          VA02_08_00_07_0 = 63,
          VA02_09_00_00_0 = 64,
          VA02_09_00_00_1 = 65,
          VA02_09_00_01_0 = 66,
          VA02_09_00_01_1 = 67,
          VA02_09_00_02_0 = 68,
          VA02_09_00_03_0 = 69,
          VA02_09_00_03_1 = 70,
          VA02_09_00_04_0 = 71,
          VA02_09_00_04_1 = 72,
          VA02_10_00_00_0 = 73,
          VA02_10_00_01_0 = 74,
          VA02_10_00_02_0 = 75,
          VA02_10_00_03_0 = 76,
          VA02_11_00_00_0 = 77,
          VA02_11_01_00_0 = 78,
          VA02_11_02_00_0 = 79,
          VA02_11_03_00_0 = 80,
          VA02_11_04_00_0 = 81,
          VA02_11_05_00_0 = 82,
          VA02_11_06_00_0 = 83,
          VA02_11_07_00_0 = 84,
          VA02_11_08_00_0 = 85,
          VA02_11_09_00_0 = 86,
          VA02_13_00_00_0 = 87,
          VA02_13_00_00_1 = 88,
          VA02_13_00_01_0 = 89,
          VA02_13_00_01_1 = 90,
          VA02_13_00_02_0 = 91,
          VA02_13_00_02_1 = 92,
          VA02_13_00_02_2 = 93,
          VA02_13_00_03_0 = 94,
          VA02_13_00_04_0 = 95,
          VA02_13_00_05_0 = 96,
          VA02_13_00_06_0 = 97,
          VA02_13_00_07_0 = 98,
          VA02_13_00_08_0 = 99,
          VA02_13_00_09_0 = 100,
          VA02_14_00_00_0 = 101,
          VA02_14_00_01_0 = 102,
          VA02_15_00_00_0 = 103,
          VA02_15_00_01_0 = 104,
          VA02_15_00_02_0 = 105,
          VA02_15_00_03_0 = 106,
          VA02_16_00_00_0 = 107,
          VA02_16_00_01_0 = 108,
          VA02_17_00_00_0 = 109,
          VA02_17_00_01_0 = 110,
          VA02_18_00_00_0 = 111,
          VA02_18_00_01_0 = 112,
          VA02_19_00_00_0 = 113,
          VA02_19_00_00_1 = 114,
          VA02_19_00_01_0 = 115,
          VA02_19_00_01_1 = 116,
          VA02_19_00_02_0 = 117,
          VA02_19_00_02_1 = 118,
          VA02_19_00_03_0 = 119,
          VA02_19_00_03_1 = 120,
          VA02_19_00_04_0 = 121,
          VA02_19_00_04_1 = 122,
          VA02_19_00_05_0 = 123,
          VA02_19_00_05_1 = 124,
          VA02_20_00_00_0 = 125,
          VA02_20_00_01_0 = 126,
          VA02_20_01_00_0 = 127,
          VA02_20_01_01_0 = 128,
          VA02_21_00_00_0 = 129,
          VA02_21_00_01_0 = 130,
          VA02_21_00_02_0 = 131,
          VA02_22_00_03_0 = 132,
          VA02_22_00_04_0 = 133,
          VA02_22_00_05_0 = 134,
          VA02_22_00_06_0 = 135,
          VA02_22_00_07_0 = 136,
          VA02_22_00_08_0 = 137,
          VA02_23_00_00_0 = 138,
          VA02_24_00_00_0 = 139,
          VA02_26_00_00_0 = 140,
          VA02_26_00_01_0 = 141,
          VA02_26_00_02_0 = 142,
          VA02_27_00_00_0 = 143,
          VA02_27_00_01_0 = 144,
          VA02_27_00_02_0 = 145,
          VA02_28_00_00_0 = 146,
          VA02_28_00_00_1 = 147,
          VA02_29_00_00_0 = 148,
          VA02_29_00_00_1 = 149,
          VA02_30_00_00_0 = 150,
          VA02_30_00_01_0 = 151,
          VA02_31_00_00_0 = 152,
          VA02_31_00_01_0 = 153,
          VA02_32_00_00_0 = 154,
          VA02_32_00_01_0 = 155,
          VA01_01_00_00_0 = 156,
          VA01_02_00_00_0 = 157,
          VA01_03_00_00_0 = 158,
          VA01_03_00_00_1 = 159,
          VA01_03_00_01_0 = 160,
          VA01_03_00_01_1 = 161,
          VA01_03_00_02_0 = 162,
          VA01_03_00_02_1 = 163,
          VA01_03_00_03_0 = 164,
          VA01_03_00_04_0 = 165,
          VA01_03_00_04_1 = 166,
          VA01_03_00_05_0 = 167,
          VA01_03_00_05_1 = 168,
          VA01_04_00_00_0 = 169,
          VA01_04_00_01_0 = 170,
          VA01_04_00_02_0 = 171,
          VA01_05_00_00_0 = 172,
          VA01_05_00_00_1 = 173,
          VA01_06_01_00_0 = 174,
          VA01_06_01_01_0 = 175,
          VA01_06_01_02_0 = 176,
          VA01_06_01_03_0 = 177,
          VA01_07_00_00_0 = 178,
          VA01_07_01_00_0 = 179,
          VA01_07_02_00_0 = 180,
          VA01_07_03_00_0 = 181,
          VA01_07_04_00_0 = 182,
          VA01_07_05_00_0 = 183,
          VA01_07_06_00_0 = 184,
          VA01_07_07_00_0 = 185,
          VA01_07_08_00_0 = 186,
          VA01_07_09_00_0 = 187,
          VA01_07_09_01_0 = 188,
          VA01_07_10_00_0 = 189,
          VA01_07_11_00_0 = 190,
          VA01_08_00_00_0 = 191,
          VA01_08_00_01_0 = 192,
          VA01_08_00_02_0 = 193,
          VA01_08_00_03_0 = 194,
          VA01_08_00_04_0 = 195,
          VA01_08_00_05_0 = 196,
          VA01_08_00_06_0 = 197,
          VA01_08_00_06_1 = 198,
          VA01_08_00_07_0 = 199,
          VA01_08_00_08_0 = 200,
          VA01_08_00_08_1 = 201,
          VA01_08_00_09_0 = 202,
          VA01_08_00_09_1 = 203,
          VA01_08_00_10_0 = 204,
          VA01_08_00_11_0 = 205,
          VA01_08_00_11_1 = 206,
          VA01_08_00_12_0 = 207,
          VA01_08_00_13_0 = 208,
          VA01_08_00_13_1 = 209,
          VA01_08_00_14_0 = 210,
          VA01_08_00_14_1 = 211,
          VA01_08_00_15_0 = 212,
          VA01_08_00_15_1 = 213,
          VA01_08_00_16_0 = 214,
          VA01_08_00_17_0 = 215,
          VA01_08_00_18_0 = 216,
          VA01_08_00_19_0 = 217,
          VA01_08_00_19_1 = 218,
          VA01_09_00_00_0 = 219,
          VA01_09_00_00_1 = 220,
          VA01_09_00_01_0 = 221,
          VA01_09_00_02_0 = 222,
          VA01_09_00_02_1 = 223,
          VA01_09_00_03_0 = 224,
          VA01_09_00_03_1 = 225,
          VA01_09_00_04_0 = 226,
          VA01_09_00_05_0 = 227,
          VA01_09_00_05_1 = 228,
          VA01_09_00_06_0 = 229,
          VA01_10_00_00_0 = 230,
          VA01_10_00_01_0 = 231,
          VA01_10_00_02_0 = 232,
          VA01_10_00_03_0 = 233,
          VA01_11_00_00_0 = 234,
          VA01_11_01_00_0 = 235,
          VA01_11_02_00_0 = 236,
          VA01_11_03_00_0 = 237,
          VA01_11_04_00_0 = 238,
          VA01_11_05_00_0 = 239,
          VA01_11_06_00_0 = 240,
          VA01_11_07_00_0 = 241,
          VA01_11_08_00_0 = 242,
          VA01_11_09_00_0 = 243,
          VA01_13_00_00_0 = 244,
          VA01_13_00_01_0 = 245,
          VA01_13_00_01_1 = 246,
          VA01_13_00_02_0 = 247,
          VA01_13_00_02_1 = 248,
          VA01_13_00_03_0 = 249,
          VA01_13_00_04_0 = 250,
          VA01_13_00_05_0 = 251,
          VA01_13_00_06_0 = 252,
          VA01_13_00_06_1 = 253,
          VA01_13_00_06_2 = 254,
          VA01_13_00_07_0 = 255,
          VA01_13_00_08_0 = 256,
          VA01_13_00_09_0 = 257,
          VA01_13_00_10_0 = 258,
          VA01_13_00_11_0 = 259,
          VA01_13_00_12_0 = 260,
          VA01_13_00_13_0 = 261,
          VA01_13_00_14_0 = 262,
          VA01_13_00_15_0 = 263,
          VA01_13_00_16_0 = 264,
          VA01_13_00_16_1 = 265,
          VA01_13_00_16_2 = 266,
          VA01_14_00_00_0 = 267,
          VA01_14_00_01_0 = 268,
          VA01_15_00_00_0 = 269,
          VA01_15_00_01_0 = 270,
          VA01_15_00_02_0 = 271,
          VA01_15_00_03_0 = 272,
          VA01_16_00_00_0 = 273,
          VA01_16_00_01_0 = 274,
          VA01_17_00_00_0 = 275,
          VA01_17_00_01_0 = 276,
          VA01_18_00_00_0 = 277,
          VA01_18_00_01_0 = 278,
          VA01_19_00_00_0 = 279,
          VA01_19_00_00_1 = 280,
          VA01_19_00_01_0 = 281,
          VA01_19_00_01_1 = 282,
          VA01_19_00_02_0 = 283,
          VA01_19_00_02_1 = 284,
          VA01_19_00_03_0 = 285,
          VA01_19_00_03_1 = 286,
          VA01_19_00_04_0 = 287,
          VA01_19_00_04_1 = 288,
          VA01_19_00_05_0 = 289,
          VA01_19_00_05_1 = 290,
          VA01_19_00_06_0 = 291,
          VA01_19_00_06_1 = 292,
          VA01_19_00_07_0 = 293,
          VA01_19_00_07_1 = 294,
          VA01_19_00_08_0 = 295,
          VA01_19_00_08_1 = 296,
          VA01_19_00_09_0 = 297,
          VA01_19_00_09_1 = 298,
          VA01_20_00_00_0 = 299,
          VA01_20_00_01_0 = 300,
          VA01_20_01_00_0 = 301,
          VA01_20_01_01_0 = 302,
          VA01_21_00_00_0 = 303,
          VA01_21_00_01_0 = 304,
          VA01_21_00_02_0 = 305,
          VA01_22_00_03_0 = 306,
          VA01_22_00_04_0 = 307,
          VA01_22_00_05_0 = 308,
          VA01_22_00_06_0 = 309,
          VA01_22_00_07_0 = 310,
          VA01_22_00_08_0 = 311,
          VA01_23_00_00_0 = 312,
          VA01_24_00_00_0 = 313,
          VA01_26_00_00_0 = 314,
          VA01_26_00_01_0 = 315,
          VA01_26_00_02_0 = 316,
          VA01_27_00_00_0 = 317,
          VA01_27_00_01_0 = 318,
          VA01_27_00_02_0 = 319,
          VA01_28_00_00_0 = 320,
          VA01_29_00_00_0 = 321,
          VA01_30_00_00_0 = 322,
          VA01_30_00_01_0 = 323,
          VA01_31_00_00_0 = 324,
          VA01_31_00_01_0 = 325,
          VA01_32_00_00_0 = 326,
          VA01_32_00_01_0 = 327,

          Label_Max = 328,
      }

      static public readonly string[] LabelName = new string[] {
          "bgm_cl_battle",
          "BGM_TEST",
          "se_attack",
          "se_card_break",
          "se_card_move_01",
          "se_card_set",
          "se_cut_in_01",
          "se_cut_in_02",
          "se_draw",
          "se_field_card_up",
          "se_lp_count_loop",
          "se_lp_count_stop",
          "se_lp_zero",
          "se_player_damage",
          "se_summon",
          "se_turn_change",
          "VA02_01_00_00_0",
          "VA02_01_00_00_1",
          "VA02_02_00_00_0",
          "VA02_03_00_00_0",
          "VA02_03_00_00_1",
          "VA02_03_00_01_0",
          "VA02_03_00_01_1",
          "VA02_03_00_02_0",
          "VA02_03_00_02_1",
          "VA02_03_00_03_0",
          "VA02_03_00_03_1",
          "VA02_03_00_04_0",
          "VA02_03_00_04_1",
          "VA02_03_00_05_0",
          "VA02_03_00_05_1",
          "VA02_04_00_00_0",
          "VA02_04_00_01_0",
          "VA02_04_00_02_0",
          "VA02_05_00_00_0",
          "VA02_05_00_00_1",
          "VA02_06_01_00_0",
          "VA02_06_01_01_0",
          "VA02_06_01_02_0",
          "VA02_06_01_03_0",
          "VA02_07_00_00_0",
          "VA02_07_01_00_0",
          "VA02_07_02_00_0",
          "VA02_07_03_00_0",
          "VA02_07_04_00_0",
          "VA02_07_05_00_0",
          "VA02_07_06_00_0",
          "VA02_07_07_00_0",
          "VA02_07_08_00_0",
          "VA02_07_09_00_0",
          "VA02_07_09_01_0",
          "VA02_07_10_00_0",
          "VA02_07_11_00_0",
          "VA02_08_00_00_0",
          "VA02_08_00_01_0",
          "VA02_08_00_01_1",
          "VA02_08_00_02_0",
          "VA02_08_00_03_0",
          "VA02_08_00_03_1",
          "VA02_08_00_04_0",
          "VA02_08_00_04_1",
          "VA02_08_00_05_0",
          "VA02_08_00_06_0",
          "VA02_08_00_07_0",
          "VA02_09_00_00_0",
          "VA02_09_00_00_1",
          "VA02_09_00_01_0",
          "VA02_09_00_01_1",
          "VA02_09_00_02_0",
          "VA02_09_00_03_0",
          "VA02_09_00_03_1",
          "VA02_09_00_04_0",
          "VA02_09_00_04_1",
          "VA02_10_00_00_0",
          "VA02_10_00_01_0",
          "VA02_10_00_02_0",
          "VA02_10_00_03_0",
          "VA02_11_00_00_0",
          "VA02_11_01_00_0",
          "VA02_11_02_00_0",
          "VA02_11_03_00_0",
          "VA02_11_04_00_0",
          "VA02_11_05_00_0",
          "VA02_11_06_00_0",
          "VA02_11_07_00_0",
          "VA02_11_08_00_0",
          "VA02_11_09_00_0",
          "VA02_13_00_00_0",
          "VA02_13_00_00_1",
          "VA02_13_00_01_0",
          "VA02_13_00_01_1",
          "VA02_13_00_02_0",
          "VA02_13_00_02_1",
          "VA02_13_00_02_2",
          "VA02_13_00_03_0",
          "VA02_13_00_04_0",
          "VA02_13_00_05_0",
          "VA02_13_00_06_0",
          "VA02_13_00_07_0",
          "VA02_13_00_08_0",
          "VA02_13_00_09_0",
          "VA02_14_00_00_0",
          "VA02_14_00_01_0",
          "VA02_15_00_00_0",
          "VA02_15_00_01_0",
          "VA02_15_00_02_0",
          "VA02_15_00_03_0",
          "VA02_16_00_00_0",
          "VA02_16_00_01_0",
          "VA02_17_00_00_0",
          "VA02_17_00_01_0",
          "VA02_18_00_00_0",
          "VA02_18_00_01_0",
          "VA02_19_00_00_0",
          "VA02_19_00_00_1",
          "VA02_19_00_01_0",
          "VA02_19_00_01_1",
          "VA02_19_00_02_0",
          "VA02_19_00_02_1",
          "VA02_19_00_03_0",
          "VA02_19_00_03_1",
          "VA02_19_00_04_0",
          "VA02_19_00_04_1",
          "VA02_19_00_05_0",
          "VA02_19_00_05_1",
          "VA02_20_00_00_0",
          "VA02_20_00_01_0",
          "VA02_20_01_00_0",
          "VA02_20_01_01_0",
          "VA02_21_00_00_0",
          "VA02_21_00_01_0",
          "VA02_21_00_02_0",
          "VA02_22_00_03_0",
          "VA02_22_00_04_0",
          "VA02_22_00_05_0",
          "VA02_22_00_06_0",
          "VA02_22_00_07_0",
          "VA02_22_00_08_0",
          "VA02_23_00_00_0",
          "VA02_24_00_00_0",
          "VA02_26_00_00_0",
          "VA02_26_00_01_0",
          "VA02_26_00_02_0",
          "VA02_27_00_00_0",
          "VA02_27_00_01_0",
          "VA02_27_00_02_0",
          "VA02_28_00_00_0",
          "VA02_28_00_00_1",
          "VA02_29_00_00_0",
          "VA02_29_00_00_1",
          "VA02_30_00_00_0",
          "VA02_30_00_01_0",
          "VA02_31_00_00_0",
          "VA02_31_00_01_0",
          "VA02_32_00_00_0",
          "VA02_32_00_01_0",
          "VA01_01_00_00_0",
          "VA01_02_00_00_0",
          "VA01_03_00_00_0",
          "VA01_03_00_00_1",
          "VA01_03_00_01_0",
          "VA01_03_00_01_1",
          "VA01_03_00_02_0",
          "VA01_03_00_02_1",
          "VA01_03_00_03_0",
          "VA01_03_00_04_0",
          "VA01_03_00_04_1",
          "VA01_03_00_05_0",
          "VA01_03_00_05_1",
          "VA01_04_00_00_0",
          "VA01_04_00_01_0",
          "VA01_04_00_02_0",
          "VA01_05_00_00_0",
          "VA01_05_00_00_1",
          "VA01_06_01_00_0",
          "VA01_06_01_01_0",
          "VA01_06_01_02_0",
          "VA01_06_01_03_0",
          "VA01_07_00_00_0",
          "VA01_07_01_00_0",
          "VA01_07_02_00_0",
          "VA01_07_03_00_0",
          "VA01_07_04_00_0",
          "VA01_07_05_00_0",
          "VA01_07_06_00_0",
          "VA01_07_07_00_0",
          "VA01_07_08_00_0",
          "VA01_07_09_00_0",
          "VA01_07_09_01_0",
          "VA01_07_10_00_0",
          "VA01_07_11_00_0",
          "VA01_08_00_00_0",
          "VA01_08_00_01_0",
          "VA01_08_00_02_0",
          "VA01_08_00_03_0",
          "VA01_08_00_04_0",
          "VA01_08_00_05_0",
          "VA01_08_00_06_0",
          "VA01_08_00_06_1",
          "VA01_08_00_07_0",
          "VA01_08_00_08_0",
          "VA01_08_00_08_1",
          "VA01_08_00_09_0",
          "VA01_08_00_09_1",
          "VA01_08_00_10_0",
          "VA01_08_00_11_0",
          "VA01_08_00_11_1",
          "VA01_08_00_12_0",
          "VA01_08_00_13_0",
          "VA01_08_00_13_1",
          "VA01_08_00_14_0",
          "VA01_08_00_14_1",
          "VA01_08_00_15_0",
          "VA01_08_00_15_1",
          "VA01_08_00_16_0",
          "VA01_08_00_17_0",
          "VA01_08_00_18_0",
          "VA01_08_00_19_0",
          "VA01_08_00_19_1",
          "VA01_09_00_00_0",
          "VA01_09_00_00_1",
          "VA01_09_00_01_0",
          "VA01_09_00_02_0",
          "VA01_09_00_02_1",
          "VA01_09_00_03_0",
          "VA01_09_00_03_1",
          "VA01_09_00_04_0",
          "VA01_09_00_05_0",
          "VA01_09_00_05_1",
          "VA01_09_00_06_0",
          "VA01_10_00_00_0",
          "VA01_10_00_01_0",
          "VA01_10_00_02_0",
          "VA01_10_00_03_0",
          "VA01_11_00_00_0",
          "VA01_11_01_00_0",
          "VA01_11_02_00_0",
          "VA01_11_03_00_0",
          "VA01_11_04_00_0",
          "VA01_11_05_00_0",
          "VA01_11_06_00_0",
          "VA01_11_07_00_0",
          "VA01_11_08_00_0",
          "VA01_11_09_00_0",
          "VA01_13_00_00_0",
          "VA01_13_00_01_0",
          "VA01_13_00_01_1",
          "VA01_13_00_02_0",
          "VA01_13_00_02_1",
          "VA01_13_00_03_0",
          "VA01_13_00_04_0",
          "VA01_13_00_05_0",
          "VA01_13_00_06_0",
          "VA01_13_00_06_1",
          "VA01_13_00_06_2",
          "VA01_13_00_07_0",
          "VA01_13_00_08_0",
          "VA01_13_00_09_0",
          "VA01_13_00_10_0",
          "VA01_13_00_11_0",
          "VA01_13_00_12_0",
          "VA01_13_00_13_0",
          "VA01_13_00_14_0",
          "VA01_13_00_15_0",
          "VA01_13_00_16_0",
          "VA01_13_00_16_1",
          "VA01_13_00_16_2",
          "VA01_14_00_00_0",
          "VA01_14_00_01_0",
          "VA01_15_00_00_0",
          "VA01_15_00_01_0",
          "VA01_15_00_02_0",
          "VA01_15_00_03_0",
          "VA01_16_00_00_0",
          "VA01_16_00_01_0",
          "VA01_17_00_00_0",
          "VA01_17_00_01_0",
          "VA01_18_00_00_0",
          "VA01_18_00_01_0",
          "VA01_19_00_00_0",
          "VA01_19_00_00_1",
          "VA01_19_00_01_0",
          "VA01_19_00_01_1",
          "VA01_19_00_02_0",
          "VA01_19_00_02_1",
          "VA01_19_00_03_0",
          "VA01_19_00_03_1",
          "VA01_19_00_04_0",
          "VA01_19_00_04_1",
          "VA01_19_00_05_0",
          "VA01_19_00_05_1",
          "VA01_19_00_06_0",
          "VA01_19_00_06_1",
          "VA01_19_00_07_0",
          "VA01_19_00_07_1",
          "VA01_19_00_08_0",
          "VA01_19_00_08_1",
          "VA01_19_00_09_0",
          "VA01_19_00_09_1",
          "VA01_20_00_00_0",
          "VA01_20_00_01_0",
          "VA01_20_01_00_0",
          "VA01_20_01_01_0",
          "VA01_21_00_00_0",
          "VA01_21_00_01_0",
          "VA01_21_00_02_0",
          "VA01_22_00_03_0",
          "VA01_22_00_04_0",
          "VA01_22_00_05_0",
          "VA01_22_00_06_0",
          "VA01_22_00_07_0",
          "VA01_22_00_08_0",
          "VA01_23_00_00_0",
          "VA01_24_00_00_0",
          "VA01_26_00_00_0",
          "VA01_26_00_01_0",
          "VA01_26_00_02_0",
          "VA01_27_00_00_0",
          "VA01_27_00_01_0",
          "VA01_27_00_02_0",
          "VA01_28_00_00_0",
          "VA01_29_00_00_0",
          "VA01_30_00_00_0",
          "VA01_30_00_01_0",
          "VA01_31_00_00_0",
          "VA01_31_00_01_0",
          "VA01_32_00_00_0",
          "VA01_32_00_01_0",
      };

      public class LabelString {
          static public readonly string NON = Label.NON.ToString();
          static public readonly string bgm_cl_battle = Label.bgm_cl_battle.ToString();
          static public readonly string BGM_TEST = Label.BGM_TEST.ToString();
          static public readonly string se_attack = Label.se_attack.ToString();
          static public readonly string se_card_break = Label.se_card_break.ToString();
          static public readonly string se_card_move_01 = Label.se_card_move_01.ToString();
          static public readonly string se_card_set = Label.se_card_set.ToString();
          static public readonly string se_cut_in_01 = Label.se_cut_in_01.ToString();
          static public readonly string se_cut_in_02 = Label.se_cut_in_02.ToString();
          static public readonly string se_draw = Label.se_draw.ToString();
          static public readonly string se_field_card_up = Label.se_field_card_up.ToString();
          static public readonly string se_lp_count_loop = Label.se_lp_count_loop.ToString();
          static public readonly string se_lp_count_stop = Label.se_lp_count_stop.ToString();
          static public readonly string se_lp_zero = Label.se_lp_zero.ToString();
          static public readonly string se_player_damage = Label.se_player_damage.ToString();
          static public readonly string se_summon = Label.se_summon.ToString();
          static public readonly string se_turn_change = Label.se_turn_change.ToString();
          static public readonly string VA02_01_00_00_0 = Label.VA02_01_00_00_0.ToString();
          static public readonly string VA02_01_00_00_1 = Label.VA02_01_00_00_1.ToString();
          static public readonly string VA02_02_00_00_0 = Label.VA02_02_00_00_0.ToString();
          static public readonly string VA02_03_00_00_0 = Label.VA02_03_00_00_0.ToString();
          static public readonly string VA02_03_00_00_1 = Label.VA02_03_00_00_1.ToString();
          static public readonly string VA02_03_00_01_0 = Label.VA02_03_00_01_0.ToString();
          static public readonly string VA02_03_00_01_1 = Label.VA02_03_00_01_1.ToString();
          static public readonly string VA02_03_00_02_0 = Label.VA02_03_00_02_0.ToString();
          static public readonly string VA02_03_00_02_1 = Label.VA02_03_00_02_1.ToString();
          static public readonly string VA02_03_00_03_0 = Label.VA02_03_00_03_0.ToString();
          static public readonly string VA02_03_00_03_1 = Label.VA02_03_00_03_1.ToString();
          static public readonly string VA02_03_00_04_0 = Label.VA02_03_00_04_0.ToString();
          static public readonly string VA02_03_00_04_1 = Label.VA02_03_00_04_1.ToString();
          static public readonly string VA02_03_00_05_0 = Label.VA02_03_00_05_0.ToString();
          static public readonly string VA02_03_00_05_1 = Label.VA02_03_00_05_1.ToString();
          static public readonly string VA02_04_00_00_0 = Label.VA02_04_00_00_0.ToString();
          static public readonly string VA02_04_00_01_0 = Label.VA02_04_00_01_0.ToString();
          static public readonly string VA02_04_00_02_0 = Label.VA02_04_00_02_0.ToString();
          static public readonly string VA02_05_00_00_0 = Label.VA02_05_00_00_0.ToString();
          static public readonly string VA02_05_00_00_1 = Label.VA02_05_00_00_1.ToString();
          static public readonly string VA02_06_01_00_0 = Label.VA02_06_01_00_0.ToString();
          static public readonly string VA02_06_01_01_0 = Label.VA02_06_01_01_0.ToString();
          static public readonly string VA02_06_01_02_0 = Label.VA02_06_01_02_0.ToString();
          static public readonly string VA02_06_01_03_0 = Label.VA02_06_01_03_0.ToString();
          static public readonly string VA02_07_00_00_0 = Label.VA02_07_00_00_0.ToString();
          static public readonly string VA02_07_01_00_0 = Label.VA02_07_01_00_0.ToString();
          static public readonly string VA02_07_02_00_0 = Label.VA02_07_02_00_0.ToString();
          static public readonly string VA02_07_03_00_0 = Label.VA02_07_03_00_0.ToString();
          static public readonly string VA02_07_04_00_0 = Label.VA02_07_04_00_0.ToString();
          static public readonly string VA02_07_05_00_0 = Label.VA02_07_05_00_0.ToString();
          static public readonly string VA02_07_06_00_0 = Label.VA02_07_06_00_0.ToString();
          static public readonly string VA02_07_07_00_0 = Label.VA02_07_07_00_0.ToString();
          static public readonly string VA02_07_08_00_0 = Label.VA02_07_08_00_0.ToString();
          static public readonly string VA02_07_09_00_0 = Label.VA02_07_09_00_0.ToString();
          static public readonly string VA02_07_09_01_0 = Label.VA02_07_09_01_0.ToString();
          static public readonly string VA02_07_10_00_0 = Label.VA02_07_10_00_0.ToString();
          static public readonly string VA02_07_11_00_0 = Label.VA02_07_11_00_0.ToString();
          static public readonly string VA02_08_00_00_0 = Label.VA02_08_00_00_0.ToString();
          static public readonly string VA02_08_00_01_0 = Label.VA02_08_00_01_0.ToString();
          static public readonly string VA02_08_00_01_1 = Label.VA02_08_00_01_1.ToString();
          static public readonly string VA02_08_00_02_0 = Label.VA02_08_00_02_0.ToString();
          static public readonly string VA02_08_00_03_0 = Label.VA02_08_00_03_0.ToString();
          static public readonly string VA02_08_00_03_1 = Label.VA02_08_00_03_1.ToString();
          static public readonly string VA02_08_00_04_0 = Label.VA02_08_00_04_0.ToString();
          static public readonly string VA02_08_00_04_1 = Label.VA02_08_00_04_1.ToString();
          static public readonly string VA02_08_00_05_0 = Label.VA02_08_00_05_0.ToString();
          static public readonly string VA02_08_00_06_0 = Label.VA02_08_00_06_0.ToString();
          static public readonly string VA02_08_00_07_0 = Label.VA02_08_00_07_0.ToString();
          static public readonly string VA02_09_00_00_0 = Label.VA02_09_00_00_0.ToString();
          static public readonly string VA02_09_00_00_1 = Label.VA02_09_00_00_1.ToString();
          static public readonly string VA02_09_00_01_0 = Label.VA02_09_00_01_0.ToString();
          static public readonly string VA02_09_00_01_1 = Label.VA02_09_00_01_1.ToString();
          static public readonly string VA02_09_00_02_0 = Label.VA02_09_00_02_0.ToString();
          static public readonly string VA02_09_00_03_0 = Label.VA02_09_00_03_0.ToString();
          static public readonly string VA02_09_00_03_1 = Label.VA02_09_00_03_1.ToString();
          static public readonly string VA02_09_00_04_0 = Label.VA02_09_00_04_0.ToString();
          static public readonly string VA02_09_00_04_1 = Label.VA02_09_00_04_1.ToString();
          static public readonly string VA02_10_00_00_0 = Label.VA02_10_00_00_0.ToString();
          static public readonly string VA02_10_00_01_0 = Label.VA02_10_00_01_0.ToString();
          static public readonly string VA02_10_00_02_0 = Label.VA02_10_00_02_0.ToString();
          static public readonly string VA02_10_00_03_0 = Label.VA02_10_00_03_0.ToString();
          static public readonly string VA02_11_00_00_0 = Label.VA02_11_00_00_0.ToString();
          static public readonly string VA02_11_01_00_0 = Label.VA02_11_01_00_0.ToString();
          static public readonly string VA02_11_02_00_0 = Label.VA02_11_02_00_0.ToString();
          static public readonly string VA02_11_03_00_0 = Label.VA02_11_03_00_0.ToString();
          static public readonly string VA02_11_04_00_0 = Label.VA02_11_04_00_0.ToString();
          static public readonly string VA02_11_05_00_0 = Label.VA02_11_05_00_0.ToString();
          static public readonly string VA02_11_06_00_0 = Label.VA02_11_06_00_0.ToString();
          static public readonly string VA02_11_07_00_0 = Label.VA02_11_07_00_0.ToString();
          static public readonly string VA02_11_08_00_0 = Label.VA02_11_08_00_0.ToString();
          static public readonly string VA02_11_09_00_0 = Label.VA02_11_09_00_0.ToString();
          static public readonly string VA02_13_00_00_0 = Label.VA02_13_00_00_0.ToString();
          static public readonly string VA02_13_00_00_1 = Label.VA02_13_00_00_1.ToString();
          static public readonly string VA02_13_00_01_0 = Label.VA02_13_00_01_0.ToString();
          static public readonly string VA02_13_00_01_1 = Label.VA02_13_00_01_1.ToString();
          static public readonly string VA02_13_00_02_0 = Label.VA02_13_00_02_0.ToString();
          static public readonly string VA02_13_00_02_1 = Label.VA02_13_00_02_1.ToString();
          static public readonly string VA02_13_00_02_2 = Label.VA02_13_00_02_2.ToString();
          static public readonly string VA02_13_00_03_0 = Label.VA02_13_00_03_0.ToString();
          static public readonly string VA02_13_00_04_0 = Label.VA02_13_00_04_0.ToString();
          static public readonly string VA02_13_00_05_0 = Label.VA02_13_00_05_0.ToString();
          static public readonly string VA02_13_00_06_0 = Label.VA02_13_00_06_0.ToString();
          static public readonly string VA02_13_00_07_0 = Label.VA02_13_00_07_0.ToString();
          static public readonly string VA02_13_00_08_0 = Label.VA02_13_00_08_0.ToString();
          static public readonly string VA02_13_00_09_0 = Label.VA02_13_00_09_0.ToString();
          static public readonly string VA02_14_00_00_0 = Label.VA02_14_00_00_0.ToString();
          static public readonly string VA02_14_00_01_0 = Label.VA02_14_00_01_0.ToString();
          static public readonly string VA02_15_00_00_0 = Label.VA02_15_00_00_0.ToString();
          static public readonly string VA02_15_00_01_0 = Label.VA02_15_00_01_0.ToString();
          static public readonly string VA02_15_00_02_0 = Label.VA02_15_00_02_0.ToString();
          static public readonly string VA02_15_00_03_0 = Label.VA02_15_00_03_0.ToString();
          static public readonly string VA02_16_00_00_0 = Label.VA02_16_00_00_0.ToString();
          static public readonly string VA02_16_00_01_0 = Label.VA02_16_00_01_0.ToString();
          static public readonly string VA02_17_00_00_0 = Label.VA02_17_00_00_0.ToString();
          static public readonly string VA02_17_00_01_0 = Label.VA02_17_00_01_0.ToString();
          static public readonly string VA02_18_00_00_0 = Label.VA02_18_00_00_0.ToString();
          static public readonly string VA02_18_00_01_0 = Label.VA02_18_00_01_0.ToString();
          static public readonly string VA02_19_00_00_0 = Label.VA02_19_00_00_0.ToString();
          static public readonly string VA02_19_00_00_1 = Label.VA02_19_00_00_1.ToString();
          static public readonly string VA02_19_00_01_0 = Label.VA02_19_00_01_0.ToString();
          static public readonly string VA02_19_00_01_1 = Label.VA02_19_00_01_1.ToString();
          static public readonly string VA02_19_00_02_0 = Label.VA02_19_00_02_0.ToString();
          static public readonly string VA02_19_00_02_1 = Label.VA02_19_00_02_1.ToString();
          static public readonly string VA02_19_00_03_0 = Label.VA02_19_00_03_0.ToString();
          static public readonly string VA02_19_00_03_1 = Label.VA02_19_00_03_1.ToString();
          static public readonly string VA02_19_00_04_0 = Label.VA02_19_00_04_0.ToString();
          static public readonly string VA02_19_00_04_1 = Label.VA02_19_00_04_1.ToString();
          static public readonly string VA02_19_00_05_0 = Label.VA02_19_00_05_0.ToString();
          static public readonly string VA02_19_00_05_1 = Label.VA02_19_00_05_1.ToString();
          static public readonly string VA02_20_00_00_0 = Label.VA02_20_00_00_0.ToString();
          static public readonly string VA02_20_00_01_0 = Label.VA02_20_00_01_0.ToString();
          static public readonly string VA02_20_01_00_0 = Label.VA02_20_01_00_0.ToString();
          static public readonly string VA02_20_01_01_0 = Label.VA02_20_01_01_0.ToString();
          static public readonly string VA02_21_00_00_0 = Label.VA02_21_00_00_0.ToString();
          static public readonly string VA02_21_00_01_0 = Label.VA02_21_00_01_0.ToString();
          static public readonly string VA02_21_00_02_0 = Label.VA02_21_00_02_0.ToString();
          static public readonly string VA02_22_00_03_0 = Label.VA02_22_00_03_0.ToString();
          static public readonly string VA02_22_00_04_0 = Label.VA02_22_00_04_0.ToString();
          static public readonly string VA02_22_00_05_0 = Label.VA02_22_00_05_0.ToString();
          static public readonly string VA02_22_00_06_0 = Label.VA02_22_00_06_0.ToString();
          static public readonly string VA02_22_00_07_0 = Label.VA02_22_00_07_0.ToString();
          static public readonly string VA02_22_00_08_0 = Label.VA02_22_00_08_0.ToString();
          static public readonly string VA02_23_00_00_0 = Label.VA02_23_00_00_0.ToString();
          static public readonly string VA02_24_00_00_0 = Label.VA02_24_00_00_0.ToString();
          static public readonly string VA02_26_00_00_0 = Label.VA02_26_00_00_0.ToString();
          static public readonly string VA02_26_00_01_0 = Label.VA02_26_00_01_0.ToString();
          static public readonly string VA02_26_00_02_0 = Label.VA02_26_00_02_0.ToString();
          static public readonly string VA02_27_00_00_0 = Label.VA02_27_00_00_0.ToString();
          static public readonly string VA02_27_00_01_0 = Label.VA02_27_00_01_0.ToString();
          static public readonly string VA02_27_00_02_0 = Label.VA02_27_00_02_0.ToString();
          static public readonly string VA02_28_00_00_0 = Label.VA02_28_00_00_0.ToString();
          static public readonly string VA02_28_00_00_1 = Label.VA02_28_00_00_1.ToString();
          static public readonly string VA02_29_00_00_0 = Label.VA02_29_00_00_0.ToString();
          static public readonly string VA02_29_00_00_1 = Label.VA02_29_00_00_1.ToString();
          static public readonly string VA02_30_00_00_0 = Label.VA02_30_00_00_0.ToString();
          static public readonly string VA02_30_00_01_0 = Label.VA02_30_00_01_0.ToString();
          static public readonly string VA02_31_00_00_0 = Label.VA02_31_00_00_0.ToString();
          static public readonly string VA02_31_00_01_0 = Label.VA02_31_00_01_0.ToString();
          static public readonly string VA02_32_00_00_0 = Label.VA02_32_00_00_0.ToString();
          static public readonly string VA02_32_00_01_0 = Label.VA02_32_00_01_0.ToString();
          static public readonly string VA01_01_00_00_0 = Label.VA01_01_00_00_0.ToString();
          static public readonly string VA01_02_00_00_0 = Label.VA01_02_00_00_0.ToString();
          static public readonly string VA01_03_00_00_0 = Label.VA01_03_00_00_0.ToString();
          static public readonly string VA01_03_00_00_1 = Label.VA01_03_00_00_1.ToString();
          static public readonly string VA01_03_00_01_0 = Label.VA01_03_00_01_0.ToString();
          static public readonly string VA01_03_00_01_1 = Label.VA01_03_00_01_1.ToString();
          static public readonly string VA01_03_00_02_0 = Label.VA01_03_00_02_0.ToString();
          static public readonly string VA01_03_00_02_1 = Label.VA01_03_00_02_1.ToString();
          static public readonly string VA01_03_00_03_0 = Label.VA01_03_00_03_0.ToString();
          static public readonly string VA01_03_00_04_0 = Label.VA01_03_00_04_0.ToString();
          static public readonly string VA01_03_00_04_1 = Label.VA01_03_00_04_1.ToString();
          static public readonly string VA01_03_00_05_0 = Label.VA01_03_00_05_0.ToString();
          static public readonly string VA01_03_00_05_1 = Label.VA01_03_00_05_1.ToString();
          static public readonly string VA01_04_00_00_0 = Label.VA01_04_00_00_0.ToString();
          static public readonly string VA01_04_00_01_0 = Label.VA01_04_00_01_0.ToString();
          static public readonly string VA01_04_00_02_0 = Label.VA01_04_00_02_0.ToString();
          static public readonly string VA01_05_00_00_0 = Label.VA01_05_00_00_0.ToString();
          static public readonly string VA01_05_00_00_1 = Label.VA01_05_00_00_1.ToString();
          static public readonly string VA01_06_01_00_0 = Label.VA01_06_01_00_0.ToString();
          static public readonly string VA01_06_01_01_0 = Label.VA01_06_01_01_0.ToString();
          static public readonly string VA01_06_01_02_0 = Label.VA01_06_01_02_0.ToString();
          static public readonly string VA01_06_01_03_0 = Label.VA01_06_01_03_0.ToString();
          static public readonly string VA01_07_00_00_0 = Label.VA01_07_00_00_0.ToString();
          static public readonly string VA01_07_01_00_0 = Label.VA01_07_01_00_0.ToString();
          static public readonly string VA01_07_02_00_0 = Label.VA01_07_02_00_0.ToString();
          static public readonly string VA01_07_03_00_0 = Label.VA01_07_03_00_0.ToString();
          static public readonly string VA01_07_04_00_0 = Label.VA01_07_04_00_0.ToString();
          static public readonly string VA01_07_05_00_0 = Label.VA01_07_05_00_0.ToString();
          static public readonly string VA01_07_06_00_0 = Label.VA01_07_06_00_0.ToString();
          static public readonly string VA01_07_07_00_0 = Label.VA01_07_07_00_0.ToString();
          static public readonly string VA01_07_08_00_0 = Label.VA01_07_08_00_0.ToString();
          static public readonly string VA01_07_09_00_0 = Label.VA01_07_09_00_0.ToString();
          static public readonly string VA01_07_09_01_0 = Label.VA01_07_09_01_0.ToString();
          static public readonly string VA01_07_10_00_0 = Label.VA01_07_10_00_0.ToString();
          static public readonly string VA01_07_11_00_0 = Label.VA01_07_11_00_0.ToString();
          static public readonly string VA01_08_00_00_0 = Label.VA01_08_00_00_0.ToString();
          static public readonly string VA01_08_00_01_0 = Label.VA01_08_00_01_0.ToString();
          static public readonly string VA01_08_00_02_0 = Label.VA01_08_00_02_0.ToString();
          static public readonly string VA01_08_00_03_0 = Label.VA01_08_00_03_0.ToString();
          static public readonly string VA01_08_00_04_0 = Label.VA01_08_00_04_0.ToString();
          static public readonly string VA01_08_00_05_0 = Label.VA01_08_00_05_0.ToString();
          static public readonly string VA01_08_00_06_0 = Label.VA01_08_00_06_0.ToString();
          static public readonly string VA01_08_00_06_1 = Label.VA01_08_00_06_1.ToString();
          static public readonly string VA01_08_00_07_0 = Label.VA01_08_00_07_0.ToString();
          static public readonly string VA01_08_00_08_0 = Label.VA01_08_00_08_0.ToString();
          static public readonly string VA01_08_00_08_1 = Label.VA01_08_00_08_1.ToString();
          static public readonly string VA01_08_00_09_0 = Label.VA01_08_00_09_0.ToString();
          static public readonly string VA01_08_00_09_1 = Label.VA01_08_00_09_1.ToString();
          static public readonly string VA01_08_00_10_0 = Label.VA01_08_00_10_0.ToString();
          static public readonly string VA01_08_00_11_0 = Label.VA01_08_00_11_0.ToString();
          static public readonly string VA01_08_00_11_1 = Label.VA01_08_00_11_1.ToString();
          static public readonly string VA01_08_00_12_0 = Label.VA01_08_00_12_0.ToString();
          static public readonly string VA01_08_00_13_0 = Label.VA01_08_00_13_0.ToString();
          static public readonly string VA01_08_00_13_1 = Label.VA01_08_00_13_1.ToString();
          static public readonly string VA01_08_00_14_0 = Label.VA01_08_00_14_0.ToString();
          static public readonly string VA01_08_00_14_1 = Label.VA01_08_00_14_1.ToString();
          static public readonly string VA01_08_00_15_0 = Label.VA01_08_00_15_0.ToString();
          static public readonly string VA01_08_00_15_1 = Label.VA01_08_00_15_1.ToString();
          static public readonly string VA01_08_00_16_0 = Label.VA01_08_00_16_0.ToString();
          static public readonly string VA01_08_00_17_0 = Label.VA01_08_00_17_0.ToString();
          static public readonly string VA01_08_00_18_0 = Label.VA01_08_00_18_0.ToString();
          static public readonly string VA01_08_00_19_0 = Label.VA01_08_00_19_0.ToString();
          static public readonly string VA01_08_00_19_1 = Label.VA01_08_00_19_1.ToString();
          static public readonly string VA01_09_00_00_0 = Label.VA01_09_00_00_0.ToString();
          static public readonly string VA01_09_00_00_1 = Label.VA01_09_00_00_1.ToString();
          static public readonly string VA01_09_00_01_0 = Label.VA01_09_00_01_0.ToString();
          static public readonly string VA01_09_00_02_0 = Label.VA01_09_00_02_0.ToString();
          static public readonly string VA01_09_00_02_1 = Label.VA01_09_00_02_1.ToString();
          static public readonly string VA01_09_00_03_0 = Label.VA01_09_00_03_0.ToString();
          static public readonly string VA01_09_00_03_1 = Label.VA01_09_00_03_1.ToString();
          static public readonly string VA01_09_00_04_0 = Label.VA01_09_00_04_0.ToString();
          static public readonly string VA01_09_00_05_0 = Label.VA01_09_00_05_0.ToString();
          static public readonly string VA01_09_00_05_1 = Label.VA01_09_00_05_1.ToString();
          static public readonly string VA01_09_00_06_0 = Label.VA01_09_00_06_0.ToString();
          static public readonly string VA01_10_00_00_0 = Label.VA01_10_00_00_0.ToString();
          static public readonly string VA01_10_00_01_0 = Label.VA01_10_00_01_0.ToString();
          static public readonly string VA01_10_00_02_0 = Label.VA01_10_00_02_0.ToString();
          static public readonly string VA01_10_00_03_0 = Label.VA01_10_00_03_0.ToString();
          static public readonly string VA01_11_00_00_0 = Label.VA01_11_00_00_0.ToString();
          static public readonly string VA01_11_01_00_0 = Label.VA01_11_01_00_0.ToString();
          static public readonly string VA01_11_02_00_0 = Label.VA01_11_02_00_0.ToString();
          static public readonly string VA01_11_03_00_0 = Label.VA01_11_03_00_0.ToString();
          static public readonly string VA01_11_04_00_0 = Label.VA01_11_04_00_0.ToString();
          static public readonly string VA01_11_05_00_0 = Label.VA01_11_05_00_0.ToString();
          static public readonly string VA01_11_06_00_0 = Label.VA01_11_06_00_0.ToString();
          static public readonly string VA01_11_07_00_0 = Label.VA01_11_07_00_0.ToString();
          static public readonly string VA01_11_08_00_0 = Label.VA01_11_08_00_0.ToString();
          static public readonly string VA01_11_09_00_0 = Label.VA01_11_09_00_0.ToString();
          static public readonly string VA01_13_00_00_0 = Label.VA01_13_00_00_0.ToString();
          static public readonly string VA01_13_00_01_0 = Label.VA01_13_00_01_0.ToString();
          static public readonly string VA01_13_00_01_1 = Label.VA01_13_00_01_1.ToString();
          static public readonly string VA01_13_00_02_0 = Label.VA01_13_00_02_0.ToString();
          static public readonly string VA01_13_00_02_1 = Label.VA01_13_00_02_1.ToString();
          static public readonly string VA01_13_00_03_0 = Label.VA01_13_00_03_0.ToString();
          static public readonly string VA01_13_00_04_0 = Label.VA01_13_00_04_0.ToString();
          static public readonly string VA01_13_00_05_0 = Label.VA01_13_00_05_0.ToString();
          static public readonly string VA01_13_00_06_0 = Label.VA01_13_00_06_0.ToString();
          static public readonly string VA01_13_00_06_1 = Label.VA01_13_00_06_1.ToString();
          static public readonly string VA01_13_00_06_2 = Label.VA01_13_00_06_2.ToString();
          static public readonly string VA01_13_00_07_0 = Label.VA01_13_00_07_0.ToString();
          static public readonly string VA01_13_00_08_0 = Label.VA01_13_00_08_0.ToString();
          static public readonly string VA01_13_00_09_0 = Label.VA01_13_00_09_0.ToString();
          static public readonly string VA01_13_00_10_0 = Label.VA01_13_00_10_0.ToString();
          static public readonly string VA01_13_00_11_0 = Label.VA01_13_00_11_0.ToString();
          static public readonly string VA01_13_00_12_0 = Label.VA01_13_00_12_0.ToString();
          static public readonly string VA01_13_00_13_0 = Label.VA01_13_00_13_0.ToString();
          static public readonly string VA01_13_00_14_0 = Label.VA01_13_00_14_0.ToString();
          static public readonly string VA01_13_00_15_0 = Label.VA01_13_00_15_0.ToString();
          static public readonly string VA01_13_00_16_0 = Label.VA01_13_00_16_0.ToString();
          static public readonly string VA01_13_00_16_1 = Label.VA01_13_00_16_1.ToString();
          static public readonly string VA01_13_00_16_2 = Label.VA01_13_00_16_2.ToString();
          static public readonly string VA01_14_00_00_0 = Label.VA01_14_00_00_0.ToString();
          static public readonly string VA01_14_00_01_0 = Label.VA01_14_00_01_0.ToString();
          static public readonly string VA01_15_00_00_0 = Label.VA01_15_00_00_0.ToString();
          static public readonly string VA01_15_00_01_0 = Label.VA01_15_00_01_0.ToString();
          static public readonly string VA01_15_00_02_0 = Label.VA01_15_00_02_0.ToString();
          static public readonly string VA01_15_00_03_0 = Label.VA01_15_00_03_0.ToString();
          static public readonly string VA01_16_00_00_0 = Label.VA01_16_00_00_0.ToString();
          static public readonly string VA01_16_00_01_0 = Label.VA01_16_00_01_0.ToString();
          static public readonly string VA01_17_00_00_0 = Label.VA01_17_00_00_0.ToString();
          static public readonly string VA01_17_00_01_0 = Label.VA01_17_00_01_0.ToString();
          static public readonly string VA01_18_00_00_0 = Label.VA01_18_00_00_0.ToString();
          static public readonly string VA01_18_00_01_0 = Label.VA01_18_00_01_0.ToString();
          static public readonly string VA01_19_00_00_0 = Label.VA01_19_00_00_0.ToString();
          static public readonly string VA01_19_00_00_1 = Label.VA01_19_00_00_1.ToString();
          static public readonly string VA01_19_00_01_0 = Label.VA01_19_00_01_0.ToString();
          static public readonly string VA01_19_00_01_1 = Label.VA01_19_00_01_1.ToString();
          static public readonly string VA01_19_00_02_0 = Label.VA01_19_00_02_0.ToString();
          static public readonly string VA01_19_00_02_1 = Label.VA01_19_00_02_1.ToString();
          static public readonly string VA01_19_00_03_0 = Label.VA01_19_00_03_0.ToString();
          static public readonly string VA01_19_00_03_1 = Label.VA01_19_00_03_1.ToString();
          static public readonly string VA01_19_00_04_0 = Label.VA01_19_00_04_0.ToString();
          static public readonly string VA01_19_00_04_1 = Label.VA01_19_00_04_1.ToString();
          static public readonly string VA01_19_00_05_0 = Label.VA01_19_00_05_0.ToString();
          static public readonly string VA01_19_00_05_1 = Label.VA01_19_00_05_1.ToString();
          static public readonly string VA01_19_00_06_0 = Label.VA01_19_00_06_0.ToString();
          static public readonly string VA01_19_00_06_1 = Label.VA01_19_00_06_1.ToString();
          static public readonly string VA01_19_00_07_0 = Label.VA01_19_00_07_0.ToString();
          static public readonly string VA01_19_00_07_1 = Label.VA01_19_00_07_1.ToString();
          static public readonly string VA01_19_00_08_0 = Label.VA01_19_00_08_0.ToString();
          static public readonly string VA01_19_00_08_1 = Label.VA01_19_00_08_1.ToString();
          static public readonly string VA01_19_00_09_0 = Label.VA01_19_00_09_0.ToString();
          static public readonly string VA01_19_00_09_1 = Label.VA01_19_00_09_1.ToString();
          static public readonly string VA01_20_00_00_0 = Label.VA01_20_00_00_0.ToString();
          static public readonly string VA01_20_00_01_0 = Label.VA01_20_00_01_0.ToString();
          static public readonly string VA01_20_01_00_0 = Label.VA01_20_01_00_0.ToString();
          static public readonly string VA01_20_01_01_0 = Label.VA01_20_01_01_0.ToString();
          static public readonly string VA01_21_00_00_0 = Label.VA01_21_00_00_0.ToString();
          static public readonly string VA01_21_00_01_0 = Label.VA01_21_00_01_0.ToString();
          static public readonly string VA01_21_00_02_0 = Label.VA01_21_00_02_0.ToString();
          static public readonly string VA01_22_00_03_0 = Label.VA01_22_00_03_0.ToString();
          static public readonly string VA01_22_00_04_0 = Label.VA01_22_00_04_0.ToString();
          static public readonly string VA01_22_00_05_0 = Label.VA01_22_00_05_0.ToString();
          static public readonly string VA01_22_00_06_0 = Label.VA01_22_00_06_0.ToString();
          static public readonly string VA01_22_00_07_0 = Label.VA01_22_00_07_0.ToString();
          static public readonly string VA01_22_00_08_0 = Label.VA01_22_00_08_0.ToString();
          static public readonly string VA01_23_00_00_0 = Label.VA01_23_00_00_0.ToString();
          static public readonly string VA01_24_00_00_0 = Label.VA01_24_00_00_0.ToString();
          static public readonly string VA01_26_00_00_0 = Label.VA01_26_00_00_0.ToString();
          static public readonly string VA01_26_00_01_0 = Label.VA01_26_00_01_0.ToString();
          static public readonly string VA01_26_00_02_0 = Label.VA01_26_00_02_0.ToString();
          static public readonly string VA01_27_00_00_0 = Label.VA01_27_00_00_0.ToString();
          static public readonly string VA01_27_00_01_0 = Label.VA01_27_00_01_0.ToString();
          static public readonly string VA01_27_00_02_0 = Label.VA01_27_00_02_0.ToString();
          static public readonly string VA01_28_00_00_0 = Label.VA01_28_00_00_0.ToString();
          static public readonly string VA01_29_00_00_0 = Label.VA01_29_00_00_0.ToString();
          static public readonly string VA01_30_00_00_0 = Label.VA01_30_00_00_0.ToString();
          static public readonly string VA01_30_00_01_0 = Label.VA01_30_00_01_0.ToString();
          static public readonly string VA01_31_00_00_0 = Label.VA01_31_00_00_0.ToString();
          static public readonly string VA01_31_00_01_0 = Label.VA01_31_00_01_0.ToString();
          static public readonly string VA01_32_00_00_0 = Label.VA01_32_00_00_0.ToString();
          static public readonly string VA01_32_00_01_0 = Label.VA01_32_00_01_0.ToString();

      }

      public enum Snapshot {

          Snapshot_Max = 0,
      }

      static public readonly string[] SnapshotName = new string[] {
      };

      public enum AudioClip {
          bgm_cl_battle = 0,
          bgm_cl_lobby = 1,
          bgm_cl_wait = 2,
          BGM_TEST = 3,
          VA02_01_00_00_0 = 4,
          VA02_01_00_00_1 = 5,
          VA02_02_00_00_0 = 6,
          VA02_03_00_00_0 = 7,
          VA02_03_00_00_1 = 8,
          VA02_03_00_01_0 = 9,
          VA02_03_00_01_1 = 10,
          VA02_03_00_02_0 = 11,
          VA02_03_00_02_1 = 12,
          VA02_03_00_03_0 = 13,
          VA02_03_00_03_1 = 14,
          VA02_03_00_04_0 = 15,
          VA02_03_00_04_1 = 16,
          VA02_03_00_05_0 = 17,
          VA02_03_00_05_1 = 18,
          VA02_04_00_00_0 = 19,
          VA02_04_00_01_0 = 20,
          VA02_04_00_02_0 = 21,
          VA02_05_00_00_0 = 22,
          VA02_05_00_00_1 = 23,
          VA02_06_01_00_0 = 24,
          VA02_06_01_01_0 = 25,
          VA02_06_01_02_0 = 26,
          VA02_06_01_03_0 = 27,
          VA02_07_00_00_0 = 28,
          VA02_07_01_00_0 = 29,
          VA02_07_02_00_0 = 30,
          VA02_07_03_00_0 = 31,
          VA02_07_04_00_0 = 32,
          VA02_07_05_00_0 = 33,
          VA02_07_06_00_0 = 34,
          VA02_07_07_00_0 = 35,
          VA02_07_08_00_0 = 36,
          VA02_07_09_00_0 = 37,
          VA02_07_09_01_0 = 38,
          VA02_07_10_00_0 = 39,
          VA02_07_11_00_0 = 40,
          VA02_08_00_00_0 = 41,
          VA02_08_00_01_0 = 42,
          VA02_08_00_01_1 = 43,
          VA02_08_00_02_0 = 44,
          VA02_08_00_03_0 = 45,
          VA02_08_00_03_1 = 46,
          VA02_08_00_04_0 = 47,
          VA02_08_00_04_1 = 48,
          VA02_08_00_05_0 = 49,
          VA02_08_00_06_0 = 50,
          VA02_08_00_07_0 = 51,
          VA02_09_00_00_0 = 52,
          VA02_09_00_00_1 = 53,
          VA02_09_00_01_0 = 54,
          VA02_09_00_01_1 = 55,
          VA02_09_00_02_0 = 56,
          VA02_09_00_03_0 = 57,
          VA02_09_00_03_1 = 58,
          VA02_09_00_04_0 = 59,
          VA02_09_00_04_1 = 60,
          VA02_10_00_00_0 = 61,
          VA02_10_00_01_0 = 62,
          VA02_10_00_02_0 = 63,
          VA02_10_00_03_0 = 64,
          VA02_11_00_00_0 = 65,
          VA02_11_01_00_0 = 66,
          VA02_11_02_00_0 = 67,
          VA02_11_03_00_0 = 68,
          VA02_11_04_00_0 = 69,
          VA02_11_05_00_0 = 70,
          VA02_11_06_00_0 = 71,
          VA02_11_07_00_0 = 72,
          VA02_11_08_00_0 = 73,
          VA02_11_09_00_0 = 74,
          VA02_13_00_00_0 = 75,
          VA02_13_00_00_1 = 76,
          VA02_13_00_01_0 = 77,
          VA02_13_00_01_1 = 78,
          VA02_13_00_02_0 = 79,
          VA02_13_00_02_1 = 80,
          VA02_13_00_02_2 = 81,
          VA02_13_00_03_0 = 82,
          VA02_13_00_04_0 = 83,
          VA02_13_00_05_0 = 84,
          VA02_13_00_06_0 = 85,
          VA02_13_00_07_0 = 86,
          VA02_13_00_08_0 = 87,
          VA02_13_00_09_0 = 88,
          VA02_14_00_00_0 = 89,
          VA02_14_00_01_0 = 90,
          VA02_15_00_00_0 = 91,
          VA02_15_00_01_0 = 92,
          VA02_15_00_02_0 = 93,
          VA02_15_00_03_0 = 94,
          VA02_16_00_00_0 = 95,
          VA02_16_00_01_0 = 96,
          VA02_17_00_00_0 = 97,
          VA02_17_00_01_0 = 98,
          VA02_18_00_00_0 = 99,
          VA02_18_00_01_0 = 100,
          VA02_19_00_00_0 = 101,
          VA02_19_00_00_1 = 102,
          VA02_19_00_01_0 = 103,
          VA02_19_00_01_1 = 104,
          VA02_19_00_02_0 = 105,
          VA02_19_00_02_1 = 106,
          VA02_19_00_03_0 = 107,
          VA02_19_00_03_1 = 108,
          VA02_19_00_04_0 = 109,
          VA02_19_00_04_1 = 110,
          VA02_19_00_05_0 = 111,
          VA02_19_00_05_1 = 112,
          VA02_20_00_00_0 = 113,
          VA02_20_00_01_0 = 114,
          VA02_20_01_00_0 = 115,
          VA02_20_01_01_0 = 116,
          VA02_21_00_00_0 = 117,
          VA02_21_00_01_0 = 118,
          VA02_21_00_02_0 = 119,
          VA02_22_00_03_0 = 120,
          VA02_22_00_04_0 = 121,
          VA02_22_00_05_0 = 122,
          VA02_22_00_06_0 = 123,
          VA02_22_00_07_0 = 124,
          VA02_22_00_08_0 = 125,
          VA02_23_00_00_0 = 126,
          VA02_24_00_00_0 = 127,
          VA02_26_00_00_0 = 128,
          VA02_26_00_01_0 = 129,
          VA02_26_00_02_0 = 130,
          VA02_27_00_00_0 = 131,
          VA02_27_00_01_0 = 132,
          VA02_27_00_02_0 = 133,
          VA02_28_00_00_0 = 134,
          VA02_28_00_00_1 = 135,
          VA02_29_00_00_0 = 136,
          VA02_29_00_00_1 = 137,
          VA02_30_00_00_0 = 138,
          VA02_30_00_01_0 = 139,
          VA02_31_00_00_0 = 140,
          VA02_31_00_01_0 = 141,
          VA02_32_00_00_0 = 142,
          VA02_32_00_01_0 = 143,
          se_attack = 144,
          se_card_break = 145,
          se_card_move_01 = 146,
          se_card_set = 147,
          se_cut_in_01 = 148,
          se_cut_in_02 = 149,
          se_draw = 150,
          se_field_card_up = 151,
          se_lp_count_loop = 152,
          se_lp_count_stop = 153,
          se_lp_zero = 154,
          se_player_damage = 155,
          se_summon = 156,
          se_turn_change = 157,
          se_ui_tap = 158,
          VA01_01_00_00_0 = 159,
          VA01_02_00_00_0 = 160,
          VA01_03_00_00_0 = 161,
          VA01_03_00_00_1 = 162,
          VA01_03_00_01_0 = 163,
          VA01_03_00_01_1 = 164,
          VA01_03_00_02_0 = 165,
          VA01_03_00_02_1 = 166,
          VA01_03_00_03_0 = 167,
          VA01_03_00_04_0 = 168,
          VA01_03_00_04_1 = 169,
          VA01_03_00_05_0 = 170,
          VA01_03_00_05_1 = 171,
          VA01_04_00_00_0 = 172,
          VA01_04_00_01_0 = 173,
          VA01_04_00_02_0 = 174,
          VA01_05_00_00_0 = 175,
          VA01_05_00_00_1 = 176,
          VA01_06_01_00_0 = 177,
          VA01_06_01_01_0 = 178,
          VA01_06_01_02_0 = 179,
          VA01_06_01_03_0 = 180,
          VA01_07_00_00_0 = 181,
          VA01_07_01_00_0 = 182,
          VA01_07_02_00_0 = 183,
          VA01_07_03_00_0 = 184,
          VA01_07_04_00_0 = 185,
          VA01_07_05_00_0 = 186,
          VA01_07_06_00_0 = 187,
          VA01_07_07_00_0 = 188,
          VA01_07_08_00_0 = 189,
          VA01_07_09_00_0 = 190,
          VA01_07_09_01_0 = 191,
          VA01_07_10_00_0 = 192,
          VA01_07_11_00_0 = 193,
          VA01_08_00_00_0 = 194,
          VA01_08_00_01_0 = 195,
          VA01_08_00_02_0 = 196,
          VA01_08_00_03_0 = 197,
          VA01_08_00_04_0 = 198,
          VA01_08_00_05_0 = 199,
          VA01_08_00_06_0 = 200,
          VA01_08_00_06_1 = 201,
          VA01_08_00_07_0 = 202,
          VA01_08_00_08_0 = 203,
          VA01_08_00_08_1 = 204,
          VA01_08_00_09_0 = 205,
          VA01_08_00_09_1 = 206,
          VA01_08_00_10_0 = 207,
          VA01_08_00_11_0 = 208,
          VA01_08_00_11_1 = 209,
          VA01_08_00_12_0 = 210,
          VA01_08_00_13_0 = 211,
          VA01_08_00_13_1 = 212,
          VA01_08_00_14_0 = 213,
          VA01_08_00_14_1 = 214,
          VA01_08_00_15_0 = 215,
          VA01_08_00_15_1 = 216,
          VA01_08_00_16_0 = 217,
          VA01_08_00_17_0 = 218,
          VA01_08_00_18_0 = 219,
          VA01_08_00_19_0 = 220,
          VA01_08_00_19_1 = 221,
          VA01_09_00_00_0 = 222,
          VA01_09_00_00_1 = 223,
          VA01_09_00_01_0 = 224,
          VA01_09_00_02_0 = 225,
          VA01_09_00_02_1 = 226,
          VA01_09_00_03_0 = 227,
          VA01_09_00_03_1 = 228,
          VA01_09_00_04_0 = 229,
          VA01_09_00_05_0 = 230,
          VA01_09_00_05_1 = 231,
          VA01_09_00_06_0 = 232,
          VA01_10_00_00_0 = 233,
          VA01_10_00_01_0 = 234,
          VA01_10_00_02_0 = 235,
          VA01_10_00_03_0 = 236,
          VA01_11_00_00_0 = 237,
          VA01_11_01_00_0 = 238,
          VA01_11_02_00_0 = 239,
          VA01_11_03_00_0 = 240,
          VA01_11_04_00_0 = 241,
          VA01_11_05_00_0 = 242,
          VA01_11_06_00_0 = 243,
          VA01_11_07_00_0 = 244,
          VA01_11_08_00_0 = 245,
          VA01_11_09_00_0 = 246,
          VA01_13_00_00_0 = 247,
          VA01_13_00_01_0 = 248,
          VA01_13_00_01_1 = 249,
          VA01_13_00_02_0 = 250,
          VA01_13_00_02_1 = 251,
          VA01_13_00_03_0 = 252,
          VA01_13_00_04_0 = 253,
          VA01_13_00_05_0 = 254,
          VA01_13_00_06_0 = 255,
          VA01_13_00_06_1 = 256,
          VA01_13_00_06_2 = 257,
          VA01_13_00_07_0 = 258,
          VA01_13_00_08_0 = 259,
          VA01_13_00_09_0 = 260,
          VA01_13_00_10_0 = 261,
          VA01_13_00_11_0 = 262,
          VA01_13_00_12_0 = 263,
          VA01_13_00_13_0 = 264,
          VA01_13_00_14_0 = 265,
          VA01_13_00_15_0 = 266,
          VA01_13_00_16_0 = 267,
          VA01_13_00_16_1 = 268,
          VA01_13_00_16_2 = 269,
          VA01_14_00_00_0 = 270,
          VA01_14_00_01_0 = 271,
          VA01_15_00_00_0 = 272,
          VA01_15_00_01_0 = 273,
          VA01_15_00_02_0 = 274,
          VA01_15_00_03_0 = 275,
          VA01_16_00_00_0 = 276,
          VA01_16_00_01_0 = 277,
          VA01_17_00_00_0 = 278,
          VA01_17_00_01_0 = 279,
          VA01_18_00_00_0 = 280,
          VA01_18_00_01_0 = 281,
          VA01_19_00_00_0 = 282,
          VA01_19_00_00_1 = 283,
          VA01_19_00_01_0 = 284,
          VA01_19_00_01_1 = 285,
          VA01_19_00_02_0 = 286,
          VA01_19_00_02_1 = 287,
          VA01_19_00_03_0 = 288,
          VA01_19_00_03_1 = 289,
          VA01_19_00_04_0 = 290,
          VA01_19_00_04_1 = 291,
          VA01_19_00_05_0 = 292,
          VA01_19_00_05_1 = 293,
          VA01_19_00_06_0 = 294,
          VA01_19_00_06_1 = 295,
          VA01_19_00_07_0 = 296,
          VA01_19_00_07_1 = 297,
          VA01_19_00_08_0 = 298,
          VA01_19_00_08_1 = 299,
          VA01_19_00_09_0 = 300,
          VA01_19_00_09_1 = 301,
          VA01_20_00_00_0 = 302,
          VA01_20_00_01_0 = 303,
          VA01_20_01_00_0 = 304,
          VA01_20_01_01_0 = 305,
          VA01_21_00_00_0 = 306,
          VA01_21_00_01_0 = 307,
          VA01_21_00_02_0 = 308,
          VA01_22_00_03_0 = 309,
          VA01_22_00_04_0 = 310,
          VA01_22_00_05_0 = 311,
          VA01_22_00_06_0 = 312,
          VA01_22_00_07_0 = 313,
          VA01_22_00_08_0 = 314,
          VA01_23_00_00_0 = 315,
          VA01_24_00_00_0 = 316,
          VA01_26_00_00_0 = 317,
          VA01_26_00_01_0 = 318,
          VA01_26_00_02_0 = 319,
          VA01_27_00_00_0 = 320,
          VA01_27_00_01_0 = 321,
          VA01_27_00_02_0 = 322,
          VA01_28_00_00_0 = 323,
          VA01_29_00_00_0 = 324,
          VA01_30_00_00_0 = 325,
          VA01_30_00_01_0 = 326,
          VA01_31_00_00_0 = 327,
          VA01_31_00_01_0 = 328,
          VA01_32_00_00_0 = 329,
          VA01_32_00_01_0 = 330,
      }

      static public readonly string[] AudioClipName = new string[] {
          "bgm_cl_battle",
          "bgm_cl_lobby",
          "bgm_cl_wait",
          "BGM_TEST",
          "VA02_01_00_00_0",
          "VA02_01_00_00_1",
          "VA02_02_00_00_0",
          "VA02_03_00_00_0",
          "VA02_03_00_00_1",
          "VA02_03_00_01_0",
          "VA02_03_00_01_1",
          "VA02_03_00_02_0",
          "VA02_03_00_02_1",
          "VA02_03_00_03_0",
          "VA02_03_00_03_1",
          "VA02_03_00_04_0",
          "VA02_03_00_04_1",
          "VA02_03_00_05_0",
          "VA02_03_00_05_1",
          "VA02_04_00_00_0",
          "VA02_04_00_01_0",
          "VA02_04_00_02_0",
          "VA02_05_00_00_0",
          "VA02_05_00_00_1",
          "VA02_06_01_00_0",
          "VA02_06_01_01_0",
          "VA02_06_01_02_0",
          "VA02_06_01_03_0",
          "VA02_07_00_00_0",
          "VA02_07_01_00_0",
          "VA02_07_02_00_0",
          "VA02_07_03_00_0",
          "VA02_07_04_00_0",
          "VA02_07_05_00_0",
          "VA02_07_06_00_0",
          "VA02_07_07_00_0",
          "VA02_07_08_00_0",
          "VA02_07_09_00_0",
          "VA02_07_09_01_0",
          "VA02_07_10_00_0",
          "VA02_07_11_00_0",
          "VA02_08_00_00_0",
          "VA02_08_00_01_0",
          "VA02_08_00_01_1",
          "VA02_08_00_02_0",
          "VA02_08_00_03_0",
          "VA02_08_00_03_1",
          "VA02_08_00_04_0",
          "VA02_08_00_04_1",
          "VA02_08_00_05_0",
          "VA02_08_00_06_0",
          "VA02_08_00_07_0",
          "VA02_09_00_00_0",
          "VA02_09_00_00_1",
          "VA02_09_00_01_0",
          "VA02_09_00_01_1",
          "VA02_09_00_02_0",
          "VA02_09_00_03_0",
          "VA02_09_00_03_1",
          "VA02_09_00_04_0",
          "VA02_09_00_04_1",
          "VA02_10_00_00_0",
          "VA02_10_00_01_0",
          "VA02_10_00_02_0",
          "VA02_10_00_03_0",
          "VA02_11_00_00_0",
          "VA02_11_01_00_0",
          "VA02_11_02_00_0",
          "VA02_11_03_00_0",
          "VA02_11_04_00_0",
          "VA02_11_05_00_0",
          "VA02_11_06_00_0",
          "VA02_11_07_00_0",
          "VA02_11_08_00_0",
          "VA02_11_09_00_0",
          "VA02_13_00_00_0",
          "VA02_13_00_00_1",
          "VA02_13_00_01_0",
          "VA02_13_00_01_1",
          "VA02_13_00_02_0",
          "VA02_13_00_02_1",
          "VA02_13_00_02_2",
          "VA02_13_00_03_0",
          "VA02_13_00_04_0",
          "VA02_13_00_05_0",
          "VA02_13_00_06_0",
          "VA02_13_00_07_0",
          "VA02_13_00_08_0",
          "VA02_13_00_09_0",
          "VA02_14_00_00_0",
          "VA02_14_00_01_0",
          "VA02_15_00_00_0",
          "VA02_15_00_01_0",
          "VA02_15_00_02_0",
          "VA02_15_00_03_0",
          "VA02_16_00_00_0",
          "VA02_16_00_01_0",
          "VA02_17_00_00_0",
          "VA02_17_00_01_0",
          "VA02_18_00_00_0",
          "VA02_18_00_01_0",
          "VA02_19_00_00_0",
          "VA02_19_00_00_1",
          "VA02_19_00_01_0",
          "VA02_19_00_01_1",
          "VA02_19_00_02_0",
          "VA02_19_00_02_1",
          "VA02_19_00_03_0",
          "VA02_19_00_03_1",
          "VA02_19_00_04_0",
          "VA02_19_00_04_1",
          "VA02_19_00_05_0",
          "VA02_19_00_05_1",
          "VA02_20_00_00_0",
          "VA02_20_00_01_0",
          "VA02_20_01_00_0",
          "VA02_20_01_01_0",
          "VA02_21_00_00_0",
          "VA02_21_00_01_0",
          "VA02_21_00_02_0",
          "VA02_22_00_03_0",
          "VA02_22_00_04_0",
          "VA02_22_00_05_0",
          "VA02_22_00_06_0",
          "VA02_22_00_07_0",
          "VA02_22_00_08_0",
          "VA02_23_00_00_0",
          "VA02_24_00_00_0",
          "VA02_26_00_00_0",
          "VA02_26_00_01_0",
          "VA02_26_00_02_0",
          "VA02_27_00_00_0",
          "VA02_27_00_01_0",
          "VA02_27_00_02_0",
          "VA02_28_00_00_0",
          "VA02_28_00_00_1",
          "VA02_29_00_00_0",
          "VA02_29_00_00_1",
          "VA02_30_00_00_0",
          "VA02_30_00_01_0",
          "VA02_31_00_00_0",
          "VA02_31_00_01_0",
          "VA02_32_00_00_0",
          "VA02_32_00_01_0",
          "se_attack",
          "se_card_break",
          "se_card_move_01",
          "se_card_set",
          "se_cut_in_01",
          "se_cut_in_02",
          "se_draw",
          "se_field_card_up",
          "se_lp_count_loop",
          "se_lp_count_stop",
          "se_lp_zero",
          "se_player_damage",
          "se_summon",
          "se_turn_change",
          "se_ui_tap",
          "VA01_01_00_00_0",
          "VA01_02_00_00_0",
          "VA01_03_00_00_0",
          "VA01_03_00_00_1",
          "VA01_03_00_01_0",
          "VA01_03_00_01_1",
          "VA01_03_00_02_0",
          "VA01_03_00_02_1",
          "VA01_03_00_03_0",
          "VA01_03_00_04_0",
          "VA01_03_00_04_1",
          "VA01_03_00_05_0",
          "VA01_03_00_05_1",
          "VA01_04_00_00_0",
          "VA01_04_00_01_0",
          "VA01_04_00_02_0",
          "VA01_05_00_00_0",
          "VA01_05_00_00_1",
          "VA01_06_01_00_0",
          "VA01_06_01_01_0",
          "VA01_06_01_02_0",
          "VA01_06_01_03_0",
          "VA01_07_00_00_0",
          "VA01_07_01_00_0",
          "VA01_07_02_00_0",
          "VA01_07_03_00_0",
          "VA01_07_04_00_0",
          "VA01_07_05_00_0",
          "VA01_07_06_00_0",
          "VA01_07_07_00_0",
          "VA01_07_08_00_0",
          "VA01_07_09_00_0",
          "VA01_07_09_01_0",
          "VA01_07_10_00_0",
          "VA01_07_11_00_0",
          "VA01_08_00_00_0",
          "VA01_08_00_01_0",
          "VA01_08_00_02_0",
          "VA01_08_00_03_0",
          "VA01_08_00_04_0",
          "VA01_08_00_05_0",
          "VA01_08_00_06_0",
          "VA01_08_00_06_1",
          "VA01_08_00_07_0",
          "VA01_08_00_08_0",
          "VA01_08_00_08_1",
          "VA01_08_00_09_0",
          "VA01_08_00_09_1",
          "VA01_08_00_10_0",
          "VA01_08_00_11_0",
          "VA01_08_00_11_1",
          "VA01_08_00_12_0",
          "VA01_08_00_13_0",
          "VA01_08_00_13_1",
          "VA01_08_00_14_0",
          "VA01_08_00_14_1",
          "VA01_08_00_15_0",
          "VA01_08_00_15_1",
          "VA01_08_00_16_0",
          "VA01_08_00_17_0",
          "VA01_08_00_18_0",
          "VA01_08_00_19_0",
          "VA01_08_00_19_1",
          "VA01_09_00_00_0",
          "VA01_09_00_00_1",
          "VA01_09_00_01_0",
          "VA01_09_00_02_0",
          "VA01_09_00_02_1",
          "VA01_09_00_03_0",
          "VA01_09_00_03_1",
          "VA01_09_00_04_0",
          "VA01_09_00_05_0",
          "VA01_09_00_05_1",
          "VA01_09_00_06_0",
          "VA01_10_00_00_0",
          "VA01_10_00_01_0",
          "VA01_10_00_02_0",
          "VA01_10_00_03_0",
          "VA01_11_00_00_0",
          "VA01_11_01_00_0",
          "VA01_11_02_00_0",
          "VA01_11_03_00_0",
          "VA01_11_04_00_0",
          "VA01_11_05_00_0",
          "VA01_11_06_00_0",
          "VA01_11_07_00_0",
          "VA01_11_08_00_0",
          "VA01_11_09_00_0",
          "VA01_13_00_00_0",
          "VA01_13_00_01_0",
          "VA01_13_00_01_1",
          "VA01_13_00_02_0",
          "VA01_13_00_02_1",
          "VA01_13_00_03_0",
          "VA01_13_00_04_0",
          "VA01_13_00_05_0",
          "VA01_13_00_06_0",
          "VA01_13_00_06_1",
          "VA01_13_00_06_2",
          "VA01_13_00_07_0",
          "VA01_13_00_08_0",
          "VA01_13_00_09_0",
          "VA01_13_00_10_0",
          "VA01_13_00_11_0",
          "VA01_13_00_12_0",
          "VA01_13_00_13_0",
          "VA01_13_00_14_0",
          "VA01_13_00_15_0",
          "VA01_13_00_16_0",
          "VA01_13_00_16_1",
          "VA01_13_00_16_2",
          "VA01_14_00_00_0",
          "VA01_14_00_01_0",
          "VA01_15_00_00_0",
          "VA01_15_00_01_0",
          "VA01_15_00_02_0",
          "VA01_15_00_03_0",
          "VA01_16_00_00_0",
          "VA01_16_00_01_0",
          "VA01_17_00_00_0",
          "VA01_17_00_01_0",
          "VA01_18_00_00_0",
          "VA01_18_00_01_0",
          "VA01_19_00_00_0",
          "VA01_19_00_00_1",
          "VA01_19_00_01_0",
          "VA01_19_00_01_1",
          "VA01_19_00_02_0",
          "VA01_19_00_02_1",
          "VA01_19_00_03_0",
          "VA01_19_00_03_1",
          "VA01_19_00_04_0",
          "VA01_19_00_04_1",
          "VA01_19_00_05_0",
          "VA01_19_00_05_1",
          "VA01_19_00_06_0",
          "VA01_19_00_06_1",
          "VA01_19_00_07_0",
          "VA01_19_00_07_1",
          "VA01_19_00_08_0",
          "VA01_19_00_08_1",
          "VA01_19_00_09_0",
          "VA01_19_00_09_1",
          "VA01_20_00_00_0",
          "VA01_20_00_01_0",
          "VA01_20_01_00_0",
          "VA01_20_01_01_0",
          "VA01_21_00_00_0",
          "VA01_21_00_01_0",
          "VA01_21_00_02_0",
          "VA01_22_00_03_0",
          "VA01_22_00_04_0",
          "VA01_22_00_05_0",
          "VA01_22_00_06_0",
          "VA01_22_00_07_0",
          "VA01_22_00_08_0",
          "VA01_23_00_00_0",
          "VA01_24_00_00_0",
          "VA01_26_00_00_0",
          "VA01_26_00_01_0",
          "VA01_26_00_02_0",
          "VA01_27_00_00_0",
          "VA01_27_00_01_0",
          "VA01_27_00_02_0",
          "VA01_28_00_00_0",
          "VA01_29_00_00_0",
          "VA01_30_00_00_0",
          "VA01_30_00_01_0",
          "VA01_31_00_00_0",
          "VA01_31_00_01_0",
          "VA01_32_00_00_0",
          "VA01_32_00_01_0",
      };
  }
}
