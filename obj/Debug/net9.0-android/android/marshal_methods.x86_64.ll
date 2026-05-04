; ModuleID = 'marshal_methods.x86_64.ll'
source_filename = "marshal_methods.x86_64.ll"
target datalayout = "e-m:e-p270:32:32-p271:32:32-p272:64:64-i64:64-f80:128-n8:16:32:64-S128"
target triple = "x86_64-unknown-linux-android21"

%struct.MarshalMethodName = type {
	i64, ; uint64_t id
	ptr ; char* name
}

%struct.MarshalMethodsManagedClass = type {
	i32, ; uint32_t token
	ptr ; MonoClass klass
}

@assembly_image_cache = dso_local local_unnamed_addr global [409 x ptr] zeroinitializer, align 16

; Each entry maps hash of an assembly name to an index into the `assembly_image_cache` array
@assembly_image_cache_hashes = dso_local local_unnamed_addr constant [1227 x i64] [
	i64 u0x001e58127c546039, ; 0: lib_System.Globalization.dll.so => 42
	i64 u0x0024d0f62dee05bd, ; 1: Xamarin.KotlinX.Coroutines.Core.dll => 301
	i64 u0x003a63debab53248, ; 2: lib-it-Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll.so => 348
	i64 u0x0071cf2d27b7d61e, ; 3: lib_Xamarin.AndroidX.SwipeRefreshLayout.dll.so => 279
	i64 u0x01109b0e4d99e61f, ; 4: System.ComponentModel.Annotations.dll => 13
	i64 u0x02123411c4e01926, ; 5: lib_Xamarin.AndroidX.Navigation.Runtime.dll.so => 269
	i64 u0x0284512fad379f7e, ; 6: System.Runtime.Handles => 105
	i64 u0x0297093beda3df86, ; 7: Supabase.Gotrue.dll => 209
	i64 u0x02abedc11addc1ed, ; 8: lib_Mono.Android.Runtime.dll.so => 171
	i64 u0x02f55bf70672f5c8, ; 9: lib_System.IO.FileSystem.DriveInfo.dll.so => 48
	i64 u0x030bd80f997d3bb2, ; 10: tr/Microsoft.TestPlatform.CrossPlatEngine.resources.dll => 396
	i64 u0x032267b2a94db371, ; 11: lib_Xamarin.AndroidX.AppCompat.dll.so => 225
	i64 u0x033a1d0324ba06bd, ; 12: Microsoft.IO.RecyclableMemoryStream.dll => 189
	i64 u0x03621c804933a890, ; 13: System.Buffers => 7
	i64 u0x0399610510a38a38, ; 14: lib_System.Private.DataContractSerialization.dll.so => 86
	i64 u0x043032f1d071fae0, ; 15: ru/Microsoft.Maui.Controls.resources => 329
	i64 u0x044440a55165631e, ; 16: lib-cs-Microsoft.Maui.Controls.resources.dll.so => 307
	i64 u0x046eb1581a80c6b0, ; 17: vi/Microsoft.Maui.Controls.resources => 335
	i64 u0x0470607fd33c32db, ; 18: Microsoft.IdentityModel.Abstractions.dll => 185
	i64 u0x047408741db2431a, ; 19: Xamarin.AndroidX.DynamicAnimation => 245
	i64 u0x0517ef04e06e9f76, ; 20: System.Net.Primitives => 71
	i64 u0x0565d18c6da3de38, ; 21: Xamarin.AndroidX.RecyclerView => 272
	i64 u0x0581db89237110e9, ; 22: lib_System.Collections.dll.so => 12
	i64 u0x05989cb940b225a9, ; 23: Microsoft.Maui.dll => 192
	i64 u0x05a1c25e78e22d87, ; 24: lib_System.Runtime.CompilerServices.Unsafe.dll.so => 102
	i64 u0x06076b5d2b581f08, ; 25: zh-HK/Microsoft.Maui.Controls.resources => 336
	i64 u0x06388ffe9f6c161a, ; 26: System.Xml.Linq.dll => 156
	i64 u0x06600c4c124cb358, ; 27: System.Configuration.dll => 19
	i64 u0x067f95c5ddab55b3, ; 28: lib_Xamarin.AndroidX.Fragment.Ktx.dll.so => 250
	i64 u0x0680a433c781bb3d, ; 29: Xamarin.AndroidX.Collection.Jvm => 232
	i64 u0x069fff96ec92a91d, ; 30: System.Xml.XPath.dll => 161
	i64 u0x070b0847e18dab68, ; 31: Xamarin.AndroidX.Emoji2.ViewsHelper.dll => 247
	i64 u0x0739448d84d3b016, ; 32: lib_Xamarin.AndroidX.VectorDrawable.dll.so => 282
	i64 u0x07469f2eecce9e85, ; 33: mscorlib.dll => 167
	i64 u0x07c57877c7ba78ad, ; 34: ru/Microsoft.Maui.Controls.resources.dll => 329
	i64 u0x07dcdc7460a0c5e4, ; 35: System.Collections.NonGeneric => 10
	i64 u0x08122e52765333c8, ; 36: lib_Microsoft.Extensions.Logging.Debug.dll.so => 182
	i64 u0x088610fc2509f69e, ; 37: lib_Xamarin.AndroidX.VectorDrawable.Animated.dll.so => 283
	i64 u0x08a7c865576bbde7, ; 38: System.Reflection.Primitives => 96
	i64 u0x08c9d051a4a817e5, ; 39: Xamarin.AndroidX.CustomView.PoolingContainer.dll => 243
	i64 u0x08f3c9788ee2153c, ; 40: Xamarin.AndroidX.DrawerLayout => 244
	i64 u0x09138715c92dba90, ; 41: lib_System.ComponentModel.Annotations.dll.so => 13
	i64 u0x0919c28b89381a0b, ; 42: lib_Microsoft.Extensions.Options.dll.so => 183
	i64 u0x092266563089ae3e, ; 43: lib_System.Collections.NonGeneric.dll.so => 10
	i64 u0x09d144a7e214d457, ; 44: System.Security.Cryptography => 127
	i64 u0x09e2b9f743db21a8, ; 45: lib_System.Reflection.Metadata.dll.so => 95
	i64 u0x09fc81a9766d0a68, ; 46: lib-pl-Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll.so => 354
	i64 u0x0abb3e2b271edc45, ; 47: System.Threading.Channels.dll => 140
	i64 u0x0ad1aa9dd2bf38c8, ; 48: es/Microsoft.TestPlatform.CommunicationUtilities.resources => 371
	i64 u0x0b06b1feab070143, ; 49: System.Formats.Tar => 39
	i64 u0x0b0c5396e17cc79c, ; 50: tr/Microsoft.TestPlatform.CoreUtilities.resources.dll => 359
	i64 u0x0b3b632c3bbee20c, ; 51: sk/Microsoft.Maui.Controls.resources => 330
	i64 u0x0b6aff547b84fbe9, ; 52: Xamarin.KotlinX.Serialization.Core.Jvm => 304
	i64 u0x0bd636527481738b, ; 53: de/Microsoft.TestPlatform.CoreUtilities.resources.dll => 341
	i64 u0x0be2e1f8ce4064ed, ; 54: Xamarin.AndroidX.ViewPager => 285
	i64 u0x0c3ca6cc978e2aae, ; 55: pt-BR/Microsoft.Maui.Controls.resources => 326
	i64 u0x0c59ad9fbbd43abe, ; 56: Mono.Android => 172
	i64 u0x0c65741e86371ee3, ; 57: lib_Xamarin.Android.Glide.GifDecoder.dll.so => 219
	i64 u0x0c74af560004e816, ; 58: Microsoft.Win32.Registry.dll => 5
	i64 u0x0c7790f60165fc06, ; 59: lib_Microsoft.Maui.Essentials.dll.so => 193
	i64 u0x0c83c82812e96127, ; 60: lib_System.Net.Mail.dll.so => 67
	i64 u0x0cce4bce83380b7f, ; 61: Xamarin.AndroidX.Security.SecurityCrypto => 276
	i64 u0x0d13cd7cce4284e4, ; 62: System.Security.SecureString => 130
	i64 u0x0d518d16a10d1bcf, ; 63: Supabase.Functions => 208
	i64 u0x0d63f4f73521c24f, ; 64: lib_Xamarin.AndroidX.SavedState.SavedState.Ktx.dll.so => 275
	i64 u0x0d856d6da3c8d01a, ; 65: tr/Microsoft.TestPlatform.CrossPlatEngine.resources => 396
	i64 u0x0e04e702012f8463, ; 66: Xamarin.AndroidX.Emoji2 => 246
	i64 u0x0e14e73a54dda68e, ; 67: lib_System.Net.NameResolution.dll.so => 68
	i64 u0x0e2e96803ecb3446, ; 68: Supabase.Storage => 212
	i64 u0x0e70e6d2a1c3889b, ; 69: pt-BR/Microsoft.TestPlatform.CrossPlatEngine.resources => 390
	i64 u0x0f37dd7a62ae99af, ; 70: lib_Xamarin.AndroidX.Collection.Ktx.dll.so => 233
	i64 u0x0f5e7abaa7cf470a, ; 71: System.Net.HttpListener => 66
	i64 u0x0fc6e5711dabb83e, ; 72: lib_Supabase.Realtime.dll.so => 211
	i64 u0x1001f97bbe242e64, ; 73: System.IO.UnmanagedMemoryStream => 57
	i64 u0x102a31b45304b1da, ; 74: Xamarin.AndroidX.CustomView => 242
	i64 u0x103d83c4fd369687, ; 75: lib-fr-Microsoft.VisualStudio.TestPlatform.Common.resources.dll.so => 376
	i64 u0x1065c4cb554c3d75, ; 76: System.IO.IsolatedStorage.dll => 52
	i64 u0x10afd09c733f8274, ; 77: lib-ru-Microsoft.TestPlatform.CrossPlatEngine.resources.dll.so => 393
	i64 u0x10f6cfcbcf801616, ; 78: System.IO.Compression.Brotli => 43
	i64 u0x114443cdcf2091f1, ; 79: System.Security.Cryptography.Primitives => 125
	i64 u0x1149736c793b51a5, ; 80: es/Microsoft.VisualStudio.TestPlatform.Common.resources => 373
	i64 u0x118317d05a849d83, ; 81: ru/Microsoft.TestPlatform.CrossPlatEngine.resources.dll => 393
	i64 u0x11a603952763e1d4, ; 82: System.Net.Mail => 67
	i64 u0x11a70d0e1009fb11, ; 83: System.Net.WebSockets.dll => 81
	i64 u0x11b03392f318c4df, ; 84: lib-cs-Microsoft.TestPlatform.CoreUtilities.resources.dll.so => 339
	i64 u0x11f26371eee0d3c1, ; 85: lib_Xamarin.AndroidX.Lifecycle.Runtime.Ktx.dll.so => 260
	i64 u0x11fbe62d469cc1c8, ; 86: Microsoft.VisualStudio.DesignTools.TapContract.dll => 406
	i64 u0x12128b3f59302d47, ; 87: lib_System.Xml.Serialization.dll.so => 158
	i64 u0x123639456fb056da, ; 88: System.Reflection.Emit.Lightweight.dll => 92
	i64 u0x12521e9764603eaa, ; 89: lib_System.Resources.Reader.dll.so => 99
	i64 u0x125b7f94acb989db, ; 90: Xamarin.AndroidX.RecyclerView.dll => 272
	i64 u0x12d3b63863d4ab0b, ; 91: lib_System.Threading.Overlapped.dll.so => 141
	i64 u0x134eab1061c395ee, ; 92: System.Transactions => 151
	i64 u0x138567fa954faa55, ; 93: Xamarin.AndroidX.Browser => 229
	i64 u0x13a01de0cbc3f06c, ; 94: lib-fr-Microsoft.Maui.Controls.resources.dll.so => 313
	i64 u0x13beedefb0e28a45, ; 95: lib_System.Xml.XmlDocument.dll.so => 162
	i64 u0x13f1e5e209e91af4, ; 96: lib_Java.Interop.dll.so => 169
	i64 u0x13f1e880c25d96d1, ; 97: he/Microsoft.Maui.Controls.resources => 314
	i64 u0x143d8ea60a6a4011, ; 98: Microsoft.Extensions.DependencyInjection.Abstractions => 179
	i64 u0x1497051b917530bd, ; 99: lib_System.Net.WebSockets.dll.so => 81
	i64 u0x14b78ce3adce0011, ; 100: Microsoft.VisualStudio.DesignTools.TapContract => 406
	i64 u0x14d612a531c79c05, ; 101: Xamarin.JSpecify.dll => 296
	i64 u0x14e68447938213b7, ; 102: Xamarin.AndroidX.Collection.Ktx.dll => 233
	i64 u0x152a448bd1e745a7, ; 103: Microsoft.Win32.Primitives => 4
	i64 u0x1557de0138c445f4, ; 104: lib_Microsoft.Win32.Registry.dll.so => 5
	i64 u0x15bdc156ed462f2f, ; 105: lib_System.IO.FileSystem.dll.so => 51
	i64 u0x15e300c2c1668655, ; 106: System.Resources.Writer.dll => 101
	i64 u0x165324494d6bd19f, ; 107: lib-zh-Hant-Microsoft.VisualStudio.TestPlatform.Common.resources.dll.so => 403
	i64 u0x16bf2a22df043a09, ; 108: System.IO.Pipes.dll => 56
	i64 u0x16ea2b318ad2d830, ; 109: System.Security.Cryptography.Algorithms => 120
	i64 u0x16eeae54c7ebcc08, ; 110: System.Reflection.dll => 98
	i64 u0x17125c9a85b4929f, ; 111: lib_netstandard.dll.so => 168
	i64 u0x1716866f7416792e, ; 112: lib_System.Security.AccessControl.dll.so => 118
	i64 u0x1741b3b7f79f2b2a, ; 113: ko/Microsoft.TestPlatform.CrossPlatEngine.resources.dll => 384
	i64 u0x174f71c46216e44a, ; 114: Xamarin.KotlinX.Coroutines.Core => 301
	i64 u0x1752c12f1e1fc00c, ; 115: System.Core => 21
	i64 u0x17b56e25558a5d36, ; 116: lib-hu-Microsoft.Maui.Controls.resources.dll.so => 317
	i64 u0x17f9358913beb16a, ; 117: System.Text.Encodings.Web => 137
	i64 u0x1809fb23f29ba44a, ; 118: lib_System.Reflection.TypeExtensions.dll.so => 97
	i64 u0x18402a709e357f3b, ; 119: lib_Xamarin.KotlinX.Serialization.Core.Jvm.dll.so => 304
	i64 u0x18a9befae51bb361, ; 120: System.Net.WebClient => 77
	i64 u0x18f0ce884e87d89a, ; 121: nb/Microsoft.Maui.Controls.resources.dll => 323
	i64 u0x19777fba3c41b398, ; 122: Xamarin.AndroidX.Startup.StartupRuntime.dll => 278
	i64 u0x19a04823e8c89a80, ; 123: ru/Microsoft.TestPlatform.CoreUtilities.resources => 357
	i64 u0x19a4c090f14ebb66, ; 124: System.Security.Claims => 119
	i64 u0x19e70bbcc0e9676a, ; 125: lib-tr-Microsoft.TestPlatform.CommunicationUtilities.resources.dll.so => 395
	i64 u0x1a91866a319e9259, ; 126: lib_System.Collections.Concurrent.dll.so => 8
	i64 u0x1aac34d1917ba5d3, ; 127: lib_System.dll.so => 165
	i64 u0x1aad60783ffa3e5b, ; 128: lib-th-Microsoft.Maui.Controls.resources.dll.so => 332
	i64 u0x1aea8f1c3b282172, ; 129: lib_System.Net.Ping.dll.so => 70
	i64 u0x1b4b7a1d0d265fa2, ; 130: Xamarin.Android.Glide.DiskLruCache => 218
	i64 u0x1bbdb16cfa73e785, ; 131: Xamarin.AndroidX.Lifecycle.Runtime.Ktx.Android => 261
	i64 u0x1bc766e07b2b4241, ; 132: Xamarin.AndroidX.ResourceInspection.Annotation.dll => 273
	i64 u0x1c753b5ff15bce1b, ; 133: Mono.Android.Runtime.dll => 171
	i64 u0x1c95d7a11f528151, ; 134: ja/Microsoft.TestPlatform.CoreUtilities.resources => 349
	i64 u0x1cd47467799d8250, ; 135: System.Threading.Tasks.dll => 145
	i64 u0x1d23eafdc6dc346c, ; 136: System.Globalization.Calendars.dll => 40
	i64 u0x1da4110562816681, ; 137: Xamarin.AndroidX.Security.SecurityCrypto.dll => 276
	i64 u0x1db6820994506bf5, ; 138: System.IO.FileSystem.AccessControl.dll => 47
	i64 u0x1dbb0c2c6a999acb, ; 139: System.Diagnostics.StackTrace => 30
	i64 u0x1dbe05df8c9dcc4c, ; 140: pt-BR/Microsoft.TestPlatform.CrossPlatEngine.resources.dll => 390
	i64 u0x1dd146dba7b674b6, ; 141: es/Microsoft.TestPlatform.CoreUtilities.resources => 343
	i64 u0x1e1814b3f98ac2f7, ; 142: pl/Microsoft.TestPlatform.CrossPlatEngine.resources => 387
	i64 u0x1e3d87657e9659bc, ; 143: Xamarin.AndroidX.Navigation.UI => 270
	i64 u0x1e57ec3104eb59d9, ; 144: lib_Supabase.Gotrue.dll.so => 209
	i64 u0x1e5cbd6cf4a062c3, ; 145: lib-ru-Microsoft.TestPlatform.CoreUtilities.resources.dll.so => 357
	i64 u0x1e71143913d56c10, ; 146: lib-ko-Microsoft.Maui.Controls.resources.dll.so => 321
	i64 u0x1e7c31185e2fb266, ; 147: lib_System.Threading.Tasks.Parallel.dll.so => 144
	i64 u0x1ed8fcce5e9b50a0, ; 148: Microsoft.Extensions.Options.dll => 183
	i64 u0x1f055d15d807e1b2, ; 149: System.Xml.XmlSerializer => 163
	i64 u0x1f10494118749d51, ; 150: es/Microsoft.VisualStudio.TestPlatform.Common.resources.dll => 373
	i64 u0x1f1ed22c1085f044, ; 151: lib_System.Diagnostics.FileVersionInfo.dll.so => 28
	i64 u0x1f5fab12e0e9fe92, ; 152: zh-Hans/Microsoft.VisualStudio.TestPlatform.Common.resources.dll => 400
	i64 u0x1f61df9c5b94d2c1, ; 153: lib_System.Numerics.dll.so => 84
	i64 u0x1f750bb5421397de, ; 154: lib_Xamarin.AndroidX.Tracing.Tracing.dll.so => 280
	i64 u0x20237ea48006d7a8, ; 155: lib_System.Net.WebClient.dll.so => 77
	i64 u0x206ac5f478a0c100, ; 156: lib_Microsoft.VisualStudio.CodeCoverage.Shim.dll.so => 175
	i64 u0x209375905fcc1bad, ; 157: lib_System.IO.Compression.Brotli.dll.so => 43
	i64 u0x20aa4eb4c5cf3260, ; 158: Supabase.Realtime.dll => 211
	i64 u0x20fab3cf2dfbc8df, ; 159: lib_System.Diagnostics.Process.dll.so => 29
	i64 u0x20fda42305e0c27c, ; 160: fr/Microsoft.TestPlatform.CoreUtilities.resources => 345
	i64 u0x2110167c128cba15, ; 161: System.Globalization => 42
	i64 u0x21185b049020dba8, ; 162: it/Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll => 348
	i64 u0x21419508838f7547, ; 163: System.Runtime.CompilerServices.VisualC => 103
	i64 u0x2174319c0d835bc9, ; 164: System.Runtime => 117
	i64 u0x2198e5bc8b7153fa, ; 165: Xamarin.AndroidX.Annotation.Experimental.dll => 223
	i64 u0x219ea1b751a4dee4, ; 166: lib_System.IO.Compression.ZipFile.dll.so => 45
	i64 u0x21cc7e445dcd5469, ; 167: System.Reflection.Emit.ILGeneration => 91
	i64 u0x21d141b60a0fcffd, ; 168: ja/Microsoft.TestPlatform.CommunicationUtilities.resources => 380
	i64 u0x220fd4f2e7c48170, ; 169: th/Microsoft.Maui.Controls.resources => 332
	i64 u0x224538d85ed15a82, ; 170: System.IO.Pipes => 56
	i64 u0x22834f75f61db9d6, ; 171: lib-de-Microsoft.TestPlatform.CoreUtilities.resources.dll.so => 341
	i64 u0x22908438c6bed1af, ; 172: lib_System.Threading.Timer.dll.so => 148
	i64 u0x22fbc14e981e3b45, ; 173: lib_Microsoft.VisualStudio.DesignTools.MobileTapContracts.dll.so => 405
	i64 u0x237be844f1f812c7, ; 174: System.Threading.Thread.dll => 146
	i64 u0x23852b3bdc9f7096, ; 175: System.Resources.ResourceManager => 100
	i64 u0x23986dd7e5d4fc01, ; 176: System.IO.FileSystem.Primitives.dll => 49
	i64 u0x23cdb501930f8b56, ; 177: zh-Hant/Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll => 364
	i64 u0x2407aef2bbe8fadf, ; 178: System.Console => 20
	i64 u0x240abe014b27e7d3, ; 179: Xamarin.AndroidX.Core.dll => 238
	i64 u0x247619fe4413f8bf, ; 180: System.Runtime.Serialization.Primitives.dll => 114
	i64 u0x2476c0dcbe737761, ; 181: lib-ru-Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll.so => 358
	i64 u0x2482dd51b9f3d49e, ; 182: lib-de-Microsoft.VisualStudio.TestPlatform.Common.resources.dll.so => 370
	i64 u0x24de8d301281575e, ; 183: Xamarin.Android.Glide => 216
	i64 u0x24f5ac60a2807d3d, ; 184: lib-de-Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll.so => 342
	i64 u0x252073cc3caa62c2, ; 185: fr/Microsoft.Maui.Controls.resources.dll => 313
	i64 u0x253d9311471a284b, ; 186: lib-ko-Microsoft.VisualStudio.TestPlatform.Common.resources.dll.so => 385
	i64 u0x256b8d41255f01b1, ; 187: Xamarin.Google.Crypto.Tink.Android => 291
	i64 u0x2662c629b96b0b30, ; 188: lib_Xamarin.Kotlin.StdLib.dll.so => 297
	i64 u0x268c1439f13bcc29, ; 189: lib_Microsoft.Extensions.Primitives.dll.so => 184
	i64 u0x26a670e154a9c54b, ; 190: System.Reflection.Extensions.dll => 94
	i64 u0x26d077d9678fe34f, ; 191: System.IO.dll => 58
	i64 u0x270a44600c921861, ; 192: System.IdentityModel.Tokens.Jwt => 213
	i64 u0x272cce099681f8f9, ; 193: it/Microsoft.VisualStudio.TestPlatform.Common.resources => 379
	i64 u0x273f3515de5faf0d, ; 194: id/Microsoft.Maui.Controls.resources.dll => 318
	i64 u0x2742545f9094896d, ; 195: hr/Microsoft.Maui.Controls.resources => 316
	i64 u0x2759af78ab94d39b, ; 196: System.Net.WebSockets => 81
	i64 u0x2784c364cd130b5f, ; 197: zh-Hant/Microsoft.VisualStudio.TestPlatform.ObjectModel.resources => 364
	i64 u0x27b2b16f3e9de038, ; 198: Xamarin.Google.Crypto.Tink.Android.dll => 291
	i64 u0x27b410442fad6cf1, ; 199: Java.Interop.dll => 169
	i64 u0x27b97e0d52c3034a, ; 200: System.Diagnostics.Debug => 26
	i64 u0x27fd268e370669b5, ; 201: lib-pl-Microsoft.TestPlatform.CoreUtilities.resources.dll.so => 353
	i64 u0x2801845a2c71fbfb, ; 202: System.Net.Primitives.dll => 71
	i64 u0x286835e259162700, ; 203: lib_Xamarin.AndroidX.ProfileInstaller.ProfileInstaller.dll.so => 271
	i64 u0x2949f3617a02c6b2, ; 204: Xamarin.AndroidX.ExifInterface => 248
	i64 u0x29f947844fb7fc11, ; 205: Microsoft.Maui.Controls.HotReload.Forms => 404
	i64 u0x2a128783efe70ba0, ; 206: uk/Microsoft.Maui.Controls.resources.dll => 334
	i64 u0x2a3b095612184159, ; 207: lib_System.Net.NetworkInformation.dll.so => 69
	i64 u0x2a6507a5ffabdf28, ; 208: System.Diagnostics.TraceSource.dll => 33
	i64 u0x2ad156c8e1354139, ; 209: fi/Microsoft.Maui.Controls.resources => 312
	i64 u0x2ad5d6b13b7a3e04, ; 210: System.ComponentModel.DataAnnotations.dll => 14
	i64 u0x2af298f63581d886, ; 211: System.Text.RegularExpressions.dll => 139
	i64 u0x2af615542f04da50, ; 212: System.IdentityModel.Tokens.Jwt.dll => 213
	i64 u0x2afc1c4f898552ee, ; 213: lib_System.Formats.Asn1.dll.so => 38
	i64 u0x2b148910ed40fbf9, ; 214: zh-Hant/Microsoft.Maui.Controls.resources.dll => 338
	i64 u0x2b6989d78cba9a15, ; 215: Xamarin.AndroidX.Concurrent.Futures.dll => 234
	i64 u0x2b9c6ed377c46f79, ; 216: pl/Microsoft.VisualStudio.TestPlatform.ObjectModel.resources => 354
	i64 u0x2c42baa2af5f9385, ; 217: lib_MimeMapping.dll.so => 203
	i64 u0x2c8bd14bb93a7d82, ; 218: lib-pl-Microsoft.Maui.Controls.resources.dll.so => 325
	i64 u0x2cbd9262ca785540, ; 219: lib_System.Text.Encoding.CodePages.dll.so => 134
	i64 u0x2cbff1013f9b8dfd, ; 220: tr/Microsoft.VisualStudio.TestPlatform.ObjectModel.resources => 360
	i64 u0x2cc9e1fed6257257, ; 221: lib_System.Reflection.Emit.Lightweight.dll.so => 92
	i64 u0x2cd723e9fe623c7c, ; 222: lib_System.Private.Xml.Linq.dll.so => 88
	i64 u0x2d169d318a968379, ; 223: System.Threading.dll => 149
	i64 u0x2d47774b7d993f59, ; 224: sv/Microsoft.Maui.Controls.resources.dll => 331
	i64 u0x2d4964276447c38d, ; 225: Microsoft.VisualStudio.TestPlatform.Common.dll => 201
	i64 u0x2d5ffcae1ad0aaca, ; 226: System.Data.dll => 24
	i64 u0x2db915caf23548d2, ; 227: System.Text.Json.dll => 138
	i64 u0x2dcaa0bb15a4117a, ; 228: System.IO.UnmanagedMemoryStream.dll => 57
	i64 u0x2e5a40c319acb800, ; 229: System.IO.FileSystem => 51
	i64 u0x2e6f1f226821322a, ; 230: el/Microsoft.Maui.Controls.resources.dll => 310
	i64 u0x2f02f94df3200fe5, ; 231: System.Diagnostics.Process => 29
	i64 u0x2f2e98e1c89b1aff, ; 232: System.Xml.ReaderWriter => 157
	i64 u0x2f5911d9ba814e4e, ; 233: System.Diagnostics.Tracing => 34
	i64 u0x2f80abc36f27bc7c, ; 234: zh-Hans/Microsoft.TestPlatform.CoreUtilities.resources => 361
	i64 u0x2f84070a459bc31f, ; 235: lib_System.Xml.dll.so => 164
	i64 u0x308c48973687d4f4, ; 236: es/Microsoft.VisualStudio.TestPlatform.ObjectModel.resources => 344
	i64 u0x309ee9eeec09a71e, ; 237: lib_Xamarin.AndroidX.Fragment.dll.so => 249
	i64 u0x309f2bedefa9a318, ; 238: Microsoft.IdentityModel.Abstractions => 185
	i64 u0x30c6dda129408828, ; 239: System.IO.IsolatedStorage => 52
	i64 u0x31195fef5d8fb552, ; 240: _Microsoft.Android.Resource.Designer.dll => 408
	i64 u0x312c8ed623cbfc8d, ; 241: Xamarin.AndroidX.Window.dll => 287
	i64 u0x31496b779ed0663d, ; 242: lib_System.Reflection.DispatchProxy.dll.so => 90
	i64 u0x31542629d178e196, ; 243: ja/Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll => 350
	i64 u0x315f08d19390dc36, ; 244: Xamarin.Google.ErrorProne.TypeAnnotations => 293
	i64 u0x32243413e774362a, ; 245: Xamarin.AndroidX.CardView.dll => 230
	i64 u0x3235427f8d12dae1, ; 246: lib_System.Drawing.Primitives.dll.so => 35
	i64 u0x329753a17a517811, ; 247: fr/Microsoft.Maui.Controls.resources => 313
	i64 u0x32aa989ff07a84ff, ; 248: lib_System.Xml.ReaderWriter.dll.so => 157
	i64 u0x32dbba5d256b19d3, ; 249: Supabase.Core => 207
	i64 u0x33829542f112d59b, ; 250: System.Collections.Immutable => 9
	i64 u0x33a31443733849fe, ; 251: lib-es-Microsoft.Maui.Controls.resources.dll.so => 311
	i64 u0x33bac6925892b840, ; 252: lib-fr-Microsoft.TestPlatform.CommunicationUtilities.resources.dll.so => 374
	i64 u0x33ddfaff9f4ab0b6, ; 253: pl/Microsoft.VisualStudio.TestPlatform.Common.resources.dll => 388
	i64 u0x341abc357fbb4ebf, ; 254: lib_System.Net.Sockets.dll.so => 76
	i64 u0x3496c1e2dcaf5ecc, ; 255: lib_System.IO.Pipes.AccessControl.dll.so => 55
	i64 u0x34dfd74fe2afcf37, ; 256: Microsoft.Maui => 192
	i64 u0x34e292762d9615df, ; 257: cs/Microsoft.Maui.Controls.resources.dll => 307
	i64 u0x3508234247f48404, ; 258: Microsoft.Maui.Controls => 190
	i64 u0x353590da528c9d22, ; 259: System.ComponentModel.Annotations => 13
	i64 u0x3549870798b4cd30, ; 260: lib_Xamarin.AndroidX.ViewPager2.dll.so => 286
	i64 u0x355282fc1c909694, ; 261: Microsoft.Extensions.Configuration => 176
	i64 u0x3552fc5d578f0fbf, ; 262: Xamarin.AndroidX.Arch.Core.Common => 227
	i64 u0x355c649948d55d97, ; 263: lib_System.Runtime.Intrinsics.dll.so => 109
	i64 u0x35ea9d1c6834bc8c, ; 264: Xamarin.AndroidX.Lifecycle.ViewModel.Ktx.dll => 264
	i64 u0x35efa503bbf573eb, ; 265: ja/Microsoft.TestPlatform.CoreUtilities.resources.dll => 349
	i64 u0x36064e25c02b76de, ; 266: lib-es-Microsoft.VisualStudio.TestPlatform.Common.resources.dll.so => 373
	i64 u0x3628ab68db23a01a, ; 267: lib_System.Diagnostics.Tools.dll.so => 32
	i64 u0x3673b042508f5b6b, ; 268: lib_System.Runtime.Extensions.dll.so => 104
	i64 u0x36740f1a8ecdc6c4, ; 269: System.Numerics => 84
	i64 u0x36b2b50fdf589ae2, ; 270: System.Reflection.Emit.Lightweight => 92
	i64 u0x36c248b5d8ac90d1, ; 271: lib-cs-Microsoft.TestPlatform.CommunicationUtilities.resources.dll.so => 365
	i64 u0x36cada77dc79928b, ; 272: System.IO.MemoryMappedFiles => 53
	i64 u0x36cca91e2c8b494d, ; 273: cs/Microsoft.TestPlatform.CommunicationUtilities.resources => 365
	i64 u0x374ef46b06791af6, ; 274: System.Reflection.Primitives.dll => 96
	i64 u0x376bf93e521a5417, ; 275: lib_Xamarin.Jetbrains.Annotations.dll.so => 295
	i64 u0x37bc29f3183003b6, ; 276: lib_System.IO.dll.so => 58
	i64 u0x37ce62d295c85a6d, ; 277: Microsoft.TestPlatform.CommunicationUtilities => 198
	i64 u0x380134e03b1e160a, ; 278: System.Collections.Immutable.dll => 9
	i64 u0x38049b5c59b39324, ; 279: System.Runtime.CompilerServices.Unsafe => 102
	i64 u0x385c17636bb6fe6e, ; 280: Xamarin.AndroidX.CustomView.dll => 242
	i64 u0x38869c811d74050e, ; 281: System.Net.NameResolution.dll => 68
	i64 u0x393c226616977fdb, ; 282: lib_Xamarin.AndroidX.ViewPager.dll.so => 285
	i64 u0x395b3053dde89e41, ; 283: lib_System.Reactive.dll.so => 214
	i64 u0x395e37c3334cf82a, ; 284: lib-ca-Microsoft.Maui.Controls.resources.dll.so => 306
	i64 u0x39a87563fdb248a0, ; 285: System.Reactive.dll => 214
	i64 u0x39aa39fda111d9d3, ; 286: Newtonsoft.Json => 204
	i64 u0x39d9203f6923d013, ; 287: pt-BR/Microsoft.TestPlatform.CoreUtilities.resources => 355
	i64 u0x3ab5859054645f72, ; 288: System.Security.Cryptography.Primitives.dll => 125
	i64 u0x3ad75090c3fac0e9, ; 289: lib_Xamarin.AndroidX.ResourceInspection.Annotation.dll.so => 273
	i64 u0x3ae44ac43a1fbdbb, ; 290: System.Runtime.Serialization => 116
	i64 u0x3b860f9932505633, ; 291: lib_System.Text.Encoding.Extensions.dll.so => 135
	i64 u0x3bea9ebe8c027c01, ; 292: lib_Microsoft.IdentityModel.Tokens.dll.so => 188
	i64 u0x3c3aafb6b3a00bf6, ; 293: lib_System.Security.Cryptography.X509Certificates.dll.so => 126
	i64 u0x3c4049146b59aa90, ; 294: System.Runtime.InteropServices.JavaScript => 106
	i64 u0x3c7c495f58ac5ee9, ; 295: Xamarin.Kotlin.StdLib => 297
	i64 u0x3c7e5ed3d5db71bb, ; 296: System.Security => 131
	i64 u0x3cd9d281d402eb9b, ; 297: Xamarin.AndroidX.Browser.dll => 229
	i64 u0x3d1c50cc001a991e, ; 298: Xamarin.Google.Guava.ListenableFuture.dll => 294
	i64 u0x3d2b1913edfc08d7, ; 299: lib_System.Threading.ThreadPool.dll.so => 147
	i64 u0x3d46f0b995082740, ; 300: System.Xml.Linq => 156
	i64 u0x3d8a8f400514a790, ; 301: Xamarin.AndroidX.Fragment.Ktx.dll => 250
	i64 u0x3d9c2a242b040a50, ; 302: lib_Xamarin.AndroidX.Core.dll.so => 238
	i64 u0x3dbb6b9f5ab90fa7, ; 303: lib_Xamarin.AndroidX.DynamicAnimation.dll.so => 245
	i64 u0x3e5441657549b213, ; 304: Xamarin.AndroidX.ResourceInspection.Annotation => 273
	i64 u0x3e55ec9cf67513dd, ; 305: it/Microsoft.TestPlatform.CoreUtilities.resources => 347
	i64 u0x3e57d4d195c53c2e, ; 306: System.Reflection.TypeExtensions => 97
	i64 u0x3e616ab4ed1f3f15, ; 307: lib_System.Data.dll.so => 24
	i64 u0x3e933c211cbe5737, ; 308: pt-BR/Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll => 356
	i64 u0x3f1d226e6e06db7e, ; 309: Xamarin.AndroidX.SlidingPaneLayout.dll => 277
	i64 u0x3f510adf788828dd, ; 310: System.Threading.Tasks.Extensions => 143
	i64 u0x4007fff231dcbc12, ; 311: lib_Supabase.Postgrest.dll.so => 210
	i64 u0x407a10bb4bf95829, ; 312: lib_Xamarin.AndroidX.Navigation.Common.dll.so => 267
	i64 u0x40c98b6bd77346d4, ; 313: Microsoft.VisualBasic.dll => 3
	i64 u0x41833cf766d27d96, ; 314: mscorlib => 167
	i64 u0x41a7621619e2b2d8, ; 315: ko/Microsoft.VisualStudio.TestPlatform.Common.resources => 385
	i64 u0x41cab042be111c34, ; 316: lib_Xamarin.AndroidX.AppCompat.AppCompatResources.dll.so => 226
	i64 u0x423a9ecc4d905a88, ; 317: lib_System.Resources.ResourceManager.dll.so => 100
	i64 u0x423bf51ae7def810, ; 318: System.Xml.XPath => 161
	i64 u0x42462ff15ddba223, ; 319: System.Resources.Reader.dll => 99
	i64 u0x4291015ff4e5ef71, ; 320: Xamarin.AndroidX.Core.ViewTree.dll => 240
	i64 u0x42974eea8663ff1a, ; 321: lib-zh-Hant-Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll.so => 364
	i64 u0x42a31b86e6ccc3f0, ; 322: System.Diagnostics.Contracts => 25
	i64 u0x430e95b891249788, ; 323: lib_System.Reflection.Emit.dll.so => 93
	i64 u0x4324964c50301029, ; 324: ko/Microsoft.TestPlatform.CoreUtilities.resources.dll => 351
	i64 u0x43375950ec7c1b6a, ; 325: netstandard.dll => 168
	i64 u0x4339b4850a2262da, ; 326: lib-tr-Microsoft.VisualStudio.TestPlatform.Common.resources.dll.so => 397
	i64 u0x434c4e1d9284cdae, ; 327: Mono.Android.dll => 172
	i64 u0x43505013578652a0, ; 328: lib_Xamarin.AndroidX.Activity.Ktx.dll.so => 221
	i64 u0x437d06c381ed575a, ; 329: lib_Microsoft.VisualBasic.dll.so => 3
	i64 u0x43950f84de7cc79a, ; 330: pl/Microsoft.Maui.Controls.resources.dll => 325
	i64 u0x43e8ca5bc927ff37, ; 331: lib_Xamarin.AndroidX.Emoji2.ViewsHelper.dll.so => 247
	i64 u0x448bd33429269b19, ; 332: Microsoft.CSharp => 1
	i64 u0x4499fa3c8e494654, ; 333: lib_System.Runtime.Serialization.Primitives.dll.so => 114
	i64 u0x4515080865a951a5, ; 334: Xamarin.Kotlin.StdLib.dll => 297
	i64 u0x4545802489b736b9, ; 335: Xamarin.AndroidX.Fragment.Ktx => 250
	i64 u0x454b4d1e66bb783c, ; 336: Xamarin.AndroidX.Lifecycle.Process => 257
	i64 u0x458d2df79ac57c1d, ; 337: lib_System.IdentityModel.Tokens.Jwt.dll.so => 213
	i64 u0x45c40276a42e283e, ; 338: System.Diagnostics.TraceSource => 33
	i64 u0x45d443f2a29adc37, ; 339: System.AppContext.dll => 6
	i64 u0x46a4213bc97fe5ae, ; 340: lib-ru-Microsoft.Maui.Controls.resources.dll.so => 329
	i64 u0x47358bd471172e1d, ; 341: lib_System.Xml.Linq.dll.so => 156
	i64 u0x47daf4e1afbada10, ; 342: pt/Microsoft.Maui.Controls.resources => 327
	i64 u0x480c0a47dd42dd81, ; 343: lib_System.IO.MemoryMappedFiles.dll.so => 53
	i64 u0x49e952f19a4e2022, ; 344: System.ObjectModel => 85
	i64 u0x49f9e6948a8131e4, ; 345: lib_Xamarin.AndroidX.VersionedParcelable.dll.so => 284
	i64 u0x4a5667b2462a664b, ; 346: lib_Xamarin.AndroidX.Navigation.UI.dll.so => 270
	i64 u0x4a7a18981dbd56bc, ; 347: System.IO.Compression.FileSystem.dll => 44
	i64 u0x4aa5c60350917c06, ; 348: lib_Xamarin.AndroidX.Lifecycle.LiveData.Core.Ktx.dll.so => 256
	i64 u0x4aba48d8d6ad4487, ; 349: fr/Microsoft.TestPlatform.CrossPlatEngine.resources.dll => 375
	i64 u0x4b07a0ed0ab33ff4, ; 350: System.Runtime.Extensions.dll => 104
	i64 u0x4b576d47ac054f3c, ; 351: System.IO.FileSystem.AccessControl => 47
	i64 u0x4b7b6532ded934b7, ; 352: System.Text.Json => 138
	i64 u0x4c692e3597859a8d, ; 353: lib-es-Microsoft.TestPlatform.CoreUtilities.resources.dll.so => 343
	i64 u0x4c7755cf07ad2d5f, ; 354: System.Net.Http.Json.dll => 64
	i64 u0x4c8aed57adcb77ea, ; 355: ko/Microsoft.TestPlatform.CoreUtilities.resources => 351
	i64 u0x4cc5f15266470798, ; 356: lib_Xamarin.AndroidX.Loader.dll.so => 266
	i64 u0x4cf6f67dc77aacd2, ; 357: System.Net.NetworkInformation.dll => 69
	i64 u0x4d3183dd245425d4, ; 358: System.Net.WebSockets.Client.dll => 80
	i64 u0x4d479f968a05e504, ; 359: System.Linq.Expressions.dll => 59
	i64 u0x4d55a010ffc4faff, ; 360: System.Private.Xml => 89
	i64 u0x4d5cbe77561c5b2e, ; 361: System.Web.dll => 154
	i64 u0x4d77512dbd86ee4c, ; 362: lib_Xamarin.AndroidX.Arch.Core.Common.dll.so => 227
	i64 u0x4d7793536e79c309, ; 363: System.ServiceProcess => 133
	i64 u0x4d95fccc1f67c7ca, ; 364: System.Runtime.Loader.dll => 110
	i64 u0x4dcf44c3c9b076a2, ; 365: it/Microsoft.Maui.Controls.resources.dll => 319
	i64 u0x4dd9247f1d2c3235, ; 366: Xamarin.AndroidX.Loader.dll => 266
	i64 u0x4e2aeee78e2c4a87, ; 367: Xamarin.AndroidX.ProfileInstaller.ProfileInstaller => 271
	i64 u0x4e32f00cb0937401, ; 368: Mono.Android.Runtime => 171
	i64 u0x4e5eea4668ac2b18, ; 369: System.Text.Encoding.CodePages => 134
	i64 u0x4e8cf86fe3ecbfcd, ; 370: Supabase => 206
	i64 u0x4eb45a5b05732f08, ; 371: Microsoft.VisualStudio.TestPlatform.ObjectModel => 197
	i64 u0x4ebd0c4b82c5eefc, ; 372: lib_System.Threading.Channels.dll.so => 140
	i64 u0x4ee8eaa9c9c1151a, ; 373: System.Globalization.Calendars => 40
	i64 u0x4f0f420f6c43234c, ; 374: MimeMapping => 203
	i64 u0x4f21ee6ef9eb527e, ; 375: ca/Microsoft.Maui.Controls.resources => 306
	i64 u0x4fbe928aa2088256, ; 376: lib_ChessMAUI.dll.so => 0
	i64 u0x4ffd65baff757598, ; 377: Microsoft.IdentityModel.Tokens => 188
	i64 u0x5037f0be3c28c7a3, ; 378: lib_Microsoft.Maui.Controls.dll.so => 190
	i64 u0x50a1ca3e86e04d00, ; 379: pl/Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll => 354
	i64 u0x50a9a34dd08bb5dc, ; 380: lib-ko-Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll.so => 352
	i64 u0x50c3a29b21050d45, ; 381: System.Linq.Parallel.dll => 60
	i64 u0x510381d618ea4816, ; 382: tr/Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll => 360
	i64 u0x5131bbe80989093f, ; 383: Xamarin.AndroidX.Lifecycle.ViewModel.Android.dll => 263
	i64 u0x516324a5050a7e3c, ; 384: System.Net.WebProxy => 79
	i64 u0x516a814cd64ca4e3, ; 385: lib-cs-Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll.so => 340
	i64 u0x516d6f0b21a303de, ; 386: lib_System.Diagnostics.Contracts.dll.so => 25
	i64 u0x51bb8a2afe774e32, ; 387: System.Drawing => 36
	i64 u0x5247c5c32a4140f0, ; 388: System.Resources.Reader => 99
	i64 u0x5248797948d354b4, ; 389: pl/Microsoft.TestPlatform.CrossPlatEngine.resources.dll => 387
	i64 u0x526bb15e3c386364, ; 390: Xamarin.AndroidX.Lifecycle.Runtime.Ktx.dll => 260
	i64 u0x526ce79eb8e90527, ; 391: lib_System.Net.Primitives.dll.so => 71
	i64 u0x52829f00b4467c38, ; 392: lib_System.Data.Common.dll.so => 22
	i64 u0x529ffe06f39ab8db, ; 393: Xamarin.AndroidX.Core => 238
	i64 u0x52ff996554dbf352, ; 394: Microsoft.Maui.Graphics => 194
	i64 u0x535f7e40e8fef8af, ; 395: lib-sk-Microsoft.Maui.Controls.resources.dll.so => 330
	i64 u0x53978aac584c666e, ; 396: lib_System.Security.Cryptography.Cng.dll.so => 121
	i64 u0x53a96d5c86c9e194, ; 397: System.Net.NetworkInformation => 69
	i64 u0x53be1038a61e8d44, ; 398: System.Runtime.InteropServices.RuntimeInformation.dll => 107
	i64 u0x53c3014b9437e684, ; 399: lib-zh-HK-Microsoft.Maui.Controls.resources.dll.so => 336
	i64 u0x5435e6f049e9bc37, ; 400: System.Security.Claims.dll => 119
	i64 u0x54795225dd1587af, ; 401: lib_System.Runtime.dll.so => 117
	i64 u0x547a34f14e5f6210, ; 402: Xamarin.AndroidX.Lifecycle.Common.dll => 252
	i64 u0x556e8b63b660ab8b, ; 403: Xamarin.AndroidX.Lifecycle.Common.Jvm.dll => 253
	i64 u0x5588627c9a108ec9, ; 404: System.Collections.Specialized => 11
	i64 u0x55a898e4f42e3fae, ; 405: Microsoft.VisualBasic.Core.dll => 2
	i64 u0x55f6359464ac7396, ; 406: ru/Microsoft.VisualStudio.TestPlatform.Common.resources.dll => 394
	i64 u0x55fa0c610fe93bb1, ; 407: lib_System.Security.Cryptography.OpenSsl.dll.so => 124
	i64 u0x56442b99bc64bb47, ; 408: System.Runtime.Serialization.Xml.dll => 115
	i64 u0x56a8b26e1aeae27b, ; 409: System.Threading.Tasks.Dataflow => 142
	i64 u0x56ac440afb41fec6, ; 410: lib-pt-BR-Microsoft.TestPlatform.CommunicationUtilities.resources.dll.so => 389
	i64 u0x56b49f2e92804d70, ; 411: fr/Microsoft.VisualStudio.TestPlatform.ObjectModel.resources => 346
	i64 u0x56ef7c03f3efeb6a, ; 412: tr/Microsoft.TestPlatform.CommunicationUtilities.resources => 395
	i64 u0x56f932d61e93c07f, ; 413: System.Globalization.Extensions => 41
	i64 u0x571c5cfbec5ae8e2, ; 414: System.Private.Uri => 87
	i64 u0x576499c9f52fea31, ; 415: Xamarin.AndroidX.Annotation => 222
	i64 u0x579a06fed6eec900, ; 416: System.Private.CoreLib.dll => 173
	i64 u0x57c542c14049b66d, ; 417: System.Diagnostics.DiagnosticSource => 27
	i64 u0x581a8bd5cfda563e, ; 418: System.Threading.Timer => 148
	i64 u0x58601b2dda4a27b9, ; 419: lib-ja-Microsoft.Maui.Controls.resources.dll.so => 320
	i64 u0x58688d9af496b168, ; 420: Microsoft.Extensions.DependencyInjection.dll => 178
	i64 u0x588c167a79db6bfb, ; 421: lib_Xamarin.Google.ErrorProne.Annotations.dll.so => 292
	i64 u0x58bcba604ef3982b, ; 422: it/Microsoft.TestPlatform.CrossPlatEngine.resources => 378
	i64 u0x5906028ae5151104, ; 423: Xamarin.AndroidX.Activity.Ktx => 221
	i64 u0x595a356d23e8da9a, ; 424: lib_Microsoft.CSharp.dll.so => 1
	i64 u0x59f9e60b9475085f, ; 425: lib_Xamarin.AndroidX.Annotation.Experimental.dll.so => 223
	i64 u0x5a745f5101a75527, ; 426: lib_System.IO.Compression.FileSystem.dll.so => 44
	i64 u0x5a89a886ae30258d, ; 427: lib_Xamarin.AndroidX.CoordinatorLayout.dll.so => 237
	i64 u0x5a8f6699f4a1caa9, ; 428: lib_System.Threading.dll.so => 149
	i64 u0x5ae9cd33b15841bf, ; 429: System.ComponentModel => 18
	i64 u0x5b54391bdc6fcfe6, ; 430: System.Private.DataContractSerialization => 86
	i64 u0x5b5f0e240a06a2a2, ; 431: da/Microsoft.Maui.Controls.resources.dll => 308
	i64 u0x5b6152301435b2c6, ; 432: fr/Microsoft.VisualStudio.TestPlatform.Common.resources => 376
	i64 u0x5b8109e8e14c5e3e, ; 433: System.Globalization.Extensions.dll => 41
	i64 u0x5bddd04d72a9e350, ; 434: Xamarin.AndroidX.Lifecycle.LiveData.Core.Ktx => 256
	i64 u0x5bdf16b09da116ab, ; 435: Xamarin.AndroidX.Collection => 231
	i64 u0x5c019d5266093159, ; 436: lib_Xamarin.AndroidX.Lifecycle.Runtime.Ktx.Android.dll.so => 261
	i64 u0x5c30a4a35f9cc8c4, ; 437: lib_System.Reflection.Extensions.dll.so => 94
	i64 u0x5c393624b8176517, ; 438: lib_Microsoft.Extensions.Logging.dll.so => 180
	i64 u0x5c53c29f5073b0c9, ; 439: System.Diagnostics.FileVersionInfo => 28
	i64 u0x5c87463c575c7616, ; 440: lib_System.Globalization.Extensions.dll.so => 41
	i64 u0x5d0a4a29b02d9d3c, ; 441: System.Net.WebHeaderCollection.dll => 78
	i64 u0x5d40c9b15181641f, ; 442: lib_Xamarin.AndroidX.Emoji2.dll.so => 246
	i64 u0x5d6ca10d35e9485b, ; 443: lib_Xamarin.AndroidX.Concurrent.Futures.dll.so => 234
	i64 u0x5d7ec76c1c703055, ; 444: System.Threading.Tasks.Parallel => 144
	i64 u0x5db0cbbd1028510e, ; 445: lib_System.Runtime.InteropServices.dll.so => 108
	i64 u0x5db30905d3e5013b, ; 446: Xamarin.AndroidX.Collection.Jvm.dll => 232
	i64 u0x5e3144737cb8cd02, ; 447: ru/Microsoft.VisualStudio.TestPlatform.ObjectModel.resources => 358
	i64 u0x5e467bc8f09ad026, ; 448: System.Collections.Specialized.dll => 11
	i64 u0x5e5173b3208d97e7, ; 449: System.Runtime.Handles.dll => 105
	i64 u0x5ea92fdb19ec8c4c, ; 450: System.Text.Encodings.Web.dll => 137
	i64 u0x5eb8046dd40e9ac3, ; 451: System.ComponentModel.Primitives => 16
	i64 u0x5ec272d219c9aba4, ; 452: System.Security.Cryptography.Csp.dll => 122
	i64 u0x5eee1376d94c7f5e, ; 453: System.Net.HttpListener.dll => 66
	i64 u0x5f36ccf5c6a57e24, ; 454: System.Xml.ReaderWriter.dll => 157
	i64 u0x5f4294b9b63cb842, ; 455: System.Data.Common => 22
	i64 u0x5f9a2d823f664957, ; 456: lib-el-Microsoft.Maui.Controls.resources.dll.so => 310
	i64 u0x5fa6da9c3cd8142a, ; 457: lib_Xamarin.KotlinX.Serialization.Core.dll.so => 303
	i64 u0x5fac98e0b37a5b9d, ; 458: System.Runtime.CompilerServices.Unsafe.dll => 102
	i64 u0x60333e64b19917a3, ; 459: zh-Hans/Microsoft.TestPlatform.CoreUtilities.resources.dll => 361
	i64 u0x60956f8c6a518939, ; 460: it/Microsoft.TestPlatform.CrossPlatEngine.resources.dll => 378
	i64 u0x609f4b7b63d802d4, ; 461: lib_Microsoft.Extensions.DependencyInjection.dll.so => 178
	i64 u0x60cd4e33d7e60134, ; 462: Xamarin.KotlinX.Coroutines.Core.Jvm => 302
	i64 u0x60f62d786afcf130, ; 463: System.Memory => 63
	i64 u0x61547c937b554b2e, ; 464: de/Microsoft.TestPlatform.CrossPlatEngine.resources.dll => 369
	i64 u0x61bb78c89f867353, ; 465: System.IO => 58
	i64 u0x61be8d1299194243, ; 466: Microsoft.Maui.Controls.Xaml => 191
	i64 u0x61d2cba29557038f, ; 467: de/Microsoft.Maui.Controls.resources => 309
	i64 u0x61d88f399afb2f45, ; 468: lib_System.Runtime.Loader.dll.so => 110
	i64 u0x622eef6f9e59068d, ; 469: System.Private.CoreLib => 173
	i64 u0x63cdbd66ac39bb46, ; 470: lib_Microsoft.VisualStudio.DesignTools.XamlTapContract.dll.so => 407
	i64 u0x63d5e3aa4ef9b931, ; 471: Xamarin.KotlinX.Coroutines.Android.dll => 300
	i64 u0x63f1f6883c1e23c2, ; 472: lib_System.Collections.Immutable.dll.so => 9
	i64 u0x6400f68068c1e9f1, ; 473: Xamarin.Google.Android.Material.dll => 289
	i64 u0x640e3b14dbd325c2, ; 474: System.Security.Cryptography.Algorithms.dll => 120
	i64 u0x64587004560099b9, ; 475: System.Reflection => 98
	i64 u0x64b1529a438a3c45, ; 476: lib_System.Runtime.Handles.dll.so => 105
	i64 u0x652f54c2c36d4689, ; 477: fr/Microsoft.TestPlatform.CommunicationUtilities.resources.dll => 374
	i64 u0x6565fba2cd8f235b, ; 478: Xamarin.AndroidX.Lifecycle.ViewModel.Ktx => 264
	i64 u0x65ecac39144dd3cc, ; 479: Microsoft.Maui.Controls.dll => 190
	i64 u0x65ece51227bfa724, ; 480: lib_System.Runtime.Numerics.dll.so => 111
	i64 u0x661722438787b57f, ; 481: Xamarin.AndroidX.Annotation.Jvm.dll => 224
	i64 u0x6679b2337ee6b22a, ; 482: lib_System.IO.FileSystem.Primitives.dll.so => 49
	i64 u0x6692e924eade1b29, ; 483: lib_System.Console.dll.so => 20
	i64 u0x66a4e5c6a3fb0bae, ; 484: lib_Xamarin.AndroidX.Lifecycle.ViewModel.Android.dll.so => 263
	i64 u0x66d13304ce1a3efa, ; 485: Xamarin.AndroidX.CursorAdapter => 241
	i64 u0x66d845528e2c129f, ; 486: tr/Microsoft.VisualStudio.TestPlatform.Common.resources => 397
	i64 u0x674303f65d8fad6f, ; 487: lib_System.Net.Quic.dll.so => 72
	i64 u0x6756ca4cad62e9d6, ; 488: lib_Xamarin.AndroidX.ConstraintLayout.Core.dll.so => 236
	i64 u0x677e48285338f3dc, ; 489: Microsoft.VisualStudio.TestPlatform.Common => 201
	i64 u0x67c0802770244408, ; 490: System.Windows.dll => 155
	i64 u0x68100b69286e27cd, ; 491: lib_System.Formats.Tar.dll.so => 39
	i64 u0x68558ec653afa616, ; 492: lib-da-Microsoft.Maui.Controls.resources.dll.so => 308
	i64 u0x6872ec7a2e36b1ac, ; 493: System.Drawing.Primitives.dll => 35
	i64 u0x68751859283bf70e, ; 494: ru/Microsoft.VisualStudio.TestPlatform.Common.resources => 394
	i64 u0x68bb2c417aa9b61c, ; 495: Xamarin.KotlinX.AtomicFU.dll => 298
	i64 u0x68fbbbe2eb455198, ; 496: System.Formats.Asn1 => 38
	i64 u0x69063fc0ba8e6bdd, ; 497: he/Microsoft.Maui.Controls.resources.dll => 314
	i64 u0x69a3e26c76f6eec4, ; 498: Xamarin.AndroidX.Window.Extensions.Core.Core.dll => 288
	i64 u0x6a21bf131c5369b9, ; 499: ja/Microsoft.TestPlatform.CommunicationUtilities.resources.dll => 380
	i64 u0x6a4d7577b2317255, ; 500: System.Runtime.InteropServices.dll => 108
	i64 u0x6ace3b74b15ee4a4, ; 501: nb/Microsoft.Maui.Controls.resources => 323
	i64 u0x6afcedb171067e2b, ; 502: System.Core.dll => 21
	i64 u0x6bef98e124147c24, ; 503: Xamarin.Jetbrains.Annotations => 295
	i64 u0x6ca323bb74a4c28a, ; 504: Supabase.Storage.dll => 212
	i64 u0x6ce874bff138ce2b, ; 505: Xamarin.AndroidX.Lifecycle.ViewModel.dll => 262
	i64 u0x6d12bfaa99c72b1f, ; 506: lib_Microsoft.Maui.Graphics.dll.so => 194
	i64 u0x6d70755158ca866e, ; 507: lib_System.ComponentModel.EventBasedAsync.dll.so => 15
	i64 u0x6d79993361e10ef2, ; 508: Microsoft.Extensions.Primitives => 184
	i64 u0x6d7eeca99577fc8b, ; 509: lib_System.Net.WebProxy.dll.so => 79
	i64 u0x6d8515b19946b6a2, ; 510: System.Net.WebProxy.dll => 79
	i64 u0x6d86d56b84c8eb71, ; 511: lib_Xamarin.AndroidX.CursorAdapter.dll.so => 241
	i64 u0x6d9bea6b3e895cf7, ; 512: Microsoft.Extensions.Primitives.dll => 184
	i64 u0x6dcdef53874ce2ad, ; 513: zh-Hant/Microsoft.TestPlatform.CrossPlatEngine.resources => 402
	i64 u0x6e25a02c3833319a, ; 514: lib_Xamarin.AndroidX.Navigation.Fragment.dll.so => 268
	i64 u0x6e79c6bd8627412a, ; 515: Xamarin.AndroidX.SavedState.SavedState.Ktx => 275
	i64 u0x6e838d9a2a6f6c9e, ; 516: lib_System.ValueTuple.dll.so => 152
	i64 u0x6e9965ce1095e60a, ; 517: lib_System.Core.dll.so => 21
	i64 u0x6efaf83377d1cbc8, ; 518: pt-BR/Microsoft.TestPlatform.CommunicationUtilities.resources.dll => 389
	i64 u0x6f549bdbd19c7a4d, ; 519: Supabase.Gotrue => 209
	i64 u0x6fbbd276f7d22f0b, ; 520: lib_testhost.dll.so => 202
	i64 u0x6fd2265da78b93a4, ; 521: lib_Microsoft.Maui.dll.so => 192
	i64 u0x6fdfc7de82c33008, ; 522: cs/Microsoft.Maui.Controls.resources => 307
	i64 u0x6ffc4967cc47ba57, ; 523: System.IO.FileSystem.Watcher.dll => 50
	i64 u0x701cd46a1c25a5fe, ; 524: System.IO.FileSystem.dll => 51
	i64 u0x70e99f48c05cb921, ; 525: tr/Microsoft.Maui.Controls.resources.dll => 333
	i64 u0x70fd3deda22442d2, ; 526: lib-nb-Microsoft.Maui.Controls.resources.dll.so => 323
	i64 u0x71485e7ffdb4b958, ; 527: System.Reflection.Extensions => 94
	i64 u0x7162a2fce67a945f, ; 528: lib_Xamarin.Android.Glide.Annotations.dll.so => 217
	i64 u0x71a495ea3761dde8, ; 529: lib-it-Microsoft.Maui.Controls.resources.dll.so => 319
	i64 u0x71ad672adbe48f35, ; 530: System.ComponentModel.Primitives.dll => 16
	i64 u0x71c7c6e3f4f66d11, ; 531: Microsoft.TestPlatform.CommunicationUtilities.dll => 198
	i64 u0x720f102581a4a5c8, ; 532: Xamarin.AndroidX.Core.ViewTree => 240
	i64 u0x725f5a9e82a45c81, ; 533: System.Security.Cryptography.Encoding => 123
	i64 u0x728b2919ce60b3fb, ; 534: lib-tr-Microsoft.TestPlatform.CrossPlatEngine.resources.dll.so => 396
	i64 u0x72b1fb4109e08d7b, ; 535: lib-hr-Microsoft.Maui.Controls.resources.dll.so => 316
	i64 u0x72e0300099accce1, ; 536: System.Xml.XPath.XDocument => 160
	i64 u0x730bfb248998f67a, ; 537: System.IO.Compression.ZipFile => 45
	i64 u0x732b2d67b9e5c47b, ; 538: Xamarin.Google.ErrorProne.Annotations.dll => 292
	i64 u0x734b76fdc0dc05bb, ; 539: lib_GoogleGson.dll.so => 174
	i64 u0x739da4816de9dc38, ; 540: lib-ja-Microsoft.TestPlatform.CrossPlatEngine.resources.dll.so => 381
	i64 u0x73a6be34e822f9d1, ; 541: lib_System.Runtime.Serialization.dll.so => 116
	i64 u0x73e4ce94e2eb6ffc, ; 542: lib_System.Memory.dll.so => 63
	i64 u0x743a1eccf080489a, ; 543: WindowsBase.dll => 166
	i64 u0x755a91767330b3d4, ; 544: lib_Microsoft.Extensions.Configuration.dll.so => 176
	i64 u0x7560b05f2364d75d, ; 545: ko/Microsoft.VisualStudio.TestPlatform.ObjectModel.resources => 352
	i64 u0x75c326eb821b85c4, ; 546: lib_System.ComponentModel.DataAnnotations.dll.so => 14
	i64 u0x76012e7334db86e5, ; 547: lib_Xamarin.AndroidX.SavedState.dll.so => 274
	i64 u0x76ca07b878f44da0, ; 548: System.Runtime.Numerics.dll => 111
	i64 u0x7736c8a96e51a061, ; 549: lib_Xamarin.AndroidX.Annotation.Jvm.dll.so => 224
	i64 u0x778a805e625329ef, ; 550: System.Linq.Parallel => 60
	i64 u0x779290cc2b801eb7, ; 551: Xamarin.KotlinX.AtomicFU.Jvm => 299
	i64 u0x77f716bbffa08efc, ; 552: cs/Microsoft.VisualStudio.TestPlatform.Common.resources.dll => 367
	i64 u0x77f8a4acc2fdc449, ; 553: System.Security.Cryptography.Cng.dll => 121
	i64 u0x780bc73597a503a9, ; 554: lib-ms-Microsoft.Maui.Controls.resources.dll.so => 322
	i64 u0x782c5d8eb99ff201, ; 555: lib_Microsoft.VisualBasic.Core.dll.so => 2
	i64 u0x783606d1e53e7a1a, ; 556: th/Microsoft.Maui.Controls.resources.dll => 332
	i64 u0x78a45e51311409b6, ; 557: Xamarin.AndroidX.Fragment.dll => 249
	i64 u0x78ed4ab8f9d800a1, ; 558: Xamarin.AndroidX.Lifecycle.ViewModel => 262
	i64 u0x7914e7def1e0ff0a, ; 559: lib-fr-Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll.so => 346
	i64 u0x793f297a57fc09c4, ; 560: ja/Microsoft.TestPlatform.CrossPlatEngine.resources.dll => 381
	i64 u0x7975de26537ed011, ; 561: es/Microsoft.TestPlatform.CrossPlatEngine.resources => 372
	i64 u0x7983848272d2cc4c, ; 562: zh-Hant/Microsoft.TestPlatform.CommunicationUtilities.resources => 401
	i64 u0x7a19a916afbea1b1, ; 563: Microsoft.TestPlatform.CrossPlatEngine.dll => 199
	i64 u0x7a39601d6f0bb831, ; 564: lib_Xamarin.KotlinX.AtomicFU.dll.so => 298
	i64 u0x7a5207a7c82d30b4, ; 565: lib_Xamarin.JSpecify.dll.so => 296
	i64 u0x7a7e7eddf79c5d26, ; 566: lib_Xamarin.AndroidX.Lifecycle.ViewModel.dll.so => 262
	i64 u0x7a9a57d43b0845fa, ; 567: System.AppContext => 6
	i64 u0x7ab9e9434ce6844c, ; 568: ru/Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll => 358
	i64 u0x7ad0f4f1e5d08183, ; 569: Xamarin.AndroidX.Collection.dll => 231
	i64 u0x7adb8da2ac89b647, ; 570: fi/Microsoft.Maui.Controls.resources.dll => 312
	i64 u0x7b13d9eaa944ade8, ; 571: Xamarin.AndroidX.DynamicAnimation.dll => 245
	i64 u0x7b4927e421291c41, ; 572: Microsoft.IdentityModel.JsonWebTokens.dll => 186
	i64 u0x7bef86a4335c4870, ; 573: System.ComponentModel.TypeConverter => 17
	i64 u0x7c0820144cd34d6a, ; 574: sk/Microsoft.Maui.Controls.resources.dll => 330
	i64 u0x7c2a0bd1e0f988fc, ; 575: lib-de-Microsoft.Maui.Controls.resources.dll.so => 309
	i64 u0x7c41d387501568ba, ; 576: System.Net.WebClient.dll => 77
	i64 u0x7c482cd79bd24b13, ; 577: lib_Xamarin.AndroidX.ConstraintLayout.dll.so => 235
	i64 u0x7cd2ec8eaf5241cd, ; 578: System.Security.dll => 131
	i64 u0x7cf0abbe92e6aafc, ; 579: lib_Microsoft.TestPlatform.CoreUtilities.dll.so => 195
	i64 u0x7cf9ae50dd350622, ; 580: Xamarin.Jetbrains.Annotations.dll => 295
	i64 u0x7d62f30cd753b008, ; 581: lib-it-Microsoft.TestPlatform.CommunicationUtilities.resources.dll.so => 377
	i64 u0x7d649b75d580bb42, ; 582: ms/Microsoft.Maui.Controls.resources.dll => 322
	i64 u0x7d8ee2bdc8e3aad1, ; 583: System.Numerics.Vectors => 83
	i64 u0x7df5df8db8eaa6ac, ; 584: Microsoft.Extensions.Logging.Debug => 182
	i64 u0x7dfc3d6d9d8d7b70, ; 585: System.Collections => 12
	i64 u0x7e04026de720bd4c, ; 586: lib-pt-BR-Microsoft.TestPlatform.CoreUtilities.resources.dll.so => 355
	i64 u0x7e2e564fa2f76c65, ; 587: lib_System.Diagnostics.Tracing.dll.so => 34
	i64 u0x7e302e110e1e1346, ; 588: lib_System.Security.Claims.dll.so => 119
	i64 u0x7e4465b3f78ad8d0, ; 589: Xamarin.KotlinX.Serialization.Core.dll => 303
	i64 u0x7e571cad5915e6c3, ; 590: lib_Xamarin.AndroidX.Lifecycle.Process.dll.so => 257
	i64 u0x7e6b1ca712437d7d, ; 591: Xamarin.AndroidX.Emoji2.ViewsHelper => 247
	i64 u0x7e946809d6008ef2, ; 592: lib_System.ObjectModel.dll.so => 85
	i64 u0x7ea0272c1b4a9635, ; 593: lib_Xamarin.Android.Glide.dll.so => 216
	i64 u0x7ecc13347c8fd849, ; 594: lib_System.ComponentModel.dll.so => 18
	i64 u0x7f00ddd9b9ca5a13, ; 595: Xamarin.AndroidX.ViewPager.dll => 285
	i64 u0x7f09ace4268bf4b4, ; 596: lib-pt-BR-Microsoft.VisualStudio.TestPlatform.Common.resources.dll.so => 391
	i64 u0x7f9351cd44b1273f, ; 597: Microsoft.Extensions.Configuration.Abstractions => 177
	i64 u0x7fbd557c99b3ce6f, ; 598: lib_Xamarin.AndroidX.Lifecycle.LiveData.Core.dll.so => 255
	i64 u0x8076a9a44a2ca331, ; 599: System.Net.Quic => 72
	i64 u0x809b1d586a1c34b9, ; 600: Plugin.Maui.Audio => 205
	i64 u0x80b7e726b0280681, ; 601: Microsoft.VisualStudio.DesignTools.MobileTapContracts => 405
	i64 u0x80da183a87731838, ; 602: System.Reflection.Metadata => 95
	i64 u0x81289c582be53053, ; 603: fr/Microsoft.VisualStudio.TestPlatform.Common.resources.dll => 376
	i64 u0x812c069d5cdecc17, ; 604: System.dll => 165
	i64 u0x81381be520a60adb, ; 605: Xamarin.AndroidX.Interpolator.dll => 251
	i64 u0x81657cec2b31e8aa, ; 606: System.Net => 82
	i64 u0x81ab745f6c0f5ce6, ; 607: zh-Hant/Microsoft.Maui.Controls.resources => 338
	i64 u0x8277f2be6b5ce05f, ; 608: Xamarin.AndroidX.AppCompat => 225
	i64 u0x828f06563b30bc50, ; 609: lib_Xamarin.AndroidX.CardView.dll.so => 230
	i64 u0x82920a8d9194a019, ; 610: Xamarin.KotlinX.AtomicFU.Jvm.dll => 299
	i64 u0x82b399cb01b531c4, ; 611: lib_System.Web.dll.so => 154
	i64 u0x82df8f5532a10c59, ; 612: lib_System.Drawing.dll.so => 36
	i64 u0x82f0b6e911d13535, ; 613: lib_System.Transactions.dll.so => 151
	i64 u0x82f6403342e12049, ; 614: uk/Microsoft.Maui.Controls.resources => 334
	i64 u0x836e663d8ebfc240, ; 615: lib-cs-Microsoft.VisualStudio.TestPlatform.Common.resources.dll.so => 367
	i64 u0x83a2d9ad3c54f4f8, ; 616: MimeMapping.dll => 203
	i64 u0x83a7afd2c49adc86, ; 617: lib_Microsoft.IdentityModel.Abstractions.dll.so => 185
	i64 u0x83c14ba66c8e2b8c, ; 618: zh-Hans/Microsoft.Maui.Controls.resources => 337
	i64 u0x846ce984efea52c7, ; 619: System.Threading.Tasks.Parallel.dll => 144
	i64 u0x84ae73148a4557d2, ; 620: lib_System.IO.Pipes.dll.so => 56
	i64 u0x84b01102c12a9232, ; 621: System.Runtime.Serialization.Json.dll => 113
	i64 u0x850c5ba0b57ce8e7, ; 622: lib_Xamarin.AndroidX.Collection.dll.so => 231
	i64 u0x851d02edd334b044, ; 623: Xamarin.AndroidX.VectorDrawable => 282
	i64 u0x852cb4adda21b24b, ; 624: pl/Microsoft.VisualStudio.TestPlatform.Common.resources => 388
	i64 u0x85c919db62150978, ; 625: Xamarin.AndroidX.Transition.dll => 281
	i64 u0x85e69d1f06b1172c, ; 626: pt-BR/Microsoft.VisualStudio.TestPlatform.ObjectModel.resources => 356
	i64 u0x8628f3a18eddfda3, ; 627: cs/Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll => 340
	i64 u0x8662aaeb94fef37f, ; 628: lib_System.Dynamic.Runtime.dll.so => 37
	i64 u0x86a909228dc7657b, ; 629: lib-zh-Hant-Microsoft.Maui.Controls.resources.dll.so => 338
	i64 u0x86b3e00c36b84509, ; 630: Microsoft.Extensions.Configuration.dll => 176
	i64 u0x86b62cb077ec4fd7, ; 631: System.Runtime.Serialization.Xml => 115
	i64 u0x8706ffb12bf3f53d, ; 632: Xamarin.AndroidX.Annotation.Experimental => 223
	i64 u0x872a5b14c18d328c, ; 633: System.ComponentModel.DataAnnotations => 14
	i64 u0x872fb9615bc2dff0, ; 634: Xamarin.Android.Glide.Annotations.dll => 217
	i64 u0x87c69b87d9283884, ; 635: lib_System.Threading.Thread.dll.so => 146
	i64 u0x87e297a00c4e1d67, ; 636: lib-pl-Microsoft.VisualStudio.TestPlatform.Common.resources.dll.so => 388
	i64 u0x87f6569b25707834, ; 637: System.IO.Compression.Brotli.dll => 43
	i64 u0x883453c815b80bd1, ; 638: es/Microsoft.TestPlatform.CommunicationUtilities.resources.dll => 371
	i64 u0x8842b3a5d2d3fb36, ; 639: Microsoft.Maui.Essentials => 193
	i64 u0x88926583efe7ee86, ; 640: Xamarin.AndroidX.Activity.Ktx.dll => 221
	i64 u0x88ba6bc4f7762b03, ; 641: lib_System.Reflection.dll.so => 98
	i64 u0x88bda98e0cffb7a9, ; 642: lib_Xamarin.KotlinX.Coroutines.Core.Jvm.dll.so => 302
	i64 u0x88d8040a69d1a1d6, ; 643: lib-cs-Microsoft.TestPlatform.CrossPlatEngine.resources.dll.so => 366
	i64 u0x8930322c7bd8f768, ; 644: netstandard => 168
	i64 u0x897a606c9e39c75f, ; 645: lib_System.ComponentModel.Primitives.dll.so => 16
	i64 u0x89911a22005b92b7, ; 646: System.IO.FileSystem.DriveInfo.dll => 48
	i64 u0x89c5188089ec2cd5, ; 647: lib_System.Runtime.InteropServices.RuntimeInformation.dll.so => 107
	i64 u0x8a19e3dc71b34b2c, ; 648: System.Reflection.TypeExtensions.dll => 97
	i64 u0x8ad229ea26432ee2, ; 649: Xamarin.AndroidX.Loader => 266
	i64 u0x8b4ff5d0fdd5faa1, ; 650: lib_System.Diagnostics.DiagnosticSource.dll.so => 27
	i64 u0x8b541d476eb3774c, ; 651: System.Security.Principal.Windows => 128
	i64 u0x8b8d01333a96d0b5, ; 652: System.Diagnostics.Process.dll => 29
	i64 u0x8b9ceca7acae3451, ; 653: lib-he-Microsoft.Maui.Controls.resources.dll.so => 314
	i64 u0x8bee3bee77f5fe32, ; 654: lib-tr-Microsoft.TestPlatform.CoreUtilities.resources.dll.so => 359
	i64 u0x8c71fea781700a57, ; 655: lib-fr-Microsoft.TestPlatform.CoreUtilities.resources.dll.so => 345
	i64 u0x8cb8f612b633affb, ; 656: Xamarin.AndroidX.SavedState.SavedState.Ktx.dll => 275
	i64 u0x8cdfdb4ce85fb925, ; 657: lib_System.Security.Principal.Windows.dll.so => 128
	i64 u0x8cdfe7b8f4caa426, ; 658: System.IO.Compression.FileSystem => 44
	i64 u0x8d0f420977c2c1c7, ; 659: Xamarin.AndroidX.CursorAdapter.dll => 241
	i64 u0x8d52f7ea2796c531, ; 660: Xamarin.AndroidX.Emoji2.dll => 246
	i64 u0x8d7b8ab4b3310ead, ; 661: System.Threading => 149
	i64 u0x8d8e92e1abfa80c1, ; 662: Microsoft.VisualStudio.TestPlatform.ObjectModel.dll => 197
	i64 u0x8da188285aadfe8e, ; 663: System.Collections.Concurrent => 8
	i64 u0x8ec6e06a61c1baeb, ; 664: lib_Newtonsoft.Json.dll.so => 204
	i64 u0x8ed807bfe9858dfc, ; 665: Xamarin.AndroidX.Navigation.Common => 267
	i64 u0x8ee08b8194a30f48, ; 666: lib-hi-Microsoft.Maui.Controls.resources.dll.so => 315
	i64 u0x8ef7601039857a44, ; 667: lib-ro-Microsoft.Maui.Controls.resources.dll.so => 328
	i64 u0x8f32c6f611f6ffab, ; 668: pt/Microsoft.Maui.Controls.resources.dll => 327
	i64 u0x8f44b45eb046bbd1, ; 669: System.ServiceModel.Web.dll => 132
	i64 u0x8f840188c33b8d8e, ; 670: lib-it-Microsoft.TestPlatform.CoreUtilities.resources.dll.so => 347
	i64 u0x8f8829d21c8985a4, ; 671: lib-pt-BR-Microsoft.Maui.Controls.resources.dll.so => 326
	i64 u0x8fbf5b0114c6dcef, ; 672: System.Globalization.dll => 42
	i64 u0x8fcc8c2a81f3d9e7, ; 673: Xamarin.KotlinX.Serialization.Core => 303
	i64 u0x90263f8448b8f572, ; 674: lib_System.Diagnostics.TraceSource.dll.so => 33
	i64 u0x903101b46fb73a04, ; 675: _Microsoft.Android.Resource.Designer => 408
	i64 u0x90393bd4865292f3, ; 676: lib_System.IO.Compression.dll.so => 46
	i64 u0x905e2b8e7ae91ae6, ; 677: System.Threading.Tasks.Extensions.dll => 143
	i64 u0x90607c0601131e4c, ; 678: ja/Microsoft.VisualStudio.TestPlatform.Common.resources.dll => 382
	i64 u0x90634f86c5ebe2b5, ; 679: Xamarin.AndroidX.Lifecycle.ViewModel.Android => 263
	i64 u0x907b636704ad79ef, ; 680: lib_Microsoft.Maui.Controls.Xaml.dll.so => 191
	i64 u0x90e3b29c34a8bacd, ; 681: cs/Microsoft.TestPlatform.CrossPlatEngine.resources => 366
	i64 u0x90e9efbfd68593e0, ; 682: lib_Xamarin.AndroidX.Lifecycle.LiveData.dll.so => 254
	i64 u0x91418dc638b29e68, ; 683: lib_Xamarin.AndroidX.CustomView.dll.so => 242
	i64 u0x9157bd523cd7ed36, ; 684: lib_System.Text.Json.dll.so => 138
	i64 u0x91a74f07b30d37e2, ; 685: System.Linq.dll => 62
	i64 u0x91cb86ea3b17111d, ; 686: System.ServiceModel.Web => 132
	i64 u0x91e4c2b1b7e54d02, ; 687: ChessMAUI.dll => 0
	i64 u0x91fa41a87223399f, ; 688: ca/Microsoft.Maui.Controls.resources.dll => 306
	i64 u0x92054e486c0c7ea7, ; 689: System.IO.FileSystem.DriveInfo => 48
	i64 u0x9285ff6974b99a0c, ; 690: lib_Microsoft.VisualStudio.TestPlatform.ObjectModel.dll.so => 197
	i64 u0x928614058c40c4cd, ; 691: lib_System.Xml.XPath.XDocument.dll.so => 160
	i64 u0x92b138fffca2b01e, ; 692: lib_Xamarin.AndroidX.Arch.Core.Runtime.dll.so => 228
	i64 u0x92dfc2bfc6c6a888, ; 693: Xamarin.AndroidX.Lifecycle.LiveData => 254
	i64 u0x9323549f9795759a, ; 694: fr/Microsoft.TestPlatform.CrossPlatEngine.resources => 375
	i64 u0x932544f9f6326c59, ; 695: cs/Microsoft.TestPlatform.CrossPlatEngine.resources.dll => 366
	i64 u0x933da2c779423d68, ; 696: Xamarin.Android.Glide.Annotations => 217
	i64 u0x9388aad9b7ae40ce, ; 697: lib_Xamarin.AndroidX.Lifecycle.Common.dll.so => 252
	i64 u0x93ceac81192e5127, ; 698: ru/Microsoft.TestPlatform.CommunicationUtilities.resources => 392
	i64 u0x93cfa73ab28d6e35, ; 699: ms/Microsoft.Maui.Controls.resources => 322
	i64 u0x93d676acd99f6b0b, ; 700: lib-ko-Microsoft.TestPlatform.CoreUtilities.resources.dll.so => 351
	i64 u0x93edd05f7ccd2684, ; 701: zh-Hans/Microsoft.TestPlatform.CommunicationUtilities.resources => 398
	i64 u0x941c00d21e5c0679, ; 702: lib_Xamarin.AndroidX.Transition.dll.so => 281
	i64 u0x944077d8ca3c6580, ; 703: System.IO.Compression.dll => 46
	i64 u0x94506ab8fd24b6a2, ; 704: lib-ru-Microsoft.VisualStudio.TestPlatform.Common.resources.dll.so => 394
	i64 u0x948cffedc8ed7960, ; 705: System.Xml => 164
	i64 u0x948d746a7702861f, ; 706: Microsoft.IdentityModel.Logging.dll => 187
	i64 u0x94c8990839c4bdb1, ; 707: lib_Xamarin.AndroidX.Interpolator.dll.so => 251
	i64 u0x94db97ab790dcc4e, ; 708: de/Microsoft.TestPlatform.CommunicationUtilities.resources.dll => 368
	i64 u0x9564283c37ed59a9, ; 709: lib_Microsoft.IdentityModel.Logging.dll.so => 187
	i64 u0x967fc325e09bfa8c, ; 710: es/Microsoft.Maui.Controls.resources => 311
	i64 u0x9680534091d8ebe7, ; 711: Microsoft.TestPlatform.CoreUtilities => 195
	i64 u0x9686161486d34b81, ; 712: lib_Xamarin.AndroidX.ExifInterface.dll.so => 248
	i64 u0x9732d8dbddea3d9a, ; 713: id/Microsoft.Maui.Controls.resources => 318
	i64 u0x978be80e5210d31b, ; 714: Microsoft.Maui.Graphics.dll => 194
	i64 u0x97b8c771ea3e4220, ; 715: System.ComponentModel.dll => 18
	i64 u0x97d426e0c73026d9, ; 716: zh-Hans/Microsoft.VisualStudio.TestPlatform.Common.resources => 400
	i64 u0x97e144c9d3c6976e, ; 717: System.Collections.Concurrent.dll => 8
	i64 u0x983328d830ed5285, ; 718: ja/Microsoft.VisualStudio.TestPlatform.ObjectModel.resources => 350
	i64 u0x984184e3c70d4419, ; 719: GoogleGson => 174
	i64 u0x9843944103683dd3, ; 720: Xamarin.AndroidX.Core.Core.Ktx => 239
	i64 u0x98d720cc4597562c, ; 721: System.Security.Cryptography.OpenSsl => 124
	i64 u0x991d510397f92d9d, ; 722: System.Linq.Expressions => 59
	i64 u0x996ceeb8a3da3d67, ; 723: System.Threading.Overlapped.dll => 141
	i64 u0x99a00ca5270c6878, ; 724: Xamarin.AndroidX.Navigation.Runtime => 269
	i64 u0x99cdc6d1f2d3a72f, ; 725: ko/Microsoft.Maui.Controls.resources.dll => 321
	i64 u0x99dfd0621c103c6a, ; 726: pl/Microsoft.TestPlatform.CommunicationUtilities.resources => 386
	i64 u0x9a01b1da98b6ee10, ; 727: Xamarin.AndroidX.Lifecycle.Runtime.dll => 258
	i64 u0x9a5ccc274fd6e6ee, ; 728: Jsr305Binding.dll => 290
	i64 u0x9a816d9654deff7c, ; 729: Microsoft.IO.RecyclableMemoryStream => 189
	i64 u0x9a9efc949c5553d0, ; 730: Microsoft.TestPlatform.CrossPlatEngine => 199
	i64 u0x9ac327a470ec1011, ; 731: zh-Hant/Microsoft.VisualStudio.TestPlatform.Common.resources => 403
	i64 u0x9ae6940b11c02876, ; 732: lib_Xamarin.AndroidX.Window.dll.so => 287
	i64 u0x9b211a749105beac, ; 733: System.Transactions.Local => 150
	i64 u0x9b8734714671022d, ; 734: System.Threading.Tasks.Dataflow.dll => 142
	i64 u0x9bc6aea27fbf034f, ; 735: lib_Xamarin.KotlinX.Coroutines.Core.dll.so => 301
	i64 u0x9bd8cc74558ad4c7, ; 736: Xamarin.KotlinX.AtomicFU => 298
	i64 u0x9c0747881cb9ce0f, ; 737: lib_Microsoft.VisualStudio.TestPlatform.Common.dll.so => 201
	i64 u0x9c244ac7cda32d26, ; 738: System.Security.Cryptography.X509Certificates.dll => 126
	i64 u0x9c36a0f95393e81c, ; 739: Supabase.Postgrest.dll => 210
	i64 u0x9c465f280cf43733, ; 740: lib_Xamarin.KotlinX.Coroutines.Android.dll.so => 300
	i64 u0x9c8f6872beab6408, ; 741: System.Xml.XPath.XDocument.dll => 160
	i64 u0x9ce01cf91101ae23, ; 742: System.Xml.XmlDocument => 162
	i64 u0x9d128180c81d7ce6, ; 743: Xamarin.AndroidX.CustomView.PoolingContainer => 243
	i64 u0x9d5dbcf5a48583fe, ; 744: lib_Xamarin.AndroidX.Activity.dll.so => 220
	i64 u0x9d74dee1a7725f34, ; 745: Microsoft.Extensions.Configuration.Abstractions.dll => 177
	i64 u0x9dec633c9d5b47d1, ; 746: pl/Microsoft.TestPlatform.CoreUtilities.resources => 353
	i64 u0x9e4534b6adaf6e84, ; 747: nl/Microsoft.Maui.Controls.resources => 324
	i64 u0x9e4b95dec42769f7, ; 748: System.Diagnostics.Debug.dll => 26
	i64 u0x9eaf1efdf6f7267e, ; 749: Xamarin.AndroidX.Navigation.Common.dll => 267
	i64 u0x9ef542cf1f78c506, ; 750: Xamarin.AndroidX.Lifecycle.LiveData.Core => 255
	i64 u0x9fe82da909b0232f, ; 751: lib-es-Microsoft.TestPlatform.CrossPlatEngine.resources.dll.so => 372
	i64 u0xa00832eb975f56a8, ; 752: lib_System.Net.dll.so => 82
	i64 u0xa075d4a589900eea, ; 753: Microsoft.TestPlatform.CoreUtilities.dll => 195
	i64 u0xa0869eed93c2a9c4, ; 754: de/Microsoft.VisualStudio.TestPlatform.Common.resources.dll => 370
	i64 u0xa0ad78236b7b267f, ; 755: Xamarin.AndroidX.Window => 287
	i64 u0xa0d8259f4cc284ec, ; 756: lib_System.Security.Cryptography.dll.so => 127
	i64 u0xa0e17ca50c77a225, ; 757: lib_Xamarin.Google.Crypto.Tink.Android.dll.so => 291
	i64 u0xa0ff9b3e34d92f11, ; 758: lib_System.Resources.Writer.dll.so => 101
	i64 u0xa12fbfb4da97d9f3, ; 759: System.Threading.Timer.dll => 148
	i64 u0xa135be3d6497d3d3, ; 760: Supabase.Core.dll => 207
	i64 u0xa1440773ee9d341e, ; 761: Xamarin.Google.Android.Material => 289
	i64 u0xa1aaf74891d6c40d, ; 762: zh-Hans/Microsoft.TestPlatform.CrossPlatEngine.resources.dll => 399
	i64 u0xa1b9d7c27f47219f, ; 763: Xamarin.AndroidX.Navigation.UI.dll => 270
	i64 u0xa2248bd0696b6838, ; 764: de/Microsoft.VisualStudio.TestPlatform.ObjectModel.resources => 342
	i64 u0xa2572680829d2c7c, ; 765: System.IO.Pipelines.dll => 54
	i64 u0xa26597e57ee9c7f6, ; 766: System.Xml.XmlDocument.dll => 162
	i64 u0xa28eed67cc59de26, ; 767: de/Microsoft.TestPlatform.CoreUtilities.resources => 341
	i64 u0xa308401900e5bed3, ; 768: lib_mscorlib.dll.so => 167
	i64 u0xa395572e7da6c99d, ; 769: lib_System.Security.dll.so => 131
	i64 u0xa3e683f24b43af6f, ; 770: System.Dynamic.Runtime.dll => 37
	i64 u0xa4145becdee3dc4f, ; 771: Xamarin.AndroidX.VectorDrawable.Animated => 283
	i64 u0xa46aa1eaa214539b, ; 772: ko/Microsoft.Maui.Controls.resources => 321
	i64 u0xa4edc8f2ceae241a, ; 773: System.Data.Common.dll => 22
	i64 u0xa5494f40f128ce6a, ; 774: System.Runtime.Serialization.Formatters.dll => 112
	i64 u0xa54b74df83dce92b, ; 775: System.Reflection.DispatchProxy => 90
	i64 u0xa5b7152421ed6d98, ; 776: lib_System.IO.FileSystem.Watcher.dll.so => 50
	i64 u0xa5c3844f17b822db, ; 777: lib_System.Linq.Parallel.dll.so => 60
	i64 u0xa5ce5c755bde8cb8, ; 778: lib_System.Security.Cryptography.Csp.dll.so => 122
	i64 u0xa5e599d1e0524750, ; 779: System.Numerics.Vectors.dll => 83
	i64 u0xa5ec622b0ec0b1de, ; 780: lib-zh-Hant-Microsoft.TestPlatform.CrossPlatEngine.resources.dll.so => 402
	i64 u0xa5f1ba49b85dd355, ; 781: System.Security.Cryptography.dll => 127
	i64 u0xa61975a5a37873ea, ; 782: lib_System.Xml.XmlSerializer.dll.so => 163
	i64 u0xa6593e21584384d2, ; 783: lib_Jsr305Binding.dll.so => 290
	i64 u0xa66cbee0130865f7, ; 784: lib_WindowsBase.dll.so => 166
	i64 u0xa67dbee13e1df9ca, ; 785: Xamarin.AndroidX.SavedState.dll => 274
	i64 u0xa684b098dd27b296, ; 786: lib_Xamarin.AndroidX.Security.SecurityCrypto.dll.so => 276
	i64 u0xa68a420042bb9b1f, ; 787: Xamarin.AndroidX.DrawerLayout.dll => 244
	i64 u0xa699ca46a71edab7, ; 788: lib-fr-Microsoft.TestPlatform.CrossPlatEngine.resources.dll.so => 375
	i64 u0xa6b8372afbf97bc4, ; 789: zh-Hant/Microsoft.TestPlatform.CommunicationUtilities.resources.dll => 401
	i64 u0xa6d26156d1cacc7c, ; 790: Xamarin.Android.Glide.dll => 216
	i64 u0xa75386b5cb9595aa, ; 791: Xamarin.AndroidX.Lifecycle.Runtime.Android => 259
	i64 u0xa763fbb98df8d9fb, ; 792: lib_Microsoft.Win32.Primitives.dll.so => 4
	i64 u0xa77e5c2a0a89bf52, ; 793: pt-BR/Microsoft.TestPlatform.CommunicationUtilities.resources => 389
	i64 u0xa78ce3745383236a, ; 794: Xamarin.AndroidX.Lifecycle.Common.Jvm => 253
	i64 u0xa7c31b56b4dc7b33, ; 795: hu/Microsoft.Maui.Controls.resources => 317
	i64 u0xa7eab29ed44b4e7a, ; 796: Mono.Android.Export => 170
	i64 u0xa8195217cbf017b7, ; 797: Microsoft.VisualBasic.Core => 2
	i64 u0xa859a95830f367ff, ; 798: lib_Xamarin.AndroidX.Lifecycle.ViewModel.Ktx.dll.so => 264
	i64 u0xa86b1fc409a87385, ; 799: ko/Microsoft.TestPlatform.CommunicationUtilities.resources => 383
	i64 u0xa8b52f21e0dbe690, ; 800: System.Runtime.Serialization.dll => 116
	i64 u0xa8c84ce526c2b4bd, ; 801: Microsoft.VisualStudio.DesignTools.XamlTapContract.dll => 407
	i64 u0xa8e6320dd07580ef, ; 802: lib_Microsoft.IdentityModel.JsonWebTokens.dll.so => 186
	i64 u0xa8ee4ed7de2efaee, ; 803: Xamarin.AndroidX.Annotation.dll => 222
	i64 u0xa95590e7c57438a4, ; 804: System.Configuration => 19
	i64 u0xaa1fca6b5e760881, ; 805: cs/Microsoft.VisualStudio.TestPlatform.Common.resources => 367
	i64 u0xaa2219c8e3449ff5, ; 806: Microsoft.Extensions.Logging.Abstractions => 181
	i64 u0xaa443ac34067eeef, ; 807: System.Private.Xml.dll => 89
	i64 u0xaa52de307ef5d1dd, ; 808: System.Net.Http => 65
	i64 u0xaa9a7b0214a5cc5c, ; 809: System.Diagnostics.StackTrace.dll => 30
	i64 u0xaaaf86367285a918, ; 810: Microsoft.Extensions.DependencyInjection.Abstractions.dll => 179
	i64 u0xaaf84bb3f052a265, ; 811: el/Microsoft.Maui.Controls.resources => 310
	i64 u0xab9af77b5b67a0b8, ; 812: Xamarin.AndroidX.ConstraintLayout.Core => 236
	i64 u0xab9c1b2687d86b0b, ; 813: lib_System.Linq.Expressions.dll.so => 59
	i64 u0xac2af3fa195a15ce, ; 814: System.Runtime.Numerics => 111
	i64 u0xac5376a2a538dc10, ; 815: Xamarin.AndroidX.Lifecycle.LiveData.Core.dll => 255
	i64 u0xac5acae88f60357e, ; 816: System.Diagnostics.Tools.dll => 32
	i64 u0xac79c7e46047ad98, ; 817: System.Security.Principal.Windows.dll => 128
	i64 u0xac847bccdcbca201, ; 818: pt-BR/Microsoft.VisualStudio.TestPlatform.Common.resources => 391
	i64 u0xac98d31068e24591, ; 819: System.Xml.XDocument => 159
	i64 u0xacbf9049299e586e, ; 820: fr/Microsoft.TestPlatform.CoreUtilities.resources.dll => 345
	i64 u0xacd46e002c3ccb97, ; 821: ro/Microsoft.Maui.Controls.resources => 328
	i64 u0xacdd9e4180d56dda, ; 822: Xamarin.AndroidX.Concurrent.Futures => 234
	i64 u0xacf42eea7ef9cd12, ; 823: System.Threading.Channels => 140
	i64 u0xad89c07347f1bad6, ; 824: nl/Microsoft.Maui.Controls.resources.dll => 324
	i64 u0xadbb53caf78a79d2, ; 825: System.Web.HttpUtility => 153
	i64 u0xadc90ab061a9e6e4, ; 826: System.ComponentModel.TypeConverter.dll => 17
	i64 u0xadca1b9030b9317e, ; 827: Xamarin.AndroidX.Collection.Ktx => 233
	i64 u0xadd8eda2edf396ad, ; 828: Xamarin.Android.Glide.GifDecoder => 219
	i64 u0xadf4cf30debbeb9a, ; 829: System.Net.ServicePoint.dll => 75
	i64 u0xadf511667bef3595, ; 830: System.Net.Security => 74
	i64 u0xae0aaa94fdcfce0f, ; 831: System.ComponentModel.EventBasedAsync.dll => 15
	i64 u0xae282bcd03739de7, ; 832: Java.Interop => 169
	i64 u0xae53579c90db1107, ; 833: System.ObjectModel.dll => 85
	i64 u0xaec7c0c7e2ed4575, ; 834: lib_Xamarin.KotlinX.AtomicFU.Jvm.dll.so => 299
	i64 u0xaf732d0b2193b8f5, ; 835: System.Security.Cryptography.OpenSsl.dll => 124
	i64 u0xafdb94dbccd9d11c, ; 836: Xamarin.AndroidX.Lifecycle.LiveData.dll => 254
	i64 u0xafe29f45095518e7, ; 837: lib_Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll.so => 265
	i64 u0xb03ae931fb25607e, ; 838: Xamarin.AndroidX.ConstraintLayout => 235
	i64 u0xb05b6f0a6cc8ddbb, ; 839: lib_Microsoft.IO.RecyclableMemoryStream.dll.so => 189
	i64 u0xb05cc42cd94c6d9d, ; 840: lib-sv-Microsoft.Maui.Controls.resources.dll.so => 331
	i64 u0xb0ac21bec8f428c5, ; 841: Xamarin.AndroidX.Lifecycle.Runtime.Ktx.Android.dll => 261
	i64 u0xb0bb43dc52ea59f9, ; 842: System.Diagnostics.Tracing.dll => 34
	i64 u0xb0de1244f89766a0, ; 843: de/Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll => 342
	i64 u0xb1ab91f55445d469, ; 844: tr/Microsoft.VisualStudio.TestPlatform.Common.resources.dll => 397
	i64 u0xb1dd05401aa8ee63, ; 845: System.Security.AccessControl => 118
	i64 u0xb220631954820169, ; 846: System.Text.RegularExpressions => 139
	i64 u0xb2376e1dbf8b4ed7, ; 847: System.Security.Cryptography.Csp => 122
	i64 u0xb2a1959fe95c5402, ; 848: lib_System.Runtime.InteropServices.JavaScript.dll.so => 106
	i64 u0xb2a3f67f3bf29fce, ; 849: da/Microsoft.Maui.Controls.resources => 308
	i64 u0xb3011a0a57f7ffb2, ; 850: Microsoft.VisualStudio.DesignTools.MobileTapContracts.dll => 405
	i64 u0xb3563d03ddeabb41, ; 851: testhost => 202
	i64 u0xb3874072ee0ecf8c, ; 852: Xamarin.AndroidX.VectorDrawable.Animated.dll => 283
	i64 u0xb3f0a0fcda8d3ebc, ; 853: Xamarin.AndroidX.CardView => 230
	i64 u0xb46be1aa6d4fff93, ; 854: hi/Microsoft.Maui.Controls.resources => 315
	i64 u0xb477491be13109d8, ; 855: ar/Microsoft.Maui.Controls.resources => 305
	i64 u0xb4bd7015ecee9d86, ; 856: System.IO.Pipelines => 54
	i64 u0xb4c53d9749c5f226, ; 857: lib_System.IO.FileSystem.AccessControl.dll.so => 47
	i64 u0xb4ff710863453fda, ; 858: System.Diagnostics.FileVersionInfo.dll => 28
	i64 u0xb572cefcb3e84372, ; 859: fr/Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll => 346
	i64 u0xb5c38bf497a4cfe2, ; 860: lib_System.Threading.Tasks.dll.so => 145
	i64 u0xb5c7fcdafbc67ee4, ; 861: Microsoft.Extensions.Logging.Abstractions.dll => 181
	i64 u0xb5ea31d5244c6626, ; 862: System.Threading.ThreadPool.dll => 147
	i64 u0xb63a2b2f81d293b5, ; 863: lib-zh-Hans-Microsoft.VisualStudio.TestPlatform.Common.resources.dll.so => 400
	i64 u0xb70bba3e8ad424c4, ; 864: zh-Hant/Microsoft.TestPlatform.CoreUtilities.resources => 363
	i64 u0xb7212c4683a94afe, ; 865: System.Drawing.Primitives => 35
	i64 u0xb7413fb4ec17539f, ; 866: Microsoft.VisualStudio.CodeCoverage.Shim.dll => 175
	i64 u0xb7b7753d1f319409, ; 867: sv/Microsoft.Maui.Controls.resources => 331
	i64 u0xb81a2c6e0aee50fe, ; 868: lib_System.Private.CoreLib.dll.so => 173
	i64 u0xb8ab7a8b6b548eda, ; 869: lib-ja-Microsoft.TestPlatform.CommunicationUtilities.resources.dll.so => 380
	i64 u0xb8b0a9b3dfbc5cb7, ; 870: Xamarin.AndroidX.Window.Extensions.Core.Core => 288
	i64 u0xb8c60af47c08d4da, ; 871: System.Net.ServicePoint => 75
	i64 u0xb8e68d20aad91196, ; 872: lib_System.Xml.XPath.dll.so => 161
	i64 u0xb9185c33a1643eed, ; 873: Microsoft.CSharp.dll => 1
	i64 u0xb92de0f64ec81253, ; 874: lib-it-Microsoft.TestPlatform.CrossPlatEngine.resources.dll.so => 378
	i64 u0xb9b8001adf4ed7cc, ; 875: lib_Xamarin.AndroidX.SlidingPaneLayout.dll.so => 277
	i64 u0xb9f64d3b230def68, ; 876: lib-pt-Microsoft.Maui.Controls.resources.dll.so => 327
	i64 u0xb9fc3c8a556e3691, ; 877: ja/Microsoft.Maui.Controls.resources => 320
	i64 u0xba4670aa94a2b3c6, ; 878: lib_System.Xml.XDocument.dll.so => 159
	i64 u0xba48785529705af9, ; 879: System.Collections.dll => 12
	i64 u0xba965b8c86359996, ; 880: lib_System.Windows.dll.so => 155
	i64 u0xbac10ca6c95505bc, ; 881: lib_Microsoft.TestPlatform.Utilities.dll.so => 200
	i64 u0xbb286883bc35db36, ; 882: System.Transactions.dll => 151
	i64 u0xbb54fd4c9d1101e1, ; 883: lib_Supabase.dll.so => 206
	i64 u0xbb65706fde942ce3, ; 884: System.Net.Sockets => 76
	i64 u0xbb8580155e3d6496, ; 885: lib-zh-Hant-Microsoft.TestPlatform.CoreUtilities.resources.dll.so => 363
	i64 u0xbba28979413cad9e, ; 886: lib_System.Runtime.CompilerServices.VisualC.dll.so => 103
	i64 u0xbbd180354b67271a, ; 887: System.Runtime.Serialization.Formatters => 112
	i64 u0xbc260cdba33291a3, ; 888: Xamarin.AndroidX.Arch.Core.Common.dll => 227
	i64 u0xbd0e2c0d55246576, ; 889: System.Net.Http.dll => 65
	i64 u0xbd3fbd85b9e1cb29, ; 890: lib_System.Net.HttpListener.dll.so => 66
	i64 u0xbd437a2cdb333d0d, ; 891: Xamarin.AndroidX.ViewPager2 => 286
	i64 u0xbd4f572d2bd0a789, ; 892: System.IO.Compression.ZipFile.dll => 45
	i64 u0xbd5d0b88d3d647a5, ; 893: lib_Xamarin.AndroidX.Browser.dll.so => 229
	i64 u0xbd877b14d0b56392, ; 894: System.Runtime.Intrinsics.dll => 109
	i64 u0xbe65a49036345cf4, ; 895: lib_System.Buffers.dll.so => 7
	i64 u0xbee38d4a88835966, ; 896: Xamarin.AndroidX.AppCompat.AppCompatResources => 226
	i64 u0xbef9919db45b4ca7, ; 897: System.IO.Pipes.AccessControl => 55
	i64 u0xbf02c92392c99ce0, ; 898: Websocket.Client => 215
	i64 u0xbf0fa68611139208, ; 899: lib_Xamarin.AndroidX.Annotation.dll.so => 222
	i64 u0xbf325e4d27c80a9d, ; 900: Microsoft.VisualStudio.CodeCoverage.Shim => 175
	i64 u0xbf93b6320dc8be02, ; 901: lib-tr-Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll.so => 360
	i64 u0xbfc1e1fb3095f2b3, ; 902: lib_System.Net.Http.Json.dll.so => 64
	i64 u0xc040a4ab55817f58, ; 903: ar/Microsoft.Maui.Controls.resources.dll => 305
	i64 u0xc07cadab29efeba0, ; 904: Xamarin.AndroidX.Core.Core.Ktx.dll => 239
	i64 u0xc082a4d3f341d134, ; 905: ja/Microsoft.VisualStudio.TestPlatform.Common.resources => 382
	i64 u0xc0d928351ab5ca77, ; 906: System.Console.dll => 20
	i64 u0xc0f5a221a9383aea, ; 907: System.Runtime.Intrinsics => 109
	i64 u0xc111030af54d7191, ; 908: System.Resources.Writer => 101
	i64 u0xc12b8b3afa48329c, ; 909: lib_System.Linq.dll.so => 62
	i64 u0xc183ca0b74453aa9, ; 910: lib_System.Threading.Tasks.Dataflow.dll.so => 142
	i64 u0xc1ff9ae3cdb6e1e6, ; 911: Xamarin.AndroidX.Activity.dll => 220
	i64 u0xc26c064effb1dea9, ; 912: System.Buffers.dll => 7
	i64 u0xc278de356ad8a9e3, ; 913: Microsoft.IdentityModel.Logging => 187
	i64 u0xc28c50f32f81cc73, ; 914: ja/Microsoft.Maui.Controls.resources.dll => 320
	i64 u0xc2902f6cf5452577, ; 915: lib_Mono.Android.Export.dll.so => 170
	i64 u0xc2a3bca55b573141, ; 916: System.IO.FileSystem.Watcher => 50
	i64 u0xc2bcfec99f69365e, ; 917: Xamarin.AndroidX.ViewPager2.dll => 286
	i64 u0xc2e7aa29be607b13, ; 918: zh-Hans/Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll => 362
	i64 u0xc30b52815b58ac2c, ; 919: lib_System.Runtime.Serialization.Xml.dll.so => 115
	i64 u0xc35c67c8d507a2f0, ; 920: lib-de-Microsoft.TestPlatform.CommunicationUtilities.resources.dll.so => 368
	i64 u0xc36d7d89c652f455, ; 921: System.Threading.Overlapped => 141
	i64 u0xc396b285e59e5493, ; 922: GoogleGson.dll => 174
	i64 u0xc3c86c1e5e12f03d, ; 923: WindowsBase => 166
	i64 u0xc421b61fd853169d, ; 924: lib_System.Net.WebSockets.Client.dll.so => 80
	i64 u0xc463e077917aa21d, ; 925: System.Runtime.Serialization.Json => 113
	i64 u0xc4d3858ed4d08512, ; 926: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll => 265
	i64 u0xc50fded0ded1418c, ; 927: lib_System.ComponentModel.TypeConverter.dll.so => 17
	i64 u0xc519125d6bc8fb11, ; 928: lib_System.Net.Requests.dll.so => 73
	i64 u0xc5293b19e4dc230e, ; 929: Xamarin.AndroidX.Navigation.Fragment => 268
	i64 u0xc5325b2fcb37446f, ; 930: lib_System.Private.Xml.dll.so => 89
	i64 u0xc535cb9a21385d9b, ; 931: lib_Xamarin.Android.Glide.DiskLruCache.dll.so => 218
	i64 u0xc55c1b9734e13a76, ; 932: de/Microsoft.TestPlatform.CommunicationUtilities.resources => 368
	i64 u0xc5a0f4b95a699af7, ; 933: lib_System.Private.Uri.dll.so => 87
	i64 u0xc5cdcd5b6277579e, ; 934: lib_System.Security.Cryptography.Algorithms.dll.so => 120
	i64 u0xc5ec286825cb0bf4, ; 935: Xamarin.AndroidX.Tracing.Tracing => 280
	i64 u0xc60b2e63e1e91eff, ; 936: lib-es-Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll.so => 344
	i64 u0xc6706bc8aa7fe265, ; 937: Xamarin.AndroidX.Annotation.Jvm => 224
	i64 u0xc785d7f5c72d7b7f, ; 938: zh-Hant/Microsoft.VisualStudio.TestPlatform.Common.resources.dll => 403
	i64 u0xc7c01e7d7c93a110, ; 939: System.Text.Encoding.Extensions.dll => 135
	i64 u0xc7ce851898a4548e, ; 940: lib_System.Web.HttpUtility.dll.so => 153
	i64 u0xc809d4089d2556b2, ; 941: System.Runtime.InteropServices.JavaScript.dll => 106
	i64 u0xc813610a898af9cb, ; 942: es/Microsoft.TestPlatform.CrossPlatEngine.resources.dll => 372
	i64 u0xc858a28d9ee5a6c5, ; 943: lib_System.Collections.Specialized.dll.so => 11
	i64 u0xc8ac7c6bf1c2ec51, ; 944: System.Reflection.DispatchProxy.dll => 90
	i64 u0xc8c165b08dc3d91b, ; 945: lib-ko-Microsoft.TestPlatform.CommunicationUtilities.resources.dll.so => 383
	i64 u0xc9c62c8f354ac568, ; 946: lib_System.Diagnostics.TextWriterTraceListener.dll.so => 31
	i64 u0xc9f2dfe60d86f459, ; 947: ru/Microsoft.TestPlatform.CommunicationUtilities.resources.dll => 392
	i64 u0xca3a723e7342c5b6, ; 948: lib-tr-Microsoft.Maui.Controls.resources.dll.so => 333
	i64 u0xca5801070d9fccfb, ; 949: System.Text.Encoding => 136
	i64 u0xca77b08f507e3732, ; 950: ko/Microsoft.TestPlatform.CommunicationUtilities.resources.dll => 383
	i64 u0xcab3493c70141c2d, ; 951: pl/Microsoft.Maui.Controls.resources => 325
	i64 u0xcab69b9a31439815, ; 952: lib_Xamarin.Google.ErrorProne.TypeAnnotations.dll.so => 293
	i64 u0xcacfddc9f7c6de76, ; 953: ro/Microsoft.Maui.Controls.resources.dll => 328
	i64 u0xcadbc92899a777f0, ; 954: Xamarin.AndroidX.Startup.StartupRuntime => 278
	i64 u0xcb0f1b5fb16582cf, ; 955: de/Microsoft.VisualStudio.TestPlatform.Common.resources => 370
	i64 u0xcb76efab0f56f81a, ; 956: System.Reactive => 214
	i64 u0xcba1cb79f45292b5, ; 957: Xamarin.Android.Glide.GifDecoder.dll => 219
	i64 u0xcbb5f80c7293e696, ; 958: lib_System.Globalization.Calendars.dll.so => 40
	i64 u0xcbd4fdd9cef4a294, ; 959: lib__Microsoft.Android.Resource.Designer.dll.so => 408
	i64 u0xcc15da1e07bbd994, ; 960: Xamarin.AndroidX.SlidingPaneLayout => 277
	i64 u0xcc2876b32ef2794c, ; 961: lib_System.Text.RegularExpressions.dll.so => 139
	i64 u0xcc5c3bb714c4561e, ; 962: Xamarin.KotlinX.Coroutines.Core.Jvm.dll => 302
	i64 u0xcc76886e09b88260, ; 963: Xamarin.KotlinX.Serialization.Core.Jvm.dll => 304
	i64 u0xcc9b8dd7378cf190, ; 964: de/Microsoft.TestPlatform.CrossPlatEngine.resources => 369
	i64 u0xcc9fa2923aa1c9ef, ; 965: System.Diagnostics.Contracts.dll => 25
	i64 u0xccf25c4b634ccd3a, ; 966: zh-Hans/Microsoft.Maui.Controls.resources.dll => 337
	i64 u0xcd10a42808629144, ; 967: System.Net.Requests => 73
	i64 u0xcdca1b920e9f53ba, ; 968: Xamarin.AndroidX.Interpolator => 251
	i64 u0xcdd0c48b6937b21c, ; 969: Xamarin.AndroidX.SwipeRefreshLayout => 279
	i64 u0xcde1fa22dc303670, ; 970: Microsoft.VisualStudio.DesignTools.XamlTapContract => 407
	i64 u0xcf23d8093f3ceadf, ; 971: System.Diagnostics.DiagnosticSource.dll => 27
	i64 u0xcf5ff6b6b2c4c382, ; 972: System.Net.Mail.dll => 67
	i64 u0xcf8fc898f98b0d34, ; 973: System.Private.Xml.Linq => 88
	i64 u0xcff4ffceeeffeaf9, ; 974: Microsoft.TestPlatform.Utilities.dll => 200
	i64 u0xd04b5f59ed596e31, ; 975: System.Reflection.Metadata.dll => 95
	i64 u0xd063299fcfc0c93f, ; 976: lib_System.Runtime.Serialization.Json.dll.so => 113
	i64 u0xd0de8a113e976700, ; 977: System.Diagnostics.TextWriterTraceListener => 31
	i64 u0xd0fc33d5ae5d4cb8, ; 978: System.Runtime.Extensions => 104
	i64 u0xd1194e1d8a8de83c, ; 979: lib_Xamarin.AndroidX.Lifecycle.Common.Jvm.dll.so => 253
	i64 u0xd119c7d204f86f36, ; 980: zh-Hans/Microsoft.TestPlatform.CommunicationUtilities.resources.dll => 398
	i64 u0xd12beacdfc14f696, ; 981: System.Dynamic.Runtime => 37
	i64 u0xd15c17807efbb9b8, ; 982: lib-zh-Hans-Microsoft.TestPlatform.CoreUtilities.resources.dll.so => 361
	i64 u0xd198e7ce1b6a8344, ; 983: System.Net.Quic.dll => 72
	i64 u0xd1a1e3d55a9ee6be, ; 984: lib-ja-Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll.so => 350
	i64 u0xd228645233db0371, ; 985: ko/Microsoft.TestPlatform.CrossPlatEngine.resources => 384
	i64 u0xd2ddf301d2cf0a11, ; 986: lib-zh-Hant-Microsoft.TestPlatform.CommunicationUtilities.resources.dll.so => 401
	i64 u0xd2f519e947a963c6, ; 987: lib-zh-Hans-Microsoft.TestPlatform.CrossPlatEngine.resources.dll.so => 399
	i64 u0xd3144156a3727ebe, ; 988: Xamarin.Google.Guava.ListenableFuture => 294
	i64 u0xd333d0af9e423810, ; 989: System.Runtime.InteropServices => 108
	i64 u0xd33a415cb4278969, ; 990: System.Security.Cryptography.Encoding.dll => 123
	i64 u0xd3426d966bb704f5, ; 991: Xamarin.AndroidX.AppCompat.AppCompatResources.dll => 226
	i64 u0xd3651b6fc3125825, ; 992: System.Private.Uri.dll => 87
	i64 u0xd373685349b1fe8b, ; 993: Microsoft.Extensions.Logging.dll => 180
	i64 u0xd3801faafafb7698, ; 994: System.Private.DataContractSerialization.dll => 86
	i64 u0xd38f5324f2de5a01, ; 995: pl/Microsoft.TestPlatform.CoreUtilities.resources.dll => 353
	i64 u0xd3e4c8d6a2d5d470, ; 996: it/Microsoft.Maui.Controls.resources => 319
	i64 u0xd3edcc1f25459a50, ; 997: System.Reflection.Emit => 93
	i64 u0xd4645626dffec99d, ; 998: lib_Microsoft.Extensions.DependencyInjection.Abstractions.dll.so => 179
	i64 u0xd4fa0abb79079ea9, ; 999: System.Security.Principal.dll => 129
	i64 u0xd5507e11a2b2839f, ; 1000: Xamarin.AndroidX.Lifecycle.ViewModelSavedState => 265
	i64 u0xd5b30b8a969ae327, ; 1001: lib-pt-BR-Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll.so => 356
	i64 u0xd5c99f214e25e314, ; 1002: tr/Microsoft.TestPlatform.CommunicationUtilities.resources.dll => 395
	i64 u0xd5d04bef8478ea19, ; 1003: Xamarin.AndroidX.Tracing.Tracing.dll => 280
	i64 u0xd60815f26a12e140, ; 1004: Microsoft.Extensions.Logging.Debug.dll => 182
	i64 u0xd64327d7e48dc586, ; 1005: Microsoft.TestPlatform.PlatformAbstractions => 196
	i64 u0xd65786d27a4ad960, ; 1006: lib_Microsoft.Maui.Controls.HotReload.Forms.dll.so => 404
	i64 u0xd6694f8359737e4e, ; 1007: Xamarin.AndroidX.SavedState => 274
	i64 u0xd690cd9b9d6087ec, ; 1008: zh-Hans/Microsoft.VisualStudio.TestPlatform.ObjectModel.resources => 362
	i64 u0xd6949e129339eae5, ; 1009: lib_Xamarin.AndroidX.Core.Core.Ktx.dll.so => 239
	i64 u0xd6d21782156bc35b, ; 1010: Xamarin.AndroidX.SwipeRefreshLayout.dll => 279
	i64 u0xd6de019f6af72435, ; 1011: Xamarin.AndroidX.ConstraintLayout.Core.dll => 236
	i64 u0xd6f697a581fc6fe3, ; 1012: Xamarin.Google.ErrorProne.TypeAnnotations.dll => 293
	i64 u0xd70956d1e6deefb9, ; 1013: Jsr305Binding => 290
	i64 u0xd72329819cbbbc44, ; 1014: lib_Microsoft.Extensions.Configuration.Abstractions.dll.so => 177
	i64 u0xd72c760af136e863, ; 1015: System.Xml.XmlSerializer.dll => 163
	i64 u0xd753f071e44c2a03, ; 1016: lib_System.Security.SecureString.dll.so => 130
	i64 u0xd773c8875eb1d58b, ; 1017: pt-BR/Microsoft.VisualStudio.TestPlatform.Common.resources.dll => 391
	i64 u0xd7b3764ada9d341d, ; 1018: lib_Microsoft.Extensions.Logging.Abstractions.dll.so => 181
	i64 u0xd7f0088bc5ad71f2, ; 1019: Xamarin.AndroidX.VersionedParcelable => 284
	i64 u0xd8e15bf8a86e8383, ; 1020: lib-ru-Microsoft.TestPlatform.CommunicationUtilities.resources.dll.so => 392
	i64 u0xd8fb25e28ae30a12, ; 1021: Xamarin.AndroidX.ProfileInstaller.ProfileInstaller.dll => 271
	i64 u0xd92203629c41d757, ; 1022: es/Microsoft.TestPlatform.CoreUtilities.resources.dll => 343
	i64 u0xda1dfa4c534a9251, ; 1023: Microsoft.Extensions.DependencyInjection => 178
	i64 u0xdad05a11827959a3, ; 1024: System.Collections.NonGeneric.dll => 10
	i64 u0xdaefdfe71aa53cf9, ; 1025: System.IO.FileSystem.Primitives => 49
	i64 u0xdb35ce39e5aefa9d, ; 1026: Microsoft.TestPlatform.Utilities => 200
	i64 u0xdb5383ab5865c007, ; 1027: lib-vi-Microsoft.Maui.Controls.resources.dll.so => 335
	i64 u0xdb58816721c02a59, ; 1028: lib_System.Reflection.Emit.ILGeneration.dll.so => 91
	i64 u0xdbeda89f832aa805, ; 1029: vi/Microsoft.Maui.Controls.resources.dll => 335
	i64 u0xdbf2a779fbc3ac31, ; 1030: System.Transactions.Local.dll => 150
	i64 u0xdbf9607a441b4505, ; 1031: System.Linq => 62
	i64 u0xdbfc90157a0de9b0, ; 1032: lib_System.Text.Encoding.dll.so => 136
	i64 u0xdc75032002d1a212, ; 1033: lib_System.Transactions.Local.dll.so => 150
	i64 u0xdca8be7403f92d4f, ; 1034: lib_System.Linq.Queryable.dll.so => 61
	i64 u0xdce2c53525640bf3, ; 1035: Microsoft.Extensions.Logging => 180
	i64 u0xdceda8d644ac18a6, ; 1036: Supabase.dll => 206
	i64 u0xdd2b722d78ef5f43, ; 1037: System.Runtime.dll => 117
	i64 u0xdd67031857c72f96, ; 1038: lib_System.Text.Encodings.Web.dll.so => 137
	i64 u0xdd70765ad6162057, ; 1039: Xamarin.JSpecify => 296
	i64 u0xdd85ae658f78a064, ; 1040: it/Microsoft.TestPlatform.CommunicationUtilities.resources => 377
	i64 u0xdd92e229ad292030, ; 1041: System.Numerics.dll => 84
	i64 u0xdde30e6b77aa6f6c, ; 1042: lib-zh-Hans-Microsoft.Maui.Controls.resources.dll.so => 337
	i64 u0xde110ae80fa7c2e2, ; 1043: System.Xml.XDocument.dll => 159
	i64 u0xde1223bb049720d1, ; 1044: Supabase.Realtime => 211
	i64 u0xde4726fcdf63a198, ; 1045: Xamarin.AndroidX.Transition => 281
	i64 u0xde572c2b2fb32f93, ; 1046: lib_System.Threading.Tasks.Extensions.dll.so => 143
	i64 u0xde74babd7e98d55f, ; 1047: zh-Hant/Microsoft.TestPlatform.CoreUtilities.resources.dll => 363
	i64 u0xde8769ebda7d8647, ; 1048: hr/Microsoft.Maui.Controls.resources.dll => 316
	i64 u0xdee075f3477ef6be, ; 1049: Xamarin.AndroidX.ExifInterface.dll => 248
	i64 u0xdf0a60bf291db423, ; 1050: testhost.dll => 202
	i64 u0xdf25d9f36d8dc576, ; 1051: lib_Supabase.Functions.dll.so => 208
	i64 u0xdf4b773de8fb1540, ; 1052: System.Net.dll => 82
	i64 u0xdfa254ebb4346068, ; 1053: System.Net.Ping => 70
	i64 u0xe0142572c095a480, ; 1054: Xamarin.AndroidX.AppCompat.dll => 225
	i64 u0xe021eaa401792a05, ; 1055: System.Text.Encoding.dll => 136
	i64 u0xe02f89350ec78051, ; 1056: Xamarin.AndroidX.CoordinatorLayout.dll => 237
	i64 u0xe0496b9d65ef5474, ; 1057: Xamarin.Android.Glide.DiskLruCache.dll => 218
	i64 u0xe10b760bb1462e7a, ; 1058: lib_System.Security.Cryptography.Primitives.dll.so => 125
	i64 u0xe1566bbdb759c5af, ; 1059: Microsoft.Maui.Controls.HotReload.Forms.dll => 404
	i64 u0xe170fb003815111b, ; 1060: ru/Microsoft.TestPlatform.CoreUtilities.resources.dll => 357
	i64 u0xe192a588d4410686, ; 1061: lib_System.IO.Pipelines.dll.so => 54
	i64 u0xe1a08bd3fa539e0d, ; 1062: System.Runtime.Loader => 110
	i64 u0xe1a77eb8831f7741, ; 1063: System.Security.SecureString.dll => 130
	i64 u0xe1b52f9f816c70ef, ; 1064: System.Private.Xml.Linq.dll => 88
	i64 u0xe1e199c8ab02e356, ; 1065: System.Data.DataSetExtensions.dll => 23
	i64 u0xe1ecfdb7fff86067, ; 1066: System.Net.Security.dll => 74
	i64 u0xe2252a80fe853de4, ; 1067: lib_System.Security.Principal.dll.so => 129
	i64 u0xe22fa4c9c645db62, ; 1068: System.Diagnostics.TextWriterTraceListener.dll => 31
	i64 u0xe2420585aeceb728, ; 1069: System.Net.Requests.dll => 73
	i64 u0xe26692647e6bcb62, ; 1070: Xamarin.AndroidX.Lifecycle.Runtime.Ktx => 260
	i64 u0xe29b73bc11392966, ; 1071: lib-id-Microsoft.Maui.Controls.resources.dll.so => 318
	i64 u0xe2ad448dee50fbdf, ; 1072: System.Xml.Serialization => 158
	i64 u0xe2d920f978f5d85c, ; 1073: System.Data.DataSetExtensions => 23
	i64 u0xe2e426c7714fa0bc, ; 1074: Microsoft.Win32.Primitives.dll => 4
	i64 u0xe332bacb3eb4a806, ; 1075: Mono.Android.Export.dll => 170
	i64 u0xe3811d68d4fe8463, ; 1076: pt-BR/Microsoft.Maui.Controls.resources.dll => 326
	i64 u0xe3b7cbae5ad66c75, ; 1077: lib_System.Security.Cryptography.Encoding.dll.so => 123
	i64 u0xe4292b48f3224d5b, ; 1078: lib_Xamarin.AndroidX.Core.ViewTree.dll.so => 240
	i64 u0xe430f3a9f851a459, ; 1079: it/Microsoft.VisualStudio.TestPlatform.Common.resources.dll => 379
	i64 u0xe46b6798669d6491, ; 1080: it/Microsoft.TestPlatform.CoreUtilities.resources.dll => 347
	i64 u0xe494f7ced4ecd10a, ; 1081: hu/Microsoft.Maui.Controls.resources.dll => 317
	i64 u0xe4a18371d349335a, ; 1082: fr/Microsoft.TestPlatform.CommunicationUtilities.resources => 374
	i64 u0xe4a9b1e40d1e8917, ; 1083: lib-fi-Microsoft.Maui.Controls.resources.dll.so => 312
	i64 u0xe4f74a0b5bf9703f, ; 1084: System.Runtime.Serialization.Primitives => 114
	i64 u0xe5434e8a119ceb69, ; 1085: lib_Mono.Android.dll.so => 172
	i64 u0xe5519b3281388d57, ; 1086: es/Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll => 344
	i64 u0xe55703b9ce5c038a, ; 1087: System.Diagnostics.Tools => 32
	i64 u0xe57013c8afc270b5, ; 1088: Microsoft.VisualBasic => 3
	i64 u0xe5823f93b6fd8f02, ; 1089: lib-ja-Microsoft.TestPlatform.CoreUtilities.resources.dll.so => 349
	i64 u0xe62913cc36bc07ec, ; 1090: System.Xml.dll => 164
	i64 u0xe7a2374f54fa7df6, ; 1091: Microsoft.TestPlatform.PlatformAbstractions.dll => 196
	i64 u0xe7bea09c4900a191, ; 1092: Xamarin.AndroidX.VectorDrawable.dll => 282
	i64 u0xe7e03cc18dcdeb49, ; 1093: lib_System.Diagnostics.StackTrace.dll.so => 30
	i64 u0xe7e147ff99a7a380, ; 1094: lib_System.Configuration.dll.so => 19
	i64 u0xe86b0df4ba9e5db8, ; 1095: lib_Xamarin.AndroidX.Lifecycle.Runtime.Android.dll.so => 259
	i64 u0xe896622fe0902957, ; 1096: System.Reflection.Emit.dll => 93
	i64 u0xe89a2a9ef110899b, ; 1097: System.Drawing.dll => 36
	i64 u0xe8c5f8c100b5934b, ; 1098: Microsoft.Win32.Registry => 5
	i64 u0xe8e267f1e76c5747, ; 1099: ko/Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll => 352
	i64 u0xe8f5584271148266, ; 1100: lib-it-Microsoft.VisualStudio.TestPlatform.Common.resources.dll.so => 379
	i64 u0xe957c3976986ab72, ; 1101: lib_Xamarin.AndroidX.Window.Extensions.Core.Core.dll.so => 288
	i64 u0xe98163eb702ae5c5, ; 1102: Xamarin.AndroidX.Arch.Core.Runtime => 228
	i64 u0xe994f23ba4c143e5, ; 1103: Xamarin.KotlinX.Coroutines.Android => 300
	i64 u0xe9b9c8c0458fd92a, ; 1104: System.Windows => 155
	i64 u0xe9d166d87a7f2bdb, ; 1105: lib_Xamarin.AndroidX.Startup.StartupRuntime.dll.so => 278
	i64 u0xe9fccb2b6d528bda, ; 1106: lib_Microsoft.TestPlatform.CommunicationUtilities.dll.so => 198
	i64 u0xea5a4efc2ad81d1b, ; 1107: Xamarin.Google.ErrorProne.Annotations => 292
	i64 u0xeb2313fe9d65b785, ; 1108: Xamarin.AndroidX.ConstraintLayout.dll => 235
	i64 u0xebbd07ee33f91b98, ; 1109: cs/Microsoft.TestPlatform.CoreUtilities.resources.dll => 339
	i64 u0xed19c616b3fcb7eb, ; 1110: Xamarin.AndroidX.VersionedParcelable.dll => 284
	i64 u0xed60c6fa891c051a, ; 1111: lib_Microsoft.VisualStudio.DesignTools.TapContract.dll.so => 406
	i64 u0xedc4817167106c23, ; 1112: System.Net.Sockets.dll => 76
	i64 u0xedc632067fb20ff3, ; 1113: System.Memory.dll => 63
	i64 u0xedc8e4ca71a02a8b, ; 1114: Xamarin.AndroidX.Navigation.Runtime.dll => 269
	i64 u0xee81f5b3f1c4f83b, ; 1115: System.Threading.ThreadPool => 147
	i64 u0xeeb7ebb80150501b, ; 1116: lib_Xamarin.AndroidX.Collection.Jvm.dll.so => 232
	i64 u0xeefc635595ef57f0, ; 1117: System.Security.Cryptography.Cng => 121
	i64 u0xef03b1b5a04e9709, ; 1118: System.Text.Encoding.CodePages.dll => 134
	i64 u0xef602c523fe2e87a, ; 1119: lib_Xamarin.Google.Guava.ListenableFuture.dll.so => 294
	i64 u0xef72742e1bcca27a, ; 1120: Microsoft.Maui.Essentials.dll => 193
	i64 u0xef953b033c672f29, ; 1121: lib-es-Microsoft.TestPlatform.CommunicationUtilities.resources.dll.so => 371
	i64 u0xefd1e0c4e5c9b371, ; 1122: System.Resources.ResourceManager.dll => 100
	i64 u0xefe8f8d5ed3c72ea, ; 1123: System.Formats.Tar.dll => 39
	i64 u0xefec0b7fdc57ec42, ; 1124: Xamarin.AndroidX.Activity => 220
	i64 u0xf00c29406ea45e19, ; 1125: es/Microsoft.Maui.Controls.resources.dll => 311
	i64 u0xf05d60ee9c883557, ; 1126: tr/Microsoft.TestPlatform.CoreUtilities.resources => 359
	i64 u0xf0919efa1f6dd08b, ; 1127: ko/Microsoft.VisualStudio.TestPlatform.Common.resources.dll => 385
	i64 u0xf09e47b6ae914f6e, ; 1128: System.Net.NameResolution => 68
	i64 u0xf0ac2b489fed2e35, ; 1129: lib_System.Diagnostics.Debug.dll.so => 26
	i64 u0xf0bb49dadd3a1fe1, ; 1130: lib_System.Net.ServicePoint.dll.so => 75
	i64 u0xf0de2537ee19c6ca, ; 1131: lib_System.Net.WebHeaderCollection.dll.so => 78
	i64 u0xf1138779fa181c68, ; 1132: lib_Xamarin.AndroidX.Lifecycle.Runtime.dll.so => 258
	i64 u0xf11b621fc87b983f, ; 1133: Microsoft.Maui.Controls.Xaml.dll => 191
	i64 u0xf161f4f3c3b7e62c, ; 1134: System.Data => 24
	i64 u0xf16eb650d5a464bc, ; 1135: System.ValueTuple => 152
	i64 u0xf16ee46c72c74cc3, ; 1136: lib-zh-Hans-Microsoft.TestPlatform.CommunicationUtilities.resources.dll.so => 398
	i64 u0xf180df49d46ae0c3, ; 1137: lib-pl-Microsoft.TestPlatform.CrossPlatEngine.resources.dll.so => 387
	i64 u0xf1a9bf28434ea734, ; 1138: lib_Microsoft.TestPlatform.PlatformAbstractions.dll.so => 196
	i64 u0xf1c4b4005493d871, ; 1139: System.Formats.Asn1.dll => 38
	i64 u0xf1f2700eb64586d8, ; 1140: lib-ja-Microsoft.VisualStudio.TestPlatform.Common.resources.dll.so => 382
	i64 u0xf2215f093c2e39da, ; 1141: zh-Hans/Microsoft.TestPlatform.CrossPlatEngine.resources => 399
	i64 u0xf238bd79489d3a96, ; 1142: lib-nl-Microsoft.Maui.Controls.resources.dll.so => 324
	i64 u0xf2dbb2bd4159cecf, ; 1143: cs/Microsoft.VisualStudio.TestPlatform.ObjectModel.resources => 340
	i64 u0xf2feea356ba760af, ; 1144: Xamarin.AndroidX.Arch.Core.Runtime.dll => 228
	i64 u0xf300e085f8acd238, ; 1145: lib_System.ServiceProcess.dll.so => 133
	i64 u0xf34e52b26e7e059d, ; 1146: System.Runtime.CompilerServices.VisualC.dll => 103
	i64 u0xf37221fda4ef8830, ; 1147: lib_Xamarin.Google.Android.Material.dll.so => 289
	i64 u0xf3ad9b8fb3eefd12, ; 1148: lib_System.IO.UnmanagedMemoryStream.dll.so => 57
	i64 u0xf3bceee8ddbe792c, ; 1149: lib-zh-Hans-Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll.so => 362
	i64 u0xf3ddfe05336abf29, ; 1150: System => 165
	i64 u0xf408654b2a135055, ; 1151: System.Reflection.Emit.ILGeneration.dll => 91
	i64 u0xf4103170a1de5bd0, ; 1152: System.Linq.Queryable.dll => 61
	i64 u0xf41899065cd39903, ; 1153: it/Microsoft.TestPlatform.CommunicationUtilities.resources.dll => 377
	i64 u0xf42d20c23173d77c, ; 1154: lib_System.ServiceModel.Web.dll.so => 132
	i64 u0xf437b94630896f3f, ; 1155: lib_Plugin.Maui.Audio.dll.so => 205
	i64 u0xf47e294cbe68c2b0, ; 1156: lib_Websocket.Client.dll.so => 215
	i64 u0xf4a6f2caea7d0759, ; 1157: lib-pt-BR-Microsoft.TestPlatform.CrossPlatEngine.resources.dll.so => 390
	i64 u0xf4c1dd70a5496a17, ; 1158: System.IO.Compression => 46
	i64 u0xf4ecf4b9afc64781, ; 1159: System.ServiceProcess.dll => 133
	i64 u0xf4eeeaa566e9b970, ; 1160: lib_Xamarin.AndroidX.CustomView.PoolingContainer.dll.so => 243
	i64 u0xf518f63ead11fcd1, ; 1161: System.Threading.Tasks => 145
	i64 u0xf55063cf108c810b, ; 1162: lib-ko-Microsoft.TestPlatform.CrossPlatEngine.resources.dll.so => 384
	i64 u0xf5fc7602fe27b333, ; 1163: System.Net.WebHeaderCollection => 78
	i64 u0xf6077741019d7428, ; 1164: Xamarin.AndroidX.CoordinatorLayout => 237
	i64 u0xf61ade9836ad4692, ; 1165: Microsoft.IdentityModel.Tokens.dll => 188
	i64 u0xf6742cbf457c450b, ; 1166: Xamarin.AndroidX.Lifecycle.Runtime.Android.dll => 259
	i64 u0xf6c0e7d55a7a4e4f, ; 1167: Microsoft.IdentityModel.JsonWebTokens => 186
	i64 u0xf70c0a7bf8ccf5af, ; 1168: System.Web => 154
	i64 u0xf736b1e335efbd0e, ; 1169: lib_Microsoft.TestPlatform.CrossPlatEngine.dll.so => 199
	i64 u0xf77b20923f07c667, ; 1170: de/Microsoft.Maui.Controls.resources.dll => 309
	i64 u0xf7cb1a53bb79a80b, ; 1171: it/Microsoft.VisualStudio.TestPlatform.ObjectModel.resources => 348
	i64 u0xf7d5da3db84fa88c, ; 1172: Supabase.Postgrest => 210
	i64 u0xf7e2cac4c45067b3, ; 1173: lib_System.Numerics.Vectors.dll.so => 83
	i64 u0xf7e74930e0e3d214, ; 1174: zh-HK/Microsoft.Maui.Controls.resources.dll => 336
	i64 u0xf7fa0bf77fe677cc, ; 1175: Newtonsoft.Json.dll => 204
	i64 u0xf84773b5c81e3cef, ; 1176: lib-uk-Microsoft.Maui.Controls.resources.dll.so => 334
	i64 u0xf857b67c3c41dd09, ; 1177: ChessMAUI => 0
	i64 u0xf8aac5ea82de1348, ; 1178: System.Linq.Queryable => 61
	i64 u0xf8b77539b362d3ba, ; 1179: lib_System.Reflection.Primitives.dll.so => 96
	i64 u0xf8e045dc345b2ea3, ; 1180: lib_Xamarin.AndroidX.RecyclerView.dll.so => 272
	i64 u0xf915dc29808193a1, ; 1181: System.Web.HttpUtility.dll => 153
	i64 u0xf91db4f1ad5d43b1, ; 1182: Plugin.Maui.Audio.dll => 205
	i64 u0xf96c777a2a0686f4, ; 1183: hi/Microsoft.Maui.Controls.resources.dll => 315
	i64 u0xf9be54c8bcf8ff3b, ; 1184: System.Security.AccessControl.dll => 118
	i64 u0xf9eec5bb3a6aedc6, ; 1185: Microsoft.Extensions.Options => 183
	i64 u0xfa0e82300e67f913, ; 1186: lib_System.AppContext.dll.so => 6
	i64 u0xfa1bcb7eb4635889, ; 1187: lib-pl-Microsoft.TestPlatform.CommunicationUtilities.resources.dll.so => 386
	i64 u0xfa2fdb27e8a2c8e8, ; 1188: System.ComponentModel.EventBasedAsync => 15
	i64 u0xfa3f278f288b0e84, ; 1189: lib_System.Net.Security.dll.so => 74
	i64 u0xfa5ed7226d978949, ; 1190: lib-ar-Microsoft.Maui.Controls.resources.dll.so => 305
	i64 u0xfa645d91e9fc4cba, ; 1191: System.Threading.Thread => 146
	i64 u0xfab70c768d9e19a8, ; 1192: cs/Microsoft.TestPlatform.CoreUtilities.resources => 339
	i64 u0xfac9976111a1d757, ; 1193: ru/Microsoft.TestPlatform.CrossPlatEngine.resources => 393
	i64 u0xfad4d2c770e827f9, ; 1194: lib_System.IO.IsolatedStorage.dll.so => 52
	i64 u0xfae6188b90814b4b, ; 1195: lib-de-Microsoft.TestPlatform.CrossPlatEngine.resources.dll.so => 369
	i64 u0xfb06dd2338e6f7c4, ; 1196: System.Net.Ping.dll => 70
	i64 u0xfb087abe5365e3b7, ; 1197: lib_System.Data.DataSetExtensions.dll.so => 23
	i64 u0xfb35173928a89083, ; 1198: Supabase.Functions.dll => 208
	i64 u0xfb7a682b00f50271, ; 1199: lib_Supabase.Storage.dll.so => 212
	i64 u0xfb81857440fb5675, ; 1200: ja/Microsoft.TestPlatform.CrossPlatEngine.resources => 381
	i64 u0xfb846e949baff5ea, ; 1201: System.Xml.Serialization.dll => 158
	i64 u0xfbad3e4ce4b98145, ; 1202: System.Security.Cryptography.X509Certificates => 126
	i64 u0xfbba65887a38c94f, ; 1203: lib_Supabase.Core.dll.so => 207
	i64 u0xfbf0a31c9fc34bc4, ; 1204: lib_System.Net.Http.dll.so => 65
	i64 u0xfc6b7527cc280b3f, ; 1205: lib_System.Runtime.Serialization.Formatters.dll.so => 112
	i64 u0xfc719aec26adf9d9, ; 1206: Xamarin.AndroidX.Navigation.Fragment.dll => 268
	i64 u0xfc82690c2fe2735c, ; 1207: Xamarin.AndroidX.Lifecycle.Process.dll => 257
	i64 u0xfc93fc307d279893, ; 1208: System.IO.Pipes.AccessControl.dll => 55
	i64 u0xfcd302092ada6328, ; 1209: System.IO.MemoryMappedFiles.dll => 53
	i64 u0xfcf1fa722a11f18d, ; 1210: pt-BR/Microsoft.TestPlatform.CoreUtilities.resources.dll => 355
	i64 u0xfd22f00870e40ae0, ; 1211: lib_Xamarin.AndroidX.DrawerLayout.dll.so => 244
	i64 u0xfd359d3fc9c4cd3e, ; 1212: zh-Hant/Microsoft.TestPlatform.CrossPlatEngine.resources.dll => 402
	i64 u0xfd49b3c1a76e2748, ; 1213: System.Runtime.InteropServices.RuntimeInformation => 107
	i64 u0xfd536c702f64dc47, ; 1214: System.Text.Encoding.Extensions => 135
	i64 u0xfd583f7657b6a1cb, ; 1215: Xamarin.AndroidX.Fragment => 249
	i64 u0xfd8dd91a2c26bd5d, ; 1216: Xamarin.AndroidX.Lifecycle.Runtime => 258
	i64 u0xfda36abccf05cf5c, ; 1217: System.Net.WebSockets.Client => 80
	i64 u0xfddbe9695626a7f5, ; 1218: Xamarin.AndroidX.Lifecycle.Common => 252
	i64 u0xfeae9952cf03b8cb, ; 1219: tr/Microsoft.Maui.Controls.resources => 333
	i64 u0xfebe1950717515f9, ; 1220: Xamarin.AndroidX.Lifecycle.LiveData.Core.Ktx.dll => 256
	i64 u0xfec393f017b1cab8, ; 1221: cs/Microsoft.TestPlatform.CommunicationUtilities.resources.dll => 365
	i64 u0xff270a55858bac8d, ; 1222: System.Security.Principal => 129
	i64 u0xff9b54613e0d2cc8, ; 1223: System.Net.Http.Json => 64
	i64 u0xff9ffcf46ea08259, ; 1224: pl/Microsoft.TestPlatform.CommunicationUtilities.resources.dll => 386
	i64 u0xffa1fe933cabf8e4, ; 1225: Websocket.Client.dll => 215
	i64 u0xffdb7a971be4ec73 ; 1226: System.ValueTuple.dll => 152
], align 16

