; ModuleID = 'marshal_methods.armeabi-v7a.ll'
source_filename = "marshal_methods.armeabi-v7a.ll"
target datalayout = "e-m:e-p:32:32-Fi8-i64:64-v128:64:128-a:0:32-n32-S64"
target triple = "armv7-unknown-linux-android21"

%struct.MarshalMethodName = type {
	i64, ; uint64_t id
	ptr ; char* name
}

%struct.MarshalMethodsManagedClass = type {
	i32, ; uint32_t token
	ptr ; MonoClass klass
}

@assembly_image_cache = dso_local local_unnamed_addr global [336 x ptr] zeroinitializer, align 4

; Each entry maps hash of an assembly name to an index into the `assembly_image_cache` array
@assembly_image_cache_hashes = dso_local local_unnamed_addr constant [1008 x i32] [
	i32 u0x0027eb9e, ; 0: System.Net.NetworkInformation.dll => 70
	i32 u0x00345a11, ; 1: lib_System.Net.Requests.dll.so => 74
	i32 u0x009b21bb, ; 2: System.Net.NameResolution.dll => 69
	i32 u0x00c8cc5d, ; 3: lib_Xamarin.AndroidX.Loader.dll.so => 256
	i32 u0x00e0bbf7, ; 4: lib_System.Xml.XmlSerializer.dll.so => 166
	i32 u0x00efe298, ; 5: System.Runtime.Intrinsics.dll => 111
	i32 u0x0119bc86, ; 6: lib_Microsoft.Extensions.DependencyInjection.Abstractions.dll.so => 184
	i32 u0x01cdfed1, ; 7: System.Linq.AsyncEnumerable => 59
	i32 u0x01f2c4e1, ; 8: Xamarin.AndroidX.Lifecycle.Runtime => 247
	i32 u0x0211b5dc, ; 9: Xamarin.Google.Guava.ListenableFuture.dll => 288
	i32 u0x02139ac3, ; 10: System.IO.FileSystem.DriveInfo => 48
	i32 u0x025a8054, ; 11: System.Net.WebSockets.dll => 83
	i32 u0x02664405, ; 12: lib-uk-Microsoft.Maui.Controls.resources.dll.so => 326
	i32 u0x028aa24d, ; 13: System.Threading.Thread => 149
	i32 u0x03358480, ; 14: lib_Microsoft.Maui.dll.so => 196
	i32 u0x0335cdbc, ; 15: ca/Microsoft.Maui.Controls.resources => 298
	i32 u0x03f75868, ; 16: System.Diagnostics.StackTrace => 30
	i32 u0x0410f24b, ; 17: System.Security.Cryptography.Primitives => 127
	i32 u0x044bb714, ; 18: Microsoft.Maui.Graphics.dll => 198
	i32 u0x04e7b0a1, ; 19: System.Runtime.CompilerServices.VisualC.dll => 105
	i32 u0x056606a6, ; 20: lib_System.Collections.NonGeneric.dll.so => 10
	i32 u0x05dc54b4, ; 21: Microsoft.Extensions.Diagnostics.Abstractions => 185
	i32 u0x060d4943, ; 22: Xamarin.AndroidX.SlidingPaneLayout => 270
	i32 u0x0621fa55, ; 23: lib_System.Net.ServerSentEvents.dll.so => 76
	i32 u0x065dd880, ; 24: lib_System.Linq.Queryable.dll.so => 62
	i32 u0x06989c2e, ; 25: Xamarin.AndroidX.Navigation.Runtime.Android.dll => 261
	i32 u0x06c2cd46, ; 26: zh-HK/Microsoft.Maui.Controls.resources => 328
	i32 u0x06e4e181, ; 27: lib_Xamarin.Google.Guava.ListenableFuture.dll.so => 288
	i32 u0x06ee56d3, ; 28: lib_System.Net.Mail.dll.so => 68
	i32 u0x06ffddbc, ; 29: System.Runtime.InteropServices => 110
	i32 u0x0720e5bb, ; 30: Xamarin.AndroidX.Navigation.Common.Android.dll => 258
	i32 u0x072f9521, ; 31: Xamarin.AndroidX.SlidingPaneLayout.dll => 270
	i32 u0x074aea82, ; 32: System.Threading.Channels.dll => 143
	i32 u0x0772c6a7, ; 33: lib_System.Diagnostics.TextWriterTraceListener.dll.so => 31
	i32 u0x0881c32f, ; 34: System.Net.WebHeaderCollection => 80
	i32 u0x08f064cf, ; 35: System.Security.Cryptography.Primitives.dll => 127
	i32 u0x097ed3c0, ; 36: System.ComponentModel.Annotations => 13
	i32 u0x098905a2, ; 37: lib_Xamarin.AndroidX.Concurrent.Futures.dll.so => 223
	i32 u0x09d975c3, ; 38: Xamarin.AndroidX.Collection.dll => 220
	i32 u0x0a0c2bd0, ; 39: lib_Xamarin.AndroidX.Activity.dll.so => 209
	i32 u0x0a81994f, ; 40: System.ServiceProcess => 135
	i32 u0x0ade3a75, ; 41: Xamarin.AndroidX.SwipeRefreshLayout.dll => 272
	i32 u0x0ae43932, ; 42: lib_Xamarin.AndroidX.Tracing.Tracing.dll.so => 273
	i32 u0x0aee6a3d, ; 43: lib-vi-Microsoft.Maui.Controls.resources.dll.so => 327
	i32 u0x0aeedc53, ; 44: lib_Xamarin.Google.Android.Material.dll.so => 284
	i32 u0x0afca281, ; 45: System.ValueTuple.dll => 155
	i32 u0x0b0de1c3, ; 46: lib_System.Xml.XPath.XDocument.dll.so => 163
	i32 u0x0b63b1e1, ; 47: lib_System.Net.Http.Json.dll.so => 65
	i32 u0x0b721a36, ; 48: lib-pl-Microsoft.Maui.Controls.resources.dll.so => 317
	i32 u0x0ba65f85, ; 49: vi/Microsoft.Maui.Controls.resources.dll => 327
	i32 u0x0ba8e231, ; 50: lib_System.Net.ServicePoint.dll.so => 77
	i32 u0x0be195c3, ; 51: zh-HK/Microsoft.Maui.Controls.resources.dll => 328
	i32 u0x0c38ff48, ; 52: System.ComponentModel => 18
	i32 u0x0c5df1c2, ; 53: lib_Microsoft.VisualStudio.DesignTools.XamlTapContract.dll.so => 334
	i32 u0x0c7b2e71, ; 54: Xamarin.AndroidX.Browser.dll => 218
	i32 u0x0cbc4dce, ; 55: System.Threading.AccessControl.dll => 142
	i32 u0x0cfa66a6, ; 56: lib_System.IO.Compression.FileSystem.dll.so => 44
	i32 u0x0d1f8edb, ; 57: System.Diagnostics.Debug => 26
	i32 u0x0d73bff4, ; 58: lib_Microsoft.Extensions.Logging.Debug.dll.so => 190
	i32 u0x0dc10265, ; 59: Microsoft.CSharp.dll => 1
	i32 u0x0dc2edec, ; 60: lib_Xamarin.AndroidX.Core.ViewTree.dll.so => 229
	i32 u0x0dc2f416, ; 61: lib_Xamarin.AndroidX.CustomView.dll.so => 231
	i32 u0x0dcb05c4, ; 62: System.Linq.Parallel => 61
	i32 u0x0dd133ce, ; 63: System.Globalization => 42
	i32 u0x0e3c65a0, ; 64: lib_System.Threading.AccessControl.dll.so => 142
	i32 u0x0e762ada, ; 65: lib-nb-Microsoft.Maui.Controls.resources.dll.so => 315
	i32 u0x0eb2f8c5, ; 66: System.Reflection.Emit.Lightweight => 94
	i32 u0x0ec71be0, ; 67: lib_System.Security.SecureString.dll.so => 132
	i32 u0x0ecfdca9, ; 68: lib_Xamarin.Android.Glide.dll.so => 205
	i32 u0x0f99119d, ; 69: Xamarin.AndroidX.ConstraintLayout.dll => 224
	i32 u0x107abf20, ; 70: System.Threading.Timer.dll => 151
	i32 u0x109c6ab8, ; 71: Xamarin.AndroidX.Lifecycle.LiveData.dll => 243
	i32 u0x10b7d2b7, ; 72: Xamarin.AndroidX.Interpolator => 240
	i32 u0x10bf9929, ; 73: cs/Microsoft.Maui.Controls.resources.dll => 299
	i32 u0x10c1d9f6, ; 74: lib_System.Data.DataSetExtensions.dll.so => 23
	i32 u0x113d3381, ; 75: lib-sk-Microsoft.Maui.Controls.resources.dll.so => 322
	i32 u0x1159791e, ; 76: System.IO.Pipes.AccessControl.dll => 55
	i32 u0x11d123fd, ; 77: System.Net.Ping.dll => 71
	i32 u0x13031348, ; 78: Xamarin.AndroidX.Activity.dll => 209
	i32 u0x132b30dd, ; 79: System.Numerics => 86
	i32 u0x1331a702, ; 80: lib_Xamarin.Google.Crypto.Tink.Android.dll.so => 286
	i32 u0x136bf828, ; 81: lib_System.Runtime.dll.so => 119
	i32 u0x14095832, ; 82: ja/Microsoft.Maui.Controls.resources.dll => 312
	i32 u0x146817a2, ; 83: Xamarin.AndroidX.Lifecycle.Common => 241
	i32 u0x14afd810, ; 84: SQLitePCLRaw.lib.e_sqlite3.android.dll => 203
	i32 u0x14eaf2a7, ; 85: lib_System.ComponentModel.Annotations.dll.so => 13
	i32 u0x153e1455, ; 86: it/Microsoft.Maui.Controls.resources.dll => 311
	i32 u0x15502fa0, ; 87: cs/Microsoft.Maui.Controls.resources => 299
	i32 u0x15766b7b, ; 88: System.ServiceModel.Web => 134
	i32 u0x15c177ae, ; 89: lib_Microsoft.Extensions.Configuration.dll.so => 181
	i32 u0x15e184df, ; 90: lib_System.Runtime.Loader.dll.so => 112
	i32 u0x15ebe147, ; 91: System.IO.Pipes => 56
	i32 u0x16101ba2, ; 92: lib_Microsoft.VisualStudio.DesignTools.MobileTapContracts.dll.so => 332
	i32 u0x1658bf94, ; 93: System.Transactions.Local => 153
	i32 u0x16646418, ; 94: System.Net.ServicePoint.dll => 77
	i32 u0x16a510e1, ; 95: System.Threading.Thread.dll => 149
	i32 u0x16d476c4, ; 96: System.IO.Hashing.dll => 176
	i32 u0x16fe439a, ; 97: System.Memory.dll => 64
	i32 u0x1766c1f7, ; 98: System.Threading.ThreadPool.dll => 150
	i32 u0x1778984a, ; 99: lib_Xamarin.AndroidX.ResourceInspection.Annotation.dll.so => 265
	i32 u0x17969339, ; 100: _Microsoft.Android.Resource.Designer => 335
	i32 u0x180c08d0, ; 101: WindowsBase => 169
	i32 u0x195d1904, ; 102: Xamarin.AndroidX.Lifecycle.Runtime.Android => 248
	i32 u0x198cd3eb, ; 103: lib_System.Security.Cryptography.Encoding.dll.so => 125
	i32 u0x19f6996b, ; 104: sv/Microsoft.Maui.Controls.resources.dll => 323
	i32 u0x1a4e3ec4, ; 105: Xamarin.AndroidX.ConstraintLayout.Core => 225
	i32 u0x1a61054f, ; 106: System.Collections => 12
	i32 u0x1ae0ec2c, ; 107: Xamarin.AndroidX.Fragment.dll => 238
	i32 u0x1ae969b2, ; 108: System.Security.Cryptography.X509Certificates => 128
	i32 u0x1b317bfd, ; 109: System.Web.HttpUtility.dll => 156
	i32 u0x1b46a9fd, ; 110: lib_Xamarin.AndroidX.Lifecycle.Runtime.Ktx.dll.so => 249
	i32 u0x1b5932ea, ; 111: lib_Mono.Android.Runtime.dll.so => 174
	i32 u0x1b611806, ; 112: System.Runtime.Serialization.Primitives.dll => 116
	i32 u0x1bc4415d, ; 113: mscorlib => 170
	i32 u0x1bc6ffe7, ; 114: lib_Java.Interop.dll.so => 172
	i32 u0x1bff388e, ; 115: System.dll => 168
	i32 u0x1c690cb9, ; 116: Xamarin.AndroidX.Interpolator.dll => 240
	i32 u0x1c78d08a, ; 117: lib_System.Private.Uri.dll.so => 89
	i32 u0x1d48410e, ; 118: lib_Xamarin.AndroidX.SlidingPaneLayout.dll.so => 270
	i32 u0x1d4d8185, ; 119: lib_System.Runtime.Serialization.dll.so => 118
	i32 u0x1dbae811, ; 120: System.ObjectModel => 87
	i32 u0x1dd2dc50, ; 121: id/Microsoft.Maui.Controls.resources.dll => 310
	i32 u0x1e092f31, ; 122: fi/Microsoft.Maui.Controls.resources.dll => 304
	i32 u0x1e9789de, ; 123: Microsoft.Extensions.Primitives.dll => 192
	i32 u0x1f1dceb7, ; 124: lib_System.Security.Cryptography.Primitives.dll.so => 127
	i32 u0x1f443e2d, ; 125: lib_System.AppContext.dll.so => 6
	i32 u0x1f6088c2, ; 126: System.Transactions.dll => 154
	i32 u0x1f6bf43d, ; 127: hi/Microsoft.Maui.Controls.resources => 307
	i32 u0x1f9b4faa, ; 128: System.Linq.Queryable => 62
	i32 u0x20216150, ; 129: Microsoft.Extensions.Logging => 188
	i32 u0x20303736, ; 130: System.IO.FileSystem.dll => 51
	i32 u0x2080b118, ; 131: System.Runtime.Extensions => 106
	i32 u0x20924146, ; 132: System.Runtime.Serialization.Xml => 117
	i32 u0x20bbb280, ; 133: System.Globalization.Calendars => 40
	i32 u0x2116ab2f, ; 134: Xamarin.JSpecify.dll => 290
	i32 u0x213954e7, ; 135: Jsr305Binding => 285
	i32 u0x218bdf07, ; 136: Xamarin.AndroidX.Core.ViewTree.dll => 229
	i32 u0x22697083, ; 137: System.Security.Cryptography.Cng => 123
	i32 u0x234b6fb2, ; 138: pt-BR/Microsoft.Maui.Controls.resources.dll => 318
	i32 u0x236793de, ; 139: lib_GoogleGson.dll.so => 180
	i32 u0x2386616a, ; 140: lib_System.ServiceModel.Web.dll.so => 134
	i32 u0x2397454a, ; 141: lib_System.Collections.Specialized.dll.so => 11
	i32 u0x239cf51b, ; 142: CommunityToolkit.Maui => 178
	i32 u0x23d83352, ; 143: System.IO.IsolatedStorage.dll => 52
	i32 u0x23eaab34, ; 144: lib_System.Core.dll.so => 21
	i32 u0x24154ecb, ; 145: System.IO.Compression.FileSystem => 44
	i32 u0x2459aaf0, ; 146: lib_System.Net.Sockets.dll.so => 78
	i32 u0x2493d7b9, ; 147: System.Security.Cryptography.Algorithms => 122
	i32 u0x2512d1c5, ; 148: Xamarin.AndroidX.Lifecycle.Runtime.Android.dll => 248
	i32 u0x2568904f, ; 149: Xamarin.AndroidX.CustomView => 231
	i32 u0x26233b86, ; 150: Xamarin.AndroidX.Emoji2.ViewsHelper.dll => 236
	i32 u0x26249f17, ; 151: lib_Xamarin.AndroidX.CustomView.PoolingContainer.dll.so => 232
	i32 u0x262968a7, ; 152: lib_System.Reflection.Extensions.dll.so => 96
	i32 u0x262d781c, ; 153: lib-de-Microsoft.Maui.Controls.resources.dll.so => 301
	i32 u0x2645b6c3, ; 154: lib_CommunityToolkit.Maui.Core.dll.so => 179
	i32 u0x2660a755, ; 155: System.Net => 84
	i32 u0x27787397, ; 156: System.Text.Encodings.Web.dll => 139
	i32 u0x278c7790, ; 157: Xamarin.AndroidX.VersionedParcelable => 278
	i32 u0x27b53050, ; 158: lib_System.Data.Common.dll.so => 22
	i32 u0x27b6d01f, ; 159: Xamarin.AndroidX.Arch.Core.Common.dll => 216
	i32 u0x2814a96c, ; 160: System.Collections.Concurrent => 8
	i32 u0x282acf5e, ; 161: lib_System.IO.FileSystem.dll.so => 51
	i32 u0x28607aa1, ; 162: lib-pt-BR-Microsoft.Maui.Controls.resources.dll.so => 318
	i32 u0x28bdabca, ; 163: System.Net.Security => 75
	i32 u0x2904cf94, ; 164: ca/Microsoft.Maui.Controls.resources.dll => 298
	i32 u0x29293ff5, ; 165: System.Xml.Linq.dll => 159
	i32 u0x29352520, ; 166: Xamarin.KotlinX.Coroutines.Android.dll => 292
	i32 u0x29423679, ; 167: lib_Xamarin.AndroidX.CursorAdapter.dll.so => 230
	i32 u0x295a9e3d, ; 168: System.Windows => 158
	i32 u0x296c7566, ; 169: lib_System.Xml.dll.so => 167
	i32 u0x29af2b3b, ; 170: System.Reflection.Emit => 95
	i32 u0x29bd7e5b, ; 171: Xamarin.Jetbrains.Annotations => 289
	i32 u0x29be9df3, ; 172: System.IO.Compression.ZipFile => 45
	i32 u0x2a1e8ecb, ; 173: ko/Microsoft.Maui.Controls.resources.dll => 313
	i32 u0x2a4afd4a, ; 174: de/Microsoft.Maui.Controls.resources.dll => 301
	i32 u0x2b15ed29, ; 175: System.Runtime.Loader.dll => 112
	i32 u0x2ba1ca8c, ; 176: lib_System.Security.dll.so => 133
	i32 u0x2bcfb3d9, ; 177: lib_Plugin.Maui.Audio.dll.so => 199
	i32 u0x2bd14e96, ; 178: System.Security.SecureString.dll => 132
	i32 u0x2ca248c0, ; 179: SQLitePCLRaw.batteries_v2 => 201
	i32 u0x2cd6293c, ; 180: System.Diagnostics.Contracts.dll => 25
	i32 u0x2d052d0c, ; 181: Xamarin.Android.Glide.Annotations.dll => 206
	i32 u0x2d322560, ; 182: lib_System.Xml.XmlDocument.dll.so => 165
	i32 u0x2d445acd, ; 183: System.Net.Requests => 74
	i32 u0x2d745423, ; 184: System.IO.Pipes.dll => 56
	i32 u0x2e394f87, ; 185: System.IO.Compression => 46
	i32 u0x2eec5558, ; 186: lib_System.Reflection.dll.so => 100
	i32 u0x2f0980eb, ; 187: Microsoft.Extensions.Options => 191
	i32 u0x2f0fe5eb, ; 188: lib_System.Reflection.DispatchProxy.dll.so => 92
	i32 u0x2f1c1e69, ; 189: Xamarin.AndroidX.CustomView.PoolingContainer.dll => 232
	i32 u0x2ff6fb9f, ; 190: System.Data.Common => 22
	i32 u0x302809e9, ; 191: Xamarin.AndroidX.Lifecycle.LiveData.Core.Ktx.dll => 245
	i32 u0x30a0e95c, ; 192: lib_System.Threading.Thread.dll.so => 149
	i32 u0x311247b5, ; 193: System.Private.Uri.dll => 89
	i32 u0x317d5b75, ; 194: System.IO.Compression.Brotli => 43
	i32 u0x31a103c6, ; 195: System.Xml.XPath.dll => 164
	i32 u0x31b69d60, ; 196: System.Net.Quic => 73
	i32 u0x3312831d, ; 197: lib_Xamarin.AndroidX.DrawerLayout.dll.so => 233
	i32 u0x33e88be1, ; 198: ar/Microsoft.Maui.Controls.resources => 297
	i32 u0x340ac0b8, ; 199: Microsoft.VisualBasic => 3
	i32 u0x34505120, ; 200: System.Globalization.dll => 42
	i32 u0x3463c971, ; 201: System.Net.Http.Json => 65
	i32 u0x34a30d77, ; 202: System.IO.Hashing => 176
	i32 u0x34a66c56, ; 203: lib_System.IO.Pipes.dll.so => 56
	i32 u0x351454c7, ; 204: lib_SQLitePCLRaw.lib.e_sqlite3.android.dll.so => 203
	i32 u0x352e5794, ; 205: lib_Xamarin.Google.ErrorProne.Annotations.dll.so => 287
	i32 u0x35e25008, ; 206: System.ComponentModel.Primitives.dll => 16
	i32 u0x3612ff2c, ; 207: lib_System.IO.dll.so => 58
	i32 u0x364e69a3, ; 208: System.IO.MemoryMappedFiles.dll => 53
	i32 u0x36e9595b, ; 209: lib_System.Transactions.dll.so => 154
	i32 u0x370eff4f, ; 210: lib_System.Globalization.Extensions.dll.so => 41
	i32 u0x373f6a31, ; 211: tr/Microsoft.Maui.Controls.resources.dll => 325
	i32 u0x3751ef41, ; 212: Xamarin.Google.Guava.ListenableFuture => 288
	i32 u0x3787b992, ; 213: lib_System.ComponentModel.DataAnnotations.dll.so => 14
	i32 u0x37ea9cd7, ; 214: lib_Xamarin.AndroidX.Lifecycle.ViewModel.Android.dll.so => 252
	i32 u0x382704bd, ; 215: lib_Xamarin.AndroidX.Emoji2.ViewsHelper.dll.so => 236
	i32 u0x38c136f7, ; 216: System.Runtime.InteropServices.JavaScript.dll => 108
	i32 u0x38d89c1d, ; 217: lib_Xamarin.AndroidX.Lifecycle.Common.Jvm.dll.so => 242
	i32 u0x38fe5498, ; 218: Xamarin.AndroidX.Window.WindowCore.Jvm => 283
	i32 u0x39481653, ; 219: lib_mscorlib.dll.so => 170
	i32 u0x399f1f06, ; 220: Xamarin.Google.Crypto.Tink.Android => 286
	i32 u0x39adca5e, ; 221: Xamarin.AndroidX.Lifecycle.Common.dll => 241
	i32 u0x3a20ecf3, ; 222: System.Diagnostics.Tracing => 34
	i32 u0x3a2aaa1d, ; 223: System.Xml.XDocument => 162
	i32 u0x3a8b0a79, ; 224: lib_Xamarin.KotlinX.Coroutines.Android.dll.so => 292
	i32 u0x3acd0267, ; 225: System.Private.DataContractSerialization.dll => 88
	i32 u0x3ad7b407, ; 226: System.Diagnostics.Tools => 32
	i32 u0x3b008d80, ; 227: lib_Xamarin.AndroidX.DynamicAnimation.dll.so => 234
	i32 u0x3b2c715c, ; 228: System.Collections.dll => 12
	i32 u0x3b3271e4, ; 229: zh-Hans/Microsoft.Maui.Controls.resources => 329
	i32 u0x3b458447, ; 230: lib_System.Threading.Tasks.Dataflow.dll.so => 145
	i32 u0x3b45fb35, ; 231: System.IO.FileSystem => 51
	i32 u0x3b4797e5, ; 232: es/Microsoft.Maui.Controls.resources => 303
	i32 u0x3bb6bd33, ; 233: System.IO.UnmanagedMemoryStream.dll => 57
	i32 u0x3c5e5b62, ; 234: Xamarin.AndroidX.SavedState.dll => 266
	i32 u0x3cbffa41, ; 235: System.Drawing => 36
	i32 u0x3d548d92, ; 236: Microsoft.Extensions.DependencyInjection.Abstractions => 184
	i32 u0x3d5a6611, ; 237: da/Microsoft.Maui.Controls.resources.dll => 300
	i32 u0x3d7be038, ; 238: Xamarin.Google.ErrorProne.Annotations.dll => 287
	i32 u0x3dbaaf8f, ; 239: Xamarin.AndroidX.AppCompat => 214
	i32 u0x3dc84a49, ; 240: System.Drawing.Primitives.dll => 35
	i32 u0x3df150e9, ; 241: lib_Xamarin.AndroidX.Interpolator.dll.so => 240
	i32 u0x3e444eb4, ; 242: System.Linq.Expressions.dll => 60
	i32 u0x3e5c42fd, ; 243: lib_System.Reflection.TypeExtensions.dll.so => 99
	i32 u0x3e865cbd, ; 244: Microsoft.Extensions.Diagnostics.Abstractions.dll => 185
	i32 u0x3eb776a1, ; 245: Xamarin.AndroidX.Lifecycle.ViewModel.dll => 251
	i32 u0x3ebd41f6, ; 246: lib_System.Collections.dll.so => 12
	i32 u0x3ecd3024, ; 247: lib_System.Resources.Reader.dll.so => 101
	i32 u0x3eea4db8, ; 248: lib_Microsoft.Extensions.Primitives.dll.so => 192
	i32 u0x3f2b54e4, ; 249: Temporizador.dll => 0
	i32 u0x3f3e1e33, ; 250: lib_Xamarin.AndroidX.Lifecycle.Process.dll.so => 246
	i32 u0x3f9dcf8c, ; 251: GoogleGson => 180
	i32 u0x408b17f4, ; 252: System.ComponentModel.TypeConverter => 17
	i32 u0x409e66d8, ; 253: Xamarin.Kotlin.StdLib => 291
	i32 u0x41761b2c, ; 254: System => 168
	i32 u0x422dfa8a, ; 255: Microsoft.Extensions.Hosting.Abstractions => 187
	i32 u0x4232ae7b, ; 256: lib_System.Reflection.Emit.dll.so => 95
	i32 u0x42be2972, ; 257: lib_System.Text.Encodings.Web.dll.so => 139
	i32 u0x42c091c1, ; 258: lib_Xamarin.Android.Glide.GifDecoder.dll.so => 208
	i32 u0x42da3e50, ; 259: Xamarin.AndroidX.Lifecycle.Runtime.Ktx.dll => 249
	i32 u0x43362f15, ; 260: Microsoft.Extensions.Logging.Debug => 190
	i32 u0x4393e151, ; 261: lib-th-Microsoft.Maui.Controls.resources.dll.so => 324
	i32 u0x441f18e1, ; 262: lib_System.Security.Cryptography.OpenSsl.dll.so => 126
	i32 u0x444e5c8e, ; 263: lib_System.ComponentModel.TypeConverter.dll.so => 17
	i32 u0x44549c93, ; 264: lib_System.Net.WebProxy.dll.so => 81
	i32 u0x4474042c, ; 265: lib_System.Numerics.Vectors.dll.so => 85
	i32 u0x447dc2e6, ; 266: Xamarin.AndroidX.Window => 281
	i32 u0x44845810, ; 267: lib_System.Net.Http.dll.so => 66
	i32 u0x44c3958b, ; 268: lib_System.Private.DataContractSerialization.dll.so => 88
	i32 u0x4528fc75, ; 269: System.Net.ServerSentEvents.dll => 76
	i32 u0x45bde382, ; 270: lib_System.Windows.dll.so => 158
	i32 u0x45c677b2, ; 271: System.Web.dll => 157
	i32 u0x45ec7ce1, ; 272: Microsoft.Extensions.FileProviders.Abstractions.dll => 186
	i32 u0x460b48eb, ; 273: Xamarin.AndroidX.VectorDrawable.Animated => 277
	i32 u0x463a8801, ; 274: Xamarin.AndroidX.Navigation.Runtime.dll => 260
	i32 u0x464305ed, ; 275: fi/Microsoft.Maui.Controls.resources => 304
	i32 u0x466ae52b, ; 276: lib_System.Threading.Overlapped.dll.so => 144
	i32 u0x47a87de7, ; 277: lib_System.Resources.Writer.dll.so => 103
	i32 u0x47b79c15, ; 278: pl/Microsoft.Maui.Controls.resources.dll => 317
	i32 u0x47c7b4fa, ; 279: Xamarin.AndroidX.Arch.Core.Common => 216
	i32 u0x480a69ad, ; 280: System.Diagnostics.Process => 29
	i32 u0x48aa6be3, ; 281: System.IO.IsolatedStorage => 52
	i32 u0x48bf92c4, ; 282: lib_Xamarin.AndroidX.Collection.dll.so => 220
	i32 u0x49654709, ; 283: lib_System.Threading.Timer.dll.so => 151
	i32 u0x499b8219, ; 284: nb/Microsoft.Maui.Controls.resources.dll => 315
	i32 u0x49dd578a, ; 285: lib_Microsoft.Extensions.Hosting.Abstractions.dll.so => 187
	i32 u0x4a0189ae, ; 286: lib-hi-Microsoft.Maui.Controls.resources.dll.so => 307
	i32 u0x4a4cd262, ; 287: Xamarin.AndroidX.Collection.Jvm.dll => 221
	i32 u0x4a8cb221, ; 288: lib_Xamarin.JSpecify.dll.so => 290
	i32 u0x4aaf6f7c, ; 289: Microsoft.Win32.Registry => 5
	i32 u0x4ae97402, ; 290: lib_Microsoft.Maui.Graphics.dll.so => 198
	i32 u0x4b275854, ; 291: Xamarin.KotlinX.Serialization.Core.Jvm => 296
	i32 u0x4b5eebe5, ; 292: Xamarin.AndroidX.Startup.StartupRuntime.dll => 271
	i32 u0x4b64b158, ; 293: Xamarin.KotlinX.Coroutines.Core.dll => 293
	i32 u0x4b863c7a, ; 294: lib_System.Private.Xml.Linq.dll.so => 90
	i32 u0x4b8a64a7, ; 295: Xamarin.AndroidX.VectorDrawable => 276
	i32 u0x4bb12d98, ; 296: lib_System.Runtime.Serialization.Xml.dll.so => 117
	i32 u0x4be46b58, ; 297: Xamarin.AndroidX.Collection.Ktx => 222
	i32 u0x4c071bea, ; 298: Xamarin.KotlinX.Coroutines.Android => 292
	i32 u0x4c3393c5, ; 299: Xamarin.AndroidX.Annotation.Jvm => 213
	i32 u0x4d0585a0, ; 300: SQLitePCLRaw.core.dll => 202
	i32 u0x4d14ee2b, ; 301: Xamarin.AndroidX.DrawerLayout.dll => 233
	i32 u0x4de0ce3b, ; 302: lib_Xamarin.AndroidX.ProfileInstaller.ProfileInstaller.dll.so => 263
	i32 u0x4e08a30b, ; 303: System.Private.DataContractSerialization => 88
	i32 u0x4e50159c, ; 304: Xamarin.AndroidX.Navigation.Common.Android => 258
	i32 u0x4ea003f9, ; 305: lib_Xamarin.AndroidX.Navigation.Common.Android.dll.so => 258
	i32 u0x4ed70c83, ; 306: Xamarin.AndroidX.Window.dll => 281
	i32 u0x4eed2679, ; 307: System.Linq => 63
	i32 u0x4f97822f, ; 308: System.Runtime.Serialization.Json.dll => 115
	i32 u0x50255dd9, ; 309: lib-hr-Microsoft.Maui.Controls.resources.dll.so => 308
	i32 u0x50550fba, ; 310: Plugin.Maui.Audio => 199
	i32 u0x50acdfd7, ; 311: lib-ca-Microsoft.Maui.Controls.resources.dll.so => 298
	i32 u0x50f5c1df, ; 312: lib_Xamarin.AndroidX.Lifecycle.ViewModelSavedState.Android.dll.so => 255
	i32 u0x514d38cd, ; 313: System.IO => 58
	i32 u0x52114ed3, ; 314: Xamarin.AndroidX.SavedState => 266
	i32 u0x523dc4c1, ; 315: System.Resources.ResourceManager => 102
	i32 u0x52f8d494, ; 316: lib_Microsoft.Maui.Controls.Compatibility.dll.so => 193
	i32 u0x533678bd, ; 317: lib_System.Private.CoreLib.dll.so => 177
	i32 u0x53701274, ; 318: lib_System.IO.FileSystem.Watcher.dll.so => 50
	i32 u0x53936ab4, ; 319: System.Configuration.dll => 19
	i32 u0x53cefc50, ; 320: Xamarin.AndroidX.CoordinatorLayout => 226
	i32 u0x53d71638, ; 321: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.Android.dll => 255
	i32 u0x53f80ba6, ; 322: System.Runtime.Serialization.Formatters.dll => 114
	i32 u0x5423e47b, ; 323: System.Runtime.CompilerServices.Unsafe => 104
	i32 u0x54246761, ; 324: lib_System.Diagnostics.Tools.dll.so => 32
	i32 u0x5498bac9, ; 325: lib_Microsoft.VisualBasic.dll.so => 3
	i32 u0x54ca50cb, ; 326: System.Runtime.CompilerServices.VisualC => 105
	i32 u0x557217fe, ; 327: lib_System.Numerics.dll.so => 86
	i32 u0x557b5293, ; 328: System.Runtime.Handles => 107
	i32 u0x558bc221, ; 329: Xamarin.Google.Crypto.Tink.Android.dll => 286
	i32 u0x55ab7451, ; 330: Xamarin.AndroidX.Lifecycle.Common.Jvm => 242
	i32 u0x55d10363, ; 331: System.Net.Quic.dll => 73
	i32 u0x55dfaca3, ; 332: lib_Microsoft.Win32.Primitives.dll.so => 4
	i32 u0x55e55df2, ; 333: Xamarin.AndroidX.Lifecycle.ViewModel.Android => 252
	i32 u0x568cd628, ; 334: System.Formats.Asn1.dll => 38
	i32 u0x569fcb36, ; 335: System.Diagnostics.Tools.dll => 32
	i32 u0x56c018af, ; 336: lib_System.IO.UnmanagedMemoryStream.dll.so => 57
	i32 u0x56e36530, ; 337: System.Runtime.Extensions.dll => 106
	i32 u0x56e7a7ad, ; 338: System.Net.Security.dll => 75
	i32 u0x5718a9ef, ; 339: System.Collections.Immutable.dll => 9
	i32 u0x57201017, ; 340: System.Security.Cryptography.OpenSsl => 126
	i32 u0x57261233, ; 341: System.IO.Compression.dll => 46
	i32 u0x57924923, ; 342: Xamarin.AndroidX.AppCompat.AppCompatResources => 215
	i32 u0x57a5e912, ; 343: Microsoft.Extensions.Primitives => 192
	i32 u0x5833866d, ; 344: System.Collections.Immutable => 9
	i32 u0x583e844f, ; 345: System.IO.Compression.Brotli.dll => 43
	i32 u0x58a57897, ; 346: Microsoft.Win32.Primitives => 4
	i32 u0x58cffa99, ; 347: Xamarin.AndroidX.SavedState.SavedState.Ktx.dll => 268
	i32 u0x58fd6613, ; 348: hi/Microsoft.Maui.Controls.resources.dll => 307
	i32 u0x596b5b3a, ; 349: lib_System.Drawing.Primitives.dll.so => 35
	i32 u0x5a48cf6c, ; 350: el/Microsoft.Maui.Controls.resources.dll => 302
	i32 u0x5b9331b6, ; 351: System.Diagnostics.TextWriterTraceListener => 31
	i32 u0x5be451c7, ; 352: lib_Xamarin.AndroidX.Browser.dll.so => 218
	i32 u0x5bf8ca0f, ; 353: System.Text.RegularExpressions.dll => 141
	i32 u0x5bfdbb43, ; 354: System.Reflection.Emit.dll => 95
	i32 u0x5c680b40, ; 355: System.Reflection.Extensions.dll => 96
	i32 u0x5c7be408, ; 356: sk/Microsoft.Maui.Controls.resources.dll => 322
	i32 u0x5cabc9a4, ; 357: fr/Microsoft.Maui.Controls.resources => 305
	i32 u0x5cb489e2, ; 358: Xamarin.AndroidX.Tracing.Tracing.Android => 274
	i32 u0x5d552ab7, ; 359: System.IO.FileSystem.Primitives => 49
	i32 u0x5d5a6c40, ; 360: System.Threading.Tasks.Dataflow.dll => 145
	i32 u0x5dccd455, ; 361: System.Runtime.Serialization.Json => 115
	i32 u0x5e0b6fdc, ; 362: Xamarin.KotlinX.Serialization.Core.Jvm.dll => 296
	i32 u0x5e2d7514, ; 363: System.Threading.Overlapped => 144
	i32 u0x5e2e3abe, ; 364: lib_Microsoft.VisualBasic.Core.dll.so => 2
	i32 u0x5e33306d, ; 365: sv/Microsoft.Maui.Controls.resources => 323
	i32 u0x5e7321d2, ; 366: lib_System.ComponentModel.Primitives.dll.so => 16
	i32 u0x5ed5f779, ; 367: zh-Hant/Microsoft.Maui.Controls.resources => 330
	i32 u0x5ef2ee25, ; 368: System.Runtime.Serialization.dll => 118
	i32 u0x5f3ec4dd, ; 369: Xamarin.Google.ErrorProne.Annotations => 287
	i32 u0x5f6f0b5b, ; 370: System.Xml.Serialization => 161
	i32 u0x5f93db6e, ; 371: Microsoft.Maui.Controls.HotReload.Forms.dll => 331
	i32 u0x5fa7b851, ; 372: System.Net.WebClient => 79
	i32 u0x6078995d, ; 373: System.Net.WebSockets.Client.dll => 82
	i32 u0x60892624, ; 374: lib_System.Formats.Tar.dll.so => 39
	i32 u0x60b0136a, ; 375: Xamarin.AndroidX.Loader.dll => 256
	i32 u0x60b33958, ; 376: System.Dynamic.Runtime => 37
	i32 u0x60d97228, ; 377: Xamarin.AndroidX.ViewPager2 => 280
	i32 u0x60ec189c, ; 378: lib_Xamarin.AndroidX.Arch.Core.Runtime.dll.so => 217
	i32 u0x616edae3, ; 379: CommunityToolkit.Maui.Core.dll => 179
	i32 u0x6176eff7, ; 380: Xamarin.AndroidX.Emoji2.ViewsHelper => 236
	i32 u0x6188ba7e, ; 381: Xamarin.AndroidX.CursorAdapter => 230
	i32 u0x61b9038d, ; 382: System.Net.Http.dll => 66
	i32 u0x61c036ca, ; 383: System.Text.RegularExpressions => 141
	i32 u0x61d59e0e, ; 384: System.ComponentModel.EventBasedAsync.dll => 15
	i32 u0x62021776, ; 385: lib_System.IO.Compression.dll.so => 46
	i32 u0x620a8774, ; 386: lib_System.Xml.ReaderWriter.dll.so => 160
	i32 u0x625755ef, ; 387: lib_WindowsBase.dll.so => 169
	i32 u0x62c6282e, ; 388: System.Runtime => 119
	i32 u0x62cec1a2, ; 389: lib_Xamarin.KotlinX.Coroutines.Core.Jvm.dll.so => 294
	i32 u0x62d6c1e4, ; 390: Xamarin.AndroidX.Tracing.Tracing.dll => 273
	i32 u0x62d6ea10, ; 391: Xamarin.Google.Android.Material.dll => 284
	i32 u0x638b1991, ; 392: Xamarin.AndroidX.ConstraintLayout => 224
	i32 u0x63dee9da, ; 393: System.IO.FileSystem.DriveInfo.dll => 48
	i32 u0x63fca3d0, ; 394: System.Net.Primitives.dll => 72
	i32 u0x640c0103, ; 395: System.Net.WebSockets => 83
	i32 u0x641979dd, ; 396: Xamarin.JSpecify => 290
	i32 u0x641f3e5a, ; 397: System.Security.Cryptography => 129
	i32 u0x64d1e4f5, ; 398: System.Reflection.Metadata => 97
	i32 u0x6525abc9, ; 399: System.Security.Cryptography.Csp => 124
	i32 u0x654b1498, ; 400: lib_System.Transactions.Local.dll.so => 153
	i32 u0x656b7698, ; 401: System.Diagnostics.Debug.dll => 26
	i32 u0x660284a1, ; 402: SQLitePCLRaw.lib.e_sqlite3.android => 203
	i32 u0x6670b12e, ; 403: lib_System.Security.AccessControl.dll.so => 120
	i32 u0x66888819, ; 404: Xamarin.AndroidX.Lifecycle.LiveData.Core.Ktx => 245
	i32 u0x66e27484, ; 405: System.Reflection.dll => 100
	i32 u0x66ffb0f8, ; 406: System.Diagnostics.FileVersionInfo.dll => 28
	i32 u0x6715dc86, ; 407: Xamarin.AndroidX.CardView.dll => 219
	i32 u0x67577fe1, ; 408: lib_System.Runtime.CompilerServices.VisualC.dll.so => 105
	i32 u0x677cd287, ; 409: ro/Microsoft.Maui.Controls.resources.dll => 320
	i32 u0x67fe8db2, ; 410: System.Transactions.Local.dll => 153
	i32 u0x68139a0d, ; 411: System.IO.Pipelines.dll => 54
	i32 u0x6816ab6a, ; 412: Mono.Android.Export => 173
	i32 u0x6853a83d, ; 413: Microsoft.Win32.Primitives.dll => 4
	i32 u0x68cc9d1e, ; 414: System.Resources.Reader.dll => 101
	i32 u0x68f61ae4, ; 415: lib_System.Formats.Asn1.dll.so => 38
	i32 u0x690d4b7d, ; 416: lib-zh-Hant-Microsoft.Maui.Controls.resources.dll.so => 330
	i32 u0x69239124, ; 417: System.Diagnostics.TraceSource.dll => 33
	i32 u0x693efa35, ; 418: lib_System.Net.WebHeaderCollection.dll.so => 80
	i32 u0x6942234e, ; 419: System.Reflection.Extensions => 96
	i32 u0x6947f945, ; 420: Xamarin.AndroidX.SwipeRefreshLayout => 272
	i32 u0x6988f147, ; 421: Microsoft.Extensions.Logging.dll => 188
	i32 u0x69ae5451, ; 422: lib_System.Runtime.InteropServices.JavaScript.dll.so => 108
	i32 u0x69d6d061, ; 423: lib_Xamarin.AndroidX.Window.WindowCore.Jvm.dll.so => 283
	i32 u0x69dc03cc, ; 424: System.Core.dll => 21
	i32 u0x69ec0683, ; 425: System.Globalization.Extensions.dll => 41
	i32 u0x69f4f41d, ; 426: lib_Xamarin.AndroidX.AppCompat.dll.so => 214
	i32 u0x6a216153, ; 427: Mono.Android.Runtime.dll => 174
	i32 u0x6a539b49, ; 428: lib_System.Runtime.Extensions.dll.so => 106
	i32 u0x6a96652d, ; 429: Xamarin.AndroidX.Fragment => 238
	i32 u0x6afaf338, ; 430: lib_System.Threading.dll.so => 152
	i32 u0x6b645ada, ; 431: lib-fr-Microsoft.Maui.Controls.resources.dll.so => 305
	i32 u0x6bbb2a76, ; 432: lib_Microsoft.Extensions.FileProviders.Abstractions.dll.so => 186
	i32 u0x6bcd3296, ; 433: Xamarin.AndroidX.Loader => 256
	i32 u0x6be1e423, ; 434: nb/Microsoft.Maui.Controls.resources => 315
	i32 u0x6c111525, ; 435: Xamarin.Kotlin.StdLib.dll => 291
	i32 u0x6c13413e, ; 436: Xamarin.Google.Android.Material => 284
	i32 u0x6c5562e0, ; 437: lib_Xamarin.KotlinX.Coroutines.Core.dll.so => 293
	i32 u0x6c652ce8, ; 438: Xamarin.AndroidX.Navigation.UI.dll => 262
	i32 u0x6c687fa7, ; 439: Microsoft.VisualBasic.Core => 2
	i32 u0x6c96614d, ; 440: hu/Microsoft.Maui.Controls.resources => 309
	i32 u0x6cbab720, ; 441: System.Text.Encoding.Extensions => 137
	i32 u0x6cc30c8c, ; 442: System.Runtime.Serialization.Formatters => 114
	i32 u0x6cea70ab, ; 443: Microsoft.VisualStudio.DesignTools.TapContract => 333
	i32 u0x6cf3d432, ; 444: lib_Xamarin.AndroidX.VersionedParcelable.dll.so => 278
	i32 u0x6cff90ba, ; 445: Microsoft.Extensions.Logging.Abstractions.dll => 189
	i32 u0x6dcaebf7, ; 446: uk/Microsoft.Maui.Controls.resources.dll => 326
	i32 u0x6e1ed932, ; 447: Xamarin.Android.Glide.Annotations => 206
	i32 u0x6ec71a65, ; 448: System.Linq.Expressions => 60
	i32 u0x6f7a29e4, ; 449: System.Reflection.Primitives => 98
	i32 u0x6fab02f2, ; 450: lib_Xamarin.AndroidX.ConstraintLayout.dll.so => 224
	i32 u0x7009e4c3, ; 451: System.Formats.Tar.dll => 39
	i32 u0x705fa726, ; 452: Xamarin.AndroidX.Arch.Core.Runtime.dll => 217
	i32 u0x7068d361, ; 453: Microsoft.VisualStudio.DesignTools.TapContract.dll => 333
	i32 u0x7070c6c0, ; 454: lib-zh-Hans-Microsoft.Maui.Controls.resources.dll.so => 329
	i32 u0x70972b6d, ; 455: System.Diagnostics.Contracts => 25
	i32 u0x70a66bdd, ; 456: System.Reflection.Metadata.dll => 97
	i32 u0x7124cf39, ; 457: System.Reflection.DispatchProxy => 92
	i32 u0x71490522, ; 458: System.Resources.ResourceManager.dll => 102
	i32 u0x71dc7c8b, ; 459: System.Collections.NonGeneric.dll => 10
	i32 u0x72fcebde, ; 460: lib_Xamarin.AndroidX.AppCompat.AppCompatResources.dll.so => 215
	i32 u0x731dd955, ; 461: lib_Mono.Android.dll.so => 175
	i32 u0x73674b00, ; 462: lib_SQLitePCLRaw.provider.e_sqlite3.dll.so => 204
	i32 u0x739bd4a8, ; 463: System.Private.Xml.Linq => 90
	i32 u0x73b20433, ; 464: lib_System.IO.FileSystem.Primitives.dll.so => 49
	i32 u0x73fbecbe, ; 465: lib_System.Memory.dll.so => 64
	i32 u0x74126030, ; 466: lib_System.Net.WebClient.dll.so => 79
	i32 u0x74a1c5bb, ; 467: System.Resources.Writer => 103
	i32 u0x74d743bf, ; 468: ja/Microsoft.Maui.Controls.resources => 312
	i32 u0x74eee4ef, ; 469: Xamarin.AndroidX.Security.SecurityCrypto.dll => 269
	i32 u0x75533a5e, ; 470: Microsoft.Extensions.Configuration.dll => 181
	i32 u0x7593c33f, ; 471: lib_System.IO.FileSystem.AccessControl.dll.so => 47
	i32 u0x75ec88d8, ; 472: System.Net.ServerSentEvents => 76
	i32 u0x765c50a4, ; 473: Xamarin.Android.Glide.GifDecoder => 208
	i32 u0x77ec19b4, ; 474: System.Buffers.dll => 7
	i32 u0x781074ce, ; 475: hr/Microsoft.Maui.Controls.resources => 308
	i32 u0x784d3e49, ; 476: lib_System.Net.dll.so => 84
	i32 u0x785e97f1, ; 477: Xamarin.AndroidX.Lifecycle.ViewModel => 251
	i32 u0x78b622b1, ; 478: ar/Microsoft.Maui.Controls.resources.dll => 297
	i32 u0x790376c9, ; 479: lib_Xamarin.AndroidX.Annotation.Experimental.dll.so => 212
	i32 u0x791a414b, ; 480: Xamarin.Android.Glide => 205
	i32 u0x7970be4f, ; 481: lib-he-Microsoft.Maui.Controls.resources.dll.so => 306
	i32 u0x79d00016, ; 482: it/Microsoft.Maui.Controls.resources => 311
	i32 u0x79eb68ee, ; 483: System.Private.Xml => 91
	i32 u0x7a80bd4e, ; 484: Xamarin.AndroidX.Lifecycle.LiveData.Core.dll => 244
	i32 u0x7aca0819, ; 485: System.Windows.dll => 158
	i32 u0x7b350579, ; 486: lib__Microsoft.Android.Resource.Designer.dll.so => 335
	i32 u0x7b3b4d96, ; 487: System.Linq.AsyncEnumerable.dll => 59
	i32 u0x7b473a37, ; 488: lib_Xamarin.AndroidX.Fragment.Ktx.dll.so => 239
	i32 u0x7b6f419e, ; 489: System.Diagnostics.TraceSource => 33
	i32 u0x7b884010, ; 490: lib_Temporizador.dll.so => 0
	i32 u0x7b8f6ff7, ; 491: lib_System.Runtime.Serialization.Json.dll.so => 115
	i32 u0x7bf8cdab, ; 492: System.Runtime.dll => 119
	i32 u0x7c51ebd4, ; 493: lib_System.Net.HttpListener.dll.so => 67
	i32 u0x7c9bf920, ; 494: System.Numerics.Vectors => 85
	i32 u0x7d603cde, ; 495: SQLitePCLRaw.provider.e_sqlite3.dll => 204
	i32 u0x7d702d52, ; 496: lib_System.Text.Encoding.dll.so => 138
	i32 u0x7e3cc7a5, ; 497: Microsoft.VisualStudio.DesignTools.XamlTapContract.dll => 334
	i32 u0x7ec9ffe9, ; 498: System.Console => 20
	i32 u0x7fb38cd2, ; 499: System.Collections.Specialized => 11
	i32 u0x7fc7a41e, ; 500: System.Xml.XmlSerializer.dll => 166
	i32 u0x7fd90a71, ; 501: lib_System.Text.Encoding.CodePages.dll.so => 136
	i32 u0x7fdcdc37, ; 502: lib-ko-Microsoft.Maui.Controls.resources.dll.so => 313
	i32 u0x7ff65cf5, ; 503: Microsoft.VisualBasic.dll => 3
	i32 u0x802a7166, ; 504: lib_System.Diagnostics.FileVersionInfo.dll.so => 28
	i32 u0x8030853e, ; 505: ko/Microsoft.Maui.Controls.resources => 313
	i32 u0x8044e1bd, ; 506: lib-ms-Microsoft.Maui.Controls.resources.dll.so => 314
	i32 u0x8081c489, ; 507: lib_Jsr305Binding.dll.so => 285
	i32 u0x80bd55ad, ; 508: Microsoft.Maui => 196
	i32 u0x80f2f56e, ; 509: lib_System.Runtime.Serialization.Formatters.dll.so => 114
	i32 u0x810c11c2, ; 510: ro/Microsoft.Maui.Controls.resources => 320
	i32 u0x8115bdf3, ; 511: lib_System.Resources.ResourceManager.dll.so => 102
	i32 u0x816751d8, ; 512: lib_System.Diagnostics.DiagnosticSource.dll.so => 27
	i32 u0x81a110ae, ; 513: lib_System.ComponentModel.EventBasedAsync.dll.so => 15
	i32 u0x820d22b3, ; 514: Microsoft.Extensions.Options.dll => 191
	i32 u0x82364da2, ; 515: lib_System.Buffers.dll.so => 7
	i32 u0x82a8237c, ; 516: Microsoft.Extensions.Logging.Abstractions => 189
	i32 u0x82b6c85e, ; 517: System.ObjectModel.dll => 87
	i32 u0x82bb5429, ; 518: lib_System.Linq.Expressions.dll.so => 60
	i32 u0x82c1cf3e, ; 519: lib_System.Net.Quic.dll.so => 73
	i32 u0x832ec206, ; 520: lib_System.Diagnostics.StackTrace.dll.so => 30
	i32 u0x83323b38, ; 521: Xamarin.KotlinX.Coroutines.Core.Jvm.dll => 294
	i32 u0x8334206b, ; 522: System.Net.Http => 66
	i32 u0x842e93b2, ; 523: Xamarin.AndroidX.VectorDrawable.Animated.dll => 277
	i32 u0x8471e4ec, ; 524: System.Threading.Tasks.Parallel => 147
	i32 u0x857e4dd2, ; 525: lib_System.Net.WebSockets.dll.so => 83
	i32 u0x8628f1a4, ; 526: lib-ru-Microsoft.Maui.Controls.resources.dll.so => 321
	i32 u0x863c6ac5, ; 527: System.Xml.Serialization.dll => 161
	i32 u0x865f9104, ; 528: Xamarin.AndroidX.Window.WindowCore.dll => 282
	i32 u0x867c9c52, ; 529: System.Globalization.Extensions => 41
	i32 u0x86b0fd78, ; 530: lib_Xamarin.AndroidX.Lifecycle.ViewModel.Ktx.dll.so => 253
	i32 u0x86bba59b, ; 531: lib_Microsoft.Maui.Controls.dll.so => 194
	i32 u0x8702d9a2, ; 532: System.Security.AccessControl.dll => 120
	i32 u0x871c9c1b, ; 533: Microsoft.Extensions.Configuration.Abstractions => 182
	i32 u0x872eeb7b, ; 534: Xamarin.Android.Glide.DiskLruCache.dll => 207
	i32 u0x875633cc, ; 535: fr/Microsoft.Maui.Controls.resources.dll => 305
	i32 u0x87a1a22b, ; 536: lib-it-Microsoft.Maui.Controls.resources.dll.so => 311
	i32 u0x87e25095, ; 537: Xamarin.AndroidX.RecyclerView.dll => 264
	i32 u0x87e7fdbb, ; 538: lib-nl-Microsoft.Maui.Controls.resources.dll.so => 316
	i32 u0x881f94da, ; 539: lib_netstandard.dll.so => 171
	i32 u0x8873eb17, ; 540: th/Microsoft.Maui.Controls.resources => 324
	i32 u0x887ae6a1, ; 541: lib_Xamarin.AndroidX.Lifecycle.Runtime.Android.dll.so => 248
	i32 u0x88937130, ; 542: Xamarin.AndroidX.Window.WindowCore => 282
	i32 u0x88acefcd, ; 543: System.ServiceModel.Web.dll => 134
	i32 u0x88d8bfaa, ; 544: System.Net.Sockets => 78
	i32 u0x88ed6f27, ; 545: lib_SQLitePCLRaw.batteries_v2.dll.so => 201
	i32 u0x88ffe49e, ; 546: System.Net.Mail => 68
	i32 u0x896b7878, ; 547: System.Private.CoreLib.dll => 177
	i32 u0x8a068af2, ; 548: Xamarin.AndroidX.Annotation.dll => 211
	i32 u0x8a52059a, ; 549: System.Threading.Tasks.Parallel.dll => 147
	i32 u0x8b804dbf, ; 550: System.Runtime.InteropServices.RuntimeInformation.dll => 109
	i32 u0x8bbaa2cd, ; 551: System.ValueTuple => 155
	i32 u0x8c20c628, ; 552: lib-fi-Microsoft.Maui.Controls.resources.dll.so => 304
	i32 u0x8c20f140, ; 553: lib_System.Console.dll.so => 20
	i32 u0x8c40e0db, ; 554: System.Net.Primitives => 72
	i32 u0x8c93dffb, ; 555: lib_SQLite-net.dll.so => 200
	i32 u0x8d19e4a2, ; 556: lib_Xamarin.AndroidX.Lifecycle.LiveData.dll.so => 243
	i32 u0x8d24e767, ; 557: System.Xml.ReaderWriter.dll => 160
	i32 u0x8d3fac99, ; 558: tr/Microsoft.Maui.Controls.resources => 325
	i32 u0x8d52b2e2, ; 559: Microsoft.Extensions.Configuration => 181
	i32 u0x8d52d3de, ; 560: lib_System.Threading.Tasks.dll.so => 148
	i32 u0x8dc6dbce, ; 561: System.Security.Cryptography.Csp.dll => 124
	i32 u0x8dcb0101, ; 562: lib_Xamarin.AndroidX.Navigation.Fragment.dll.so => 259
	i32 u0x8e02310f, ; 563: lib-ar-Microsoft.Maui.Controls.resources.dll.so => 297
	i32 u0x8e114655, ; 564: System.Security.Principal.Windows.dll => 130
	i32 u0x8e4e8441, ; 565: Xamarin.AndroidX.Window.WindowCore.Jvm.dll => 283
	i32 u0x8f24faee, ; 566: System.Web.HttpUtility => 156
	i32 u0x8f41c524, ; 567: Xamarin.AndroidX.Emoji2.dll => 235
	i32 u0x8f4e087a, ; 568: lib_System.Web.dll.so => 157
	i32 u0x8f8c64e2, ; 569: lib_System.Private.Xml.dll.so => 91
	i32 u0x8fa56e96, ; 570: Microsoft.VisualStudio.DesignTools.MobileTapContracts.dll => 332
	i32 u0x903eb247, ; 571: lib_Xamarin.AndroidX.Window.WindowCore.dll.so => 282
	i32 u0x905355ed, ; 572: System.Threading.Tasks.Dataflow => 145
	i32 u0x905caa9d, ; 573: nl/Microsoft.Maui.Controls.resources => 316
	i32 u0x906d466b, ; 574: Xamarin.AndroidX.Collection.Ktx.dll => 222
	i32 u0x90e50509, ; 575: lib_System.Reflection.Primitives.dll.so => 98
	i32 u0x911615a7, ; 576: lib_Xamarin.AndroidX.Fragment.dll.so => 238
	i32 u0x912896e5, ; 577: System.Console.dll => 20
	i32 u0x9130f5e7, ; 578: System.ComponentModel.DataAnnotations.dll => 14
	i32 u0x91abdf3a, ; 579: lib_Xamarin.AndroidX.Startup.StartupRuntime.dll.so => 271
	i32 u0x924edee6, ; 580: System.Text.Encoding.dll => 138
	i32 u0x928c75ca, ; 581: System.Net.Sockets.dll => 78
	i32 u0x92916334, ; 582: System.Linq.Parallel.dll => 61
	i32 u0x92f11675, ; 583: SQLitePCLRaw.batteries_v2.dll => 201
	i32 u0x92f50938, ; 584: Xamarin.AndroidX.ConstraintLayout.Core.dll => 225
	i32 u0x93554fdc, ; 585: netstandard.dll => 171
	i32 u0x93634cc0, ; 586: lib_Xamarin.AndroidX.Lifecycle.LiveData.Core.Ktx.dll.so => 245
	i32 u0x93918882, ; 587: Java.Interop.dll => 172
	i32 u0x93dba8a1, ; 588: Microsoft.Maui.Controls => 194
	i32 u0x940d5c2f, ; 589: System.ComponentModel.EventBasedAsync => 15
	i32 u0x94147f61, ; 590: System.Net.ServicePoint => 77
	i32 u0x9438d78e, ; 591: lib_System.Text.Json.dll.so => 140
	i32 u0x9469ba86, ; 592: lib_Xamarin.AndroidX.Lifecycle.Runtime.dll.so => 247
	i32 u0x94798bc5, ; 593: System.AppContext.dll => 6
	i32 u0x94a1db18, ; 594: lib-id-Microsoft.Maui.Controls.resources.dll.so => 310
	i32 u0x94fad8e5, ; 595: lib_Xamarin.AndroidX.Activity.Ktx.dll.so => 210
	i32 u0x95178668, ; 596: System.Data.DataSetExtensions => 23
	i32 u0x955cf248, ; 597: Xamarin.AndroidX.Lifecycle.Runtime.dll => 247
	i32 u0x9593ae7f, ; 598: lib_Xamarin.AndroidX.SavedState.dll.so => 266
	i32 u0x963ac2da, ; 599: sk/Microsoft.Maui.Controls.resources => 322
	i32 u0x9659e17c, ; 600: Xamarin.Android.Glide.dll => 205
	i32 u0x96bea474, ; 601: lib_Microsoft.Maui.Controls.Xaml.dll.so => 195
	i32 u0x974b89a2, ; 602: System.Reflection.Emit.Lightweight.dll => 94
	i32 u0x98ba5a04, ; 603: Microsoft.CSharp => 1
	i32 u0x9930ee42, ; 604: System.Text.Encodings.Web => 139
	i32 u0x999dcf0d, ; 605: Xamarin.AndroidX.Lifecycle.Runtime.Ktx.Android => 250
	i32 u0x99e2e424, ; 606: Xamarin.AndroidX.Lifecycle.Runtime.Ktx => 249
	i32 u0x99e370f2, ; 607: Xamarin.AndroidX.VectorDrawable.dll => 276
	i32 u0x9a1756ac, ; 608: System.Text.Encoding.Extensions.dll => 137
	i32 u0x9a20430d, ; 609: System.Net.Ping => 71
	i32 u0x9a5a3337, ; 610: System.Threading.ThreadPool => 150
	i32 u0x9a83ffe1, ; 611: Microsoft.Extensions.FileProviders.Abstractions => 186
	i32 u0x9b24ab96, ; 612: lib_System.Runtime.Serialization.Primitives.dll.so => 116
	i32 u0x9b500441, ; 613: Xamarin.KotlinX.Coroutines.Core.Jvm => 294
	i32 u0x9b5e5b1c, ; 614: lib_System.Diagnostics.Contracts.dll.so => 25
	i32 u0x9be14c08, ; 615: Xamarin.AndroidX.Fragment.Ktx => 239
	i32 u0x9bf052c1, ; 616: Microsoft.Extensions.Logging.Debug.dll => 190
	i32 u0x9bfe3a41, ; 617: System.Private.Xml.dll => 91
	i32 u0x9c165ff9, ; 618: System.Reflection.TypeExtensions.dll => 99
	i32 u0x9c375496, ; 619: Xamarin.AndroidX.CursorAdapter.dll => 230
	i32 u0x9c70e6c9, ; 620: Xamarin.AndroidX.DynamicAnimation => 234
	i32 u0x9c96ac4c, ; 621: lib_Xamarin.AndroidX.Navigation.UI.dll.so => 262
	i32 u0x9c97ad4a, ; 622: System.Diagnostics.TextWriterTraceListener.dll => 31
	i32 u0x9cc03a58, ; 623: System.IO.Compression.ZipFile.dll => 45
	i32 u0x9cd341b2, ; 624: lib_System.Threading.Tasks.Parallel.dll.so => 147
	i32 u0x9cf12c56, ; 625: Xamarin.AndroidX.Lifecycle.LiveData => 243
	i32 u0x9e78dac1, ; 626: lib_Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll.so => 254
	i32 u0x9ec022c0, ; 627: Xamarin.Android.Glide.DiskLruCache => 207
	i32 u0x9ec4cf01, ; 628: System.Runtime.Loader => 112
	i32 u0x9ecf752a, ; 629: System.Xml.XDocument.dll => 162
	i32 u0x9ee22cc0, ; 630: System.Drawing.Primitives => 35
	i32 u0x9f3b757e, ; 631: Xamarin.KotlinX.Coroutines.Core => 293
	i32 u0x9f7ea921, ; 632: lib_System.Runtime.InteropServices.dll.so => 110
	i32 u0x9f8c6f40, ; 633: System.Data.Common.dll => 22
	i32 u0xa026a50c, ; 634: System.Runtime.Serialization.Xml.dll => 117
	i32 u0xa090e36a, ; 635: System.IO.dll => 58
	i32 u0xa0fb56af, ; 636: lib_System.Text.RegularExpressions.dll.so => 141
	i32 u0xa0ff7514, ; 637: Xamarin.AndroidX.Tracing.Tracing => 273
	i32 u0xa1d8b647, ; 638: System.Threading.Tasks.dll => 148
	i32 u0xa1fd7d9f, ; 639: System.Security.Claims => 121
	i32 u0xa21f5a1f, ; 640: System.Security.Cryptography.Cng.dll => 123
	i32 u0xa25c90e5, ; 641: lib_Xamarin.AndroidX.Core.dll.so => 227
	i32 u0xa262a30f, ; 642: System.Runtime.Numerics.dll => 113
	i32 u0xa2ce8457, ; 643: lib-es-Microsoft.Maui.Controls.resources.dll.so => 303
	i32 u0xa2e0939b, ; 644: Xamarin.AndroidX.Activity => 209
	i32 u0xa30769e5, ; 645: System.Threading.Channels => 143
	i32 u0xa32eb6f0, ; 646: Xamarin.AndroidX.AppCompat.AppCompatResources.dll => 215
	i32 u0xa35f8f92, ; 647: System.IO.Pipes.AccessControl => 55
	i32 u0xa3c818c7, ; 648: lib_System.Net.WebSockets.Client.dll.so => 82
	i32 u0xa3cc7fa7, ; 649: System.Runtime.InteropServices.JavaScript => 108
	i32 u0xa3de87ea, ; 650: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.Android => 255
	i32 u0xa4672f3b, ; 651: Microsoft.Maui.Controls.Xaml => 195
	i32 u0xa493aa02, ; 652: lib_System.Collections.Concurrent.dll.so => 8
	i32 u0xa4caf7a7, ; 653: Microsoft.Maui.dll => 196
	i32 u0xa4d4aaf8, ; 654: lib_System.Security.Cryptography.Cng.dll.so => 123
	i32 u0xa4db22c6, ; 655: System.Text.Encoding.CodePages.dll => 136
	i32 u0xa4e79dfd, ; 656: Xamarin.AndroidX.Lifecycle.ViewModel.Android.dll => 252
	i32 u0xa522693c, ; 657: Xamarin.Jetbrains.Annotations.dll => 289
	i32 u0xa52ac270, ; 658: lib_Xamarin.AndroidX.Window.dll.so => 281
	i32 u0xa553c739, ; 659: lib_System.ValueTuple.dll.so => 155
	i32 u0xa5a0a402, ; 660: Xamarin.AndroidX.ViewPager.dll => 279
	i32 u0xa5b3182d, ; 661: Xamarin.AndroidX.ResourceInspection.Annotation.dll => 265
	i32 u0xa5b67c07, ; 662: Xamarin.AndroidX.Lifecycle.Common.Jvm.dll => 242
	i32 u0xa5c5753c, ; 663: lib_System.Collections.Immutable.dll.so => 9
	i32 u0xa5ea80d9, ; 664: lib_Xamarin.Android.Glide.Annotations.dll.so => 206
	i32 u0xa6133c7f, ; 665: lib_System.IO.FileSystem.DriveInfo.dll.so => 48
	i32 u0xa630ecdd, ; 666: Xamarin.AndroidX.Fragment.Ktx.dll => 239
	i32 u0xa668c988, ; 667: lib_System.Net.NameResolution.dll.so => 69
	i32 u0xa68bc8b3, ; 668: lib_Xamarin.AndroidX.Tracing.Tracing.Android.dll.so => 274
	i32 u0xa7008e0b, ; 669: Microsoft.Maui.Graphics => 198
	i32 u0xa7042ae3, ; 670: uk/Microsoft.Maui.Controls.resources => 326
	i32 u0xa715dd7e, ; 671: System.Xml.XPath.XDocument.dll => 163
	i32 u0xa741ef0b, ; 672: es/Microsoft.Maui.Controls.resources.dll => 303
	i32 u0xa744f665, ; 673: lib_Xamarin.AndroidX.Navigation.Runtime.dll.so => 260
	i32 u0xa78103bc, ; 674: Xamarin.AndroidX.CoordinatorLayout.dll => 226
	i32 u0xa8032c67, ; 675: lib_Microsoft.Win32.Registry.dll.so => 5
	i32 u0xa80db4e1, ; 676: System.Xml.dll => 167
	i32 u0xa81b119f, ; 677: lib_System.Security.Cryptography.dll.so => 129
	i32 u0xa8282c09, ; 678: System.ServiceProcess.dll => 135
	i32 u0xa8298928, ; 679: Xamarin.AndroidX.ResourceInspection.Annotation => 265
	i32 u0xa85a7b6c, ; 680: System.Xml.XmlDocument => 165
	i32 u0xa8c61dcb, ; 681: nl/Microsoft.Maui.Controls.resources.dll => 316
	i32 u0xa9366b55, ; 682: Xamarin.AndroidX.Tracing.Tracing.Android.dll => 274
	i32 u0xa9379a4f, ; 683: Xamarin.AndroidX.Lifecycle.ViewModel.Ktx.dll => 253
	i32 u0xa9d96f9b, ; 684: System.Threading.Overlapped.dll => 144
	i32 u0xaa107fc4, ; 685: Xamarin.AndroidX.ViewPager => 279
	i32 u0xaa2b531f, ; 686: lib_System.Globalization.dll.so => 42
	i32 u0xaa36a797, ; 687: Xamarin.AndroidX.Transition => 275
	i32 u0xaa4e51ff, ; 688: el/Microsoft.Maui.Controls.resources => 302
	i32 u0xaa88e550, ; 689: Mono.Android.Export.dll => 173
	i32 u0xaa8a4878, ; 690: Microsoft.Maui.Essentials => 197
	i32 u0xaaf9aad7, ; 691: CommunityToolkit.Maui.Core => 179
	i32 u0xab123e9a, ; 692: Xamarin.AndroidX.Activity.Ktx.dll => 210
	i32 u0xab5f85c3, ; 693: Jsr305Binding.dll => 285
	i32 u0xab606289, ; 694: System.Globalization.Calendars.dll => 40
	i32 u0xabbc23e8, ; 695: lib_Xamarin.KotlinX.Serialization.Core.Jvm.dll.so => 296
	i32 u0xabdea79a, ; 696: ru/Microsoft.Maui.Controls.resources => 321
	i32 u0xabf58099, ; 697: Xamarin.AndroidX.ExifInterface => 237
	i32 u0xac1dd496, ; 698: System.Net.dll => 84
	i32 u0xacd6baa9, ; 699: System.IO.UnmanagedMemoryStream => 57
	i32 u0xace3f9b4, ; 700: System.Dynamic.Runtime.dll => 37
	i32 u0xace7ba82, ; 701: lib_System.Security.Principal.Windows.dll.so => 130
	i32 u0xacf080de, ; 702: System.Reflection => 100
	i32 u0xacf097ce, ; 703: System.Threading.AccessControl => 142
	i32 u0xad2a79b6, ; 704: mscorlib.dll => 170
	i32 u0xad6f1e8a, ; 705: System.Private.CoreLib => 177
	i32 u0xad90894d, ; 706: lib_Xamarin.KotlinX.Serialization.Core.dll.so => 295
	i32 u0xaddb6d38, ; 707: Xamarin.AndroidX.ViewPager2.dll => 280
	i32 u0xae037813, ; 708: System.Numerics.Vectors.dll => 85
	i32 u0xae1ce33f, ; 709: Xamarin.AndroidX.Annotation.Experimental.dll => 212
	i32 u0xaeb2d8a5, ; 710: lib_Microsoft.Extensions.Options.dll.so => 191
	i32 u0xaf06273c, ; 711: System.Resources.Reader => 101
	i32 u0xaf3a6b91, ; 712: lib_System.Diagnostics.Debug.dll.so => 26
	i32 u0xaf4af872, ; 713: System.Diagnostics.StackTrace.dll => 30
	i32 u0xaf624531, ; 714: System.Xml.XPath.XDocument => 163
	i32 u0xaf8b1081, ; 715: lib_Xamarin.AndroidX.SavedState.SavedState.Ktx.dll.so => 268
	i32 u0xb0682092, ; 716: System.ComponentModel.dll => 18
	i32 u0xb0ed41f3, ; 717: System.Security.Principal.Windows => 130
	i32 u0xb1182a36, ; 718: lib_Xamarin.AndroidX.Navigation.Runtime.Android.dll.so => 261
	i32 u0xb128f886, ; 719: System.Security.Cryptography.Algorithms.dll => 122
	i32 u0xb18af942, ; 720: Xamarin.AndroidX.DrawerLayout => 233
	i32 u0xb1a434a2, ; 721: lib_System.Xml.Linq.dll.so => 159
	i32 u0xb1a7d210, ; 722: lib_Xamarin.AndroidX.Lifecycle.Runtime.Ktx.Android.dll.so => 250
	i32 u0xb21220a3, ; 723: Xamarin.AndroidX.Security.SecurityCrypto => 269
	i32 u0xb223fa8c, ; 724: lib-cs-Microsoft.Maui.Controls.resources.dll.so => 299
	i32 u0xb28cab85, ; 725: lib_Xamarin.Android.Glide.DiskLruCache.dll.so => 207
	i32 u0xb294d40b, ; 726: lib_System.Net.Ping.dll.so => 71
	i32 u0xb2a03f9f, ; 727: Xamarin.AndroidX.Lifecycle.Process.dll => 246
	i32 u0xb3d3821c, ; 728: Xamarin.AndroidX.Startup.StartupRuntime => 271
	i32 u0xb434b64b, ; 729: WindowsBase.dll => 169
	i32 u0xb514b305, ; 730: _Microsoft.Android.Resource.Designer.dll => 335
	i32 u0xb58d85d9, ; 731: lib_System.Runtime.Handles.dll.so => 107
	i32 u0xb62a9ccb, ; 732: Xamarin.AndroidX.SavedState.SavedState.Ktx => 268
	i32 u0xb63fa9f0, ; 733: Xamarin.AndroidX.Navigation.Common => 257
	i32 u0xb6490b5e, ; 734: lib_Mono.Android.Export.dll.so => 173
	i32 u0xb65adef9, ; 735: Mono.Android.Runtime => 174
	i32 u0xb660be12, ; 736: System.ComponentModel.Primitives => 16
	i32 u0xb6a153b2, ; 737: lib_Xamarin.AndroidX.ViewPager2.dll.so => 280
	i32 u0xb70c6fb4, ; 738: lib_Xamarin.AndroidX.VectorDrawable.dll.so => 276
	i32 u0xb755818f, ; 739: System.Threading.Tasks => 148
	i32 u0xb76be845, ; 740: hu/Microsoft.Maui.Controls.resources.dll => 309
	i32 u0xb7e7c341, ; 741: lib_System.Globalization.Calendars.dll.so => 40
	i32 u0xb838e2b0, ; 742: System.Security.Cryptography.X509Certificates.dll => 128
	i32 u0xb8c22b7f, ; 743: System.Security.Claims.dll => 121
	i32 u0xb8fd311b, ; 744: System.Formats.Asn1 => 38
	i32 u0xb979e222, ; 745: System.Runtime.Serialization => 118
	i32 u0xba0dbf1c, ; 746: System.IO.FileSystem.AccessControl.dll => 47
	i32 u0xba4127cb, ; 747: System.Threading.Tasks.Extensions => 146
	i32 u0xbaa520e7, ; 748: lib_System.ObjectModel.dll.so => 87
	i32 u0xbab301d1, ; 749: System.Security.AccessControl => 120
	i32 u0xbb95ee37, ; 750: System.Diagnostics.Tracing.dll => 34
	i32 u0xbba64c02, ; 751: GoogleGson.dll => 180
	i32 u0xbc4c6465, ; 752: System.Reflection.Primitives.dll => 98
	i32 u0xbc652da7, ; 753: System.IO.MemoryMappedFiles => 53
	i32 u0xbc85a377, ; 754: Temporizador => 0
	i32 u0xbc98c93d, ; 755: lib_Xamarin.AndroidX.Collection.Jvm.dll.so => 221
	i32 u0xbcc610a0, ; 756: lib_System.Reflection.Metadata.dll.so => 97
	i32 u0xbd113355, ; 757: lib_Xamarin.AndroidX.Navigation.Common.dll.so => 257
	i32 u0xbd78b0c8, ; 758: Xamarin.AndroidX.Navigation.Fragment.dll => 259
	i32 u0xbddce8a2, ; 759: lib_System.Security.Principal.dll.so => 131
	i32 u0xbe3f07c2, ; 760: lib_System.Runtime.CompilerServices.Unsafe.dll.so => 104
	i32 u0xbe4755f4, ; 761: System.Security.SecureString => 132
	i32 u0xbe592c0c, ; 762: System.Web => 157
	i32 u0xbefef58f, ; 763: System.Data.dll => 24
	i32 u0xbf506931, ; 764: System.Xml.XmlDocument.dll => 165
	i32 u0xbfc8f642, ; 765: Microsoft.VisualStudio.DesignTools.XamlTapContract => 334
	i32 u0xbff2e236, ; 766: System.Threading => 152
	i32 u0xc04c3c0a, ; 767: System.Runtime.Handles.dll => 107
	i32 u0xc095e070, ; 768: lib_Xamarin.AndroidX.Lifecycle.Common.dll.so => 241
	i32 u0xc10b79a7, ; 769: Xamarin.AndroidX.Core.ViewTree => 229
	i32 u0xc1c6ebf4, ; 770: System.Reflection.DispatchProxy.dll => 92
	i32 u0xc217efb6, ; 771: lib_Xamarin.AndroidX.ConstraintLayout.Core.dll.so => 225
	i32 u0xc2293e61, ; 772: Xamarin.AndroidX.SavedState.SavedState.Android.dll => 267
	i32 u0xc235e84d, ; 773: Xamarin.AndroidX.CardView => 219
	i32 u0xc2a37b91, ; 774: System.Linq.Queryable.dll => 62
	i32 u0xc2a993fa, ; 775: System.Threading.Tasks.Extensions.dll => 146
	i32 u0xc3428433, ; 776: lib_System.Reflection.Emit.ILGeneration.dll.so => 93
	i32 u0xc35f7fa4, ; 777: System.Resources.Writer.dll => 103
	i32 u0xc37f65ce, ; 778: Microsoft.Win32.Registry.dll => 5
	i32 u0xc3888e16, ; 779: System.ComponentModel.Annotations.dll => 13
	i32 u0xc3ba1d80, ; 780: lib_System.Security.Cryptography.Csp.dll.so => 124
	i32 u0xc3e9b3a2, ; 781: SQLite-net.dll => 200
	i32 u0xc4251ff9, ; 782: System.Security.Cryptography.Encoding => 125
	i32 u0xc4684d9e, ; 783: lib_System.Security.Cryptography.Algorithms.dll.so => 122
	i32 u0xc4a8494a, ; 784: System.Text.Encoding => 138
	i32 u0xc4e76306, ; 785: System.Diagnostics.FileVersionInfo => 28
	i32 u0xc591efe9, ; 786: lib_Microsoft.Extensions.Configuration.Abstractions.dll.so => 182
	i32 u0xc5b097e4, ; 787: System.Net.Requests.dll => 74
	i32 u0xc5b776df, ; 788: Xamarin.AndroidX.CustomView.dll => 231
	i32 u0xc5b79d28, ; 789: System.Data => 24
	i32 u0xc69f3b41, ; 790: lib_System.Data.dll.so => 24
	i32 u0xc71af05d, ; 791: Xamarin.AndroidX.Arch.Core.Runtime => 217
	i32 u0xc76e512c, ; 792: Xamarin.AndroidX.ProfileInstaller.ProfileInstaller.dll => 263
	i32 u0xc774da4f, ; 793: Xamarin.AndroidX.Navigation.Runtime => 260
	i32 u0xc7a3b0f0, ; 794: lib_Xamarin.AndroidX.Transition.dll.so => 275
	i32 u0xc7b797d0, ; 795: lib_Xamarin.AndroidX.Core.Core.Ktx.dll.so => 228
	i32 u0xc821fc10, ; 796: lib_System.ComponentModel.dll.so => 18
	i32 u0xc82afec1, ; 797: System.Text.Json => 140
	i32 u0xc849ca45, ; 798: SQLitePCLRaw.core => 202
	i32 u0xc8693088, ; 799: Xamarin.AndroidX.Activity.Ktx => 210
	i32 u0xc86c06e3, ; 800: Xamarin.AndroidX.Core => 227
	i32 u0xc8a662e9, ; 801: Java.Interop => 172
	i32 u0xc8d10307, ; 802: lib_System.Diagnostics.TraceSource.dll.so => 33
	i32 u0xc9094c00, ; 803: Xamarin.AndroidX.Navigation.Runtime.Android => 261
	i32 u0xc92a6809, ; 804: Xamarin.AndroidX.RecyclerView => 264
	i32 u0xca5de1fa, ; 805: System.Runtime.CompilerServices.Unsafe.dll => 104
	i32 u0xcae37e41, ; 806: System.Security.Cryptography.OpenSsl.dll => 126
	i32 u0xcaf7bd4b, ; 807: Xamarin.AndroidX.CustomView.PoolingContainer => 232
	i32 u0xcb5af55c, ; 808: lib_System.Reflection.Emit.Lightweight.dll.so => 94
	i32 u0xcc5af6ee, ; 809: Microsoft.Extensions.DependencyInjection.dll => 183
	i32 u0xcc6479a0, ; 810: System.Xml => 167
	i32 u0xcc7d82b4, ; 811: netstandard => 171
	i32 u0xcd1dd0db, ; 812: Xamarin.AndroidX.DynamicAnimation.dll => 234
	i32 u0xcd5a809f, ; 813: System.Formats.Tar => 39
	i32 u0xcdc696e0, ; 814: Microsoft.Maui.Controls.Compatibility.dll => 193
	i32 u0xcdd8cd54, ; 815: lib_Xamarin.AndroidX.Emoji2.dll.so => 235
	i32 u0xce3fa116, ; 816: lib_System.Diagnostics.Process.dll.so => 29
	i32 u0xce70fda2, ; 817: hr/Microsoft.Maui.Controls.resources.dll => 308
	i32 u0xcef19b37, ; 818: System.ComponentModel.TypeConverter.dll => 17
	i32 u0xcf3163e6, ; 819: Mono.Android => 175
	i32 u0xcf663a21, ; 820: ru/Microsoft.Maui.Controls.resources.dll => 321
	i32 u0xcfa20c36, ; 821: lib_Xamarin.AndroidX.SwipeRefreshLayout.dll.so => 272
	i32 u0xcfbaacae, ; 822: System.Text.Json.dll => 140
	i32 u0xcfd0c798, ; 823: System.Transactions => 154
	i32 u0xd0418592, ; 824: Xamarin.AndroidX.Concurrent.Futures.dll => 223
	i32 u0xd128d608, ; 825: System.Xml.Linq => 159
	i32 u0xd1854eb4, ; 826: System.Security.dll => 133
	i32 u0xd2757232, ; 827: System.Configuration => 19
	i32 u0xd2ff69f1, ; 828: System.Net.HttpListener => 67
	i32 u0xd310c033, ; 829: lib_Xamarin.Jetbrains.Annotations.dll.so => 289
	i32 u0xd328ac54, ; 830: vi/Microsoft.Maui.Controls.resources => 327
	i32 u0xd4045e1b, ; 831: lib_System.dll.so => 168
	i32 u0xd404ddfe, ; 832: lib_System.Runtime.Intrinsics.dll.so => 111
	i32 u0xd432d20b, ; 833: System.Threading.Timer => 151
	i32 u0xd457e5c9, ; 834: lib_Microsoft.CSharp.dll.so => 1
	i32 u0xd47cb45a, ; 835: lib_Xamarin.AndroidX.Arch.Core.Common.dll.so => 216
	i32 u0xd496c3c3, ; 836: lib_Xamarin.AndroidX.ExifInterface.dll.so => 237
	i32 u0xd4d2575b, ; 837: System.IO.FileSystem.AccessControl => 47
	i32 u0xd505225a, ; 838: lib_System.Xml.XPath.dll.so => 164
	i32 u0xd622b752, ; 839: lib-ro-Microsoft.Maui.Controls.resources.dll.so => 320
	i32 u0xd664cdf2, ; 840: de/Microsoft.Maui.Controls.resources => 301
	i32 u0xd6665034, ; 841: Xamarin.Android.Glide.GifDecoder.dll => 208
	i32 u0xd67a52b3, ; 842: System.Net.WebSockets.Client => 82
	i32 u0xd715a361, ; 843: System.Linq.dll => 63
	i32 u0xd7f95f5a, ; 844: da/Microsoft.Maui.Controls.resources => 300
	i32 u0xd804d57a, ; 845: System.Runtime.InteropServices.RuntimeInformation => 109
	i32 u0xd889aee8, ; 846: lib_System.Threading.Channels.dll.so => 143
	i32 u0xd8950487, ; 847: Xamarin.AndroidX.Annotation.Experimental => 212
	i32 u0xd8bba49d, ; 848: lib_Xamarin.AndroidX.RecyclerView.dll.so => 264
	i32 u0xd8dbab5d, ; 849: System.IO.FileSystem.Primitives.dll => 49
	i32 u0xd90e5f5a, ; 850: Xamarin.AndroidX.Lifecycle.LiveData.Core => 244
	i32 u0xd92e86f1, ; 851: Xamarin.KotlinX.Serialization.Core.dll => 295
	i32 u0xd930cda0, ; 852: Xamarin.AndroidX.Navigation.Fragment => 259
	i32 u0xd943a729, ; 853: System.ComponentModel.DataAnnotations => 14
	i32 u0xd96cf6f7, ; 854: pt-BR/Microsoft.Maui.Controls.resources => 318
	i32 u0xd9f65f5e, ; 855: lib-el-Microsoft.Maui.Controls.resources.dll.so => 302
	i32 u0xd9fdda56, ; 856: Microsoft.Extensions.Configuration.Abstractions.dll => 182
	i32 u0xda2f27df, ; 857: System.Net.NetworkInformation => 70
	i32 u0xda324084, ; 858: Plugin.Maui.Audio.dll => 199
	i32 u0xda4773dd, ; 859: he/Microsoft.Maui.Controls.resources => 306
	i32 u0xdabf74ac, ; 860: lib_Xamarin.AndroidX.Annotation.Jvm.dll.so => 213
	i32 u0xdae8aa5e, ; 861: Mono.Android.dll => 175
	i32 u0xdb258bb2, ; 862: Microsoft.Maui.Controls.HotReload.Forms => 331
	i32 u0xdb7f7e5d, ; 863: Xamarin.AndroidX.Browser => 218
	i32 u0xdb9df1ce, ; 864: Xamarin.AndroidX.Concurrent.Futures => 223
	i32 u0xdbb50d93, ; 865: ms/Microsoft.Maui.Controls.resources => 314
	i32 u0xdc5370c5, ; 866: lib_System.Web.HttpUtility.dll.so => 156
	i32 u0xdc68940c, ; 867: zh-Hant/Microsoft.Maui.Controls.resources.dll => 330
	i32 u0xdc96bdf5, ; 868: System.Net.WebProxy.dll => 81
	i32 u0xdcefb51d, ; 869: Xamarin.AndroidX.Core.Core.Ktx.dll => 228
	i32 u0xdd864306, ; 870: System.Runtime.Intrinsics => 111
	i32 u0xdda814c6, ; 871: Xamarin.AndroidX.Annotation => 211
	i32 u0xde068c70, ; 872: Xamarin.AndroidX.Navigation.Common.dll => 257
	i32 u0xde7354ab, ; 873: System.Net.NameResolution => 69
	i32 u0xdecad304, ; 874: System.Net.Http.Json.dll => 65
	i32 u0xdf1b1ecd, ; 875: lib_System.ServiceProcess.dll.so => 135
	i32 u0xdf6f3870, ; 876: System.Diagnostics.DiagnosticSource => 27
	i32 u0xdf9a7f42, ; 877: System.Xml.XPath => 164
	i32 u0xdfca27bc, ; 878: SQLitePCLRaw.provider.e_sqlite3 => 204
	i32 u0xdfd65a5d, ; 879: lib_System.Diagnostics.Tracing.dll.so => 34
	i32 u0xe05b6245, ; 880: Xamarin.AndroidX.Lifecycle.Runtime.Ktx.Android.dll => 250
	i32 u0xe0dec2ee, ; 881: lib_CommunityToolkit.Maui.dll.so => 178
	i32 u0xe12f62fc, ; 882: lib_System.Threading.ThreadPool.dll.so => 150
	i32 u0xe13414bb, ; 883: lib-hu-Microsoft.Maui.Controls.resources.dll.so => 309
	i32 u0xe1a41194, ; 884: lib_System.Xml.XDocument.dll.so => 162
	i32 u0xe1ae15d6, ; 885: Xamarin.AndroidX.Collection => 220
	i32 u0xe1eea3e4, ; 886: lib_System.IO.Compression.ZipFile.dll.so => 45
	i32 u0xe1f0a5d8, ; 887: lib_Xamarin.AndroidX.ViewPager.dll.so => 279
	i32 u0xe2098b0b, ; 888: System.Collections.NonGeneric => 10
	i32 u0xe250cda6, ; 889: lib_Microsoft.Extensions.Logging.dll.so => 188
	i32 u0xe2513246, ; 890: lib_System.Runtime.Numerics.dll.so => 113
	i32 u0xe28e5915, ; 891: Microsoft.Maui.Controls.Compatibility => 193
	i32 u0xe2a3f2e8, ; 892: System.Collections.Specialized.dll => 11
	i32 u0xe34ee011, ; 893: lib_System.IO.Pipelines.dll.so => 54
	i32 u0xe3774f52, ; 894: lib_System.IO.MemoryMappedFiles.dll.so => 53
	i32 u0xe3886bf7, ; 895: CommunityToolkit.Maui.dll => 178
	i32 u0xe3a54a09, ; 896: System.Net.WebProxy => 81
	i32 u0xe3c7860c, ; 897: lib_System.Security.Claims.dll.so => 121
	i32 u0xe3df9d2b, ; 898: System.Security.Cryptography.dll => 129
	i32 u0xe4436460, ; 899: System.Numerics.dll => 86
	i32 u0xe4fab729, ; 900: Microsoft.Extensions.DependencyInjection.Abstractions.dll => 184
	i32 u0xe52378b9, ; 901: System.Net.Mail.dll => 68
	i32 u0xe56ef253, ; 902: System.Runtime.InteropServices.dll => 110
	i32 u0xe625b819, ; 903: lib_Xamarin.AndroidX.CardView.dll.so => 219
	i32 u0xe6b14171, ; 904: System.Net.HttpListener.dll => 67
	i32 u0xe6ca3640, ; 905: lib_Xamarin.AndroidX.Collection.Ktx.dll.so => 222
	i32 u0xe6e179fa, ; 906: System.Security.Principal => 131
	i32 u0xe6e8f547, ; 907: lib_Microsoft.Extensions.Diagnostics.Abstractions.dll.so => 185
	i32 u0xe6f98713, ; 908: System.Security.Cryptography.Encoding.dll => 125
	i32 u0xe70c9739, ; 909: SQLite-net => 200
	i32 u0xe797fcc1, ; 910: System.Net.WebHeaderCollection.dll => 80
	i32 u0xe79e77a6, ; 911: Xamarin.AndroidX.Transition.dll => 275
	i32 u0xe7c9e2bd, ; 912: Xamarin.AndroidX.ProfileInstaller.ProfileInstaller => 263
	i32 u0xe7dc15ff, ; 913: zh-Hans/Microsoft.Maui.Controls.resources.dll => 329
	i32 u0xe839deed, ; 914: System.Collections.Concurrent.dll => 8
	i32 u0xe843daa0, ; 915: Xamarin.AndroidX.Core.dll => 227
	i32 u0xe89260c1, ; 916: Microsoft.VisualBasic.Core.dll => 2
	i32 u0xe90fdb70, ; 917: Xamarin.AndroidX.Collection.Jvm => 221
	i32 u0xe92ace5f, ; 918: lib_System.Linq.Parallel.dll.so => 61
	i32 u0xe97d0db9, ; 919: lib_System.IO.Hashing.dll.so => 176
	i32 u0xe99f7d24, ; 920: lib-tr-Microsoft.Maui.Controls.resources.dll.so => 325
	i32 u0xe9b2d35e, ; 921: System.IO.Compression.FileSystem.dll => 44
	i32 u0xe9b630ed, ; 922: Xamarin.AndroidX.VersionedParcelable.dll => 278
	i32 u0xea0092d6, ; 923: lib_System.Threading.Tasks.Extensions.dll.so => 146
	i32 u0xea213423, ; 924: System.Xml.ReaderWriter => 160
	i32 u0xea4780ec, ; 925: System.Security.Principal.dll => 131
	i32 u0xea4fb52e, ; 926: Xamarin.AndroidX.Navigation.UI => 262
	i32 u0xeab81858, ; 927: lib_Microsoft.Maui.Essentials.dll.so => 197
	i32 u0xeaf244cc, ; 928: lib_System.IO.Pipes.AccessControl.dll.so => 55
	i32 u0xeaf598f6, ; 929: lib_Microsoft.Extensions.Logging.Abstractions.dll.so => 189
	i32 u0xeb2ecede, ; 930: System.Data.DataSetExtensions.dll => 23
	i32 u0xeb5560c9, ; 931: lib_System.Runtime.InteropServices.RuntimeInformation.dll.so => 109
	i32 u0xebac8bfe, ; 932: System.Text.Encoding.CodePages => 136
	i32 u0xebb0254b, ; 933: lib_System.Net.NetworkInformation.dll.so => 70
	i32 u0xebc66336, ; 934: Xamarin.AndroidX.AppCompat.dll => 214
	i32 u0xec05582d, ; 935: Xamarin.AndroidX.Lifecycle.Process => 246
	i32 u0xed1090ae, ; 936: lib_System.Net.Primitives.dll.so => 72
	i32 u0xed409aea, ; 937: th/Microsoft.Maui.Controls.resources.dll => 324
	i32 u0xed96d41f, ; 938: lib_Xamarin.AndroidX.CoordinatorLayout.dll.so => 226
	i32 u0xedadd6e2, ; 939: he/Microsoft.Maui.Controls.resources.dll => 306
	i32 u0xedf6669b, ; 940: lib_System.Drawing.dll.so => 36
	i32 u0xee9f991d, ; 941: System.Diagnostics.Process.dll => 29
	i32 u0xeeefb9c8, ; 942: lib_System.Dynamic.Runtime.dll.so => 37
	i32 u0xef5e8475, ; 943: Xamarin.AndroidX.Annotation.Jvm.dll => 213
	i32 u0xefd01a89, ; 944: System.IO.Pipelines => 54
	i32 u0xeff49a63, ; 945: System.Memory => 64
	i32 u0xeff49c4a, ; 946: lib_System.Text.Encoding.Extensions.dll.so => 137
	i32 u0xf09122fc, ; 947: lib_System.IO.IsolatedStorage.dll.so => 52
	i32 u0xf0ba55d9, ; 948: lib_Microsoft.Maui.Controls.HotReload.Forms.dll.so => 331
	i32 u0xf121f953, ; 949: lib_Xamarin.AndroidX.Lifecycle.LiveData.Core.dll.so => 244
	i32 u0xf1304331, ; 950: Microsoft.Maui.Controls.Xaml.dll => 195
	i32 u0xf15cb56d, ; 951: Xamarin.KotlinX.Serialization.Core => 295
	i32 u0xf1676aaa, ; 952: lib-da-Microsoft.Maui.Controls.resources.dll.so => 300
	i32 u0xf1ad867b, ; 953: System.Reflection.Emit.ILGeneration => 93
	i32 u0xf27f60d1, ; 954: System.Private.Xml.Linq.dll => 90
	i32 u0xf29c5384, ; 955: id/Microsoft.Maui.Controls.resources => 310
	i32 u0xf2ce3c98, ; 956: System.Threading.dll => 152
	i32 u0xf2dd3fc4, ; 957: lib-ja-Microsoft.Maui.Controls.resources.dll.so => 312
	i32 u0xf3201983, ; 958: Microsoft.Extensions.Hosting.Abstractions.dll => 187
	i32 u0xf323e0a6, ; 959: lib_Xamarin.Kotlin.StdLib.dll.so => 291
	i32 u0xf33c42ef, ; 960: lib_Xamarin.AndroidX.VectorDrawable.Animated.dll.so => 277
	i32 u0xf3a16066, ; 961: lib_Xamarin.AndroidX.Lifecycle.ViewModel.dll.so => 251
	i32 u0xf40add04, ; 962: Microsoft.Maui.Essentials.dll => 197
	i32 u0xf42589bc, ; 963: lib_System.Security.Cryptography.X509Certificates.dll.so => 128
	i32 u0xf45985cf, ; 964: System.Drawing.dll => 36
	i32 u0xf462c30d, ; 965: System.Private.Uri => 89
	i32 u0xf479582c, ; 966: Xamarin.AndroidX.Emoji2 => 235
	i32 u0xf47b0a29, ; 967: lib_System.Configuration.dll.so => 19
	i32 u0xf48143e5, ; 968: pt/Microsoft.Maui.Controls.resources.dll => 319
	i32 u0xf5185c24, ; 969: lib-pt-Microsoft.Maui.Controls.resources.dll.so => 319
	i32 u0xf53cb11d, ; 970: lib_System.Net.Security.dll.so => 75
	i32 u0xf554c79b, ; 971: lib_System.Linq.AsyncEnumerable.dll.so => 59
	i32 u0xf5861a4f, ; 972: pl/Microsoft.Maui.Controls.resources => 317
	i32 u0xf5e94e90, ; 973: ms/Microsoft.Maui.Controls.resources.dll => 314
	i32 u0xf5f4f1f0, ; 974: Microsoft.Extensions.DependencyInjection => 183
	i32 u0xf5fdf056, ; 975: lib_Microsoft.Extensions.DependencyInjection.dll.so => 183
	i32 u0xf60736e2, ; 976: System.IO.FileSystem.Watcher => 50
	i32 u0xf6318da0, ; 977: System.AppContext => 6
	i32 u0xf73be021, ; 978: System.Reflection.Emit.ILGeneration.dll => 93
	i32 u0xf76edc75, ; 979: System.Core => 21
	i32 u0xf7e95c85, ; 980: System.Xml.XmlSerializer => 166
	i32 u0xf807b767, ; 981: System.Reflection.TypeExtensions => 99
	i32 u0xf83dd773, ; 982: System.IO.FileSystem.Watcher.dll => 50
	i32 u0xf8420da3, ; 983: lib_Microsoft.VisualStudio.DesignTools.TapContract.dll.so => 333
	i32 u0xf86129d4, ; 984: lib-sv-Microsoft.Maui.Controls.resources.dll.so => 323
	i32 u0xf93ba7d4, ; 985: System.Runtime.Serialization.Primitives => 116
	i32 u0xf94a8f86, ; 986: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll => 254
	i32 u0xf951b10e, ; 987: Microsoft.VisualStudio.DesignTools.MobileTapContracts => 332
	i32 u0xf97c5a99, ; 988: System.Security => 133
	i32 u0xf9be026d, ; 989: lib_SQLitePCLRaw.core.dll.so => 202
	i32 u0xfa21f6af, ; 990: System.Net.WebClient.dll => 79
	i32 u0xfa50891f, ; 991: lib_System.Linq.dll.so => 63
	i32 u0xfa6ae1e2, ; 992: lib_Xamarin.AndroidX.Annotation.dll.so => 211
	i32 u0xfac98279, ; 993: Xamarin.AndroidX.SavedState.SavedState.Android => 267
	i32 u0xfb0af295, ; 994: lib-zh-HK-Microsoft.Maui.Controls.resources.dll.so => 328
	i32 u0xfb1dad5d, ; 995: System.Diagnostics.DiagnosticSource.dll => 27
	i32 u0xfbc4b67c, ; 996: lib_System.IO.Compression.Brotli.dll.so => 43
	i32 u0xfc5f7d36, ; 997: pt/Microsoft.Maui.Controls.resources => 319
	i32 u0xfdaee526, ; 998: Xamarin.AndroidX.Core.Core.Ktx => 228
	i32 u0xfdd1b433, ; 999: Xamarin.AndroidX.Lifecycle.ViewModel.Ktx => 253
	i32 u0xfdf2741f, ; 1000: System.Buffers => 7
	i32 u0xfe42d509, ; 1001: lib_Xamarin.AndroidX.Security.SecurityCrypto.dll.so => 269
	i32 u0xfea12dee, ; 1002: Microsoft.Maui.Controls.dll => 194
	i32 u0xfecef6ea, ; 1003: System.Runtime.Numerics => 113
	i32 u0xff6b9aa3, ; 1004: lib_Xamarin.AndroidX.SavedState.SavedState.Android.dll.so => 267
	i32 u0xff912ee3, ; 1005: lib_System.Xml.Serialization.dll.so => 161
	i32 u0xffd4917f, ; 1006: Xamarin.AndroidX.Lifecycle.ViewModelSavedState => 254
	i32 u0xfffce3e8 ; 1007: Xamarin.AndroidX.ExifInterface.dll => 237
], align 4

