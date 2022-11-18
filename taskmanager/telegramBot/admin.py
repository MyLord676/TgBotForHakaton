from django.contrib import admin

from .models import AuthUser, Requests, Notify
from .models import AuthGroup, AuthGroupPermissions
from .models import AuthPermission
from .models import AuthUserGroups
from .models import AuthUserUserPermissions
from .models import DjangoAdminLog
from .models import DjangoContentType
from .models import DjangoMigrations
from .models import DjangoSession


@admin.register(AuthUser)
class AuthUserAdmin(admin.ModelAdmin):
    list_display = ('id', 'tg_id', 'password', 'token_requests_count',
                    'last_login', 'is_superuser', 'username',
                    'first_name', 'last_name', 'email',
                    'is_staff', 'is_active', 'date_joined',
                    'descrtext', 'first_token')


@admin.register(Requests)
class RequestsAdmin(admin.ModelAdmin):
    list_display = ('id', 'tg_id', 'ts', 'request_text')


@admin.register(Notify)
class NotifyAdmin(admin.ModelAdmin):
    list_display = ('id', 'tg_id', 'notify_text', 'sended')


@admin.register(AuthGroup)
class AuthGroupAdmin(admin.ModelAdmin):
    list_display = ('id', 'name')


@admin.register(AuthGroupPermissions)
class AuthGroupPermissionsAdmin(admin.ModelAdmin):
    list_display = ('id', 'group_id', 'permission_id')


@admin.register(AuthPermission)
class AuthPermissionAdmin(admin.ModelAdmin):
    list_display = ('id', 'name', 'content_type_id', 'codename')


@admin.register(AuthUserGroups)
class AuthUserGroupsAdmin(admin.ModelAdmin):
    list_display = ('id', 'user_id', 'group_id')


@admin.register(AuthUserUserPermissions)
class AuthUserUserPermissionsAdmin(admin.ModelAdmin):
    list_display = ('id', 'user_id', 'permission_id')


@admin.register(DjangoAdminLog)
class DjangoAdminLogAdmin(admin.ModelAdmin):
    list_display = ('id', 'action_time', 'object_id',
                    'object_repr', 'action_flag', 'change_message',
                    'content_type_id', 'user_id')


@admin.register(DjangoContentType)
class DjangoContentTypeAdmin(admin.ModelAdmin):
    list_display = ('id', 'app_label', 'model')


@admin.register(DjangoMigrations)
class DjangoMigrationsAdmin(admin.ModelAdmin):
    list_display = ('id', 'app', 'name', 'applied')


@admin.register(DjangoSession)
class DjangoSessionAdmin(admin.ModelAdmin):
    list_display = ('session_key', 'session_data', 'expire_date')