@assembly_image_cache_indices = dso_local local_unnamed_addr constant [1227 x i32] [
	i32 42, i32 301, i32 348, i32 279, i32 13, i32 269, i32 105, i32 209,
	i32 171, i32 48, i32 396, i32 225, i32 189, i32 7, i32 86, i32 329,
	i32 307, i32 335, i32 185, i32 245, i32 71, i32 272, i32 12, i32 192,
	i32 102, i32 336, i32 156, i32 19, i32 250, i32 232, i32 161, i32 247,
	i32 282, i32 167, i32 329, i32 10, i32 182, i32 283, i32 96, i32 243,
	i32 244, i32 13, i32 183, i32 10, i32 127, i32 95, i32 354, i32 140,
	i32 371, i32 39, i32 359, i32 330, i32 304, i32 341, i32 285, i32 326,
	i32 172, i32 219, i32 5, i32 193, i32 67, i32 276, i32 130, i32 208,
	i32 275, i32 396, i32 246, i32 68, i32 212, i32 390, i32 233, i32 66,
	i32 211, i32 57, i32 242, i32 376, i32 52, i32 393, i32 43, i32 125,
	i32 373, i32 393, i32 67, i32 81, i32 339, i32 260, i32 406, i32 158,
	i32 92, i32 99, i32 272, i32 141, i32 151, i32 229, i32 313, i32 162,
	i32 169, i32 314, i32 179, i32 81, i32 406, i32 296, i32 233, i32 4,
	i32 5, i32 51, i32 101, i32 403, i32 56, i32 120, i32 98, i32 168,
	i32 118, i32 384, i32 301, i32 21, i32 317, i32 137, i32 97, i32 304,
	i32 77, i32 323, i32 278, i32 357, i32 119, i32 395, i32 8, i32 165,
	i32 332, i32 70, i32 218, i32 261, i32 273, i32 171, i32 349, i32 145,
	i32 40, i32 276, i32 47, i32 30, i32 390, i32 343, i32 387, i32 270,
	i32 209, i32 357, i32 321, i32 144, i32 183, i32 163, i32 373, i32 28,
	i32 400, i32 84, i32 280, i32 77, i32 175, i32 43, i32 211, i32 29,
	i32 345, i32 42, i32 348, i32 103, i32 117, i32 223, i32 45, i32 91,
	i32 380, i32 332, i32 56, i32 341, i32 148, i32 405, i32 146, i32 100,
	i32 49, i32 364, i32 20, i32 238, i32 114, i32 358, i32 370, i32 216,
	i32 342, i32 313, i32 385, i32 291, i32 297, i32 184, i32 94, i32 58,
	i32 213, i32 379, i32 318, i32 316, i32 81, i32 364, i32 291, i32 169,
	i32 26, i32 353, i32 71, i32 271, i32 248, i32 404, i32 334, i32 69,
	i32 33, i32 312, i32 14, i32 139, i32 213, i32 38, i32 338, i32 234,
	i32 354, i32 203, i32 325, i32 134, i32 360, i32 92, i32 88, i32 149,
	i32 331, i32 201, i32 24, i32 138, i32 57, i32 51, i32 310, i32 29,
	i32 157, i32 34, i32 361, i32 164, i32 344, i32 249, i32 185, i32 52,
	i32 408, i32 287, i32 90, i32 350, i32 293, i32 230, i32 35, i32 313,
	i32 157, i32 207, i32 9, i32 311, i32 374, i32 388, i32 76, i32 55,
	i32 192, i32 307, i32 190, i32 13, i32 286, i32 176, i32 227, i32 109,
	i32 264, i32 349, i32 373, i32 32, i32 104, i32 84, i32 92, i32 365,
	i32 53, i32 365, i32 96, i32 295, i32 58, i32 198, i32 9, i32 102,
	i32 242, i32 68, i32 285, i32 214, i32 306, i32 214, i32 204, i32 355,
	i32 125, i32 273, i32 116, i32 135, i32 188, i32 126, i32 106, i32 297,
	i32 131, i32 229, i32 294, i32 147, i32 156, i32 250, i32 238, i32 245,
	i32 273, i32 347, i32 97, i32 24, i32 356, i32 277, i32 143, i32 210,
	i32 267, i32 3, i32 167, i32 385, i32 226, i32 100, i32 161, i32 99,
	i32 240, i32 364, i32 25, i32 93, i32 351, i32 168, i32 397, i32 172,
	i32 221, i32 3, i32 325, i32 247, i32 1, i32 114, i32 297, i32 250,
	i32 257, i32 213, i32 33, i32 6, i32 329, i32 156, i32 327, i32 53,
	i32 85, i32 284, i32 270, i32 44, i32 256, i32 375, i32 104, i32 47,
	i32 138, i32 343, i32 64, i32 351, i32 266, i32 69, i32 80, i32 59,
	i32 89, i32 154, i32 227, i32 133, i32 110, i32 319, i32 266, i32 271,
	i32 171, i32 134, i32 206, i32 197, i32 140, i32 40, i32 203, i32 306,
	i32 0, i32 188, i32 190, i32 354, i32 352, i32 60, i32 360, i32 263,
	i32 79, i32 340, i32 25, i32 36, i32 99, i32 387, i32 260, i32 71,
	i32 22, i32 238, i32 194, i32 330, i32 121, i32 69, i32 107, i32 336,
	i32 119, i32 117, i32 252, i32 253, i32 11, i32 2, i32 394, i32 124,
	i32 115, i32 142, i32 389, i32 346, i32 395, i32 41, i32 87, i32 222,
	i32 173, i32 27, i32 148, i32 320, i32 178, i32 292, i32 378, i32 221,
	i32 1, i32 223, i32 44, i32 237, i32 149, i32 18, i32 86, i32 308,
	i32 376, i32 41, i32 256, i32 231, i32 261, i32 94, i32 180, i32 28,
	i32 41, i32 78, i32 246, i32 234, i32 144, i32 108, i32 232, i32 358,
	i32 11, i32 105, i32 137, i32 16, i32 122, i32 66, i32 157, i32 22,
	i32 310, i32 303, i32 102, i32 361, i32 378, i32 178, i32 302, i32 63,
	i32 369, i32 58, i32 191, i32 309, i32 110, i32 173, i32 407, i32 300,
	i32 9, i32 289, i32 120, i32 98, i32 105, i32 374, i32 264, i32 190,
	i32 111, i32 224, i32 49, i32 20, i32 263, i32 241, i32 397, i32 72,
	i32 236, i32 201, i32 155, i32 39, i32 308, i32 35, i32 394, i32 298,
	i32 38, i32 314, i32 288, i32 380, i32 108, i32 323, i32 21, i32 295,
	i32 212, i32 262, i32 194, i32 15, i32 184, i32 79, i32 79, i32 241,
	i32 184, i32 402, i32 268, i32 275, i32 152, i32 21, i32 389, i32 209,
	i32 202, i32 192, i32 307, i32 50, i32 51, i32 333, i32 323, i32 94,
	i32 217, i32 319, i32 16, i32 198, i32 240, i32 123, i32 396, i32 316,
	i32 160, i32 45, i32 292, i32 174, i32 381, i32 116, i32 63, i32 166,
	i32 176, i32 352, i32 14, i32 274, i32 111, i32 224, i32 60, i32 299,
	i32 367, i32 121, i32 322, i32 2, i32 332, i32 249, i32 262, i32 346,
	i32 381, i32 372, i32 401, i32 199, i32 298, i32 296, i32 262, i32 6,
	i32 358, i32 231, i32 312, i32 245, i32 186, i32 17, i32 330, i32 309,
	i32 77, i32 235, i32 131, i32 195, i32 295, i32 377, i32 322, i32 83,
	i32 182, i32 12, i32 355, i32 34, i32 119, i32 303, i32 257, i32 247,
	i32 85, i32 216, i32 18, i32 285, i32 391, i32 177, i32 255, i32 72,
	i32 205, i32 405, i32 95, i32 376, i32 165, i32 251, i32 82, i32 338,
	i32 225, i32 230, i32 299, i32 154, i32 36, i32 151, i32 334, i32 367,
	i32 203, i32 185, i32 337, i32 144, i32 56, i32 113, i32 231, i32 282,
	i32 388, i32 281, i32 356, i32 340, i32 37, i32 338, i32 176, i32 115,
	i32 223, i32 14, i32 217, i32 146, i32 388, i32 43, i32 371, i32 193,
	i32 221, i32 98, i32 302, i32 366, i32 168, i32 16, i32 48, i32 107,
	i32 97, i32 266, i32 27, i32 128, i32 29, i32 314, i32 359, i32 345,
	i32 275, i32 128, i32 44, i32 241, i32 246, i32 149, i32 197, i32 8,
	i32 204, i32 267, i32 315, i32 328, i32 327, i32 132, i32 347, i32 326,
	i32 42, i32 303, i32 33, i32 408, i32 46, i32 143, i32 382, i32 263,
	i32 191, i32 366, i32 254, i32 242, i32 138, i32 62, i32 132, i32 0,
	i32 306, i32 48, i32 197, i32 160, i32 228, i32 254, i32 375, i32 366,
	i32 217, i32 252, i32 392, i32 322, i32 351, i32 398, i32 281, i32 46,
	i32 394, i32 164, i32 187, i32 251, i32 368, i32 187, i32 311, i32 195,
	i32 248, i32 318, i32 194, i32 18, i32 400, i32 8, i32 350, i32 174,
	i32 239, i32 124, i32 59, i32 141, i32 269, i32 321, i32 386, i32 258,
	i32 290, i32 189, i32 199, i32 403, i32 287, i32 150, i32 142, i32 301,
	i32 298, i32 201, i32 126, i32 210, i32 300, i32 160, i32 162, i32 243,
	i32 220, i32 177, i32 353, i32 324, i32 26, i32 267, i32 255, i32 372,
	i32 82, i32 195, i32 370, i32 287, i32 127, i32 291, i32 101, i32 148,
	i32 207, i32 289, i32 399, i32 270, i32 342, i32 54, i32 162, i32 341,
	i32 167, i32 131, i32 37, i32 283, i32 321, i32 22, i32 112, i32 90,
	i32 50, i32 60, i32 122, i32 83, i32 402, i32 127, i32 163, i32 290,
	i32 166, i32 274, i32 276, i32 244, i32 375, i32 401, i32 216, i32 259,
	i32 4, i32 389, i32 253, i32 317, i32 170, i32 2, i32 264, i32 383,
	i32 116, i32 407, i32 186, i32 222, i32 19, i32 367, i32 181, i32 89,
	i32 65, i32 30, i32 179, i32 310, i32 236, i32 59, i32 111, i32 255,
	i32 32, i32 128, i32 391, i32 159, i32 345, i32 328, i32 234, i32 140,
	i32 324, i32 153, i32 17, i32 233, i32 219, i32 75, i32 74, i32 15,
	i32 169, i32 85, i32 299, i32 124, i32 254, i32 265, i32 235, i32 189,
	i32 331, i32 261, i32 34, i32 342, i32 397, i32 118, i32 139, i32 122,
	i32 106, i32 308, i32 405, i32 202, i32 283, i32 230, i32 315, i32 305,
	i32 54, i32 47, i32 28, i32 346, i32 145, i32 181, i32 147, i32 400,
	i32 363, i32 35, i32 175, i32 331, i32 173, i32 380, i32 288, i32 75,
	i32 161, i32 1, i32 378, i32 277, i32 327, i32 320, i32 159, i32 12,
	i32 155, i32 200, i32 151, i32 206, i32 76, i32 363, i32 103, i32 112,
	i32 227, i32 65, i32 66, i32 286, i32 45, i32 229, i32 109, i32 7,
	i32 226, i32 55, i32 215, i32 222, i32 175, i32 360, i32 64, i32 305,
	i32 239, i32 382, i32 20, i32 109, i32 101, i32 62, i32 142, i32 220,
	i32 7, i32 187, i32 320, i32 170, i32 50, i32 286, i32 362, i32 115,
	i32 368, i32 141, i32 174, i32 166, i32 80, i32 113, i32 265, i32 17,
	i32 73, i32 268, i32 89, i32 218, i32 368, i32 87, i32 120, i32 280,
	i32 344, i32 224, i32 403, i32 135, i32 153, i32 106, i32 372, i32 11,
	i32 90, i32 383, i32 31, i32 392, i32 333, i32 136, i32 383, i32 325,
	i32 293, i32 328, i32 278, i32 370, i32 214, i32 219, i32 40, i32 408,
	i32 277, i32 139, i32 302, i32 304, i32 369, i32 25, i32 337, i32 73,
	i32 251, i32 279, i32 407, i32 27, i32 67, i32 88, i32 200, i32 95,
	i32 113, i32 31, i32 104, i32 253, i32 398, i32 37, i32 361, i32 72,
	i32 350, i32 384, i32 401, i32 399, i32 294, i32 108, i32 123, i32 226,
	i32 87, i32 180, i32 86, i32 353, i32 319, i32 93, i32 179, i32 129,
	i32 265, i32 356, i32 395, i32 280, i32 182, i32 196, i32 404, i32 274,
	i32 362, i32 239, i32 279, i32 236, i32 293, i32 290, i32 177, i32 163,
	i32 130, i32 391, i32 181, i32 284, i32 392, i32 271, i32 343, i32 178,
	i32 10, i32 49, i32 200, i32 335, i32 91, i32 335, i32 150, i32 62,
	i32 136, i32 150, i32 61, i32 180, i32 206, i32 117, i32 137, i32 296,
	i32 377, i32 84, i32 337, i32 159, i32 211, i32 281, i32 143, i32 363,
	i32 316, i32 248, i32 202, i32 208, i32 82, i32 70, i32 225, i32 136,
	i32 237, i32 218, i32 125, i32 404, i32 357, i32 54, i32 110, i32 130,
	i32 88, i32 23, i32 74, i32 129, i32 31, i32 73, i32 260, i32 318,
	i32 158, i32 23, i32 4, i32 170, i32 326, i32 123, i32 240, i32 379,
	i32 347, i32 317, i32 374, i32 312, i32 114, i32 172, i32 344, i32 32,
	i32 3, i32 349, i32 164, i32 196, i32 282, i32 30, i32 19, i32 259,
	i32 93, i32 36, i32 5, i32 352, i32 379, i32 288, i32 228, i32 300,
	i32 155, i32 278, i32 198, i32 292, i32 235, i32 339, i32 284, i32 406,
	i32 76, i32 63, i32 269, i32 147, i32 232, i32 121, i32 134, i32 294,
	i32 193, i32 371, i32 100, i32 39, i32 220, i32 311, i32 359, i32 385,
	i32 68, i32 26, i32 75, i32 78, i32 258, i32 191, i32 24, i32 152,
	i32 398, i32 387, i32 196, i32 38, i32 382, i32 399, i32 324, i32 340,
	i32 228, i32 133, i32 103, i32 289, i32 57, i32 362, i32 165, i32 91,
	i32 61, i32 377, i32 132, i32 205, i32 215, i32 390, i32 46, i32 133,
	i32 243, i32 145, i32 384, i32 78, i32 237, i32 188, i32 259, i32 186,
	i32 154, i32 199, i32 309, i32 348, i32 210, i32 83, i32 336, i32 204,
	i32 334, i32 0, i32 61, i32 96, i32 272, i32 153, i32 205, i32 315,
	i32 118, i32 183, i32 6, i32 386, i32 15, i32 74, i32 305, i32 146,
	i32 339, i32 393, i32 52, i32 369, i32 70, i32 23, i32 208, i32 212,
	i32 381, i32 158, i32 126, i32 207, i32 65, i32 112, i32 268, i32 257,
	i32 55, i32 53, i32 355, i32 244, i32 402, i32 107, i32 135, i32 249,
	i32 258, i32 80, i32 252, i32 333, i32 256, i32 365, i32 129, i32 64,
	i32 386, i32 215, i32 152
], align 16