@assembly_image_cache_indices = dso_local local_unnamed_addr constant [1008 x i32] [
	i32 70, i32 74, i32 69, i32 256, i32 166, i32 111, i32 184, i32 59,
	i32 247, i32 288, i32 48, i32 83, i32 326, i32 149, i32 196, i32 298,
	i32 30, i32 127, i32 198, i32 105, i32 10, i32 185, i32 270, i32 76,
	i32 62, i32 261, i32 328, i32 288, i32 68, i32 110, i32 258, i32 270,
	i32 143, i32 31, i32 80, i32 127, i32 13, i32 223, i32 220, i32 209,
	i32 135, i32 272, i32 273, i32 327, i32 284, i32 155, i32 163, i32 65,
	i32 317, i32 327, i32 77, i32 328, i32 18, i32 334, i32 218, i32 142,
	i32 44, i32 26, i32 190, i32 1, i32 229, i32 231, i32 61, i32 42,
	i32 142, i32 315, i32 94, i32 132, i32 205, i32 224, i32 151, i32 243,
	i32 240, i32 299, i32 23, i32 322, i32 55, i32 71, i32 209, i32 86,
	i32 286, i32 119, i32 312, i32 241, i32 203, i32 13, i32 311, i32 299,
	i32 134, i32 181, i32 112, i32 56, i32 332, i32 153, i32 77, i32 149,
	i32 176, i32 64, i32 150, i32 265, i32 335, i32 169, i32 248, i32 125,
	i32 323, i32 225, i32 12, i32 238, i32 128, i32 156, i32 249, i32 174,
	i32 116, i32 170, i32 172, i32 168, i32 240, i32 89, i32 270, i32 118,
	i32 87, i32 310, i32 304, i32 192, i32 127, i32 6, i32 154, i32 307,
	i32 62, i32 188, i32 51, i32 106, i32 117, i32 40, i32 290, i32 285,
	i32 229, i32 123, i32 318, i32 180, i32 134, i32 11, i32 178, i32 52,
	i32 21, i32 44, i32 78, i32 122, i32 248, i32 231, i32 236, i32 232,
	i32 96, i32 301, i32 179, i32 84, i32 139, i32 278, i32 22, i32 216,
	i32 8, i32 51, i32 318, i32 75, i32 298, i32 159, i32 292, i32 230,
	i32 158, i32 167, i32 95, i32 289, i32 45, i32 313, i32 301, i32 112,
	i32 133, i32 199, i32 132, i32 201, i32 25, i32 206, i32 165, i32 74,
	i32 56, i32 46, i32 100, i32 191, i32 92, i32 232, i32 22, i32 245,
	i32 149, i32 89, i32 43, i32 164, i32 73, i32 233, i32 297, i32 3,
	i32 42, i32 65, i32 176, i32 56, i32 203, i32 287, i32 16, i32 58,
	i32 53, i32 154, i32 41, i32 325, i32 288, i32 14, i32 252, i32 236,
	i32 108, i32 242, i32 283, i32 170, i32 286, i32 241, i32 34, i32 162,
	i32 292, i32 88, i32 32, i32 234, i32 12, i32 329, i32 145, i32 51,
	i32 303, i32 57, i32 266, i32 36, i32 184, i32 300, i32 287, i32 214,
	i32 35, i32 240, i32 60, i32 99, i32 185, i32 251, i32 12, i32 101,
	i32 192, i32 0, i32 246, i32 180, i32 17, i32 291, i32 168, i32 187,
	i32 95, i32 139, i32 208, i32 249, i32 190, i32 324, i32 126, i32 17,
	i32 81, i32 85, i32 281, i32 66, i32 88, i32 76, i32 158, i32 157,
	i32 186, i32 277, i32 260, i32 304, i32 144, i32 103, i32 317, i32 216,
	i32 29, i32 52, i32 220, i32 151, i32 315, i32 187, i32 307, i32 221,
	i32 290, i32 5, i32 198, i32 296, i32 271, i32 293, i32 90, i32 276,
	i32 117, i32 222, i32 292, i32 213, i32 202, i32 233, i32 263, i32 88,
	i32 258, i32 258, i32 281, i32 63, i32 115, i32 308, i32 199, i32 298,
	i32 255, i32 58, i32 266, i32 102, i32 193, i32 177, i32 50, i32 19,
	i32 226, i32 255, i32 114, i32 104, i32 32, i32 3, i32 105, i32 86,
	i32 107, i32 286, i32 242, i32 73, i32 4, i32 252, i32 38, i32 32,
	i32 57, i32 106, i32 75, i32 9, i32 126, i32 46, i32 215, i32 192,
	i32 9, i32 43, i32 4, i32 268, i32 307, i32 35, i32 302, i32 31,
	i32 218, i32 141, i32 95, i32 96, i32 322, i32 305, i32 274, i32 49,
	i32 145, i32 115, i32 296, i32 144, i32 2, i32 323, i32 16, i32 330,
	i32 118, i32 287, i32 161, i32 331, i32 79, i32 82, i32 39, i32 256,
	i32 37, i32 280, i32 217, i32 179, i32 236, i32 230, i32 66, i32 141,
	i32 15, i32 46, i32 160, i32 169, i32 119, i32 294, i32 273, i32 284,
	i32 224, i32 48, i32 72, i32 83, i32 290, i32 129, i32 97, i32 124,
	i32 153, i32 26, i32 203, i32 120, i32 245, i32 100, i32 28, i32 219,
	i32 105, i32 320, i32 153, i32 54, i32 173, i32 4, i32 101, i32 38,
	i32 330, i32 33, i32 80, i32 96, i32 272, i32 188, i32 108, i32 283,
	i32 21, i32 41, i32 214, i32 174, i32 106, i32 238, i32 152, i32 305,
	i32 186, i32 256, i32 315, i32 291, i32 284, i32 293, i32 262, i32 2,
	i32 309, i32 137, i32 114, i32 333, i32 278, i32 189, i32 326, i32 206,
	i32 60, i32 98, i32 224, i32 39, i32 217, i32 333, i32 329, i32 25,
	i32 97, i32 92, i32 102, i32 10, i32 215, i32 175, i32 204, i32 90,
	i32 49, i32 64, i32 79, i32 103, i32 312, i32 269, i32 181, i32 47,
	i32 76, i32 208, i32 7, i32 308, i32 84, i32 251, i32 297, i32 212,
	i32 205, i32 306, i32 311, i32 91, i32 244, i32 158, i32 335, i32 59,
	i32 239, i32 33, i32 0, i32 115, i32 119, i32 67, i32 85, i32 204,
	i32 138, i32 334, i32 20, i32 11, i32 166, i32 136, i32 313, i32 3,
	i32 28, i32 313, i32 314, i32 285, i32 196, i32 114, i32 320, i32 102,
	i32 27, i32 15, i32 191, i32 7, i32 189, i32 87, i32 60, i32 73,
	i32 30, i32 294, i32 66, i32 277, i32 147, i32 83, i32 321, i32 161,
	i32 282, i32 41, i32 253, i32 194, i32 120, i32 182, i32 207, i32 305,
	i32 311, i32 264, i32 316, i32 171, i32 324, i32 248, i32 282, i32 134,
	i32 78, i32 201, i32 68, i32 177, i32 211, i32 147, i32 109, i32 155,
	i32 304, i32 20, i32 72, i32 200, i32 243, i32 160, i32 325, i32 181,
	i32 148, i32 124, i32 259, i32 297, i32 130, i32 283, i32 156, i32 235,
	i32 157, i32 91, i32 332, i32 282, i32 145, i32 316, i32 222, i32 98,
	i32 238, i32 20, i32 14, i32 271, i32 138, i32 78, i32 61, i32 201,
	i32 225, i32 171, i32 245, i32 172, i32 194, i32 15, i32 77, i32 140,
	i32 247, i32 6, i32 310, i32 210, i32 23, i32 247, i32 266, i32 322,
	i32 205, i32 195, i32 94, i32 1, i32 139, i32 250, i32 249, i32 276,
	i32 137, i32 71, i32 150, i32 186, i32 116, i32 294, i32 25, i32 239,
	i32 190, i32 91, i32 99, i32 230, i32 234, i32 262, i32 31, i32 45,
	i32 147, i32 243, i32 254, i32 207, i32 112, i32 162, i32 35, i32 293,
	i32 110, i32 22, i32 117, i32 58, i32 141, i32 273, i32 148, i32 121,
	i32 123, i32 227, i32 113, i32 303, i32 209, i32 143, i32 215, i32 55,
	i32 82, i32 108, i32 255, i32 195, i32 8, i32 196, i32 123, i32 136,
	i32 252, i32 289, i32 281, i32 155, i32 279, i32 265, i32 242, i32 9,
	i32 206, i32 48, i32 239, i32 69, i32 274, i32 198, i32 326, i32 163,
	i32 303, i32 260, i32 226, i32 5, i32 167, i32 129, i32 135, i32 265,
	i32 165, i32 316, i32 274, i32 253, i32 144, i32 279, i32 42, i32 275,
	i32 302, i32 173, i32 197, i32 179, i32 210, i32 285, i32 40, i32 296,
	i32 321, i32 237, i32 84, i32 57, i32 37, i32 130, i32 100, i32 142,
	i32 170, i32 177, i32 295, i32 280, i32 85, i32 212, i32 191, i32 101,
	i32 26, i32 30, i32 163, i32 268, i32 18, i32 130, i32 261, i32 122,
	i32 233, i32 159, i32 250, i32 269, i32 299, i32 207, i32 71, i32 246,
	i32 271, i32 169, i32 335, i32 107, i32 268, i32 257, i32 173, i32 174,
	i32 16, i32 280, i32 276, i32 148, i32 309, i32 40, i32 128, i32 121,
	i32 38, i32 118, i32 47, i32 146, i32 87, i32 120, i32 34, i32 180,
	i32 98, i32 53, i32 0, i32 221, i32 97, i32 257, i32 259, i32 131,
	i32 104, i32 132, i32 157, i32 24, i32 165, i32 334, i32 152, i32 107,
	i32 241, i32 229, i32 92, i32 225, i32 267, i32 219, i32 62, i32 146,
	i32 93, i32 103, i32 5, i32 13, i32 124, i32 200, i32 125, i32 122,
	i32 138, i32 28, i32 182, i32 74, i32 231, i32 24, i32 24, i32 217,
	i32 263, i32 260, i32 275, i32 228, i32 18, i32 140, i32 202, i32 210,
	i32 227, i32 172, i32 33, i32 261, i32 264, i32 104, i32 126, i32 232,
	i32 94, i32 183, i32 167, i32 171, i32 234, i32 39, i32 193, i32 235,
	i32 29, i32 308, i32 17, i32 175, i32 321, i32 272, i32 140, i32 154,
	i32 223, i32 159, i32 133, i32 19, i32 67, i32 289, i32 327, i32 168,
	i32 111, i32 151, i32 1, i32 216, i32 237, i32 47, i32 164, i32 320,
	i32 301, i32 208, i32 82, i32 63, i32 300, i32 109, i32 143, i32 212,
	i32 264, i32 49, i32 244, i32 295, i32 259, i32 14, i32 318, i32 302,
	i32 182, i32 70, i32 199, i32 306, i32 213, i32 175, i32 331, i32 218,
	i32 223, i32 314, i32 156, i32 330, i32 81, i32 228, i32 111, i32 211,
	i32 257, i32 69, i32 65, i32 135, i32 27, i32 164, i32 204, i32 34,
	i32 250, i32 178, i32 150, i32 309, i32 162, i32 220, i32 45, i32 279,
	i32 10, i32 188, i32 113, i32 193, i32 11, i32 54, i32 53, i32 178,
	i32 81, i32 121, i32 129, i32 86, i32 184, i32 68, i32 110, i32 219,
	i32 67, i32 222, i32 131, i32 185, i32 125, i32 200, i32 80, i32 275,
	i32 263, i32 329, i32 8, i32 227, i32 2, i32 221, i32 61, i32 176,
	i32 325, i32 44, i32 278, i32 146, i32 160, i32 131, i32 262, i32 197,
	i32 55, i32 189, i32 23, i32 109, i32 136, i32 70, i32 214, i32 246,
	i32 72, i32 324, i32 226, i32 306, i32 36, i32 29, i32 37, i32 213,
	i32 54, i32 64, i32 137, i32 52, i32 331, i32 244, i32 195, i32 295,
	i32 300, i32 93, i32 90, i32 310, i32 152, i32 312, i32 187, i32 291,
	i32 277, i32 251, i32 197, i32 128, i32 36, i32 89, i32 235, i32 19,
	i32 319, i32 319, i32 75, i32 59, i32 317, i32 314, i32 183, i32 183,
	i32 50, i32 6, i32 93, i32 21, i32 166, i32 99, i32 50, i32 333,
	i32 323, i32 116, i32 254, i32 332, i32 133, i32 202, i32 79, i32 63,
	i32 211, i32 267, i32 328, i32 27, i32 43, i32 319, i32 228, i32 253,
	i32 7, i32 269, i32 194, i32 113, i32 267, i32 161, i32 254, i32 237
], align 4

