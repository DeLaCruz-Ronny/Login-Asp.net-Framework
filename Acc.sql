create database Acceso

use Acceso

create table Usuario(
	IdUsuario int primary key identity(1,1),
	Correo varchar(100),
	Clave varchar(100)
)



create proc sp_RegistrtarUsuario(
	@Correo varchar(100),
	@Clave varchar(100),
	@Registrado bit output,
	@Mensaje varchar(100) output
)
as
begin
	if(not exists(select * from Usuario where Correo = @Correo))
	begin
		insert into Usuario(Correo,Clave) values(@Correo,@Clave)
		set @Registrado = 1
		set @Mensaje = 'usuario Registrado'
	end
	else
	begin
		set @Registrado = 0
		set @Mensaje = 'Correo ya existe'
	end
end

create proc sp_validar(
	@Correo varchar(100),
	@Clave varchar(100)
)
as
begin
	if(exists(select * from Usuario where Correo = @Correo and Clave = @Clave))
		select IdUsuario from Usuario where Correo = @Correo and Clave = @Clave
	else
		select '0'
end


declare @Registrado bit, @Mensaje varchar(100)

exec sp_RegistrtarUsuario 'correo@gmail.com','5ac0852e770506dcd80f1a36d20ba7878bf82244b836d9324593bd14bc56dcb5',@Registrado output, @Mensaje output

select @Registrado
select @Mensaje

exec sp_validar 'correo@gmail.com','5ac0852e770506dcd80f1a36d20ba7878bf82244b836d9324593bd14bc56dcb5'

select * from Usuario