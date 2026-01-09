NOVEDADES:
- Se eliminó el texto del canvas controls para comer pizza y se reemplazó por uno de interacción.

TOCHECK:

- HEADACHES: actualmente se terminó el final de la primera parte, pero queda aún se podría mencionar al inicio de Headaches el dolor de cabeza que le ocasiona el walkie talkie y los aparatos electrónicos. Quizas luego de eso podriamos prender y apagar aparatos aleatoriamente? (microondas, radio, luces de la casa)
- Agregar trueno al bajar por las escaleras en Unwired?

TOFIX:
- Separar audio de microondas. Logica en MicrowaveHandler.cs
- Si habilitamos el Walkie Talkie mediante un checkpoint asset, se rompe al mostrarlo o intentar cambiar el canal debido a que no genera una estructura de canales. // Esto sucede cuando no existe extructura de audios de canales para el evento en particular //

TODO - próximo:
- Luego de terminar la primera conversación con Mica en Noises, se debe hacer hacer aparecer el panel de controles mostrando los inputs para cambiar de canal en el Walkie Talkie. De hecho, se podrian mostrar siempre que esté habilitado el Walkie Talkie. Idem para la linterna.
- Actualmente el sistema de audio por perfiles de ambiente funciona en base a la escena. Por ende, si quisieramos detener la lluvia en algun momento, la lluvia estereo del los perfiles seguiria sonando. Analizar si es conveniente o no ampliar la estructura logica.
- Finalizar Visions y comenzar con RedHour

TODO - despues:

- Agregar sonidos para cuando come la pizza.
- Agregar sonidos al tomar bolsas de basura.
- Agregar sonido de mareo y caida del jugador al suelo, dependiendo del lugar donde caiga debe sonar distinto (3 materiales: carpet, wood, mosaic)
- Agregar animación de primera activación de linterna para Unwired? (MAYBE)
- Componer distintos audios según intensidad de Electric Feel. La idea es que sean clips conformados por el sonido actual (loopeable), luego un clip que incluya el layer anterior mas un fade in del nuevo layer de audio (Play one shot), y finalmente el nuevo audio (loopeable).
- Agregar sonidos stereo en loop para ambientación de cada habitación dentro de la casa??
  -> Se podrían disparar gracias al House Areas handler creado recientemente. El problema es que el volumen de audio sería el mismo en todos los puntos del volumen del collider.
- Agregar sonido para animación "Night - Stand Up".
- Agregar sonido para animación "Night - Unwired - Look For Flashlight" de FlashlightVC

- Agregar cajas bajo la escalera, o algo.

- Actualizar textos en puertas principales y objetos contemplables al final de la escena de Evening. Por ahora se han dejado de trabajar los SO's.
- Implementar cambios de Video del Settings Menu
- Mostrar Settings Menu Canvas desde el Pause Menu Canvas. Esto implica alterar la lógica del Settings Menu Canvas ya que actualmente al retornar desde cualquier botón "Return" muestra el Main Menu Canvas.