@marshal_methods_number_of_classes = dso_local local_unnamed_addr constant i32 0, align 4

@marshal_methods_class_cache = dso_local local_unnamed_addr global [0 x %struct.MarshalMethodsManagedClass] zeroinitializer, align 4

; Names of classes in which marshal methods reside
@mm_class_names = dso_local local_unnamed_addr constant [0 x ptr] zeroinitializer, align 4

@mm_method_names = dso_local local_unnamed_addr constant [1 x %struct.MarshalMethodName] [
	%struct.MarshalMethodName {
		i64 u0x0000000000000000, ; name: 
		ptr @.MarshalMethodName.0_name; char* name
	} ; 0
], align 8

; get_function_pointer (uint32_t mono_image_index, uint32_t class_index, uint32_t method_token, void*& target_ptr)
@get_function_pointer = internal dso_local unnamed_addr global ptr null, align 4

; Functions

; Function attributes: memory(write, argmem: none, inaccessiblemem: none) "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" uwtable willreturn
define void @xamarin_app_init(ptr nocapture noundef readnone %env, ptr noundef %fn) local_unnamed_addr #0
{
	%fnIsNull = icmp eq ptr %fn, null
	br i1 %fnIsNull, label %1, label %2

1: ; preds = %0
	%putsResult = call noundef i32 @puts(ptr @.mm.0)
	call void @abort()
	unreachable 

2: ; preds = %1, %0
	store ptr %fn, ptr @get_function_pointer, align 4, !tbaa !3
	ret void
}