@marshal_methods_number_of_classes = dso_local local_unnamed_addr constant i32 0, align 4

@marshal_methods_class_cache = dso_local local_unnamed_addr global [0 x %struct.MarshalMethodsManagedClass] zeroinitializer, align 8

; Names of classes in which marshal methods reside
@mm_class_names = dso_local local_unnamed_addr constant [0 x ptr] zeroinitializer, align 8

@mm_method_names = dso_local local_unnamed_addr constant [1 x %struct.MarshalMethodName] [
	%struct.MarshalMethodName {
		i64 u0x0000000000000000, ; name: 
		ptr @.MarshalMethodName.0_name; char* name
	} ; 0
], align 8

; get_function_pointer (uint32_t mono_image_index, uint32_t class_index, uint32_t method_token, void*& target_ptr)
@get_function_pointer = internal dso_local unnamed_addr global ptr null, align 8

; Functions

; Function attributes: memory(write, argmem: none, inaccessiblemem: none) "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" uwtable willreturn
define void @xamarin_app_init(ptr nocapture noundef readnone %env, ptr noundef %fn) local_unnamed_addr #0
{
	%fnIsNull = icmp eq ptr %fn, null
	br i1 %fnIsNull, label %1, label %2

1: ; preds = %0
	%putsResult = call noundef i32 @puts(ptr @.str.0)
	call void @abort()
	unreachable 

2: ; preds = %1, %0
	store ptr %fn, ptr @get_function_pointer, align 8, !tbaa !3
	ret void
}