; Strings
@.mm.0 = private unnamed_addr constant [40 x i8] c"get_function_pointer MUST be specified\0A\00", align 1

;MarshalMethodName
@.MarshalMethodName.0_name = private unnamed_addr constant [1 x i8] c"\00", align 1

; External functions

; Function attributes: noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8"
declare void @abort() local_unnamed_addr #2

; Function attributes: nofree nounwind
declare noundef i32 @puts(ptr noundef) local_unnamed_addr #1
attributes #0 = { memory(write, argmem: none, inaccessiblemem: none) "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "target-cpu"="generic" "target-features"="+armv7-a,+d32,+dsp,+fp64,+neon,+vfp2,+vfp2sp,+vfp3,+vfp3d16,+vfp3d16sp,+vfp3sp,-aes,-fp-armv8,-fp-armv8d16,-fp-armv8d16sp,-fp-armv8sp,-fp16,-fp16fml,-fullfp16,-sha2,-thumb-mode,-vfp4,-vfp4d16,-vfp4d16sp,-vfp4sp" uwtable willreturn }
attributes #1 = { nofree nounwind }
attributes #2 = { noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "target-cpu"="generic" "target-features"="+armv7-a,+d32,+dsp,+fp64,+neon,+vfp2,+vfp2sp,+vfp3,+vfp3d16,+vfp3d16sp,+vfp3sp,-aes,-fp-armv8,-fp-armv8d16,-fp-armv8d16sp,-fp-armv8sp,-fp16,-fp16fml,-fullfp16,-sha2,-thumb-mode,-vfp4,-vfp4d16,-vfp4d16sp,-vfp4sp" }

; Metadata
!llvm.module.flags = !{!0, !1, !7}
!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 7, !"PIC Level", i32 2}
!llvm.ident = !{!2}
!2 = !{!".NET for Android remotes/origin/release/10.0.1xx @ 01024bb616e7b80417a2c6d320885bfdb956f20a"}
!3 = !{!4, !4, i64 0}
!4 = !{!"any pointer", !5, i64 0}
!5 = !{!"omnipotent char", !6, i64 0}
!6 = !{!"Simple C++ TBAA"}
!7 = !{i32 1, !"min_enum_size", i32 4}