; Strings
@.str.0 = private unnamed_addr constant [40 x i8] c"get_function_pointer MUST be specified\0A\00", align 16

;MarshalMethodName
@.MarshalMethodName.0_name = private unnamed_addr constant [1 x i8] c"\00", align 1

; External functions

; Function attributes: noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8"
declare void @abort() local_unnamed_addr #2

; Function attributes: nofree nounwind
declare noundef i32 @puts(ptr noundef) local_unnamed_addr #1
attributes #0 = { memory(write, argmem: none, inaccessiblemem: none) "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+crc32,+cx16,+cx8,+fxsr,+mmx,+popcnt,+sse,+sse2,+sse3,+sse4.1,+sse4.2,+ssse3,+x87" "tune-cpu"="generic" uwtable willreturn }
attributes #1 = { nofree nounwind }
attributes #2 = { noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+crc32,+cx16,+cx8,+fxsr,+mmx,+popcnt,+sse,+sse2,+sse3,+sse4.1,+sse4.2,+ssse3,+x87" "tune-cpu"="generic" }

; Metadata
!llvm.module.flags = !{!0, !1}
!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 7, !"PIC Level", i32 2}
!llvm.ident = !{!2}
!2 = !{!".NET for Android remotes/origin/release/9.0.1xx @ 9abff7703206541fdb83ffa80fe2c2753ad1997b"}
!3 = !{!4, !4, i64 0}
!4 = !{!"any pointer", !5, i64 0}
!5 = !{!"omnipotent char", !6, i64 0}
!6 = !{!"Simple C++ TBAA"}
